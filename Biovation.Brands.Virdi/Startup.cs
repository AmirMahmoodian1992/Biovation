using App.Metrics;
using App.Metrics.Extensions.Configuration;
using Biovation.Brands.Virdi.Command;
using Biovation.Brands.Virdi.HostedServices;
using Biovation.Brands.Virdi.Manager;
using Biovation.Brands.Virdi.Middleware;
using Biovation.Brands.Virdi.Service;
using Biovation.CommonClasses;
using Biovation.CommonClasses.Interface;
using Biovation.CommonClasses.Manager;
using Biovation.Constants;
using Biovation.Domain;
using Biovation.Repository.Api.v2;
using Biovation.Service.Api.v1;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Ninject;
using RestSharp;
using Serilog;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UCSAPICOMLib;
using UNIONCOMM.SDK.UCBioBSP;

namespace Biovation.Brands.Virdi
{
    public class Startup
    {
        public BiovationConfigurationManager BiovationConfiguration { get; set; }
        public IConfiguration Configuration { get; }

        public UCBioAPI UcBioApi;
        public UCBioAPI.Export UcBioApiExport;
        // UCSAPI
        public UCSAPI UcsApi;
        public readonly Dictionary<uint, DeviceBasicInfo> OnlineDevices = new Dictionary<uint, DeviceBasicInfo>();

        public Startup(IConfiguration configuration, IHostEnvironment environment)
        {
            Configuration = configuration;

            Serilog.Log.Logger = new LoggerConfiguration().ReadFrom.Configuration(configuration)
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

            //services.AddMvcCore().AddMetricsCore();
            services.AddHealthChecks();

            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            services.AddSingleton(Serilog.Log.Logger);
            services.AddSingleton(BiovationConfiguration);
            services.AddSingleton(BiovationConfiguration.Configuration);

            ConfigureRepositoriesServices(services);
            ConfigureConstantValues(services);
            ConfigureVirdiServices(services);

            services.AddHostedService<PingCollectorHostedService>();
            services.AddHostedService<BroadcastMetricsHostedService>();
        }

