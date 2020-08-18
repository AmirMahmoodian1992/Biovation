using Biovation.CommonClasses.Manager;
using Biovation.CommonClasses.Models.ConstantValues;
using Biovation.CommonClasses.Repository;
using Biovation.CommonClasses.Repository.RestaurantRepositories;
using Biovation.CommonClasses.Service;
using Biovation.CommonClasses.Service.RestaurantServices;
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
using Ocelot.DependencyInjection;
using Ocelot.Middleware;

namespace Biovation.Gateway
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
                .AddJsonFile("ocelot.json")
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }


        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //services.AddControllers();
            services.AddOcelot(Configuration);

            services.AddApiVersioning(config =>
            {
                config.DefaultApiVersion = new ApiVersion(1, 0);
                config.AssumeDefaultVersionWhenUnspecified = true;
                config.ReportApiVersions = true;  //the clients of the API know all supported versions
                config.ApiVersionReader = new HeaderApiVersionReader("api-version"); //if Pass version information in the HTTP headers
            });

            services.AddSwaggerGen();

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
            services.AddSingleton<GenericCodeMappingService, GenericCodeMappingService>();
            services.AddScoped<LogService, LogService>();
            services.AddSingleton<LookupService, LookupService>();
            services.AddScoped<PlateDetectionService, PlateDetectionService>();
            services.AddScoped<SettingService, SettingService>();
            services.AddScoped<TaskService, TaskService>();
            services.AddScoped<TimeZoneService, TimeZoneService>();
            services.AddScoped<UserCardService, UserCardService>();
            services.AddScoped<UserGroupService, UserGroupService>();
            services.AddScoped<UserService, UserService>();

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
            services.AddSingleton<GenericCodeMappingRepository, GenericCodeMappingRepository>();
            services.AddScoped<LogRepository, LogRepository>();
            services.AddSingleton<LookupRepository, LookupRepository>();
            services.AddScoped<PlateDetectionRepository, PlateDetectionRepository>();
            services.AddScoped<SettingRepository, SettingRepository>();
            services.AddScoped<TaskRepository, TaskRepository>();
            services.AddScoped<TimeZoneRepository, TimeZoneRepository>();
            services.AddScoped<UserCardRepository, UserCardRepository>();
            services.AddScoped<UserGroupRepository, UserGroupRepository>();
            services.AddScoped<UserRepository, UserRepository>();

            services.AddSingleton<Lookups, Lookups>();
            services.AddSingleton<GenericCodeMappings, GenericCodeMappings>();

            //Constant values
            services.AddSingleton<DeviceBrands, DeviceBrands>();
            services.AddSingleton<FaceTemplateTypes, FaceTemplateTypes>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public async void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            //app.UseMvc(routes =>
            //{
            //    routes.MapRoute(
            //        name: "default",
            //        template: "{controller=Main}/{action=Index}/{id?}");
            //});

            //app.UseHttpsRedirection();

            await app.UseOcelot();
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
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
            });


            //app.UseEndpoints(endpoints =>
            //{
            //    //endpoints.MapControllerRoute(name: "Biovation",
            //    //    pattern: "Biovation/api/{*}",
            //    //    defaults: new { controller = "Blog", action = "" });
            //    //endpoints.MapControllerRoute(name: "default",
            //    //    pattern: "Biovation/api/{controller}/{action}");
            //});
        }
    }
}
