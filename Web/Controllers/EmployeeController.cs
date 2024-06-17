using System.Security.Claims;
using Application.Interactors;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Web.Presenters;

namespace Web.Controllers;

[Authorize(Roles = "Employee")]
[Route("employee")]
public class EmployeeController(EmployeeIntreractor employeeIntreractor) : Controller
{
    [HttpGet]
    [Route("salary")]
    public async Task<IActionResult> GetSalary()
    {
        var login = HttpContext.User.FindFirst(ClaimTypes.Name)!.Value;
        
        var salary = await employeeIntreractor.GetSalary(login);// А вот и идеальный момент для применения паттернов "Презентатор" и "Модель представления" - данные из этого DTO должны быть приведены к адекватному виду (добавить символы рубля и округлить числа до целых)
        
        return View("Salary", SalaryPresenter.SalaryDtoToSalaryViewModel(salary));
    }
    
    [HttpGet]
    [Route("work")]
    public async Task<IActionResult> GetWork()
    {
        var login = HttpContext.User.FindFirst(ClaimTypes.Name)!.Value;
        
        var work = await employeeIntreractor.GetWork(login);
        
        return View("Work", work);
    }
    
    [HttpPost]
    [Route("work")]
    public async Task<IActionResult> PostWork(string currentProjectUrl, bool projectCompleted)
    {
        if (!Uri.IsWellFormedUriString(currentProjectUrl, UriKind.Absolute))// Логика валидации данных должна содержаться на уровнях Application и Domain в зависимости от типа валидации. Если валидация является частью предметной логики и должна быть применена во всех приложениях, то она должна находится на уровне Domain. Также нужно понимать разницу между валидацией данных и валидацией формата - вся валидация, происходящая на уровне представления, является валидацией формата
            return BadRequest();

        var login = HttpContext.User.FindFirst(ClaimTypes.Name)!.Value;

        await employeeIntreractor.EditWork(login, currentProjectUrl, projectCompleted);
        
        return Redirect("/employee/work");
    }
}