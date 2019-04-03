using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using WebApiPais.Models;

namespace WebApiPais
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
            services.AddDbContext<AplicationDbContext>(opcion => opcion.UseSqlServer(Configuration.GetConnectionString("DefaultConection")));

            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<AplicationDbContext>()
                .AddDefaultTokenProviders();            

        
            //json web token configuracion
            //se realiza la validacion para saber si el token enviado es correcto y 
            //poder entrar a la aplicación.
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(opcion =>
                opcion.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuers= new List<string> { "yourdomain.com" },
                    ValidAudience = "yourdomain.com",
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["va_clave_super_secreta"])),
                    ClockSkew = TimeSpan.Zero
                });
            //fin json web token

            services.AddMvc().AddJsonOptions(ConfiguracionJson);
        }

        /// <summary>
        /// metodo que permite inorar la referencia ciclica entre modelos, en este caso entre pais y provincia.
        /// </summary>
        /// <param name="obj"></param>
        private void ConfiguracionJson(MvcJsonOptions obj)
        {
            obj.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, AplicationDbContext context)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            //solicitar autorizacion y autentificacion, manejo de usuarios.
            app.UseAuthentication();

            app.UseMvc();

            //agregar datos iniciales a la tabla paises si no existen datos
            if (context.Paises.Any() == false)
            {
                context.Paises.AddRange(new List<Pais>()
                                        {
                                            new Pais {Nombre="Republica Dominicana", Provincias=new List<Provincia>(){
                                                new Provincia { Nombre="Azua" }

                                            } },
                                            new Pais {Nombre="México", Provincias= new List<Provincia>(){
                                                new Provincia { Nombre="Puebla" },
                                                new Provincia { Nombre="Queretaro" },
                                            } },
                                            new Pais {Nombre="Argentian" },
                                        }

                );
                context.SaveChanges();
            }

        }
    }
}
