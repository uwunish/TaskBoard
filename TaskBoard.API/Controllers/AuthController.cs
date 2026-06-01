using MediatR;
using Microsoft.AspNetCore.Mvc;
using TaskBoard.Application.Auth.Commands;

namespace TaskBoard.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AuthController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterCommand command)
        {
            try
            {
                var result = await _mediator.Send(command);
                return Ok(result);
            }
            catch (InvalidOperationException ee)
            {
                return Conflict(new { message = ee.Message });
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginCommand command)
        {
            try
            {
                var result = await _mediator.Send(command);
                return Ok(result);
            }
            catch (UnauthorizedAccessException ee)
            {
                return Unauthorized(new { message = ee.Message });
            }
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh(RefreshTokenCommand command)
        {
            try
            {
                var result = await _mediator.Send(command);
                return Ok(result);
            }
            catch (UnauthorizedAccessException ee)
            {
                return Unauthorized(new { message = ee.Message });
            }
        }


    }
}
