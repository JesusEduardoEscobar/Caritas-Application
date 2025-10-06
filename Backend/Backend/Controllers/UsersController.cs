using Backend.Infraestructure.Implementations;
using Backend.Infraestructure.Interfaces;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController: ControllerBase
    {
        private readonly IUsers _userService;

        public UsersController(IUsers userService)
        {
            _userService = userService;
        }

        [HttpPost("login")]
        //[ProducesResponseType(typeof(GlobalResponse<IEnumerable<dynamic>>), 200)]
        //[ProducesResponseType(typeof(GlobalResponse<IEnumerable<dynamic>>), 401)]
        //[ProducesResponseType(typeof(GlobalResponse<IEnumerable<dynamic>>), 500)]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            try
            {
                var response = await _userService.LoginUser(request.Email, request.Password);
                if (response == null || response.Data == null)
                {
                    return BadRequest();
                }
                return Ok(response);
            }
            catch (Exception ex)
            {
                var errorResponse = GlobalResponse<IEnumerable<dynamic>>.Fault(ex.Message, "-1", null);
                return StatusCode(500, errorResponse);
            }

        }
    }
}
