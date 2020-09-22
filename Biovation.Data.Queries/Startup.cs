using Biovation.CommonClasses.Manager;
using DataAccessLayerCore;
using DataAccessLayerCore.Domain;
using DataAccessLayerCore.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Biovation.Data.Queries
{
    public class Startup
    {

        public BiovationConfigurationManager BiovationConfiguration { get; set; }
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

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.Converters.Add(new TimeSpanToStringConverter());
                    options.JsonSerializerOptions.IgnoreNullValues = true;
                });

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

            services.AddSingleton(connectionInfo);
            services.AddSingleton<IConnectionFactory, DbConnectionFactory>();
            services.AddSingleton<GenericRepository, GenericRepository>();

            if (BiovationConfiguration.MigrateUp)
            {
                var migrateRes = Migration.MigrateUp(connectionInfo);
                if (!migrateRes)
                {
                    if (BiovationConfiguration.MigrateUp)
                    {
                        BiovationConfiguration.MigrateUp = false;
                    }
                }
            }

            services.AddScoped<Repository.SQL.v2.UserGroupRepository, Repository.SQL.v2.UserGroupRepository>();
            services.AddScoped<Repository.SQL.v2.UserRepository, Repository.SQL.v2.UserRepository>();
            services.AddScoped<Repository.SQL.v2.DeviceRepository, Repository.SQL.v2.DeviceRepository>();
            services.AddScoped<Repository.SQL.v2.PlateDetectionRepository, Repository.SQL.v2.PlateDetectionRepository>();
            services.AddScoped<Repository.SQL.v2.AccessGroupRepository, Repository.SQL.v2.AccessGroupRepository>();
            services.AddScoped<Repository.SQL.v2.AdminDeviceRepository, Repository.SQL.v2.AdminDeviceRepository>();
            services.AddScoped<Repository.SQL.v2.BlackListRepository, Repository.SQL.v2.BlackListRepository>();
            services.AddScoped<Repository.SQL.v2.DeviceGroupRepository, Repository.SQL.v2.DeviceGroupRepository>();
            services.AddScoped<Repository.SQL.v2.FingerTemplateRepository, Repository.SQL.v2.FingerTemplateRepository>();
            services.AddScoped<Repository.SQL.v2.FaceTemplateRepository, Repository.SQL.v2.FaceTemplateRepository>();
            services.AddScoped<Repository.SQL.v2.GenericCodeMappingRepository, Repository.SQL.v2.GenericCodeMappingRepository>();
            services.AddScoped<Repository.SQL.v2.LogRepository, Repository.SQL.v2.LogRepository>();
            services.AddScoped<Repository.SQL.v2.LookupRepository, Repository.SQL.v2.LookupRepository>();
            services.AddScoped<Repository.SQL.v2.TaskRepository, Repository.SQL.v2.TaskRepository>();
            services.AddScoped<Repository.SQL.v2.TimeZoneRepository, Repository.SQL.v2.TimeZoneRepository>();
            services.AddScoped<Repository.SQL.v2.UserCardRepository, Repository.SQL.v2.UserCardRepository>();
            services.AddScoped<Repository.SQL.v2.UserGroupRepository, Repository.SQL.v2.UserGroupRepository>();
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
