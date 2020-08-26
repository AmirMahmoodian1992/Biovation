using Biovation.Brands.Virdi.Command;
using Biovation.Brands.Virdi.Manager;
using Biovation.Brands.Virdi.Service;
using Biovation.CommonClasses;
using Biovation.CommonClasses.Interface;
using Biovation.CommonClasses.Manager;
using Biovation.Constants;
using Biovation.Domain;
using Biovation.Repository;
using Biovation.Service;
using DataAccessLayerCore;
using DataAccessLayerCore.Domain;
using DataAccessLayerCore.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Ninject;
using Serilog;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UCSAPICOMLib;
using UNIONCOMM.SDK.UCBioBSP;

namespace Biovation.Brands.Virdi
{
    public class Startup
    {
        public BiovationConfigurationManager BiovationConfiguration { get; set; }
        public IConfiguration Configuration { get; }

        public UCBioAPI UCBioApi;
        public UCBioAPI.Export UCBioApiExport;
        // UCSAPI
        public UCSAPI UcsApi;
        public readonly Dictionary<uint, DeviceBasicInfo> OnlineDevices = new Dictionary<uint, DeviceBasicInfo>();

        public Startup(IConfiguration configuration, IWebHostEnvironment environment)
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
                .AddJsonFile($"appsettings.{environment.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddSingleton(BiovationConfiguration);
            services.AddSingleton(BiovationConfiguration.Configuration);

            ConfigureRepositoriesServices(services);
            ConfigureConstantValues(services);
            ConfigureVirdiServices(services);
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
            services.AddSingleton<VirdiLogService, VirdiLogService>();

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


            LogEvents.Connect = Lookups.LogEvents.FirstOrDefault(lookup => string.Equals(lookup.Code, LogEvents.ConnectCode));
            LogEvents.Disconnect = Lookups.LogEvents.FirstOrDefault(lookup => string.Equals(lookup.Code, LogEvents.DisconnectCode));
            LogEvents.Authorized = Lookups.LogEvents.FirstOrDefault(lookup => string.Equals(lookup.Code, LogEvents.AuthorizedCode));
            LogEvents.UnAuthorized = Lookups.LogEvents.FirstOrDefault(lookup => string.Equals(lookup.Code, LogEvents.UnAuthorizedCode));
            LogEvents.AddUserToDevice = Lookups.LogEvents.FirstOrDefault(lookup => string.Equals(lookup.Code, LogEvents.AddUserToDeviceCode));
            LogEvents.RemoveUserFromDevie = Lookups.LogEvents.FirstOrDefault(lookup => string.Equals(lookup.Code, LogEvents.RemoveUserFromDeviceCode));
            LogEvents.DeviceEnabled = Lookups.LogEvents.FirstOrDefault(lookup => string.Equals(lookup.Code, LogEvents.DeviceEnabledCode));
            LogEvents.DeviceDisabled = Lookups.LogEvents.FirstOrDefault(lookup => string.Equals(lookup.Code, LogEvents.DeviceDisabledCode));
            LogEvents.EnrollSuccess = Lookups.LogEvents.FirstOrDefault(lookup => string.Equals(lookup.Code, LogEvents.EnrollSuccessCode));
            LogEvents.IdentifySuccessFace = Lookups.LogEvents.FirstOrDefault(lookup => string.Equals(lookup.Code, LogEvents.IdentifyFaceSuccessCode));

            services.AddSingleton<Lookups, Lookups>();
            services.AddSingleton(genericCodeMappings);
            //Constant values
            services.AddSingleton<DeviceBrands, DeviceBrands>();
            services.AddSingleton<LogEvents, LogEvents>();
            services.AddSingleton<LogSubEvents, LogSubEvents>();
            services.AddSingleton<FaceTemplateTypes, FaceTemplateTypes>();
            services.AddSingleton<FingerTemplateTypes, FingerTemplateTypes>();
        }


        private void ConfigureVirdiServices(IServiceCollection services)
        {
            UcsApi = new UCSAPIClass();
            UCBioApi = new UCBioAPI();
            UCBioApiExport = new UCBioAPI.Export(UCBioApi);

            var kernel = new StandardKernel();

            kernel.Load("Biovation.Identifiers.Virdi.dll");
            kernel.Load("Biovation.Hamsters.Virdi.dll");

            var identifiers = kernel.GetAll<IIdentifier>().ToList();
            var hamsterControllers = kernel.GetAll<IHamster>().ToList();

            foreach (var identifier in identifiers)
                Logger.Log("Virdi Identifier loaded: " + identifier.GetType());

            foreach (var hamsterController in hamsterControllers)
                Logger.Log("Virdi Hamster loaded: " + hamsterController.GetType());


            services.AddSingleton(UcsApi);
            services.AddSingleton(UCBioApi);
            services.AddSingleton(UCBioApiExport);
            services.AddSingleton(OnlineDevices);
            services.AddSingleton<Virdi, Virdi>();
            services.AddSingleton<Callbacks, Callbacks>();
            services.AddSingleton<VirdiServer, VirdiServer>();
            services.AddSingleton<TaskManager, TaskManager>();
            services.AddSingleton<VirdiCodeMappings, VirdiCodeMappings>();

            services.AddSingleton<CommandFactory, CommandFactory>();

            UcsApi.ServerStart(150, BiovationConfiguration.VirdiDevicesConnectionPort);

            Logger.Log(UcsApi.ErrorCode != 0
                    ? $"Error on starting service.\n   +ErrorCode:{UcsApi.ErrorCode} {UcsApi.EventError}"
                    : $"Service started on port: {BiovationConfiguration.VirdiDevicesConnectionPort}"
                , logType: UcsApi.ErrorCode != 0 ? LogType.Error : LogType.Information);
            //Callbacks.FactoryCallbacks(UcsApi, OnlineDevices);
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

            loggerFactory.AddSerilog();
            app.UseSerilogRequestLogging();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
