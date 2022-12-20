using com.doosan.fms.commonLib.JWT.SignalR;
using com.doosan.fms.signalRHub.Extension.Provider.User;
using com.doosan.fms.signalRHub.HubServer;
using com.doosan.fms.signalRHub.SignalR.DataSet;
using com.doosan.multiDatabaseDriver.DatabaseDriver.BaseDriver;
using com.doosan.multiDatabaseDriver.DatabaseDriver.Postgresql;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System;
using System.Linq;
using System.Text;
namespace com.doosan.fms.webapp.Server
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();
            services.AddRazorPages();


            #region cors
            services.AddCors();
            #endregion

            #region transient scoped singleton
            services.AddTransient<IDatabaseManger, PostgresqlManager>();
            services.AddSingleton<ICustomUserProvider, CustomUserProvider>();
            #endregion

            //services.AddControllers();


            services.AddSignalR(o =>
            {
                o.MaximumReceiveMessageSize = 1024000;
                o.EnableDetailedErrors = true;
            });


            services.AddResponseCompression(opts =>
            {
                opts.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(
                    new[] { "application/octet-stream" });
            });

           // services.AddSwaggerGen(c =>
           // {
           //     c.SwaggerDoc("v1", new OpenApiInfo { Title = "com.doosan.fms.webapi", Version = "v1" });
           // });

            #region jwt
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateIssuerSigningKey = true,
                    ValidateLifetime = false,
                    ValidIssuer = SignalRPolicies.Issuer,
                    ValidAudience = SignalRPolicies.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(SignalRPolicies.SecretKey)),
                    ClockSkew = TimeSpan.Zero
                };
            });
            #endregion


        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseResponseCompression();

            app.UseCors(builder => builder
        .AllowAnyOrigin()
        .AllowAnyMethod()
        .AllowAnyHeader());

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseWebAssemblyDebugging();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }




            //app.UseHttpsRedirection();
            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });

            app.UseBlazorFrameworkFiles();
            app.UseStaticFiles();


            app.UseRouting();

            app.UseAuthorization();
            app.UseAuthentication();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHub<FmsDataHubServer<FmsData>>($"/{FmsData.HUB_NAME}");
                endpoints.MapFallbackToFile("index.html");
            });
        }
    }
}
