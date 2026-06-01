using System;
using System.Collections.Generic;
using System.Text;
using FluentValidation;

namespace TaskBoard.Application.Boards.Commands
{
    public class CreateBoardValidator : AbstractValidator<CreateBoardCommand>
    {
        public CreateBoardValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Board name is required")
                .MaximumLength(100).WithMessage("Board name cannot exceed 100 characters");

            RuleFor(x => x.OwnerId)
                .NotEmpty().WithMessage("Owner is required.");

            RuleFor(x => x.Description)
                .MaximumLength(500).When(x => x.Description is not null)
                .WithMessage("Description cannot exceed 500 characters");
        }
    }
}
