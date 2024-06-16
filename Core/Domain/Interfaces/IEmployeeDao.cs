using Domain.Entities;

namespace Domain.Interfaces;

public interface IEmployeeDao : IDisposable, IAsyncDisposable
{
    Task<Employee?> TryGetByLogin(string login);

    Task<bool> TryUpdate(string login, Action<Employee> updater);
}