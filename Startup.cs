using ActionFilters.ActionFilters;
using esign.ActionFilter;
using esign.Helpers;
using esign.Models;
using LiteDB.Identity.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using TXTextControl.Web;

namespace esign {
	public class Startup {
		public Startup(IConfiguration configuration) {
			Configuration = configuration;
		}

		public IConfiguration Configuration { get; }

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services) {

			string connectionString = Configuration.GetConnectionString("IdentityLiteDB");
			services.AddLiteDBIdentity(connectionString).AddDefaultTokenProviders().AddDefaultUI();

			services.Configure<Credentials>(Configuration.GetSection("Credentials"));
			services.AddHttpContextAccessor();
			services.AddControllersWithViews();
			services.AddRazorPages();

			services.AddScoped<ValidationFilterAttribute>();
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env) {
			if (env.IsDevelopment()) {
				app.UseDeveloperExceptionPage();
			}
			else {
				app.UseExceptionHandler("/Home/Error");
				// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
				app.UseHsts();
			}
			app.UseHttpsRedirection();
			
			string baseDir = env.ContentRootPath;
			AppDomain.CurrentDomain.SetData("DataDirectory", System.IO.Path.Combine(baseDir, "App_Data"));


			// enable Web Sockets
			app.UseWebSockets();

			app.UseMiddleware<OpenedMiddleware>();

            // attach the Text Control WebSocketHandler middleware
            app.UseTXWebSocketMiddleware();

            app.UseStaticFiles();
			app.UseRouting();

			app.UseAuthentication();
			app.UseAuthorization();

			app.UseEndpoints(endpoints => {
				endpoints.MapRazorPages();
				endpoints.MapControllerRoute(
					 name: "default",
					 pattern: "{controller}/{action}/{id?}",
					 defaults: new { controller = "New", action = "Index" });
				
			});
		}
	}
}
