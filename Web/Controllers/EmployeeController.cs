using System.Security.Claims;
using Application.Interactors;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers;

[Authorize(Roles = "Employee")]
[Route("employee")]
public class EmployeeController(EmployeeIntreractor employeeIntreractor, ILogger<EmployeeController> logger) : Controller
{
    [HttpGet]
    [Route("salary")]
    public async Task<IActionResult> GetSalary()
    {
        var login = HttpContext.User.FindFirst(ClaimTypes.Name)!.Value;
        
        var salary = await employeeIntreractor.TryGetSalary(login);
        
        return View("Salary", salary);
    }
    
    [HttpGet]
    [Route("work")]
    public async Task<IActionResult> GetWork()
    {
        var login = HttpContext.User.FindFirst(ClaimTypes.Name)!.Value;
        
        var work = await employeeIntreractor.TryGetWork(login);
        
        if (work != null)
            return View("Work", work);
        
        logger.LogError("User with login {login} does not exist", login);
        return ActionResultFactory.CustomServerErrorView(this);

    }
    
    [HttpPost]
    [Route("work")]
    public async Task<IActionResult> PostWork(string currentProjectUrl, bool projectCompleted)
    {
        if (string.IsNullOrEmpty(currentProjectUrl))
            return BadRequest();

        var lodLogin = HttpContext.User.FindFirst(ClaimTypes.Name)!.Value;
        var user = new UserRequestDto(login, password, name, contact,
            string.IsNullOrEmpty(defaultAddress) ? null : defaultAddress);

        if (!await userInteractor.Edit(lodLogin, user))
            return BadRequest();
        
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return Redirect("/login/user");
    }
}