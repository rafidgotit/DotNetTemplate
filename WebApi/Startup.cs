using System.Net;
using System.Text;
using DataAccess;
using DataAccess.Entities;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace WebApi;

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

        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1.0", new OpenApiInfo { Title = "My Demo API", Version = "1.0" });
        });

        services.AddControllers();

        var connectionString = Configuration.GetConnectionString("DefaultConnection");
        services.AddDbContext<PortalDbContext>(x =>
        {
            x.UseSqlServer(connectionString);
            x.UseLoggerFactory(_loggerFactory);
        });

        services.AddDbContext<SecurityDbContext>(x =>
        {
            x.UseSqlServer(connectionString);
            x.UseLoggerFactory(_loggerFactory);
        });

        services.AddIdentity<PortalUser, IdentityRole>(x =>
        {
            x.Password.RequiredLength = 5;
            x.Password.RequireNonAlphanumeric = false;
            x.Password.RequireUppercase = false;
            x.Password.RequireLowercase = false;
            x.Password.RequireDigit = false;
        }).AddEntityFrameworkStores<SecurityDbContext>().AddDefaultTokenProviders();

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
        
                options.TokenValidationParameters =
                    new TokenValidationParameters
                    {
                        ValidIssuer = Configuration["Tokens:Issuer"],
                        ValidAudience = Configuration["Tokens:Audience"],
                        ValidateIssuerSigningKey = true,
                        ValidateLifetime = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Tokens:Key"]))
                    };
            })
            .AddCookie(options =>
            {
                options.Events = new CookieAuthenticationEvents
                {
                    OnRedirectToLogin = OnRedirectToLogin,
                    OnRedirectToAccessDenied = OnRedirectToAccessDenied
                };
            });

        services.AddSingleton(Configuration);
        
        //Services
        // services.AddScoped<INotificationService, NotificationService>();

        //Repository
        // services.AddScoped<IApplicationRole, ApplicationRoleRepository>();
            
        // Domain
        // services.AddScoped<IMaterialDomain, MaterialDomain>();

        // authorization
        services.AddAuthorization(options =>
        {
            // require user to have cookie auth or jwt bearer token
            options.AddPolicy("Authenticated",
                policy => policy
                    .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
                    .RequireAuthenticatedUser());
        });

        services.AddControllersWithViews().AddNewtonsoftJson(options => {
            options.SerializerSettings.ContractResolver = new DefaultContractResolver();
            options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
        }); 

        services.Configure<MvcOptions>(options =>
        {
            options.Filters.Add(new AuthorizeFilter("Authenticated"));
        });

        // Add framework services.
        services.AddCors(options =>
        {
            options.AddPolicy("CorsPolicy",
                builder => builder
                    .AllowAnyOrigin()
                    .AllowAnyHeader()
                    .AllowAnyMethod()
            );
        });
        services.AddDataProtection();

        services.AddResponseCaching();
    }
    private readonly static ILoggerFactory _loggerFactory;
    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        app.UseRouting();
        app.UseCors("CorsPolicy");
        app.UseAuthorization();
        app.UseStaticFiles();
        app.UseResponseCaching();
        //app.UseStaticFiles(new StaticFileOptions()
        //{
        //    FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), @"Resources")),
        //    RequestPath = new PathString("/Resources")
        //});

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });

        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1.0/swagger.json", "My Demo API (V 1.0)");
        });

    }

    private static Task OnRedirectToAccessDenied(RedirectContext<CookieAuthenticationOptions> ctx)
    {
        if (ctx.Request.Path.StartsWithSegments("/api") && ctx.Response.StatusCode == 200)
        {
            ctx.Response.StatusCode = 403;
        }

        return Task.CompletedTask;
    }

    private static Task OnRedirectToLogin(RedirectContext<CookieAuthenticationOptions> context)
    {
        if (context.Request.Path.StartsWithSegments("/api"))
        {
            // return 401 if not "logged in" from an API Call
            context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            return Task.CompletedTask;
        }

        // Redirect users to login page
        context.Response.Redirect(context.RedirectUri);
        return Task.CompletedTask;
    }

}