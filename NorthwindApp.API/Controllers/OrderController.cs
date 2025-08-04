using Microsoft.AspNetCore.Mvc;
using NorthwindApp.Business.Services.Abstract;
using NorthwindApp.Core.DTOs;
using NorthwindApp.Core.Results;

namespace NorthwindApp.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpGet("list")]
        public async Task<ActionResult<ApiResponse<List<OrderDTO>>>> GetAll([FromQuery] OrderFilterDto? filter = null)
        {
            var result = await _orderService.GetAllAsync(filter);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<OrderDTO>>> GetById(int id)
        {
            var result = await _orderService.GetByIdAsync(id);
            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<string>>> Add([FromBody] OrderCreateDto dto)
        {
            var result = await _orderService.AddAsync(dto);
            return Ok(result);
        }

        [HttpPut]
        public async Task<ActionResult<ApiResponse<string>>> Update([FromBody] OrderUpdateDto dto)
        {
            var result = await _orderService.UpdateAsync(dto);
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse<string>>> Delete(int id)
        {
            var result = await _orderService.DeleteAsync(id);
            return Ok(result);
        }
    }
}
