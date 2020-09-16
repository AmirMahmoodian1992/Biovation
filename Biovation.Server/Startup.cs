using Biovation.CommonClasses.Manager;
using Biovation.Constants;
using Biovation.Repository.Api.v2;
using Biovation.Service.Api.v1;
using DataAccessLayerCore;
using DataAccessLayerCore.Domain;
using DataAccessLayerCore.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RestSharp;

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
            services.AddApiVersioning(config =>
            {
                config.DefaultApiVersion = new ApiVersion(1, 0);
                config.AssumeDefaultVersionWhenUnspecified = true;
                config.ReportApiVersions = true;  //the clients of the API know all supported versions
                config.ApiVersionReader = new HeaderApiVersionReader("api-version"); //if Pass version information in the HTTP headers
            });

            services.AddSwaggerGen();

            services.AddSingleton(BiovationConfiguration);
            services.AddSingleton(BiovationConfiguration.Configuration);

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
            var restClient = (RestClient)new RestClient($"http://localhost:{BiovationConfigurationManager.BiovationWebServerPort}/biovation/api").UseSerializer(() => new RestRequestJsonSerializer());

            services.AddSingleton(restClient);
            services.AddSingleton(connectionInfo);
            services.AddSingleton<IConnectionFactory, DbConnectionFactory>();

            services.AddSingleton<GenericRepository, GenericRepository>();

            //services.AddScoped<FoodRepository, FoodRepository>();
            //services.AddScoped<MealRepository, MealRepository>();
            //services.AddScoped<ReservationRepository, ReservationRepository>();
            //services.AddScoped<RestaurantRepository, RestaurantRepository>();
            //services.AddScoped<ServeLogRepository, ServeLogRepository>();

            //services.AddScoped<AccessGroupRepository, AccessGroupRepository>();
            //services.AddScoped<AdminDeviceRepository, AdminDeviceRepository>();
            //services.AddScoped<BlackListRepository, BlackListRepository>();
            //services.AddScoped<DeviceGroupRepository, DeviceGroupRepository>();
            services.AddScoped<FaceTemplateRepository, FaceTemplateRepository>();
            //services.AddScoped<FingerTemplateRepository, FingerTemplateRepository>();
            services.AddSingleton<GenericCodeMappingRepository, GenericCodeMappingRepository>();
            services.AddScoped<LogRepository, LogRepository>();
            //services.AddSingleton<LookupRepository, LookupRepository>();
            //services.AddScoped<PlateDetectionRepository, PlateDetectionRepository>();
            services.AddScoped<SettingRepository, SettingRepository>();
            //services.AddScoped<TaskRepository, TaskRepository>();
            //services.AddScoped<TimeZoneRepository, TimeZoneRepository>();
            //services.AddScoped<UserCardRepository, UserCardRepository>();
            //services.AddScoped<UserGroupRepository, UserGroupRepository>();
            //services.AddScoped<UserRepository, UserRepository>();
            services.AddScoped<UserRepository, UserRepository>();
            services.AddScoped<DeviceRepository, DeviceRepository>();
            services.AddScoped<PlateDetectionRepository, PlateDetectionRepository>();
            services.AddScoped<AccessGroupRepository, AccessGroupRepository>();
            services.AddScoped<AdminDeviceRepository, AdminDeviceRepository>();
            services.AddScoped<BlackListRepository, BlackListRepository>();
            services.AddScoped<DeviceGroupRepository, DeviceGroupRepository>();
            services.AddScoped<FingerTemplateRepository, FingerTemplateRepository>();
            //services.AddScoped<Repository.API.v2.LogRepository, Repository.API.v2.LogRepository>();
            services.AddScoped<LookupRepository, LookupRepository>();
            services.AddScoped<TaskRepository, TaskRepository>();
            services.AddScoped<TimeZoneRepository, TimeZoneRepository>();
            services.AddScoped<UserCardRepository, UserCardRepository>();
            services.AddScoped<UserGroupRepository, UserGroupRepository>();
            //services.AddScoped<Repository.API.v1.DeviceRepository, Repository.API.v1.DeviceRepository>();






            //services.AddScoped<FoodService, FoodService>();
            //services.AddScoped<MealService, MealService>();
            //services.AddScoped<ReservationService, ReservationService>();
            //services.AddScoped<RestaurantService, RestaurantService>();
            //services.AddScoped<ServeLogService, ServeLogService>();

            services.AddScoped<AccessGroupService, AccessGroupService>();
            services.AddScoped<AdminDeviceService, AdminDeviceService>();
            services.AddScoped<BlackListService, BlackListService>();
            services.AddScoped<DeviceGroupService, DeviceGroupService>();
            services.AddScoped<FingerTemplateService, FingerTemplateService>();
            services.AddScoped<LookupService, LookupService>();
            services.AddScoped<PlateDetectionService, PlateDetectionService>();
            services.AddScoped<TaskService, TaskService>();
            services.AddScoped<TimeZoneService, TimeZoneService>();
            services.AddScoped<UserCardService, UserCardService>();
            services.AddScoped<UserGroupService, UserGroupService>();
            services.AddScoped<UserService, UserService>();
            services.AddScoped<GenericCodeMappingService, GenericCodeMappingService>();
            services.AddScoped<SettingService, SettingService>();
            services.AddScoped<LogService, LogService>();
            services.AddScoped<DeviceService,DeviceService>();

            services.AddScoped<FaceTemplateService,FaceTemplateService>();
            services.AddScoped<Service.Api.v2.SettingService, Service.Api.v2.SettingService>();
            services.AddScoped<Service.Api.v2.GenericCodeMappingService, Service.Api.v2.GenericCodeMappingService>();
            services.AddScoped<Service.Api.v2.LogService, Service.Api.v2.LogService>();
            services.AddScoped<Service.Api.v2.DeviceService, Service.Api.v2.DeviceService>();
            services.AddScoped<Service.Api.v2.UserService, Service.Api.v2.UserService>();
            services.AddScoped<Service.Api.v2.AccessGroupService, Service.Api.v2.AccessGroupService>();
            services.AddScoped<Service.Api.v2.AdminDeviceService, Service.Api.v2.AdminDeviceService>();
            services.AddScoped<Service.Api.v2.BlackListService, Service.Api.v2.BlackListService>();
            services.AddScoped<Service.Api.v2.DeviceGroupService, Service.Api.v2.DeviceGroupService>();
            services.AddScoped<Service.Api.v2.FingerTemplateService, Service.Api.v2.FingerTemplateService>();
            services.AddScoped<Service.Api.v2.LookupService, Service.Api.v2.LookupService>();
            services.AddScoped<Service.Api.v2.PlateDetectionService, Service.Api.v2.PlateDetectionService>();
            services.AddScoped<Service.Api.v2.TaskService, Service.Api.v2.TaskService>();
            services.AddScoped<Service.Api.v2.TimeZoneService, Service.Api.v2.TimeZoneService>();
            services.AddScoped<Service.Api.v2.UserCardService, Service.Api.v2.UserCardService>();
            services.AddScoped<Service.Api.v2.UserGroupService, Service.Api.v2.UserGroupService>();
            //services.AddScoped<Service.API.v1.DeviceService, Service.API.v1.DeviceService>();

            services.AddSingleton<Lookups, Lookups>();
            services.AddSingleton<GenericCodeMappings, GenericCodeMappings>();

            //Constant values
            services.AddSingleton<DeviceBrands, DeviceBrands>();
            services.AddSingleton<FaceTemplateTypes, FaceTemplateTypes>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
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

            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Biovation API");
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
