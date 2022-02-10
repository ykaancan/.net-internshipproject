using InstagramAPI.Modals;
using InstagramAPI.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.FileProviders;
using System.IO;

namespace InstagramAPI
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
            services.AddCors(options =>
            {
                options.AddDefaultPolicy(builder =>
                {
                    builder.WithOrigins("https://localhost:44354")
                    .AllowAnyHeader()
                    .AllowAnyHeader();
                });
            });
            services.AddMvc();
            services.AddDistributedMemoryCache();
            services.AddSession(options =>
            {
               
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });
            services.Configure<InstagramDatabaseSettings>(
                Configuration.GetSection(nameof(InstagramDatabaseSettings)));
            services.AddSingleton<IInstagramDatabaseSettings>(sp => sp.GetRequiredService<IOptions<InstagramDatabaseSettings>>().Value);           
            services.AddControllersWithViews();
            services.AddControllers();
            services.AddControllers().AddNewtonsoftJson(options => options.UseMemberCasing());
            services.AddScoped<IInstagramService, InstagramService>();
            services.AddScoped<IUserLoginService, UserLoginService>();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "InstagramAPI", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            
            app.UseSession();
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "InstagramAPI v1"));
            }

           
            app.UseHttpsRedirection();

            app.UseRouting();
            app.UseCors();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                             
            });
        }
    }
}
