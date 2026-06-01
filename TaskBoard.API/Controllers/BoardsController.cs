using MediatR;
using Microsoft.AspNetCore.Mvc;
using TaskBoard.Application.Boards.Commands;
using TaskBoard.Application.Boards.Queries;
using TaskBoard.Application.Common.Exceptions;

namespace TaskBoard.API.Controllers
{
    public class BoardsController : ApiControllerBase
    {
        public BoardsController(IMediator mediator) : base(mediator) { }

        // GET /api/boards - list my boards
        [HttpGet]
        public async Task<IActionResult> GetMyBoards()
        {
            var boards = await _mediator.Send(new GetMyBoardsQuery(GetUserId()));
            return Ok(boards);
        }

        // GET /api/boards/{id} - get a board with all its columns and cards
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetBoard(Guid id)
        {
            try
            {
            var board = await _mediator.Send(new GetBoardQuery(id, GetUserId()));
            return Ok(board);
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

        // POST /api/boards - create a new board
        [HttpPost]
        public async Task<IActionResult> CreateBoard(CreateBoardRequest request)
        {
            var command = new CreateBoardCommand(request.Name, request.Description, GetUserId());
            var id = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetBoard), new { id }, new {id});
        }

    }
}

// Separate request model - keeps the command clean (no OwnerId from body)
public record CreateBoardRequest(string Name, string? Description);
