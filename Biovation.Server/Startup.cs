using Biovation.CommonClasses.Manager;
using Biovation.Constants;
using Biovation.Repository;
using Biovation.Repository.RestaurantRepositories;
using Biovation.Service;
using Biovation.Service.RestaurantServices;
using DataAccessLayerCore;
using DataAccessLayerCore.Domain;
using DataAccessLayerCore.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Serilog;

namespace Biovation.Server
{
    public class Startup
    {
        public BiovationConfigurationManager BiovationConfiguration { get; set; }
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration, IWebHostEnvironment environment)
        {
            Configuration = configuration;

            //Log.Logger = new LoggerConfiguration().ReadFrom.Configuration(configuration).CreateLogger();
            BiovationConfiguration = new BiovationConfigurationManager(configuration);

            var builder = new ConfigurationBuilder()
                .SetBasePath(environment.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{environment.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            //var constraintResolver = new DefaultInlineConstraintResolver()
            //{
            //    ConstraintMap =
            //    {
            //        ["apiVersion"] = typeof( ApiVersionRouteConstraint )
            //    }
            //};

            services.AddApiVersioning(config =>
            {
                config.DefaultApiVersion = new ApiVersion(1, 0);
                config.AssumeDefaultVersionWhenUnspecified = true;
                config.ReportApiVersions = true;  //the clients of the API know all supported versions
                //config.ApiVersionReader = new HeaderApiVersionReader("api-version"); //if Pass version information in the HTTP headers
                //config.Conventions.Add(new VersionByNamespaceConvention());
                config.ApiVersionReader = new UrlSegmentApiVersionReader();
                config.ApiVersionSelector = new CurrentImplementationApiVersionSelector(config);
            });

            services.AddSwaggerGen(options =>
            {
                options.CustomSchemaIds(type => type.ToString());

                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "Biovation Swagger",
                    Description = "Swagger for API version 1,2"
                });

            });

            services.AddSingleton(BiovationConfiguration);
            services.AddSingleton(BiovationConfiguration.Configuration);

            ConfigureConstantValues(services);
            ConfigureRepositoriesServices(services);
        }

        private void ConfigureRepositoriesServices(IServiceCollection services)
        {
            var connectionInfo = new DatabaseConnectionInfo
            {
                ProviderName = BiovationConfiguration.ConnectionStringProviderName(),
                WorkstationId = BiovationConfiguration.ConnectionStringWorkstationId(),
                InitialCatalog = BiovationConfiguration.ConnectionStringInitialCatalog(),
                DataSource = BiovationConfiguration.ConnectionStringDataSource(),
                Parameters = BiovationConfiguration.ConnectionStringParameters(),
                UserId = BiovationConfiguration.ConnectionStringUsername(),
                Password = BiovationConfiguration.ConnectionStringPassword()
            };

            services.AddSingleton(connectionInfo);
            services.AddSingleton<IConnectionFactory, DbConnectionFactory>();

            services.AddSingleton<GenericRepository, GenericRepository>();

            services.AddScoped<FoodRepository, FoodRepository>();
            services.AddScoped<MealRepository, MealRepository>();
            services.AddScoped<ReservationRepository, ReservationRepository>();
            services.AddScoped<RestaurantRepository, RestaurantRepository>();
            services.AddScoped<ServeLogRepository, ServeLogRepository>();
            services.AddScoped<AccessGroupRepository, AccessGroupRepository>();
            services.AddScoped<AdminDeviceRepository, AdminDeviceRepository>();
            services.AddScoped<BlackListRepository, BlackListRepository>();
            services.AddScoped<DeviceGroupRepository, DeviceGroupRepository>();
            services.AddScoped<DeviceRepository, DeviceRepository>();
            services.AddScoped<FaceTemplateRepository, FaceTemplateRepository>();
            services.AddScoped<FingerTemplateRepository, FingerTemplateRepository>();
            services.AddScoped<GenericCodeMappingRepository, GenericCodeMappingRepository>();
            services.AddScoped<LogRepository, LogRepository>();
            services.AddScoped<LookupRepository, LookupRepository>();
            services.AddScoped<PlateDetectionRepository, PlateDetectionRepository>();
            services.AddScoped<SettingRepository, SettingRepository>();
            services.AddScoped<TaskRepository, TaskRepository>();
            services.AddScoped<TimeZoneRepository, TimeZoneRepository>();
            services.AddScoped<UserCardRepository, UserCardRepository>();
            services.AddScoped<UserGroupRepository, UserGroupRepository>();
            services.AddScoped<UserRepository, UserRepository>();

            services.AddScoped<FoodService, FoodService>();
            services.AddScoped<MealService, MealService>();
            services.AddScoped<ReservationService, ReservationService>();
            services.AddScoped<RestaurantService, RestaurantService>();
            services.AddScoped<ServeLogService, ServeLogService>();
            services.AddScoped<AccessGroupService, AccessGroupService>();
            services.AddScoped<AdminDeviceService, AdminDeviceService>();
            services.AddScoped<BlackListService, BlackListService>();
            services.AddScoped<DeviceGroupService, DeviceGroupService>();
            services.AddScoped<DeviceService, DeviceService>();
            services.AddScoped<FaceTemplateService, FaceTemplateService>();
            services.AddScoped<FingerTemplateService, FingerTemplateService>();
            services.AddScoped<GenericCodeMappingService, GenericCodeMappingService>();
            services.AddScoped<LogService, LogService>();
            services.AddScoped<LookupService, LookupService>();
            services.AddScoped<PlateDetectionService, PlateDetectionService>();
            services.AddScoped<SettingService, SettingService>();
            services.AddScoped<TaskService, TaskService>();
            services.AddScoped<TimeZoneService, TimeZoneService>();
            services.AddScoped<UserCardService, UserCardService>();
            services.AddScoped<UserGroupService, UserGroupService>();
            services.AddScoped<UserService, UserService>();
        }

