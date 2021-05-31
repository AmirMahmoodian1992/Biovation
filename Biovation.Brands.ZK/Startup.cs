using System;
using App.Metrics;
using App.Metrics.Extensions.Configuration;
using Biovation.Brands.ZK.Command;
using Biovation.Brands.ZK.Devices;
using Biovation.Brands.ZK.HostedServices;
using Biovation.Brands.ZK.Manager;
using Biovation.Brands.ZK.Middleware;
using Biovation.CommonClasses;
using Biovation.CommonClasses.Manager;
using Biovation.Constants;
using Biovation.Repository.Api.v2;
using DataAccessLayerCore.Repositories;
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
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading;
using Biovation.Domain;
using Biovation.Service.Api.v2;
using AccessGroupService = Biovation.Service.Api.v1.AccessGroupService;
using AdminDeviceService = Biovation.Service.Api.v1.AdminDeviceService;
using BlackListService = Biovation.Service.Api.v1.BlackListService;
using DeviceGroupService = Biovation.Service.Api.v1.DeviceGroupService;
using DeviceService = Biovation.Service.Api.v1.DeviceService;
using FaceTemplateService = Biovation.Service.Api.v1.FaceTemplateService;
using FingerTemplateService = Biovation.Service.Api.v1.FingerTemplateService;
using GenericCodeMappingService = Biovation.Service.Api.v1.GenericCodeMappingService;
using Log = Serilog.Log;
using LogService = Biovation.Service.Api.v1.LogService;
using LookupService = Biovation.Service.Api.v1.LookupService;
using SettingService = Biovation.Service.Api.v1.SettingService;
using TaskService = Biovation.Service.Api.v1.TaskService;
using TimeZoneService = Biovation.Service.Api.v1.TimeZoneService;
using UserCardService = Biovation.Service.Api.v1.UserCardService;
using UserGroupService = Biovation.Service.Api.v1.UserGroupService;
using UserService = Biovation.Service.Api.v1.UserService;

namespace Biovation.Brands.ZK
{
    public class Startup
    {
        //private readonly ZkTecoServer _zkTecoServer;
        public BiovationConfigurationManager BiovationConfiguration { get; set; }
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
                .AddJsonFile("appsettings.json", true, true)
                .AddJsonFile($"appsettings.{environment.EnvironmentName}.json", true, true)
                .AddEnvironmentVariables();

            var metrics = new MetricsBuilder()
                .Configuration.ReadFrom(configuration);

            Configuration = builder.Build();
            metrics.Build();
        }

        public IConfiguration Configuration { get; }

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

            services.AddSingleton(Log.Logger);
            services.AddSingleton(BiovationConfiguration);
            services.AddSingleton(BiovationConfiguration.Configuration);


