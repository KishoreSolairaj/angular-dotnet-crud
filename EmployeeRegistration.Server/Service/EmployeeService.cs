using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using EmployeeRegistration.Server;
using EmployeeRegistration.Server.Model;
using System.Data;
using EmployeeRegistration.Server.DTOs;

namespace EmployeeRegistration.Server.Service
{
    public class EmployeeService : IEmployeeService
    {
        private readonly AppDbContext _context;
        private readonly ILogger<EmployeeService> _logger;
        public EmployeeService(AppDbContext context, ILogger<EmployeeService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<int> GetNextEmployeeIdAsync()
        {
            var nextIdParam = new SqlParameter("@NextId", SqlDbType.Int) { Direction = ParameterDirection.Output };

            await _context.Database.ExecuteSqlRawAsync("EXEC SP_GetNextEmployeeId @NextId OUTPUT", nextIdParam);

            return (int)nextIdParam.Value;
        }

        public async Task<bool> CheckMobileExistsAsync(string mobile, int? excludeEmployeeId = null)
        {
            try
            {
                var mobileParam = new SqlParameter("@Mobile", mobile);

                var excludeIdParam = new SqlParameter
                {
                    ParameterName = "@ExcludeEmployeeId",
                    SqlDbType = SqlDbType.Int,
                    Direction = ParameterDirection.Input,
                    Value = excludeEmployeeId.HasValue ? (object)excludeEmployeeId.Value : DBNull.Value
                };

                var existsParam = new SqlParameter
                {
                    ParameterName = "@Exists",
                    SqlDbType = SqlDbType.Bit,
                    Direction = ParameterDirection.Output
                };

                await _context.Database.ExecuteSqlRawAsync(
                    "EXEC SP_CheckMobileExists @Mobile, @ExcludeEmployeeId, @Exists OUTPUT",
                    mobileParam, excludeIdParam, existsParam);

                return (bool)existsParam.Value;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking mobile existence: {Mobile}", mobile);
                throw;
            }
        }


        public async Task<Employee?> GetEmployeeByIdAsync(int id)
        {
            var idParam = new SqlParameter("@EmployeeId", id);

            var employees = await _context.Employees
                .FromSqlRaw("EXEC SP_GetEmployeeById @EmployeeId", idParam)
                .ToListAsync(); // Remove AsEnumerable(), just use ToListAsync() directly

            var employee = employees.FirstOrDefault();

            if (employee != null)
            {
                // Load related data separately if needed
                employee.State = await _context.States.FirstOrDefaultAsync(s => s.StateId == employee.StateId);
                employee.Country = await _context.Countries.FirstOrDefaultAsync(c => c.CountryId == employee.CountryId);
            }

            return employee;
        }

        public async Task<(List<Employee> employees, int totalCount)> GetEmployeesAsync(string? name = null, string? mobile = null, int page = 1, int pageSize = 5)
        {
            var nameParam = new SqlParameter("@Name", name ?? (object)DBNull.Value);
            var mobileParam = new SqlParameter("@Mobile", mobile ?? (object)DBNull.Value);
            var pageParam = new SqlParameter("@Page", page);
            var pageSizeParam = new SqlParameter("@PageSize", pageSize);
            var totalCountParam = new SqlParameter("@TotalCount", SqlDbType.Int) { Direction = ParameterDirection.Output };

            var employees = await _context.Employees
                .FromSqlRaw("EXEC SP_GetEmployees @Name, @Mobile, @Page, @PageSize, @TotalCount OUTPUT",
                    nameParam, mobileParam, pageParam, pageSizeParam, totalCountParam)
                .ToListAsync(); // Remove AsEnumerable(), just use ToListAsync() directly

            var totalCount = (int)totalCountParam.Value;

            // Load related data separately if needed
            if (employees.Any())
            {
                var stateIds = employees.Select(e => e.StateId).Distinct().ToList();
                var countryIds = employees.Select(e => e.CountryId).Distinct().ToList();

                var states = await _context.States.Where(s => stateIds.Contains(s.StateId)).ToListAsync();
                var countries = await _context.Countries.Where(c => countryIds.Contains(c.CountryId)).ToListAsync();

                foreach (var employee in employees)
                {
                    employee.State = states.FirstOrDefault(s => s.StateId == employee.StateId);
                    employee.Country = countries.FirstOrDefault(c => c.CountryId == employee.CountryId);
                }
            }

            return (employees, totalCount);
        }
        public async Task<Employee> CreateEmployeeAsync(Employee employee)
        {
            try
            {
                var parameters = new[]
                {
            new SqlParameter("@EmployeeName", employee.EmployeeName),
            new SqlParameter("@Age", employee.Age),
            new SqlParameter("@MobileNum", employee.MobileNum),
            new SqlParameter("@Pincode", employee.Pincode),
            new SqlParameter("@DOB", employee.DOB ?? (object)DBNull.Value),
            new SqlParameter("@AddressLine1", employee.AddressLine1),
            new SqlParameter("@AddressLine2", employee.AddressLine2 ?? (object)DBNull.Value),
            new SqlParameter("@StateId", employee.StateId),
            new SqlParameter("@CountryId", employee.CountryId),
            new SqlParameter
            {
                ParameterName = "@EmployeeId",
                SqlDbType = SqlDbType.Int,
                Direction = ParameterDirection.Output
            }
        };

                _logger.LogInformation("Creating employee: {EmployeeName}", employee.EmployeeName);

                await _context.Database.ExecuteSqlRawAsync(
                    "EXEC SP_CreateEmployee @EmployeeName, @Age, @MobileNum, @Pincode, @DOB, @AddressLine1, @AddressLine2, @StateId, @CountryId, @EmployeeId OUTPUT",
                    parameters);

                int newEmployeeId = (int)parameters[9].Value;
                employee.EmployeeId = newEmployeeId;

                _logger.LogInformation("Created employee with ID: {EmployeeId}", newEmployeeId);

                // Optionally, fetch complete employee with related data
                var createdEmployee = await GetEmployeeByIdAsync(newEmployeeId);
                return createdEmployee ?? employee;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating employee: {EmployeeName}", employee.EmployeeName);
                throw; // or handle accordingly
            }
        }

        public async Task<bool> UpdateEmployeeAsync(int id, Employee employee)
        {
            try
            {
                var parameters = new[]
                {
            new SqlParameter("@EmployeeId", id),
            new SqlParameter("@EmployeeName", employee.EmployeeName),
            new SqlParameter("@Age", employee.Age),
            new SqlParameter("@MobileNum", employee.MobileNum),
            new SqlParameter("@Pincode", employee.Pincode),
            new SqlParameter("@DOB", employee.DOB ?? (object)DBNull.Value),
            new SqlParameter("@AddressLine1", employee.AddressLine1),
            new SqlParameter("@AddressLine2", employee.AddressLine2 ?? (object)DBNull.Value),
            new SqlParameter("@StateId", employee.StateId),
            new SqlParameter("@CountryId", employee.CountryId),
            new SqlParameter
            {
                ParameterName = "@Success",
                SqlDbType = SqlDbType.Bit,
                Direction = ParameterDirection.Output
            }
        };

                _logger.LogInformation("Updating employee ID: {EmployeeId}", id);

                await _context.Database.ExecuteSqlRawAsync(
                    "EXEC SP_UpdateEmployee @EmployeeId, @EmployeeName, @Age, @MobileNum, @Pincode, @DOB, @AddressLine1, @AddressLine2, @StateId, @CountryId, @Success OUTPUT",
                    parameters);

                bool success = (bool)parameters[10].Value;

                _logger.LogInformation("Update employee ID: {EmployeeId} success: {Success}", id, success);

                return success;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating employee ID: {EmployeeId}", id);
                throw; // or handle accordingly
            }
        }

        public async Task<bool> DeleteEmployeeAsync(int id)
        {
            var idParam = new SqlParameter("@EmployeeId", id);
            var successParam = new SqlParameter("@Success", SqlDbType.Bit) { Direction = ParameterDirection.Output };

            await _context.Database.ExecuteSqlRawAsync("EXEC SP_DeleteEmployee @EmployeeId, @Success OUTPUT", idParam, successParam);

            return (bool)successParam.Value;
        }

        public async Task<List<Country>> GetCountriesAsync()
        {
            return await _context.Countries
                .FromSqlRaw("EXEC SP_GetCountries")
                .ToListAsync();
        }

        public async Task<List<State>> GetStatesByCountryAsync(int countryId)
        {
            var countryIdParam = new SqlParameter("@CountryId", countryId);

            return await _context.States
                .FromSqlRaw("EXEC SP_GetStatesByCountry @CountryId", countryIdParam)
                .ToListAsync();
        }

        public EmployeeDto MapToDto(Employee employee)
        {
            return new EmployeeDto
            {
                EmployeeId = employee.EmployeeId,
                EmployeeName = employee.EmployeeName,
                Age = employee.Age,
                MobileNum = employee.MobileNum,
                Pincode = employee.Pincode,
                DOB = employee.DOB,
                AddressLine1 = employee.AddressLine1,
                AddressLine2 = employee.AddressLine2,
                StateId = employee.StateId,
                StateName = employee.State?.StateName,
                CountryId = employee.CountryId,
                CountryName = employee.Country?.CountryName
            };
        }
    }
}