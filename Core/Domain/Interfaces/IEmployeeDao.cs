using Domain.Entities;

namespace Domain.Interfaces;

public interface IEmployeeDao : IDisposable, IAsyncDisposable
{
    Task<Employee> GetByLogin(string login);

    Task Update(string login, Action<Employee> updater);
}