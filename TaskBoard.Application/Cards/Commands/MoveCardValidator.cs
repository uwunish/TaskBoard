using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace TaskBoard.Application.Cards.Commands
{
    public class MoveCardValidator : AbstractValidator<MoveCardCommand>
    {
        public MoveCardValidator()
        {
            RuleFor(x => x.CardId).NotEmpty().WithMessage("Card name is required");
            RuleFor(x => x.TargetColumnId).NotEmpty();
            RuleFor(x => x.BoardId).NotEmpty();
            RuleFor(x => x.NewPosition).GreaterThanOrEqualTo(0);
        }
    }
}
