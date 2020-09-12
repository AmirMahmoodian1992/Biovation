using System.Collections.Generic;
using System.Linq;
using Biovation.Brands.Virdi.Command;
using Biovation.Brands.Virdi.Manager;
using Biovation.Brands.Virdi.Service;
using Biovation.CommonClasses;
using Biovation.CommonClasses.Interface;
using Biovation.CommonClasses.Manager;
using Biovation.Domain;
using Biovation.Constants;
using Biovation.Repository.Api.v2;
using Biovation.Service.Api.v1;
using DataAccessLayerCore;
using DataAccessLayerCore.Domain;
using DataAccessLayerCore.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Ninject;
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

            //Log.Logger = new LoggerConfiguration().ReadFrom.Configuration(configuration).CreateLogger();
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

            services.AddScoped<GenericRepository, GenericRepository>();
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
            services.AddScoped<SettingService, SettingService>();
            services.AddScoped<TaskService, TaskService>();
            services.AddScoped<TimeZoneService, TimeZoneService>();
            services.AddScoped<UserCardService, UserCardService>();
            services.AddScoped<UserGroupService, UserGroupService>();
            services.AddScoped<UserService, UserService>();
            services.AddScoped<VirdiLogService, VirdiLogService>();

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
            services.AddScoped<SettingRepository, SettingRepository>();
            services.AddScoped<TaskRepository, TaskRepository>();
            services.AddScoped<TimeZoneRepository, TimeZoneRepository>();
            services.AddScoped<UserCardRepository, UserCardRepository>();
            services.AddScoped<UserGroupRepository, UserGroupRepository>();
            services.AddScoped<UserRepository, UserRepository>();

            services.AddSingleton<Lookups, Lookups>();
            services.AddSingleton<GenericCodeMappings, GenericCodeMappings>();

        }

        private void ConfigureConstantValues(IServiceCollection services)
        {
            //Constant values
            //services.AddSingleton(new DeviceBrands());
            services.AddSingleton<DeviceBrands, DeviceBrands>();
            //services.AddSingleton(new LogEvents());
            services.AddSingleton<LogEvents, LogEvents>();
            services.AddSingleton<LogSubEvents, LogSubEvents>();
            //services.AddSingleton(new FingerTemplateTypes());
            services.AddSingleton<FingerTemplateTypes, FingerTemplateTypes>();
            services.AddSingleton<FaceTemplateTypes, FaceTemplateTypes>();
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
        }
    }
}
