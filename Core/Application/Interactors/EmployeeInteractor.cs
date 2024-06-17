using Application.Dtos;
using Application.Mappers;
using Domain.Interfaces;

namespace Application.Interactors;

public class EmployeeIntreractor(IEmployeeDao employeeDao) : IDisposable, IAsyncDisposable
{
    public async Task<bool> TryLogin(string login, string password)
    {
        try
        { 
            var employee = await employeeDao.GetByLogin(login);
            
            return employee.Hash(password) == employee.Password;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public async Task EditWork(string login, string projectUrl, bool projectCompleted) =>
        await employeeDao.Update(login, e =>
        {
            e.CurrentProjectUrl = projectUrl;
            e.ProjectCompleted = projectCompleted;
        });

    public async Task<WorkDto> GetWork(string login)
    {
        var employee = await employeeDao.GetByLogin(login);
        
        return EmployeeMapper.EmployeeToWorkDto(employee);
    }
    
    public async Task<SalaryDto> GetSalary(string login)
    {
        var employee = await employeeDao.GetByLogin(login);
        
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