using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace TaskBoard.API.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public abstract class ApiControllerBase : ControllerBase 
    {
        protected readonly IMediator _mediator;

        protected ApiControllerBase(IMediator mediator)
        {
            _mediator = mediator;
        }

        // reads the user ID from the JWT claim
        protected Guid GetUserId()
        {
            var value = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if(value is null)
            {
                value = User.FindFirstValue("sub");
            }

            return Guid.TryParse(value, out var id)
                ? id : throw new UnauthorizedAccessException("Invalid user token.");
        }
    }
}
