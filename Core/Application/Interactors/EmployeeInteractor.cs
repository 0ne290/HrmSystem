using Application.Dtos;
using Application.Mappers;
using Domain.Interfaces;

namespace Application.Interactors;

public class EmployeeIntreractor(IEmployeeDao employeeDao) : IDisposable, IAsyncDisposable
{
    public async Task<bool> TryLogin(string login, string password)
    {
        var employee = await employeeDao.TryGetByLogin(login);

        return employee != null && employee.Hash(password) == employee.Password;
    }

    public async Task<bool> TryEditWork(string login, string projectUrl, bool projectCompleted) =>
        await employeeDao.TryUpdate(login, e =>
        {
            e.CurrentProjectUrl = projectUrl;
            e.ProjectCompleted = projectCompleted;
        });

    public async Task<WorkDto?> TryGetWork(string login)
    {
        var employee = await employeeDao.TryGetByLogin(login);
        
        return EmployeeMapper.EmployeeToWorkDto(employee);
    }
    
    public async Task<SalaryDto?> TryGetSalary(string login)
    {
        var employee = await employeeDao.TryGetByLogin(login);
        
        return EmployeeMapper.EmployeeToSalaryDto(employee);
    }

    public void Dispose()
    {
        employeeDao.Dispose();
    }

    public async ValueTask DisposeAsync()
    {
        await employeeDao.DisposeAsync();
    }
}