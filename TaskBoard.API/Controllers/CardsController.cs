using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using System.Runtime.CompilerServices;
using TaskBoard.Application.Cards.Commands;
using TaskBoard.Application.Common.Exceptions;

namespace TaskBoard.API.Controllers
{
    public class CardsController : ApiControllerBase
    {
        public CardsController(IMediator mediator) : base(mediator) { }

        // POST /api/cards - create a new card
        [HttpPost]
        public async Task<IActionResult> CreateCard(CreateCardRequest request)
        {
            try
            {
                //// Inject the authenticated user as the creator of the card
                //var commandWithUser = command with { CreatedByUserId = GetUserId() };
                //var id = await _mediator.Send(commandWithUser);
                //return Ok(new { id });

                var command = new CreateCardCommand(
                    request.Title,
                    request.Description,
                    request.ColumnId,
                    request.BoardId,
                    GetUserId()
                    );
                var id = await _mediator.Send(command);
                return Ok(new { id });
            }
            catch (NotFoundException ee)
            {
                return NotFound(new { message = ee.Message });
            }
        }


        // PUT /api/cards/{id}/move - move a card (triggers SignalR broadcast)
        [HttpPut("{id:guid}/move")]
        public async Task<IActionResult> MoveCard(Guid id, MoveCardRequest request)
        {
            try
            {
                var command = new MoveCardCommand(
                    id,
                    request.TargetColumnId,
                    request.NewPosition,
                    request.BoardId,
                    GetUserId()
                    );
                await _mediator.Send(command);
                return NoContent(); // 204 - success, nothing to return
            }
            catch (NotFoundException ee)
            {
                return NotFound(new { message = ee.Message });
            }
            catch(ForbiddenException)
            {
                return Forbid();
            }
        }
    }
}

public record CreateCardRequest(string Title, string? Description, Guid ColumnId, Guid BoardId);
public record MoveCardRequest(Guid TargetColumnId, int NewPosition, Guid BoardId);
