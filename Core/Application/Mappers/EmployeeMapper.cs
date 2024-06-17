using Application.Dtos;
using Domain.Entities;
using Riok.Mapperly.Abstractions;

namespace Application.Mappers;

[Mapper]
public static partial class EmployeeMapper
{
    public static partial SalaryDto EmployeeToSalaryDto(Employee employee);
    
    public static partial WorkDto EmployeeToWorkDto(Employee employee);
}