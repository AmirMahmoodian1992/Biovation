using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using Biovation.CommonClasses.Manager;
using Biovation.Constants;
using Biovation.Repository.Api.v2;
using Biovation.Service.Api.v1;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RestSharp;
using Biovation.Brands.PFK.Devices;
using Biovation.Brands.PFK.Managers;
using System.Web.Mvc;
using Biovation.CommonClasses;
using Biovation.Repository.Api.v2.RelayController;
using Biovation.Service.Api.v2.RelayController;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using RimDev.AspNet.Diagnostics.HealthChecks;
using Serilog;
#if NET472
using System;
using Owin;
using System.Web.Http;

#elif NETCORE31
using App.Metrics;
using App.Metrics.Extensions.Configuration;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
#endif

namespace Biovation.Brands.PFK
{
    public class Startup
    {
        public IConfiguration ApplicationConfiguration { get; set; }
        public BiovationConfigurationManager BiovationConfiguration { get; set; }
        public readonly Dictionary<uint, Camera> OnlineDevices = new Dictionary<uint, Camera>();

#if NET472
        public void Configuration(IAppBuilder appBuilder)
        {
            // Configure Web API for self-host. 
            var config = new HttpConfiguration();
            config.Routes.MapHttpRoute(
                "DefaultApi",
                "biovation/api/{controller}/{action}/{id}",
                new { id = RouteParameter.Optional }
            );

            appBuilder.Use((context, next) =>
            {
                return next().ContinueWith(_ =>
                {
                    Logger.Log(
                        $"New Request Started on : {context.Request.Uri}, Method : {context.Request.Method} , Path : {context.Request.Path}");
                });
            });

            appBuilder.UseHealthChecks("/biovation/api/health", new List<IHealthCheck>());

            var builder = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile("appsettings.development.json", true, true);

            ApplicationConfiguration = builder.Build();
            BiovationConfiguration = new BiovationConfigurationManager(ApplicationConfiguration);

            Log.Logger = new LoggerConfiguration().ReadFrom.Configuration(ApplicationConfiguration)
                .Enrich.With(new ThreadIdEnricher())
                .Enrich.WithProperty("Version", Assembly.GetExecutingAssembly().GetName().Version)
                .CreateLogger();

            var services = new ServiceCollection();
            ConfigureServices(services);

            var resolver = new DefaultDependencyResolver(services.BuildServiceProvider());
            config.DependencyResolver = resolver;
            DependencyResolver.SetResolver(resolver);

            appBuilder.UseWebApi(config);
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddHealthChecks();

            services.AddSingleton(BiovationConfiguration);
            services.AddSingleton(BiovationConfiguration.Configuration);

            ConfigureRepositoriesServices(services);
            ConfigureConstantValues(services);

            services.AddControllersAsServices(typeof(Startup).Assembly.GetExportedTypes()
                .Where(t => !t.IsAbstract && !t.IsGenericTypeDefinition)
                .Where(t => typeof(IController).IsAssignableFrom(t)
                            || t.Name.EndsWith("Controller", StringComparison.OrdinalIgnoreCase)));

            ConfigurePfkServices(services);
        }
#endif

#if NETCORE31

        public BiovationConfigurationManager BiovationConfiguration { get; set; }
        public IConfiguration Configuration { get; }

        public readonly Dictionary<uint, Camera> OnlineDevices = new Dictionary<uint, Camera>();

