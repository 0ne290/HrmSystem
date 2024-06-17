using Domain.Entities;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Dal.Daos;

public class EmployeeDao : IEmployeeDao
{
    public EmployeeDao(HrmSystemContext dbContext)
    {
        _dbContext = dbContext;
        _dbContext.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;// Способ не отслеживать по умолчанию объекты получаемых сущностей и избавиться от постоянных AsNoTracking()
    }
    
    public async Task<Employee> GetByLogin(string login) =>
        await _dbContext.Employees.FirstAsync(u => u.Login == login);

    public async Task Update(string login, Action<Employee> updater)
    {
        var oldEmployee = await GetByLogin(login);
        var newEmployee = new Employee(oldEmployee);
        updater(newEmployee);

        if (oldEmployee.Login != newEmployee.Login)
        {
            await _dbContext.Employees.AddAsync(newEmployee);
            _dbContext.Employees.Remove(oldEmployee);
        }
        else
            _dbContext.Employees.Update(newEmployee);

        await _dbContext.SaveChangesAsync();
    }

    public void Dispose()
    {
        _dbContext.Dispose();
    }

    public async ValueTask DisposeAsync()
    {
        await _dbContext.DisposeAsync();
    }

    private readonly HrmSystemContext _dbContext;
}