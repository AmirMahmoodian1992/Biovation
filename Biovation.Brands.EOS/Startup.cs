using System;
using App.Metrics;
using App.Metrics.Extensions.Configuration;
using Biovation.Brands.EOS.Commands;
using Biovation.Brands.EOS.Devices;
using Biovation.Brands.EOS.HostedServices;
using Biovation.Brands.EOS.Manager;
using Biovation.Brands.EOS.Middleware;
using Biovation.CommonClasses;
using Biovation.CommonClasses.Manager;
using Biovation.Constants;
using Biovation.Repository.Api.v2;
using Biovation.Service.Api.v2;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RestSharp;
using Serilog;
using System.Collections.Generic;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Biovation.Domain;
using Log = Serilog.Log;

namespace Biovation.Brands.EOS
{
    public class Startup
    {
        public BiovationConfigurationManager BiovationConfiguration { get; set; }
        public IConfiguration Configuration { get; }

        public readonly Dictionary<uint, Device> OnlineDevices = new Dictionary<uint, Device>();

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
        public async Task ConfigureServices(IServiceCollection services)
        {
            services.AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.Converters.Add(new TimeSpanToStringConverter());
                    options.JsonSerializerOptions.IgnoreNullValues = true;
                }).AddMetrics();

            services.AddHealthChecks();
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            services.AddSingleton(Log.Logger);
            services.AddSingleton(BiovationConfiguration);
            services.AddSingleton(BiovationConfiguration.Configuration);
            
            ConfigureRepositoriesServices(services);
            await ConfigureConstantValues(services);
            ConfigureEosServices(services);

            services.AddHostedService<PingCollectorHostedService>();
            services.AddHostedService<BroadcastMetricsHostedService>();
        }

        private void ConfigureRepositoriesServices(IServiceCollection services)
        {
            var restClient = (RestClient)new RestClient(BiovationConfiguration.BiovationServerUri).UseSerializer(() => new RestRequestJsonSerializer());

            services.AddSingleton(restClient);
            #region checkLock

            var restRequest = new RestRequest($"v2/SystemInfo/LockStatus", Method.GET);
            try
            {
                var requestResult = restClient.ExecuteAsync<ResultViewModel<SystemInfo>>(restRequest);
                if (!requestResult.Result.Data.Success)
                {
                    Logger.Log("The Lock is not active", logType: LogType.Warning);
                    try
                    {
                        if (!(requestResult.Result.Data.Data.LockEndTime is null))
                        {
                            Logger.Log(@$"The Lock Expiration Time is {requestResult.Result.Data.Data.LockEndTime}", logType: LogType.Warning);
                        }
                    }
                    catch (Exception)
                    {
                        //ignore
                    }
                    Thread.Sleep(TimeSpan.FromSeconds(10));
                    Environment.Exit(0);
                }
            }
            catch (Exception)
            {
                Logger.Log("The connection with Lock service has a problem");
                Thread.Sleep(TimeSpan.FromSeconds(5));
                Environment.Exit(0);
            }


            #endregion


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
            services.AddSingleton<ServiceInstanceService,ServiceInstanceService>();

            services.AddSingleton<Service.Api.v1.TaskService, Service.Api.v1.TaskService>();

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
            services.AddSingleton<ServiceInstanceRepository,ServiceInstanceRepository>();

            services.AddSingleton<Lookups, Lookups>();
            services.AddSingleton<GenericCodeMappings, GenericCodeMappings>();
        }

        public async Task ConfigureConstantValues(IServiceCollection services)
        {
            var serviceCollection = new ServiceCollection();
            var restClient = (RestClient)new RestClient(BiovationConfiguration.BiovationServerUri).UseSerializer(() => new RestRequestJsonSerializer());

            var serviceInstanceId = FileActions.JsonReader("appsettings.json", "ServiceInstance", "ServiceInstanceId");
            var serviceInstance = new ServiceInstance(serviceInstanceId.Data);
            if (serviceInstance.changeId)
            {
                var setServiceInstanceId =
                    FileActions.JsonWriter("appsettings.json", "ServiceInstance","ServiceInstanceId", serviceInstance.Id);
                if (!setServiceInstanceId.Success)
                {
                    Logger.Log(LogType.Warning, "Failed to set new GUID in appsettings.json");
                }
                string hostName = Dns.GetHostName();
                serviceInstance.IpAddress = Dns.GetHostByName(hostName).AddressList[0].ToString();
            }
            serviceCollection.AddSingleton(serviceInstance);

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

            await Task.WhenAll(taskStatusesQuery, taskTypesQuery, taskItemTypesQuery);

            var lookups = new Lookups
            {
                TaskStatuses = (await taskStatusesQuery)?.Data?.Data,
                TaskTypes = (await taskTypesQuery)?.Data?.Data,
                TaskItemTypes = (await taskItemTypesQuery)?.Data?.Data,
                TaskPriorities = (await taskPrioritiesQuery)?.Data?.Data,
                FingerIndexNames = (await fingerIndexNamesQuery)?.Data?.Data,
                DeviceBrands = (await deviceBrandsQuery)?.Data?.Data,
                LogSubEvents = (await logSubEventsQuery)?.Data?.Data,
                FingerTemplateType = (await fingerTemplateTypeQuery)?.Data?.Data,
                FaceTemplateType = (await faceTemplateTypeQuery)?.Data?.Data,
                LogEvents = (await logEventsQuery)?.Data?.Data,
                MatchingTypes = (await matchingTypeQuery)?.Data?.Data
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

        private void ConfigureEosServices(IServiceCollection services)
        {
            services.AddSingleton(OnlineDevices);

            services.AddSingleton<TaskManager, TaskManager>();
            services.AddSingleton<EosCodeMappings, EosCodeMappings>();
            services.AddSingleton<BiometricTemplateManager, BiometricTemplateManager>();

            services.AddSingleton<CommandFactory, CommandFactory>();
            services.AddSingleton<DeviceFactory, DeviceFactory>();
            services.AddSingleton<EosServer, EosServer>();

            services.AddHostedService<EosHostedService>();
            services.AddHostedService<TaskManagerHostedService>();

            //var serviceProvider = services.BuildServiceProvider();
            //var eosServer = serviceProvider.GetService<EosServer>();

            //eosServer.StartServer();
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
    }
}
