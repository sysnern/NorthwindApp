using Microsoft.AspNetCore.Mvc;
using NorthwindApp.Business.Services.Abstract;
using NorthwindApp.Core.DTOs;
using NorthwindApp.Core.Results;

namespace NorthwindApp.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CustomerController : ControllerBase
    {
        private readonly ICustomerService _customerService;

        public CustomerController(ICustomerService customerService)
        {
            _customerService = customerService;
        }

        [HttpGet("list")]
        public async Task<ActionResult<ApiResponse<List<CustomerDTO>>>> GetAll([FromQuery] CustomerFilterDto? filter = null)
        {
            var result = await _customerService.GetAllAsync(filter);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<CustomerDTO>>> GetById(string id)
        {
            var result = await _customerService.GetByIdAsync(id);
            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<string>>> Add([FromBody] CustomerCreateDto dto)
        {
            var result = await _customerService.AddAsync(dto);
            return Ok(result);
        }

        [HttpPut]
        public async Task<ActionResult<ApiResponse<string>>> Update([FromBody] CustomerUpdateDto dto)
        {
            var result = await _customerService.UpdateAsync(dto);
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse<string>>> Delete(string id)
        {
            var result = await _customerService.DeleteAsync(id);
            return Ok(result);
        }
    }
}
