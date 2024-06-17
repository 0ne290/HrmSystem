using System.Security.Claims;
using Application.Interactors;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Web.Pages;
using Web.Presenters;

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
        
        var salary = await employeeIntreractor.TryGetSalary(login);// А вот и идеальный момент для применения паттернов "Презентатор" и "Модель представления" - данные из этого DTO должны быть приведены к адекватному виду (добавить символы рубля и округлить числа до целых)
        
        if (salary != null)
            return View("Salary", SalaryPresenter.SalaryDtoToSalaryViewModel(salary));
        
        logger.LogError("Employee with login {login} does not exist", login);
        return Errors.Return500(this);
    }
    
    [HttpGet]
    [Route("work")]
    public async Task<IActionResult> GetWork()
    {
        var login = HttpContext.User.FindFirst(ClaimTypes.Name)!.Value;
        
        var work = await employeeIntreractor.TryGetWork(login);
        
        if (work != null)
            return View("Work", work);
        
        logger.LogError("Employee with login {login} does not exist", login);
        return Errors.Return500(this);
    }
    
    [HttpPost]
    [Route("work")]
    public async Task<IActionResult> PostWork(string currentProjectUrl, bool projectCompleted)
    {
        if (!Uri.IsWellFormedUriString(currentProjectUrl, UriKind.Absolute))// Логика валидации данных должна содержаться на уровнях Application и Domain в зависимости от типа валидации. Если валидация является частью предметной логики и должна быть применена во всех приложениях, то она должна находится на уровне Domain. Также нужно понимать разницу между валидацией данных и валидацией формата - вся валидация, происходящая на уровне представления, является валидацией формата
            return BadRequest();

        var login = HttpContext.User.FindFirst(ClaimTypes.Name)!.Value;

        if (await employeeIntreractor.TryEditWork(login, currentProjectUrl, projectCompleted))
            return Redirect("/employee/work");

        logger.LogError("Employee with login {login} does not exist", login);
        return Errors.Return500(this);
    }
}