using Project.Api.Dtos;

public interface IProjectEmployeeService
{
    List<EmployeeDto> GetEmployees(int projectId);

    bool AssignEmployee(int projectId, AssignEmployeeDto dto);

    bool RemoveEmployee(int projectId, int userId);
}