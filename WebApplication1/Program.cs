using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebApplication1.Models;
using Microsoft.EntityFrameworkCore;
using System.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using WebApplication1.Security;


try
{
    var builder = WebApplication.CreateBuilder(args);
    builder.Services.AddMvc(options => { 
        options.EnableEndpointRouting = false;
        var policy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
        options.Filters.Add( new AuthorizeFilter(policy));
    });

    builder.Services.AddAuthentication().AddGoogle(
        options =>
        {
            options.ClientId = "1036810119430-gagop22luu0brgbj3shffad81jk2jbqt.apps.googleusercontent.com";
            options.ClientSecret = "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";
        }).AddFacebook(options =>
        {
            options.ClientId = "1017172219937555";
            options.ClientSecret = "57eea0716b401e10dfe7d4bdbb9b676e";
        });
    
    

    builder.Services.AddDbContextPool<AppDbContext>(
        options => options.UseSqlServer(builder.Configuration.GetConnectionString("EmployeeDBConnection"))
        );

    builder.Services.AddAuthorization(options =>
    {
        options.AddPolicy("DeleteRolePolicy", policy => policy.RequireClaim("Delete Role","true"));
        options.AddPolicy("EditRolePolicy", policy => policy.AddRequirements(new ManageAdminRolesAndClaimsRequirement()));
        
    });

    builder.Services.AddIdentity<ApplicationUser, IdentityRole>().AddEntityFrameworkStores<AppDbContext>().AddDefaultTokenProviders()
        .AddTokenProvider<CustomEmailConfirmationTokenProvider<ApplicationUser>>("CustomEmailConfirmation");

   /* builder.Services.ConfigureApplicationCookie(options =>
    {
        options.AccessDeniedPath = new PathString("/Administration/AccessDenied");
    });*/

    builder.Services.Configure<IdentityOptions>(options =>
    {
        options.Password.RequiredLength = 10;
        options.Password.RequiredUniqueChars = 3;
        options.SignIn.RequireConfirmedEmail = true;
        options.Tokens.EmailConfirmationTokenProvider = "CustomEmailConfirmation";
        options.Lockout.MaxFailedAccessAttempts = 5;
        options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);

    });
    builder.Services.Configure<DataProtectionTokenProviderOptions>(options =>
    {
        options.TokenLifespan = TimeSpan.FromHours(5);
    });

    builder.Services.AddScoped<IEmployeeRepository, SQLEmployeeRepository>();
    builder.Services.AddSingleton<IAuthorizationHandler,CanEditOnlyOtherAdminRolesAndClaimsHandler>();
    builder.Services.AddSingleton<IAuthorizationHandler, SuperAdminHandler>();
    builder.Services.AddSingleton<DataProtectionPurposeStrings>();


    var app = builder.Build();
    IConfiguration configuration = app.Configuration;

    app.Logger.LogInformation(5,"Test");

    /*app.MapGet("/", async context =>
    {
        await context.Response.WriteAsync(configuration["myShit"]);
    });*/

    /*if (builder.Environment.IsDevelopment())
    {
        app.UseDeveloperExceptionPage();
    }
    else
    {
        app.UseExceptionHandler("/Error");
        app.UseStatusCodePagesWithReExecute("/Error/{0}");
    }*/
    //app.UseExceptionHandler("/Error");
    //app.UseStatusCodePagesWithReExecute("/Error/{0}");
    app.UseAuthentication();

    app.UseStaticFiles();
    
    app.UseMvc(routes =>
    {
        routes.MapRoute("default", "{controller=Home}/{action=Index}/{id?}");
    });

   





    app.Run();
}
catch (Exception exception)
{
    //logger.Error(exception,"program stopped because fuck you");
    throw;
}







