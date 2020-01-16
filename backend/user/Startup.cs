using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.CookiePolicy;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace backend {
    public class Startup {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940

        readonly string MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
        public void ConfigureServices(IServiceCollection services) {
            Console.WriteLine("policy cors added");

            services.AddCors(options => {
                options.AddPolicy(MyAllowSpecificOrigins,
                    builder => {
                        builder.WithOrigins("http://animalchess.com")
                            .AllowAnyHeader()
                            .AllowAnyMethod()
                            .AllowCredentials();
                    });
            });

            // //add authentication
            // services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            //     .AddCookie(options => {
            //         options.LoginPath = "/sign_in";
            //         options.Cookie.HttpOnly = true;
            //         // options.Cookie.SecurePolicy = _environment.IsDevelopment() ?
            //         //     CookieSecurePolicy.None : CookieSecurePolicy.Always;
            //         options.Cookie.SameSite = SameSiteMode.Lax;
            //     });

            // services.Configure<CookiePolicyOptions>(options => {
            //     options.MinimumSameSitePolicy = SameSiteMode.Strict;
            //     options.HttpOnly = HttpOnlyPolicy.None;
            //     // options.Secure = _environment.IsDevelopment() ?
            //     //     CookieSecurePolicy.None : CookieSecurePolicy.Always;
            // });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env) {
            if (env.IsDevelopment()) {
                app.UseDeveloperExceptionPage();
            }

            app.UseCors("_myAllowSpecificOrigins");
            // app.UseCookiePolicy();
            // app.UseAuthentication();
            app.UseRouting();
            var server = new ChessServer();
            app.UseEndpoints(endpoints => {
                endpoints.MapGet("/", async context => {
                    var uid = await Authenticator.GetUserFromRequest(context);
                    SecurePasswordHasher.main();
                    TestJSON.test();
                    AnimalChess ac = new AnimalChess();
                    AnimalChess.main(new string[1]);
                    context.Response.ContentType = "text/html";

                    string board = new Board().ToJson();
                    Console.WriteLine("json " + board);
                    await context.Response.WriteAsync("<script src=\"https://cdnjs.cloudflare.com/ajax/libs/jquery/3.4.1/jquery.min.js\"></script><script>$.post('/sign_in', JSON.stringify({username: 'test', pass: '123'}));</script><br>Hello: "+uid+"!");
                });

                endpoints.MapPost("/sign_up", async context => {
                    await server.SignUp(context);
                });
                endpoints.MapPost("/sign_in", async context => {
                    await server.SignIn(context);
                });

                endpoints.MapPost("/sign_out", async context => {
                    await server.SignOut(context);
                });

                endpoints.MapPost("/quit_game", async context => {
                    var uid = await Authenticator.GetUserFromRequest(context);
                    await context.Response.WriteAsync($"Goodbye: {uid}!");
                });

                endpoints.MapPost("/start_game", async context => {
                    var uid = await Authenticator.GetUserFromRequest(context);
                    Console.WriteLine($"uid from cookie {uid}");
                     await server.StartGame(context, uid);
                });
                endpoints.MapPost("/make_move", async context => {
                    var uid = await Authenticator.GetUserFromRequest(context);
                    await context.Response.WriteAsync($"Hello: {uid}!");
                });

            });
        }
    }
}