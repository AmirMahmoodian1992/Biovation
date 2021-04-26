using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using DataAccessLayerCore.Repositories;
using Biovation.Repository.Api.v2;
using Biovation.Service.Api.v2;
using Biovation.CommonClasses.Manager;
using Biovation.CommonClasses;
using Biovation.Domain;
using Biovation.Constants;
using App.Metrics;
using App.Metrics.Extensions.Configuration;
using RestSharp;
using Serilog;
using System.Threading;
using System.Reflection;
using Biovation.Brands.Paliz.Middleware;
using Biovation.Brands.Paliz.HostedServices;
using Log = Serilog.Log;
using System.Collections.Generic;
using Biovation.Brands.Paliz.Manager;
using PalizTiara.Api;
using Biovation.Brands.Paliz.Devices;

namespace Biovation.Brands.Paliz
{
    public class Startup
    {
        public BiovationConfigurationManager BiovationConfiguration { get; set; }
        public readonly Dictionary<uint, DeviceBasicInfo> OnlineDevices = new Dictionary<uint, DeviceBasicInfo>();

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

            services.AddHealthChecks();

            services.AddSingleton(Log.Logger);
            services.AddSingleton(BiovationConfiguration);
            services.AddSingleton(BiovationConfiguration.Configuration);


            ConfigureRepositoriesServices(services);
            ConfigureConstantValues(services);
            ConfigurePalizServices(services);
        }

        private void ConfigureRepositoriesServices(IServiceCollection services)
        {
            var restClient = (RestClient)new RestClient(BiovationConfiguration.BiovationServerUri).UseSerializer(() => new RestRequestJsonSerializer());

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

            services.AddSingleton(restClient);
            //add your injection here 

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
            //services.AddSingleton<Service.Api.v2.UserService, Service.Api.v2.UserService>();

            services.AddSingleton<GenericRepository, GenericRepository>();
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
        }

        public void ConfigureConstantValues(IServiceCollection services)
        {
            //var serviceCollection = new ServiceCollection();
            //var restClient = (RestClient)new RestClient(BiovationConfiguration.BiovationServerUri).UseSerializer(() => new RestRequestJsonSerializer());

            //services.AddSingleton(restClient);

        
            //serviceCollection.AddScoped<Lookups, Lookups>();
            //serviceCollection.AddScoped<GenericCodeMappings, GenericCodeMappings>();

            var serviceProvider = services.BuildServiceProvider();

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
                TaskStatuses = taskStatusesQuery.Data.Data,
                TaskTypes = taskTypesQuery.Data.Data,
                TaskItemTypes = taskItemTypesQuery.Data.Data,
                TaskPriorities = taskPrioritiesQuery.Data.Data,
                FingerIndexNames = fingerIndexNamesQuery.Data.Data,
                DeviceBrands = deviceBrandsQuery.Data.Data,
                LogSubEvents = logSubEventsQuery.Data.Data,
                FingerTemplateType = fingerTemplateTypeQuery.Data.Data,
                FaceTemplateType = faceTemplateTypeQuery.Data.Data,
                LogEvents = logEventsQuery.Data.Data,
                MatchingTypes = matchingTypeQuery.Data.Data
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

        public void ConfigurePalizServices(IServiceCollection services)
        {
            TiaraServerManager.Bootstrapper();
            var tiaraServerManager = new TiaraServerManager();

            services.AddSingleton(OnlineDevices);
            services.AddSingleton(tiaraServerManager);
            services.AddSingleton<PalizCodeMappings, PalizCodeMappings>();
            services.AddSingleton<BiometricTemplateManager, BiometricTemplateManager>();
            services.AddSingleton<DeviceFactory, DeviceFactory>();
            var palizObject = new Paliz();
            services.AddSingleton(OnlineDevices);
            services.AddSingleton(palizObject);
            services.AddSingleton<TiaraServerManager, TiaraServerManager>();
            services.AddSingleton<PalizServer, PalizServer>();

            services.AddHostedService<PalizHostedService>();
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

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
