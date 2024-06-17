using Domain.Entities;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Dal.Daos;

public class EmployeeDao(HrmSystemContext dbContext) : IEmployeeDao
{
    public async Task<Employee> GetByLogin(string login) =>
        await dbContext.Employees.AsNoTracking().FirstAsync(u => u.Login == login);

    public async Task Update(string login, Action<Employee> updater)
    {
        var oldEmployee = await GetByLogin(login);
        var newEmployee = new Employee(oldEmployee);
        updater(newEmployee);

        if (oldEmployee.Login != newEmployee.Login)
        {
            await dbContext.Employees.AddAsync(newEmployee);
            dbContext.Employees.Remove(oldEmployee);
        }
        else
            dbContext.Employees.Update(newEmployee);

        await dbContext.SaveChangesAsync();
    }

    public void Dispose()
    {
        dbContext.Dispose();
    }

    public async ValueTask DisposeAsync()
    {
        await dbContext.DisposeAsync();
    }
}