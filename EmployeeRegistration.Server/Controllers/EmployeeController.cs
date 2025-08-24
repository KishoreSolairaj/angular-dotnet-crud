using EmployeeRegistration.Server.Model;
using EmployeeRegistration.Server.Service;
using EmployeeRegistration.Server.DTOs; // <-- Import DTO
using Microsoft.AspNetCore.Mvc;

namespace EmployeeRegistration.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly IEmployeeService _employeeService;

        public EmployeeController(IEmployeeService employeeService)
        {
            _employeeService = employeeService;
        }

        [HttpGet("next-id")]
        public async Task<ActionResult<int>> GetNextId()
        {
            var nextId = await _employeeService.GetNextEmployeeIdAsync();
            return Ok(nextId);
        }

        [HttpGet("check-mobile/{mobile}")]
        public async Task<ActionResult<bool>> CheckMobile(string mobile, [FromQuery] int? excludeId = null)
        {
            var exists = await _employeeService.CheckMobileExistsAsync(mobile, excludeId);
            return Ok(exists);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<EmployeeDto>>> GetEmployees(
            [FromQuery] string? name = null,
            [FromQuery] string? mobile = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 5)
        {
            var (employees, totalCount) = await _employeeService.GetEmployeesAsync(name, mobile, page, pageSize);

            // Add headers for pagination
            Response.Headers.Add("X-Total-Count", totalCount.ToString());
            Response.Headers.Add("X-Total-Pages", Math.Ceiling((double)totalCount / pageSize).ToString());

            // Map to DTOs
            var dtos = employees.Select(MapToDto).ToList();
            return Ok(dtos);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<EmployeeDto>> GetById(int id)
        {
            var employee = await _employeeService.GetEmployeeByIdAsync(id);
            if (employee == null)
                return NotFound();

            var dto = MapToDto(employee);
            return Ok(dto);
        }

        [HttpPost]
        public async Task<ActionResult<EmployeeDto>> Create([FromBody] Employee employee)
        {
            if (await _employeeService.CheckMobileExistsAsync(employee.MobileNum))
            {
                return BadRequest(new { message = "Already registered user. Please enter a new one." });
            }

            try
            {
                var createdEmployee = await _employeeService.CreateEmployeeAsync(employee);
                var dto = MapToDto(createdEmployee);
                return CreatedAtAction(nameof(GetById), new { id = dto.EmployeeId }, dto);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Update(int id, [FromBody] Employee employee)
        {
            var existingEmployee = await _employeeService.GetEmployeeByIdAsync(id);
            if (existingEmployee == null)
            {
                return NotFound();
            }

            if (await _employeeService.CheckMobileExistsAsync(employee.MobileNum, id))
            {
                return BadRequest(new { message = "Mobile number already in use by another employee." });
            }

            try
            {
                var success = await _employeeService.UpdateEmployeeAsync(id, employee);
                return success ? NoContent() : BadRequest(new { message = "Failed to update employee." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var success = await _employeeService.DeleteEmployeeAsync(id);
            return success ? NoContent() : NotFound();
        }

        // 🔁 Helper Method to map entity to DTO
        private EmployeeDto MapToDto(Employee employee)
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
