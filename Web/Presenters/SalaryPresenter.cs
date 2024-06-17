using Application.Dtos;
using Web.Models;

namespace Web.Presenters;

public static class SalaryPresenter
{
    public static SalaryViewModel SalaryDtoToSalaryViewModel(SalaryDto salary) =>
        new($"{salary.Efficiency}%", $"{decimal.Round(salary.PremiumRate)}\u20bd",
            $"{decimal.Round(salary.Premium)}\u20bd", $"{decimal.Round(salary.SalaryRate)}\u20bd",
            $"{decimal.Round(salary.Salary)}\u20bd");
}