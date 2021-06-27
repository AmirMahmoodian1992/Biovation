using App.Metrics;
using App.Metrics.Extensions.Configuration;
using Biovation.CommonClasses;
using Biovation.CommonClasses.Manager;
using Biovation.Constants;
using Biovation.Domain;
using Biovation.Repository.Api.v2;
using Biovation.Server.HostedServices;
using Biovation.Server.Jobs;
using Biovation.Server.Managers;
using Biovation.Server.Middleware;
using Biovation.Service.Api.v1;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Quartz;
using RestSharp;
using Serilog;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Log = Serilog.Log;

namespace Biovation.Server
{
    public class Startup
    {
        public BiovationConfigurationManager BiovationConfiguration { get; set; }
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration, IHostEnvironment environment)
        {
            Configuration = configuration;

            Log.Logger = new LoggerConfiguration().ReadFrom.Configuration(configuration)
                .Enrich.With(new ThreadIdEnricher())
                .Enrich.WithProperty("Version", Assembly.GetExecutingAssembly().GetName().Version)
                .CreateLogger();

            BiovationConfiguration = new BiovationConfigurationManager(configuration);

            var metrics = new MetricsBuilder()
                .Configuration.ReadFrom(configuration);

            var builder = new ConfigurationBuilder()
                .SetBasePath(environment.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{environment.EnvironmentName}.json", true, true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
            metrics.Build();
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers()
                .AddJsonOptions(options =>
                        {
                            options.JsonSerializerOptions.Converters.Add(new TimeSpanToStringConverter());
                            options.JsonSerializerOptions.IgnoreNullValues = true;
                        }).AddMetrics();

            services.AddApiVersioning(config =>
            {
                config.DefaultApiVersion = new ApiVersion(1, 0);
                config.AssumeDefaultVersionWhenUnspecified = true;
                config.ReportApiVersions = true;  //the clients of the API know all supported versions
                //config.Conventions.Add(new VersionByNamespaceConvention());
                config.ApiVersionReader = ApiVersionReader.Combine(new UrlSegmentApiVersionReader(), new HeaderApiVersionReader("api-version"));
                config.ApiVersionSelector = new CurrentImplementationApiVersionSelector(config);
                config.RegisterMiddleware = true;
            });

            services.AddVersionedApiExplorer();

            services.AddSwaggerGen(config =>
            {
                config.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {  
                    Name = "Authorization",  
                    Type = SecuritySchemeType.ApiKey,  
                    Scheme = "Bearer",  
                    BearerFormat = "JWT",  
                    In = ParameterLocation.Header,  
                    Description = "JWT Authorization header using the Bearer scheme. \r\n\r\n Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer 12345abcdef\"",  
                }); 

                config.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            },
                            Scheme = "oauth2",
                            Name = "Bearer",
                            In = ParameterLocation.Header
                        },
                        new List<string>()
                    }
                });

                //var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                //var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                //config.IncludeXmlComments(xmlPath);
            });

            services.AddMvc();
            //services.AddSingleton<IAuthorizationHandler,  OverrideTestAuthorizationHandler>();

            services.AddSingleton(BiovationConfiguration);
            services.AddSingleton(BiovationConfiguration.Configuration);
            services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();

            var serviceStatuses = new SystemInfo();
            services.AddSingleton(serviceStatuses);


            ConfigureConstantValues(services);
            ConfigureRepositoriesServices(services);

            services.AddQuartz(config =>
            {
                config.UseMicrosoftDependencyInjectionJobFactory(options =>
                {
                    options.AllowDefaultConstructor = true;
                });

                //config.UseMicrosoftDependencyInjectionScopedJobFactory();

                config.UseSimpleTypeLoader();
                config.UseInMemoryStore();
                config.UseDefaultThreadPool(tp =>
                {
                    tp.MaxConcurrency = 10;
                });

                config.AddJob<ExecuteScheduledTaskJob>(options => { options.StoreDurably(); });
                config.AddJob<ExecuteRecurringTaskJob>(options => { options.StoreDurably(); });
            });

            services.AddQuartzServer(config => { config.WaitForJobsToComplete = true; });

            services.AddTransient<ScheduledTasksManager, ScheduledTasksManager>();
            services.AddTransient<RecurringTasksManager, RecurringTasksManager>();

            services.AddHostedService<TaskMangerHostedService>();
            services.AddHostedService<ServicesHealthCheckHostedService>();
            //services.AddSingleton<object, object>();
            //services.AddSingleton<RequestDelegate, RequestDelegate>();
        }

        private void ConfigureRepositoriesServices(IServiceCollection services)
        {

            var restClient = (RestClient)new RestClient(BiovationConfiguration.BiovationServerUri).UseSerializer(() => new RestRequestJsonSerializer());

            services.AddSingleton(restClient);

            //services.AddTransient<FoodRepository, FoodRepository>();
            //services.AddTransient<MealRepository, MealRepository>();
            //services.AddTransient<ReservationRepository, ReservationRepository>();
            //services.AddTransient<RestaurantRepository, RestaurantRepository>();
            //services.AddTransient<ServeLogRepository, ServeLogRepository>();

            //services.AddTransient<AccessGroupRepository, AccessGroupRepository>();
            //services.AddTransient<AdminDeviceRepository, AdminDeviceRepository>();
            //services.AddTransient<BlackListRepository, BlackListRepository>();
            //services.AddTransient<DeviceGroupRepository, DeviceGroupRepository>();
            services.AddTransient<FaceTemplateRepository, FaceTemplateRepository>();
            //services.AddTransient<FingerTemplateRepository, FingerTemplateRepository>();
            services.AddSingleton<GenericCodeMappingRepository, GenericCodeMappingRepository>();
            services.AddTransient<LogRepository, LogRepository>();
            //services.AddSingleton<LookupRepository, LookupRepository>();
            //services.AddTransient<PlateDetectionRepository, PlateDetectionRepository>();
            services.AddTransient<SettingRepository, SettingRepository>();
            //services.AddTransient<TaskRepository, TaskRepository>();
            //services.AddTransient<TimeZoneRepository, TimeZoneRepository>();
            //services.AddTransient<UserCardRepository, UserCardRepository>();
            //services.AddTransient<UserGroupRepository, UserGroupRepository>();
            //services.AddTransient<UserRepository, UserRepository>();
            services.AddSingleton<UserRepository, UserRepository>();
            services.AddTransient<DeviceRepository, DeviceRepository>();
            services.AddTransient<PlateDetectionRepository, PlateDetectionRepository>();
            services.AddTransient<AccessGroupRepository, AccessGroupRepository>();
            services.AddTransient<AdminDeviceRepository, AdminDeviceRepository>();
            services.AddTransient<BlackListRepository, BlackListRepository>();
            services.AddTransient<DeviceGroupRepository, DeviceGroupRepository>();
            services.AddTransient<FingerTemplateRepository, FingerTemplateRepository>();
            //services.AddTransient<Repository.API.v2.LogRepository, Repository.API.v2.LogRepository>();
            services.AddTransient<LookupRepository, LookupRepository>();
            services.AddTransient<GenericCodeMappingRepository, GenericCodeMappingRepository>();
            services.AddTransient<LogRepository, LogRepository>();
            services.AddTransient<LookupRepository, LookupRepository>();
            services.AddTransient<PlateDetectionRepository, PlateDetectionRepository>();
            services.AddTransient<SettingRepository, SettingRepository>();
            services.AddTransient<TaskRepository, TaskRepository>();
            services.AddTransient<TimeZoneRepository, TimeZoneRepository>();
            services.AddTransient<UserCardRepository, UserCardRepository>();
            services.AddTransient<UserGroupRepository, UserGroupRepository>();
            //services.AddTransient<Repository.API.v1.DeviceRepository, Repository.API.v1.DeviceRepository>();


            //services.AddTransient<FoodService, FoodService>();
            //services.AddTransient<MealService, MealService>();
            //services.AddTransient<ReservationService, ReservationService>();
            //services.AddTransient<RestaurantService, RestaurantService>();
            //services.AddTransient<ServeLogService, ServeLogService>();

            services.AddTransient<AccessGroupService, AccessGroupService>();
            services.AddTransient<AdminDeviceService, AdminDeviceService>();
            services.AddTransient<BlackListService, BlackListService>();
            services.AddTransient<DeviceGroupService, DeviceGroupService>();
            services.AddTransient<FingerTemplateService, FingerTemplateService>();
            services.AddTransient<GenericCodeMappingService, GenericCodeMappingService>();
            services.AddTransient<LogService, LogService>();
            services.AddTransient<LookupService, LookupService>();
            services.AddTransient<PlateDetectionService, PlateDetectionService>();
            services.AddTransient<TaskService, TaskService>();
            services.AddTransient<TimeZoneService, TimeZoneService>();
            services.AddTransient<UserCardService, UserCardService>();
            services.AddTransient<UserGroupService, UserGroupService>();
            services.AddTransient<UserService, UserService>();
            services.AddTransient<GenericCodeMappingService, GenericCodeMappingService>();
            services.AddTransient<SettingService, SettingService>();
            services.AddTransient<LogService, LogService>();
            services.AddTransient<DeviceService, DeviceService>();

            services.AddTransient<FaceTemplateService, FaceTemplateService>();
            services.AddTransient<Service.Api.v2.SettingService, Service.Api.v2.SettingService>();
            services.AddTransient<Service.Api.v2.GenericCodeMappingService, Service.Api.v2.GenericCodeMappingService>();
            services.AddTransient<Service.Api.v2.LogService, Service.Api.v2.LogService>();
            services.AddTransient<Service.Api.v2.DeviceService, Service.Api.v2.DeviceService>();
            services.AddSingleton<Service.Api.v2.UserService, Service.Api.v2.UserService>();
            services.AddTransient<Service.Api.v2.AccessGroupService, Service.Api.v2.AccessGroupService>();
            services.AddTransient<Service.Api.v2.AdminDeviceService, Service.Api.v2.AdminDeviceService>();
            services.AddTransient<Service.Api.v2.BlackListService, Service.Api.v2.BlackListService>();
            services.AddTransient<Service.Api.v2.DeviceGroupService, Service.Api.v2.DeviceGroupService>();
            services.AddTransient<Service.Api.v2.FingerTemplateService, Service.Api.v2.FingerTemplateService>();
            services.AddTransient<Service.Api.v2.LookupService, Service.Api.v2.LookupService>();
            services.AddTransient<Service.Api.v2.PlateDetectionService, Service.Api.v2.PlateDetectionService>();
            services.AddTransient<Service.Api.v2.TaskService, Service.Api.v2.TaskService>();
            services.AddTransient<Service.Api.v2.TimeZoneService, Service.Api.v2.TimeZoneService>();
            services.AddTransient<Service.Api.v2.UserCardService, Service.Api.v2.UserCardService>();
            services.AddTransient<Service.Api.v2.UserGroupService, Service.Api.v2.UserGroupService>();
            //services.AddTransient<Service.API.v1.DeviceService, Service.API.v1.DeviceService>();
            services.AddSingleton<TokenGenerator, TokenGenerator>();
        }

        public void ConfigureConstantValues(IServiceCollection services)
        {
            var user = new User { Id = 0 };
            var tokenGenerator = new TokenGenerator(BiovationConfiguration);
            tokenGenerator.GenerateToken(user);

            var serviceCollection = new ServiceCollection();

            var restClient = (RestClient)new RestClient(BiovationConfiguration.BiovationServerUri).UseSerializer(() => new RestRequestJsonSerializer());

            serviceCollection.AddSingleton(restClient);

            serviceCollection.AddTransient<LookupRepository, LookupRepository>();
            serviceCollection.AddTransient<LookupService, LookupService>();
            serviceCollection.AddTransient<GenericCodeMappingRepository, GenericCodeMappingRepository>();
            serviceCollection.AddTransient<GenericCodeMappingService, GenericCodeMappingService>();

            //serviceCollection.AddTransient<Lookups, Lookups>();
            //serviceCollection.AddTransient<GenericCodeMappings, GenericCodeMappings>();


            var serviceProvider = serviceCollection.BuildServiceProvider();

            var lookupService = serviceProvider.GetService<LookupService>();

            var taskStatusesQuery = lookupService.GetLookups(lookupCategoryId: 1);
            var taskTypesQuery = lookupService.GetLookups(lookupCategoryId: 2);
            var taskItemTypesQuery = lookupService.GetLookups(lookupCategoryId: 3);
            var taskPrioritiesQuery = lookupService.GetLookups(lookupCategoryId: 4);
            var fingerIndexNamesQuery = lookupService.GetLookups(lookupCategoryId: 5);
            var deviceBrandsQuery = lookupService.GetLookups(lookupCategoryId: 6);
            var logEventsQuery = lookupService.GetLookups(lookupCategoryId: 7);
            var logSubEventsQuery = lookupService.GetLookups(lookupCategoryId: 8);
            var fingerTemplateTypeQuery = lookupService.GetLookups(lookupCategoryId: 9);
            var faceTemplateTypeQuery = lookupService.GetLookups(lookupCategoryId: 10);
            var matchingTypeQuery = lookupService.GetLookups(lookupCategoryId: 11);

            var lookups = new Lookups
            {
                TaskStatuses = taskStatusesQuery.Result,
                TaskTypes = taskTypesQuery.Result,
                TaskItemTypes = taskItemTypesQuery.Result,
                TaskPriorities = taskPrioritiesQuery.Result,
                FingerIndexNames = fingerIndexNamesQuery.Result,
                DeviceBrands = deviceBrandsQuery.Result,
                LogSubEvents = logSubEventsQuery.Result,
                FingerTemplateType = fingerTemplateTypeQuery.Result,
                FaceTemplateType = faceTemplateTypeQuery.Result,
                LogEvents = logEventsQuery.Result,
                MatchingTypes = matchingTypeQuery.Result
            };

            var genericCodeMappingService = serviceProvider.GetService<GenericCodeMappingService>();

            var logEventMappingsQuery = genericCodeMappingService.GetGenericCodeMappings(1);
            var logSubEventMappingsQuery = genericCodeMappingService.GetGenericCodeMappings(2);
            var fingerTemplateTypeMappingsQuery = genericCodeMappingService.GetGenericCodeMappings(9);
            var matchingTypeMappingsQuery = genericCodeMappingService.GetGenericCodeMappings(15);


            var genericCodeMappings = new GenericCodeMappings
            {
                LogEventMappings = logEventMappingsQuery.Result?.Data?.Data,
                LogSubEventMappings = logSubEventMappingsQuery.Result?.Data?.Data,
                FingerTemplateTypeMappings = fingerTemplateTypeMappingsQuery.Result?.Data?.Data,
                MatchingTypeMappings = matchingTypeMappingsQuery.Result?.Data?.Data
            };


            services.AddSingleton(lookups);
            services.AddSingleton(genericCodeMappings);
            //Constant values
            services.AddSingleton<LogEvents, LogEvents>();
            services.AddSingleton<TaskTypes, TaskTypes>();
            services.AddSingleton<LogSubEvents, LogSubEvents>();
            services.AddSingleton<DeviceBrands, DeviceBrands>();
            services.AddSingleton<TaskStatuses, TaskStatuses>();
            services.AddSingleton<MatchingTypes, MatchingTypes>();
            services.AddSingleton<TaskItemTypes, TaskItemTypes>();
            services.AddSingleton<TaskPriorities, TaskPriorities>();
            services.AddSingleton<FingerIndexNames, FingerIndexNames>();
            services.AddSingleton<FaceTemplateTypes, FaceTemplateTypes>();
            services.AddSingleton<FingerTemplateTypes, FingerTemplateTypes>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory, IApiVersionDescriptionProvider provider)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            //app.UseHttpsRedirection();


            //app.UseAuthorization();
            //app.UseAuthentication();
            //app.UseApiVersioning();
            app.UseMiddleware<JwtMiddleware>();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHealthChecks("/health/ready", new HealthCheckOptions()
                {
                    Predicate = check => check.Tags.Contains("ready"),
                });

                endpoints.MapHealthChecks("/health/live", new HealthCheckOptions());
            });

            loggerFactory.AddSerilog();
            app.UseSerilogRequestLogging();
            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                foreach (var description in provider.ApiVersionDescriptions)
                {
                    c.SwaggerEndpoint($"{description.GroupName}/swagger.json", $"Biovation API V{description.GroupName.ToUpperInvariant()}");
                }
            });




            //app.MapWhen(context =>
            //{
            //    {
            //        var url = context.Request.Path.Value;
            //        if (url.Contains("Biovation/api"))
            //        {
            //            if (!(Regex.IsMatch(url, @"api/[v][1 - 9] /")))
            //            {
            //                var splittedUrl = url.Split("/");
            //                var indexUrl = splittedUrl.Select((value, index) => new {value, index})
            //                    .First(urlSegment => urlSegment.value == "api").index + 1;
            //                //splittedUrl.Insert(new[] {"v1"}, indexUrl);/////whyyyyyyyyyy it doesn't work???
            //                var listSplittedUrl = splittedUrl.ToList();
            //                listSplittedUrl.Insert(indexUrl, "v1");
            //                context.Request.Path = string.Join("/", listSplittedUrl);
            //            }
            //        }

            //        return false;
            //    },
            //    {
            //        req => req.Run(
            //            ctx => Task.Run(() => ctx.Response.Redirect("/setup"))
            //    }
            //    //await next();
            //});

            //app.Use(async (context, next) =>
            //{
            //    var url = context.Request.Path.Value;
            //    if (url.Contains("Biovation/api"))
            //    {
            //        if (!(Regex.IsMatch(url, @"api/[v][1 - 9] /")))
            //        {
            //            var splittedUrl = url.Split("/");
            //            var indexUrl = splittedUrl.Select((value, index) => new {value, index})
            //                .First(urlSegment => urlSegment.value == "api").index + 1;
            //            //splittedUrl.Insert(new[] {"v1"}, indexUrl);/////whyyyyyyyyyy it doesn't work???
            //            var listSplittedUrl = splittedUrl.ToList();
            //            listSplittedUrl.Insert(indexUrl, "v1");
            //            context.Request.Path = string.Join("/", listSplittedUrl);
            //        }
            //    }

            //    //await next();
            //});

        }
    }
}
