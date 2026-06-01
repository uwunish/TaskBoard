using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace TaskBoard.Application.Cards.Commands
{
    public class CreateCardValidator : AbstractValidator<CreateCardCommand>
    {
        public CreateCardValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty()
                .MaximumLength(200);
            RuleFor(x => x.ColumnId)
                .NotEmpty();
            RuleFor(x => x.BoardId)
                .NotEmpty();
            RuleFor(x => x.CreatedByUserId)
                .NotEmpty();
            RuleFor(x => x.Description)
                .MaximumLength(2000).When(x => x.Description is not null);
        }
    }
}
