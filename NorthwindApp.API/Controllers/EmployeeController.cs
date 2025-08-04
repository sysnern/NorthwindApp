using Microsoft.AspNetCore.Mvc;
using NorthwindApp.Business.Services.Abstract;
using NorthwindApp.Core.DTOs;
using NorthwindApp.Core.Results;

namespace NorthwindApp.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmployeeController : ControllerBase
    {
        private readonly IEmployeeService _employeeService;

        public EmployeeController(IEmployeeService employeeService)
        {
            _employeeService = employeeService;
        }

        [HttpGet("list")]
        public async Task<ActionResult<ApiResponse<List<EmployeeDTO>>>> GetAll([FromQuery] EmployeeFilterDto? filter = null)
        {
            var result = await _employeeService.GetAllAsync(filter);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<EmployeeDTO>>> GetById(int id)
        {
            var result = await _employeeService.GetByIdAsync(id);
            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<string>>> Add([FromBody] EmployeeCreateDto dto)
        {
            var result = await _employeeService.AddAsync(dto);
            return Ok(result);
        }

        [HttpPut]
        public async Task<ActionResult<ApiResponse<string>>> Update([FromBody] EmployeeUpdateDto dto)
        {
            var result = await _employeeService.UpdateAsync(dto);
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse<string>>> Delete(int id)
        {
            var result = await _employeeService.DeleteAsync(id);
            return Ok(result);
        }
    }
}
