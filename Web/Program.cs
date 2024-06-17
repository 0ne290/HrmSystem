using Application.Interactors;
using Dal;
using Dal.Daos;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Serilog.Events;
using Serilog.Exceptions;
using Serilog.Exceptions.Core;
using Serilog.Exceptions.EntityFrameworkCore.Destructurers;
using Serilog.Formatting.Json;
using Web.Middlewares;

namespace Web;

internal static class Program
{
    private static async Task Main(string[] args)
    {
        Log.Logger = new LoggerConfiguration()
            //.MinimumLevel.Debug()
            .MinimumLevel.Override("Microsoft.AspNetCore.Hosting", LogEventLevel.Warning)
            .MinimumLevel.Override("Microsoft.AspNetCore.Mvc", LogEventLevel.Warning)
            .MinimumLevel.Override("Microsoft.AspNetCore.Routing", LogEventLevel.Warning)
            
            .Enrich.FromLogContext()
            .Enrich.WithExceptionDetails(new DestructuringOptionsBuilder()
                .WithDefaultDestructurers()
                .WithDestructurers(new[] { new DbUpdateExceptionDestructurer() }))
            
            .WriteTo.Async(a => a.File(new JsonFormatter(), @"E:\Logs\HrmSystem.log", retainedFileCountLimit: 4, rollOnFileSizeLimit: true, fileSizeLimitBytes: 5_368_709_120))// По умолчанию в файлы логируются обычные сообщения, но это можно исправить, передав в качестве первого параметра один из трех форматтеров - JsonFormatter, CompactJsonFormatter или RenderedCompactJsonFormatter - сравни результаты каждого из них в одном контексте
            
            .CreateLogger();
        
        try
        {
            Log.Information("Starting host build");
            
            var builder = WebApplication.CreateBuilder(args);
            
            // Add services to the container.
            builder.Services.AddSerilog();
            builder.Services.AddControllersWithViews();
            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(b =>// Этот метод глобально конфигурует все авторизационные Cookie. Если в момент выдачи авторизационного Cookie клиенту для него не будет явно сконфигурирован какой-нибудь аспект с помощью AuthenticationProperties, то для этого аспекта будет принята глобальная конфигурация именно отсюда. Немного кастомной терминологии для удобства: дата истечения Cookie - дата, когда срок действия самого Cookie истечет и клиент его удалит; дата истечения авторизации Cookie - дата, после которой будет невозможно авторизоваться с помощью этого Cookie. Вывод из всего этого: Cookie может перестать быть авторизационным "пропуском" и стать, по сути, кучкой бесполезных байтов, но при этом он может все еще храниться на клиенте и продолжать приходить на сервер
                {
                    //b.SlidingExpiration = true;// По умолчанию true. Если true, то фреймворк автоматически сам будет обновлять срок действия авторизации всех получаемых Cookie, если он истек наполовину или больше. Если false, то срок действия авторизации Cookie обновляться не будет. Это свойство никак не влияет на Cookie.MaxAge (срок действия самого Cookie)
                    //b.ExpireTimeSpan = TimeSpan.FromMinutes(2);// Срок действия авторизации Cookie. По умолчанию он, вроде, "Сессионный" - юзер закрыл браузер, Cookie стал бесполезен
                    b.Cookie.MaxAge = TimeSpan.FromDays(30);// Срок действия самого Cookie. По умолчанию он, вроде, "Сессионный" - юзер закрыл браузер, Cookie удалился
                    //b.EventsType = typeof(SomethingEvent);// Вероятно, это свойство определяет обработчик, который будет вызываться после/перед события/событием "Создание Cookie"
                    
                });
            builder.Services.AddSingleton<IAuthorizationMiddlewareResultHandler, RedirectAfterFailedAuthentication>();
            builder.Services.AddScoped<EmployeeIntreractor>(serviceProvider =>
            {
                var optionsBuilder = new DbContextOptionsBuilder<HrmSystemContext>();
                var connectionString = serviceProvider.GetService<IConfiguration>()!.GetConnectionString("MySql")!;

                var options = optionsBuilder.UseLazyLoadingProxies().UseMySql(connectionString, ServerVersion.AutoDetect(connectionString))
                    .Options;

                return new EmployeeIntreractor(new EmployeeDao(new HrmSystemContext(options)));
            });
            builder.Services.AddDbContext<HrmSystemContext>((serviceProvider, options) =>
            {
                var connectionString = serviceProvider.GetService<IConfiguration>()!.GetConnectionString("MySql")!;

                options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
            });
            builder.Services.AddHttpContextAccessor();
            builder.Services.AddCoreAdmin();

            var app = builder.Build();
            
            app.UseSerilogRequestLogging();
            app.UseMiddleware<ExceptionLoggingMiddleware>();
            //app.UseMiddleware<RequestLoggingMiddleware>();

            // Configure the HTTP request pipeline.
            //if (!app.Environment.IsDevelopment())
            //{
            //    app.UseExceptionHandler("/Home/Error");
            //    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            //    app.UseHsts();
            //}

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Login}/{action=Index}/{id?}");

            app.UseCoreAdminCustomAuth(async serviceProvider =>
            {
                var httpContext = serviceProvider.GetRequiredService<IHttpContextAccessor>().HttpContext!;

                if (!httpContext.User.IsInRole("Administrator"))
                    httpContext.Response.Redirect("/login/administrator");

                return await Task.FromResult(true);
            });
            app.UseCoreAdminCustomUrl("administrator");
            
            Log.Information("Success to build host. Starting web application");

            await app.RunAsync();
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Failed to build host");
        }
        finally
        {
            await Log.CloseAndFlushAsync();
        }
    }
}