            ConfigureRepositoriesServices(services);
            ConfigureConstantValues(services);
            ConfigureZkServices(services);
        }

        private void ConfigureRepositoriesServices(IServiceCollection services)
        {
            var restClient = (RestClient)new RestClient(BiovationConfiguration.BiovationServerUri).UseSerializer(() => new RestRequestJsonSerializer());
            #region checkLock

            string lockEndTime = "";
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

                lockEndTime = requestResult.Result.Data.Data.LockEndTime;
            }
            catch (Exception)
            {
                Logger.Log("The connection with Lock service has a problem");
                Thread.Sleep(TimeSpan.FromSeconds(5));
                Environment.Exit(0);
            }
            services.AddSingleton(restClient);

            #endregion



            var serviceInstanceId = FileActions.JsonReader("appsettings.json", "ServiceInstance", "ServiceInstanceId");
            var serviceInstance = new ServiceInstance(serviceInstanceId.Data);
            var url = (FileActions.JsonReader("appsettings.json", "Urls")).Data;
            if (serviceInstance.ChangeId)
            {
                var setServiceInstanceId =
                    FileActions.JsonWriter("appsettings.json", "ServiceInstance", "ServiceInstanceId", serviceInstance.Id);
                if (!setServiceInstanceId.Success)
                {
                    Logger.Log(LogType.Warning, "Failed to set new GUID in appsettings.json");
                }
                serviceInstance.IpAddress = Dns.GetHostEntry(Dns.GetHostName()).AddressList.FirstOrDefault(x => x.ToString().Split('.').Length == 4)?.ToString();
                 
                var splitUrl = url.Split(':');
                serviceInstance.Port = int.Parse(splitUrl.LastOrDefault() ?? string.Empty);
            }
            else
            {
                serviceInstance.IpAddress = Dns.GetHostEntry(Dns.GetHostName()).AddressList.FirstOrDefault(x => x.ToString().Split('.').Length == 4)?.ToString();
                var splitUrl = url.Split(':');
                serviceInstance.Port = int.Parse(splitUrl.LastOrDefault() ?? string.Empty);
            }

            var serviceInstanceRequest = new RestRequest($"Commands/v2/serviceInstance", Method.POST);
            serviceInstanceRequest.AddJsonBody(serviceInstance);
            //restRequest.AddHeader("Authorization");
            var serviceInstanceResult = restClient.Execute<ResultViewModel>(serviceInstanceRequest);
            if (!serviceInstanceResult.Data.Success)
            {
                Logger.Log(LogType.Warning, "Failed to insert Instance");
            }
            services.AddSingleton(serviceInstance);
            var systemInfo = new SystemInfo
            {
                Services = new List<ServiceInstance>()
             {
                 serviceInstance
             },
                LockEndTime = lockEndTime
                
            };
            services.AddSingleton(systemInfo);



            

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
            services.AddSingleton<Service.Api.v2.UserService, Service.Api.v2.UserService>();
            services.AddSingleton<Service.Api.v2.LogService, Service.Api.v2.LogService>();
            services.AddSingleton<ServiceInstanceService, ServiceInstanceService>();

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
            services.AddSingleton<ServiceInstanceRepository, ServiceInstanceRepository>();

            services.AddSingleton<Lookups, Lookups>();
            services.AddSingleton<GenericCodeMappings, GenericCodeMappings>();
        }

        public void ConfigureConstantValues(IServiceCollection services)
        {
            var serviceCollection = new ServiceCollection();
            var restClient = (RestClient)new RestClient(BiovationConfiguration.BiovationServerUri).UseSerializer(() => new RestRequestJsonSerializer());

            //var serviceInstanceId = FileActions.JsonReader("appsettings.json", "ServiceInstance", "ServiceInstanceId");
            //var serviceInstance = new ServiceInstance(serviceInstanceId.Data);
            //if (serviceInstance.changeId)
            //{
            //    var setServiceInstanceId =
            //        FileActions.JsonWriter("appsettings.json", "ServiceInstance", "ServiceInstanceId", serviceInstance.Id);
            //    if (!setServiceInstanceId.Success)
            //    {
            //        Logger.Log(LogType.Warning, "Failed to set new GUID in appsettings.json");
            //    }
            //    var hostName = Dns.GetHostName();
            //    serviceInstance.IpAddress = Dns.GetHostByName(hostName).AddressList[0].ToString();
            //    FileActions.JsonWriter("appsettings.json", "ServiceInstance", "IpAddress", serviceInstance.IpAddress);
            //    serviceInstance.Port = 9024;
            //    FileActions.JsonWriter("appsettings.json", "ServiceInstance", "Port", serviceInstance.Port.ToString());
            //}
            //else
            //{
            //    var serviceInstanceIp = FileActions.JsonReader("appsettings.json", "ServiceInstance", "IpAddress")?.Data;
            //    var serviceInstancePort = int.Parse(FileActions.JsonReader("appsettings.json", "ServiceInstance", "Port")?.Data ?? string.Empty);
            //    serviceInstance.IpAddress = serviceInstanceIp;
            //    serviceInstance.Port = serviceInstancePort;
            //}

            //var restRequest = new RestRequest($"Commands/v2/serviceInstance", Method.POST);
            //restRequest.AddJsonBody(serviceInstance);
            ////restRequest.AddHeader("Authorization");
            //var requestResult = restClient.Execute<ResultViewModel>(restRequest);
            //if (!requestResult.Data.Success)
            //{
            //    Logger.Log(LogType.Warning, "Failed to insert Instance");
            //}
            //serviceCollection.AddSingleton(serviceInstance);
            //var systemInfo = new SystemInfo
            //{
            // Services   = new List<ServiceInstance>()
            // {
            //     serviceInstance
            // }
            //};
            //serviceCollection.AddSingleton(systemInfo);

            serviceCollection.AddSingleton(restClient);

            serviceCollection.AddSingleton<GenericRepository, GenericRepository>();
            serviceCollection.AddScoped<LookupRepository, LookupRepository>();
            serviceCollection.AddScoped<LookupService, LookupService>();
            serviceCollection.AddScoped<GenericCodeMappingRepository, GenericCodeMappingRepository>();
            serviceCollection.AddScoped<GenericCodeMappingService, GenericCodeMappingService>();

            //serviceCollection.AddScoped<Lookups, Lookups>();
            //serviceCollection.AddScoped<GenericCodeMappings, GenericCodeMappings>();


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

            //Task.WaitAll(taskStatusesQuery, taskTypesQuery, taskItemTypesQuery, taskPrioritiesQuery,
            //    fingerIndexNamesQuery, deviceBrandsQuery, logEventsQuery, logSubEventsQuery, fingerTemplateTypeQuery,
            //    faceTemplateTypeQuery, matchingTypeQuery, logEventMappingsQuery, logSubEventMappingsQuery,
            //    fingerTemplateTypeMappingsQuery, matchingTypeMappingsQuery);

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

        private void ConfigureZkServices(IServiceCollection services)
        {
            services.AddSingleton(OnlineDevices);

            services.AddSingleton<ZkCodeMappings, ZkCodeMappings>();
            services.AddSingleton<TaskManager, TaskManager>();
            services.AddSingleton<BiometricTemplateManager, BiometricTemplateManager>();

            services.AddSingleton<CommandFactory, CommandFactory>();
            services.AddSingleton<DeviceFactory, DeviceFactory>();

            services.AddSingleton<ZkTecoServer, ZkTecoServer>();

            services.AddHostedService<ZKTecoHostedService>();
            services.AddHostedService<TaskManagerHostedService>();

            //var serviceProvider = services.BuildServiceProvider();
            //_zkTecoServer = serviceProvider.GetService<ZkTecoServer>();
            //await _zkTecoServer.StartServer();
            //Task.Run(() => _zkTecoServer.StartServer());
        }


        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory, IHostApplicationLifetime applicationLifetime)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            //applicationLifetime.ApplicationStopping.Register(OnServiceStopping);
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

        //private async void OnServiceStopping()
        //{
        //    if (_zkTecoServer is null)
        //        return;

        //    await _zkTecoServer.StopServer();
        //}
    }
}