        public void ConfigureConstantValues(IServiceCollection services)
        {
            var serviceCollection = new ServiceCollection();
            var connectionInfo = new DatabaseConnectionInfo
            {
                ProviderName = BiovationConfiguration.ConnectionStringProviderName(),
                WorkstationId = BiovationConfiguration.ConnectionStringWorkstationId(),
                InitialCatalog = BiovationConfiguration.ConnectionStringInitialCatalog(),
                DataSource = BiovationConfiguration.ConnectionStringDataSource(),
                Parameters = BiovationConfiguration.ConnectionStringParameters(),
                UserId = BiovationConfiguration.ConnectionStringUsername(),
                Password = BiovationConfiguration.ConnectionStringPassword()
            };

            serviceCollection.AddSingleton(connectionInfo);
            serviceCollection.AddSingleton<IConnectionFactory, DbConnectionFactory>();

            serviceCollection.AddSingleton<GenericRepository, GenericRepository>();
            serviceCollection.AddScoped<LookupRepository, LookupRepository>();
            serviceCollection.AddScoped<LookupService, LookupService>();
            serviceCollection.AddScoped<GenericCodeMappingRepository, GenericCodeMappingRepository>();
            serviceCollection.AddScoped<GenericCodeMappingService, GenericCodeMappingService>();


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


            Lookups.TaskStatuses = taskStatusesQuery.Result;
            Lookups.TaskTypes = taskTypesQuery.Result;
            Lookups.TaskItemTypes = taskItemTypesQuery.Result;
            Lookups.TaskPriorities = taskPrioritiesQuery.Result;
            Lookups.FingerIndexNames = fingerIndexNamesQuery.Result;
            Lookups.DeviceBrands = deviceBrandsQuery.Result;
            Lookups.LogSubEvents = logSubEventsQuery.Result;
            Lookups.FingerTemplateType = fingerTemplateTypeQuery.Result;
            Lookups.FaceTemplateType = faceTemplateTypeQuery.Result;
            Lookups.LogEvents = logEventsQuery.Result;
            Lookups.MatchingTypes = matchingTypeQuery.Result;

            //var lookups = new Lookups
            //{
            //    TaskStatuses = taskStatusesQuery.Result,
            //    TaskTypes = taskTypesQuery.Result,
            //    TaskItemTypes = taskItemTypesQuery.Result,
            //    TaskPriorities = taskPrioritiesQuery.Result,
            //    FingerIndexNames = fingerIndexNamesQuery.Result,
            //    DeviceBrands = deviceBrandsQuery.Result,
            //    LogSubEvents = logSubEventsQuery.Result,
            //    FingerTemplateType = fingerTemplateTypeQuery.Result,
            //    FaceTemplateType = faceTemplateTypeQuery.Result,
            //    LogEvents = logEventsQuery.Result,
            //    MatchingTypes = matchingTypeQuery.Result
            //};

            var genericCodeMappingService = serviceProvider.GetService<GenericCodeMappingService>();

            var logEventMappingsQuery = genericCodeMappingService.GetGenericCodeMappings(1);
            var logSubEventMappingsQuery = genericCodeMappingService.GetGenericCodeMappings(2);
            var fingerTemplateTypeMappingsQuery = genericCodeMappingService.GetGenericCodeMappings(9);
            var matchingTypeMappingsQuery = genericCodeMappingService.GetGenericCodeMappings(15);


            var genericCodeMappings = new GenericCodeMappings
            {
                LogEventMappings = logEventMappingsQuery.Result,
                LogSubEventMappings = logSubEventMappingsQuery.Result,
                FingerTemplateTypeMappings = fingerTemplateTypeMappingsQuery.Result,
                MatchingTypeMappings = matchingTypeMappingsQuery.Result
            };

            services.AddSingleton<Lookups, Lookups>();
            services.AddSingleton(genericCodeMappings);
            //Constant values
            services.AddSingleton<DeviceBrands, DeviceBrands>();
            services.AddSingleton<FaceTemplateTypes, FaceTemplateTypes>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            //app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            loggerFactory.AddSerilog();
            app.UseSerilogRequestLogging();
            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
            });

        }
    }
}