        public Startup(IConfiguration configuration, IHostEnvironment environment)
        {
            Configuration = configuration;

            Log.Logger = new LoggerConfiguration().ReadFrom.Configuration(configuration)
                .Enrich.With(new ThreadIdEnricher())
                .Enrich.WithProperty("Version", Assembly.GetExecutingAssembly().GetName().Version)
                .CreateLogger();

            BiovationConfiguration = new BiovationConfigurationManager(configuration);

            var builder = new ConfigurationBuilder()
                .SetBasePath(environment.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{environment.EnvironmentName}.json", true, true)
                .AddEnvironmentVariables();

            var metrics = new MetricsBuilder()
                .Configuration.ReadFrom(configuration);

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

            services.AddHealthChecks();

        services.AddSingleton(BiovationConfiguration);
            services.AddSingleton(BiovationConfiguration.Configuration);

            ConfigureRepositoriesServices(services);
            ConfigureConstantValues(services);
            ConfigurePfkServices(services);

            services.AddHostedService<PingCollectorHostedService>();
            services.AddHostedService<BroadcastMetricsHostedService>();
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
            app.UseMiddleware<JwtMiddleware>();

            loggerFactory.AddSerilog();
            app.UseSerilogRequestLogging();

            app.UseHealthChecks("/biovation/api/health");

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
#endif

        private void ConfigureRepositoriesServices(IServiceCollection services)
        {
            var restClient = (RestClient)new RestClient(BiovationConfiguration.BiovationServerUri).UseSerializer(() => new RestRequestJsonSerializer());
            services.AddSingleton(restClient);

            services.AddSingleton<AccessGroupService, AccessGroupService>();
            services.AddSingleton<AdminDeviceService, AdminDeviceService>();
            services.AddSingleton<BlackListService, BlackListService>();
            services.AddSingleton<DeviceGroupService, DeviceGroupService>();
            services.AddSingleton<DeviceService, DeviceService>();
            services.AddSingleton<Service.Api.v2.DeviceService, Service.Api.v2.DeviceService>();
            services.AddSingleton<FaceTemplateService, FaceTemplateService>();
            services.AddSingleton<FingerTemplateService, FingerTemplateService>();
            services.AddSingleton<GenericCodeMappingService, GenericCodeMappingService>();
            services.AddSingleton<PlateDetectionService, PlateDetectionService>();
            services.AddSingleton<Service.Api.v2.PlateDetectionService, Service.Api.v2.PlateDetectionService>();
            services.AddSingleton<LogService, LogService>();
            services.AddSingleton<LookupService, LookupService>();
            services.AddSingleton<SettingService, SettingService>();
            services.AddSingleton<TaskService, TaskService>();
            services.AddSingleton<TimeZoneService, TimeZoneService>();
            services.AddSingleton<UserCardService, UserCardService>();
            services.AddSingleton<UserGroupService, UserGroupService>();
            services.AddSingleton<UserService, UserService>();
            services.AddSingleton<CameraService, CameraService>();

            services.AddSingleton<AccessGroupRepository, AccessGroupRepository>();
            services.AddSingleton<AdminDeviceRepository, AdminDeviceRepository>();
            services.AddSingleton<BlackListRepository, BlackListRepository>();
            services.AddSingleton<DeviceGroupRepository, DeviceGroupRepository>();
            services.AddSingleton<DeviceRepository, DeviceRepository>();
            services.AddSingleton<FaceTemplateRepository, FaceTemplateRepository>();
            services.AddSingleton<FingerTemplateRepository, FingerTemplateRepository>();
            services.AddSingleton<GenericCodeMappingRepository, GenericCodeMappingRepository>();
            services.AddSingleton<PlateDetectionRepository, PlateDetectionRepository>();
            services.AddSingleton<LogRepository, LogRepository>();
            services.AddSingleton<LookupRepository, LookupRepository>();
            services.AddSingleton<SettingRepository, SettingRepository>();
            services.AddSingleton<TaskRepository, TaskRepository>();
            services.AddSingleton<TimeZoneRepository, TimeZoneRepository>();
            services.AddSingleton<UserCardRepository, UserCardRepository>();
            services.AddSingleton<UserGroupRepository, UserGroupRepository>();
            services.AddSingleton<UserRepository, UserRepository>();
            services.AddSingleton<CameraRepository, CameraRepository>();

            services.AddSingleton<Lookups, Lookups>();
            services.AddSingleton<GenericCodeMappings, GenericCodeMappings>();
        }

        public void ConfigureConstantValues(IServiceCollection services)
        {
            var serviceCollection = new ServiceCollection();
            var restClient = (RestClient)new RestClient(BiovationConfiguration.BiovationServerUri).UseSerializer(() => new RestRequestJsonSerializer());

            serviceCollection.AddSingleton(restClient);

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
            var cameraProtocolQuery = lookupService.GetLookups(lookupCategoryId: 12);
            var resolutionQuery = lookupService.GetLookups(lookupCategoryId: 13);
            var cameraBrandQuery = lookupService.GetLookups(lookupCategoryId: 14);


            var genericCodeMappingService = serviceProvider.GetService<GenericCodeMappingService>();

            var logEventMappingsQuery = genericCodeMappingService.GetGenericCodeMappings(1);
            var logSubEventMappingsQuery = genericCodeMappingService.GetGenericCodeMappings(2);
            var fingerTemplateTypeMappingsQuery = genericCodeMappingService.GetGenericCodeMappings(9);
            var matchingTypeMappingsQuery = genericCodeMappingService.GetGenericCodeMappings(15);
            var faceTemplateTypeMappingsQuery = genericCodeMappingService.GetGenericCodeMappings(14);
            var fingerIndexMappingsQuery = genericCodeMappingService.GetGenericCodeMappings(10);

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
                MatchingTypes = matchingTypeQuery.Result,
                CameraProtocol = cameraProtocolQuery.Result,
                CameraBrand = cameraBrandQuery.Result,
                Resolution = resolutionQuery.Result
            };

            var genericCodeMappings = new GenericCodeMappings
            {
                LogEventMappings = logEventMappingsQuery.Result?.Data?.Data,
                LogSubEventMappings = logSubEventMappingsQuery.Result?.Data?.Data,
                FingerTemplateTypeMappings = fingerTemplateTypeMappingsQuery.Result?.Data?.Data,
                FaceTemplateTypeMappings = faceTemplateTypeMappingsQuery.Result?.Data?.Data,
                FingerIndexMappings = fingerIndexMappingsQuery.Result?.Data?.Data,
                MatchingTypeMappings = matchingTypeMappingsQuery.Result?.Data?.Data
            };

            if (lookups.CameraBrand is null || lookups.CameraProtocol is null || lookups.DeviceBrands is null ||
                lookups.FingerIndexNames is null || lookups.FingerTemplateType is null || lookups.FaceTemplateType is null ||
                lookups.LogEvents is null || lookups.LogSubEvents is null || lookups.MatchingTypes is null || 
                lookups.Resolution is null || lookups.TaskItemTypes is null || lookups.TaskPriorities is null || 
                lookups.TaskStatuses is null || lookups.TaskTypes is null ||
                genericCodeMappings.FaceTemplateTypeMappings is null || genericCodeMappings.FingerIndexMappings is null ||
                genericCodeMappings.FingerTemplateTypeMappings is null || genericCodeMappings.LogEventMappings is null ||
                genericCodeMappings.LogSubEventMappings is null || genericCodeMappings.MatchingTypeMappings is null)
            {
                Logger.Log("The prerequisite services are not run or some configs may be missing.", logType: LogType.Warning);
                Logger.Log("Closing the app in 10 seconds.", logType: LogType.Warning);
                Thread.Sleep(TimeSpan.FromSeconds(10));
                Environment.Exit(0);
            }

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

        private void ConfigurePfkServices(IServiceCollection services)
        {
            services.AddSingleton(OnlineDevices);

            services.AddSingleton<DeviceFactory, DeviceFactory>();
            services.AddSingleton<PfkServer, PfkServer>();

            var serviceProvider = services.BuildServiceProvider();
            var pfkServer = serviceProvider.GetService<PfkServer>();

            pfkServer.StartServer();
        }
    }
}
