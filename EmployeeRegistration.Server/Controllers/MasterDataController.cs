using Microsoft.AspNetCore.Mvc;
using EmployeeRegistration.Server.Model;
using EmployeeRegistration.Server.Service;
namespace EmployeeRegistration.Server.Controllers

{
    [Route("api/[controller]")]
    [ApiController]
    public class MasterDataController : ControllerBase
    {
        private readonly IEmployeeService _employeeService;

        public MasterDataController(IEmployeeService employeeService)
        {
            _employeeService = employeeService;
        }

        [HttpGet("countries")]
        public async Task<ActionResult<IEnumerable<Country>>> GetCountries()
        {
            var countries = await _employeeService.GetCountriesAsync();
            return Ok(countries);
        }

        [HttpGet("states/{countryId}")]
        public async Task<ActionResult<IEnumerable<State>>> GetStatesByCountry(int countryId)
        {
            var states = await _employeeService.GetStatesByCountryAsync(countryId);
            return Ok(states);
        }
    }
}
