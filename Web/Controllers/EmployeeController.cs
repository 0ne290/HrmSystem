using System.Security.Claims;
using Application.Interactors;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers;

[Authorize(Roles = "Employee")]
[Route("employee")]
public class EmployeeController(EmployeeInteractor employeeInteractor, ILogger<EmployeeController> logger) : Controller
{
    [HttpGet]
    [Route("salary")]
    public async Task<IActionResult> GetSalary()
    {
        var login = HttpContext.User.FindFirst(ClaimTypes.Name)!.Value;
        
        var orders = await userInteractor.GetAllOrders(login);
        
        return View("Orders", orders);
    }
    
    [HttpGet]
    [Route("work")]
    public async Task<IActionResult> GetWork()
    {
        var login = HttpContext.User.FindFirst(ClaimTypes.Name)!.Value;
        
        var user = await userInteractor.GetUser(login);
        
        if (user != null)
            return View("Edit", user);
        
        logger.LogError("User with login {login} does not exist", login);
        return ActionResultFactory.CustomServerErrorView(this);

    }
    
    [HttpPost]
    [Route("work")]
    public async Task<IActionResult> PostWork(string login, string password, string name, string contact, string defaultAddress)
    {
        if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(name) || string.IsNullOrEmpty(contact))
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