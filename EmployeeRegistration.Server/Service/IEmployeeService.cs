using EmployeeRegistration.Server.Model;

namespace EmployeeRegistration.Server.Service
{
    public interface IEmployeeService
    {
        Task<int> GetNextEmployeeIdAsync();
        Task<bool> CheckMobileExistsAsync(string mobile, int? excludeEmployeeId = null);
        Task<(List<Employee> employees, int totalCount)> GetEmployeesAsync(string? name = null, string? mobile = null, int page = 1, int pageSize = 5);
        Task<Employee?> GetEmployeeByIdAsync(int id);
        Task<Employee> CreateEmployeeAsync(Employee employee);
        Task<bool> UpdateEmployeeAsync(int id, Employee employee);
        Task<bool> DeleteEmployeeAsync(int id);
        Task<List<Country>> GetCountriesAsync();
        Task<List<State>> GetStatesByCountryAsync(int countryId);
    }
}