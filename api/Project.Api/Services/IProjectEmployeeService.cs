using Project.Api.Dtos;

namespace Project.Api.Services;

public interface IProjectEmployeeService
{
    List<EmployeeDto> GetEmployees(int projectId);

    bool AssignEmployee(int projectId, AssignEmployeeDto dto);

    bool RemoveEmployee(int projectId, string employeeId);
}