using System.Security.Claims;
using Application.Interactors;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers;

[Route("login")]
public class LoginController(EmployeeIntreractor employeeIntreractor, ILogger<LoginController> logger) : Controller
{
    [HttpGet]
    [Route("employee")]
    public IActionResult GetEmployee()
    {
        return View("Employee");
    }
    
    [HttpGet]
    [Route("administrator")]
    public IActionResult GetAdministrator()
    {
        return View("Administrator");
    }
    
    // В идеале часть логики входа должна быть на фронтенде, чтобы в случае ввода неправильных логина и/или пароля происходило встраивание с помощью AJAX в тот же документ на той же странице сообщения о неправильном вводе
    [HttpPost]
    [Route("employee")]
    public async Task<IActionResult> PostEmployee(string login, string password, string remember)// Тогда действия входа должны возвращать 200 или 400. Если 200, то JavaScript-код на клиенте сам выполнит переадресацию. Если 400 - встроит в документ сообщение о неправильном вводе
    {
        if (!await employeeIntreractor.TryLogin(login, password))
            return BadRequest();
        
        var claims = new[] { new Claim(ClaimTypes.Name, login), new Claim(ClaimTypes.Role, "Employee") };
        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

        AuthenticationProperties? authProperties = null;
        if (remember == "yes")
            authProperties = new AuthenticationProperties
            {
                ExpiresUtc = DateTimeOffset.UtcNow.AddDays(7),
                IsPersistent = true
            };
        else if (remember != "no")
            logger.LogWarning("{remember} value of the remember parameter is invalid", remember);
            
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal, authProperties);
            
        return Redirect("/employee/salary");
    }
    
    [HttpPost]
    [Route("administrator")]
    public async Task<IActionResult> PostAdministrator(string login, string password, string remember)// В идеале remember должен быть bool, а все входные данные действий должны быть ViewModel с атрибутами данных, валидации и/или привязки. Если правильно использовать эти атрибуты, то ViewModel можно сделать самовалидирующимися и не понадобится загромождать действия контроллеров кодом валидации. Также в формах на клиенте должны быть tag-хэлперы, связывающие данные форм с входными параметрами действий контроллеров. Существуют еще и другие полезные хэлперы, например, выполняющие валидацию прямо на клиенте (но для них нужно дополнительно подключать JQuery-скрипты). Таким вот образом автоматизируется валидация в ASP.NET MVC
    {
        if (login != "nimda" || password != "4202drowssap")
            return BadRequest();
        
        var claims = new[] { new Claim(ClaimTypes.Role, "Administrator") };
        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

        AuthenticationProperties? authProperties = null;
        if (remember == "yes")
            authProperties = new AuthenticationProperties
            {
                ExpiresUtc = DateTimeOffset.UtcNow.AddDays(7),
                IsPersistent = true
            };
        else if (remember != "no")
            logger.LogWarning("{remember} value of the remember parameter is invalid", remember);
            
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal, authProperties);
            
        return Redirect("/administrator");
    }
}