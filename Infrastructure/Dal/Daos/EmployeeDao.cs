using Domain.Entities;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Dal.Daos;

public class EmployeeDao(HrmSystemContext dbContext) : IEmployeeDao
{
    public async Task<Employee?> TryGetByLogin(string login) =>
        await dbContext.Employees.AsNoTracking().FirstOrDefaultAsync(u => u.Login == login);

    public async Task<bool> TryUpdate(string login, Action<Employee> updater)
    {
        try
        {
            var oldEmployee = await TryGetByLogin(login);
            if (oldEmployee == null)
                return false;

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

            return true;
        }
        catch (Exception)
        {
            return false;
        }
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