        private void ConfigureRepositoriesServices(IServiceCollection services)
        {
            var restClient = (RestClient)new RestClient(BiovationConfiguration.BiovationServerUri).UseSerializer(() => new RestRequestJsonSerializer());
            services.AddSingleton(restClient);

            services.AddSingleton<AccessGroupService, AccessGroupService>();
            services.AddSingleton<AdminDeviceService, AdminDeviceService>();
            services.AddSingleton<BlackListService, BlackListService>();
            services.AddSingleton<DeviceGroupService, DeviceGroupService>();
            services.AddSingleton<DeviceService, DeviceService>();
            services.AddSingleton<FaceTemplateService, FaceTemplateService>();
            services.AddSingleton<FingerTemplateService, FingerTemplateService>();
            services.AddSingleton<GenericCodeMappingService, GenericCodeMappingService>();
            services.AddSingleton<LogService, LogService>();
            services.AddSingleton<LookupService, LookupService>();
            services.AddSingleton<SettingService, SettingService>();
            services.AddSingleton<TaskService, TaskService>();
            services.AddSingleton<TimeZoneService, TimeZoneService>();
            services.AddSingleton<UserCardService, UserCardService>();
            services.AddSingleton<UserGroupService, UserGroupService>();
            services.AddSingleton<UserService, UserService>();
            services.AddSingleton<Biovation.Service.Api.v2.UserService, Biovation.Service.Api.v2.UserService>();
            services.AddSingleton<VirdiLogService, VirdiLogService>();
            services.AddSingleton<Biovation.Service.Api.v2.UserService, Biovation.Service.Api.v2.UserService>();


            services.AddSingleton<AccessGroupRepository, AccessGroupRepository>();
            services.AddSingleton<AdminDeviceRepository, AdminDeviceRepository>();
            services.AddSingleton<BlackListRepository, BlackListRepository>();
            services.AddSingleton<DeviceGroupRepository, DeviceGroupRepository>();
            services.AddSingleton<DeviceRepository, DeviceRepository>();
            services.AddSingleton<FaceTemplateRepository, FaceTemplateRepository>();
            services.AddSingleton<FingerTemplateRepository, FingerTemplateRepository>();
            services.AddSingleton<GenericCodeMappingRepository, GenericCodeMappingRepository>();
            services.AddSingleton<LogRepository, LogRepository>();
            services.AddSingleton<LookupRepository, LookupRepository>();
            services.AddSingleton<SettingRepository, SettingRepository>();
            services.AddSingleton<TaskRepository, TaskRepository>();
            services.AddSingleton<TimeZoneRepository, TimeZoneRepository>();
            services.AddSingleton<UserCardRepository, UserCardRepository>();
            services.AddSingleton<UserGroupRepository, UserGroupRepository>();
            services.AddSingleton<UserRepository, UserRepository>();

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


            var genericCodeMappingService = serviceProvider.GetService<GenericCodeMappingService>();

            var logEventMappingsQuery = genericCodeMappingService.GetGenericCodeMappings(1);
            var logSubEventMappingsQuery = genericCodeMappingService.GetGenericCodeMappings(2);
            var fingerTemplateTypeMappingsQuery = genericCodeMappingService.GetGenericCodeMappings(9);
            var matchingTypeMappingsQuery = genericCodeMappingService.GetGenericCodeMappings(15);

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
        private void ConfigureVirdiServices(IServiceCollection services)
        {
            UcsApi = new UCSAPIClass();
            UcBioApi = new UCBioAPI();
            UcBioApiExport = new UCBioAPI.Export(UcBioApi);

            services.AddSingleton(UcsApi);
            services.AddSingleton(UcBioApi);
            services.AddSingleton(UcBioApiExport);
            services.AddSingleton(OnlineDevices);

            var kernel = new StandardKernel();

            kernel.Load("Biovation.Identifiers.Virdi.dll");
            kernel.Load("Biovation.Hamsters.Virdi.dll");

            var identifiers = kernel.GetAll<IIdentifier>().ToList();
            var hamsterControllers = kernel.GetAll<IHamster>().ToList();

            foreach (var identifier in identifiers)
                Logger.Log("Virdi Identifier loaded: " + identifier.GetType());

            foreach (var hamsterController in hamsterControllers)
                Logger.Log("Virdi Hamster loaded: " + hamsterController.GetType());

            services.AddSingleton<TaskManager, TaskManager>();
            services.AddSingleton<VirdiCodeMappings, VirdiCodeMappings>();
            services.AddSingleton<BiometricTemplateManager, BiometricTemplateManager>();

            services.AddSingleton<CommandFactory, CommandFactory>();
            services.AddSingleton<Callbacks, Callbacks>();

            var virdiObject = new Virdi();
            var virdiServer = new VirdiServer(UcsApi, OnlineDevices);
            services.AddSingleton(virdiObject);
            services.AddSingleton(virdiServer);
            //var virdiCodeMappings = new VirdiCodeMappings(serviceProvider.GetService<GenericCodeMappings>());

            //var virdiCallBacks = new Callbacks(UcsApi, serviceProvider.GetService<UserService>(), serviceProvider.GetService<DeviceService>(), serviceProvider.GetService<UserCardService>()
            //    , serviceProvider.GetService<AccessGroupService>(), serviceProvider.GetService<FingerTemplateService>(), serviceProvider.GetService<LogService>(), serviceProvider.GetService<BlackListService>()
            //    , serviceProvider.GetService<FaceTemplateService>(), serviceProvider.GetService<TaskService>(), serviceProvider.GetService<AccessGroupService>(), BiovationConfiguration, serviceProvider.GetService<VirdiLogService>()
            //    , virdiServer, serviceProvider.GetService<FingerTemplateTypes>(), serviceProvider.GetService<VirdiCodeMappings>(), serviceProvider.GetService<DeviceBrands>(), serviceProvider.GetService<LogEvents>(), serviceProvider.GetService<FaceTemplateTypes>()
            //    , serviceProvider.GetService<BiometricTemplateManager>(), serviceProvider.GetService<ILogger<Callbacks>>(), serviceProvider.GetService<TaskStatuses>());

            var serviceProvider = services.BuildServiceProvider();
            var virdiCallBacks = serviceProvider.GetService<Callbacks>();
            services.AddSingleton(virdiCallBacks);

            //services.AddSingleton(virdiCodeMappings);
            //services.AddSingleton<Virdi, Virdi>();
            //services.AddSingleton<Callbacks, Callbacks>();
            //services.AddSingleton<VirdiServer, VirdiServer>();
            //services.AddSingleton<TaskManager, TaskManager>();
            //services.AddSingleton<VirdiCodeMappings, VirdiCodeMappings>();
            //services.AddSingleton<BiometricTemplateManager, BiometricTemplateManager>();

            //services.AddSingleton<CommandFactory, CommandFactory>();

            //services.BuildServiceProvider().GetService<Callbacks>();

            UcsApi.ServerStart(150, BiovationConfiguration.VirdiDevicesConnectionPort);

            Logger.Log(UcsApi.ErrorCode != 0
                    ? $"Error on starting service.\n   +ErrorCode:{UcsApi.ErrorCode} {UcsApi.EventError}"
                    : $"Service started on port: {BiovationConfiguration.VirdiDevicesConnectionPort}"
                , logType: UcsApi.ErrorCode != 0 ? LogType.Error : LogType.Information);
            //Callbacks.FactoryCallbacks(UcsApi, OnlineDevices);
        }
        //private void ConfigureVirdiServices(IServiceCollection services)
        //{
        //    UcsApi = new UCSAPIClass();
        //    UcBioApi = new UCBioAPI();
        //    UcBioApiExport = new UCBioAPI.Export(UcBioApi);

        //    var kernel = new StandardKernel();

        //    kernel.Load("Biovation.Identifiers.Virdi.dll");
        //    kernel.Load("Biovation.Hamsters.Virdi.dll");

        //    var identifiers = kernel.GetAll<IIdentifier>().ToList();
        //    var hamsterControllers = kernel.GetAll<IHamster>().ToList();

        //    foreach (var identifier in identifiers)
        //        Logger.Log("Virdi Identifier loaded: " + identifier.GetType());

        //    foreach (var hamsterController in hamsterControllers)
        //        Logger.Log("Virdi Hamster loaded: " + hamsterController.GetType());

        //    services.AddSingleton<TaskManager, TaskManager>();
        //    services.AddSingleton<VirdiCodeMappings, VirdiCodeMappings>();
        //    services.AddSingleton<BiometricTemplateManager, BiometricTemplateManager>();

        //    services.AddSingleton<CommandFactory, CommandFactory>();
        //    services.AddSingleton<Callbacks, Callbacks>();

        //    var serviceProvider = services.BuildServiceProvider();

        //    var virdiObject = new Virdi();
        //    var virdiServer = new VirdiServer(UcsApi, OnlineDevices);
        //    //var virdiCodeMappings = new VirdiCodeMappings(serviceProvider.GetService<GenericCodeMappings>());

        //    //var virdiCallBacks = new Callbacks(UcsApi, serviceProvider.GetService<UserService>(), serviceProvider.GetService<DeviceService>(), serviceProvider.GetService<UserCardService>()
        //    //    , serviceProvider.GetService<AccessGroupService>(), serviceProvider.GetService<FingerTemplateService>(), serviceProvider.GetService<LogService>(), serviceProvider.GetService<BlackListService>()
        //    //    , serviceProvider.GetService<FaceTemplateService>(), serviceProvider.GetService<TaskService>(), serviceProvider.GetService<AccessGroupService>(), BiovationConfiguration, serviceProvider.GetService<VirdiLogService>()
        //    //    , virdiServer, serviceProvider.GetService<FingerTemplateTypes>(), serviceProvider.GetService<VirdiCodeMappings>(), serviceProvider.GetService<DeviceBrands>(), serviceProvider.GetService<LogEvents>(), serviceProvider.GetService<FaceTemplateTypes>()
        //    //    , serviceProvider.GetService<BiometricTemplateManager>(), serviceProvider.GetService<ILogger<Callbacks>>(), serviceProvider.GetService<TaskStatuses>());
        //    var virdiCallBacks = serviceProvider.GetService<Callbacks>();

        //    services.AddSingleton(UcsApi);
        //    services.AddSingleton(UcBioApi);
        //    services.AddSingleton(UcBioApiExport);
        //    services.AddSingleton(OnlineDevices);
        //    services.AddSingleton(virdiObject);
        //    services.AddSingleton(virdiCallBacks);
        //    services.AddSingleton(virdiServer);
        //    //services.AddSingleton(virdiCodeMappings);
        //    //services.AddSingleton<Virdi, Virdi>();
        //    //services.AddSingleton<Callbacks, Callbacks>();
        //    //services.AddSingleton<VirdiServer, VirdiServer>();
        //    //services.AddSingleton<TaskManager, TaskManager>();
        //    //services.AddSingleton<VirdiCodeMappings, VirdiCodeMappings>();
        //    //services.AddSingleton<BiometricTemplateManager, BiometricTemplateManager>();

        //    //services.AddSingleton<CommandFactory, CommandFactory>();

        //    //services.BuildServiceProvider().GetService<Callbacks>();

        //    UcsApi.ServerStart(150, BiovationConfiguration.VirdiDevicesConnectionPort);

        //    Logger.Log(UcsApi.ErrorCode != 0
        //            ? $"Error on starting service.\n   +ErrorCode:{UcsApi.ErrorCode} {UcsApi.EventError}"
        //            : $"Service started on port: {BiovationConfiguration.VirdiDevicesConnectionPort}"
        //        , logType: UcsApi.ErrorCode != 0 ? LogType.Error : LogType.Information);
        //    //Callbacks.FactoryCallbacks(UcsApi, OnlineDevices);
        //}

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
    }
}
