using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace TaskBoard.Application.Auth.Commands
{
    public class RegisterValidator : AbstractValidator<RegisterCommand>
    {
        public RegisterValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty()
                .EmailAddress().WithMessage("A valid email is required");

            RuleFor(x => x.Password)
                .NotEmpty()
                .MinimumLength(8).WithMessage("Password must be atleast 8 characters.")
                .Matches("[A-Z]").WithMessage("Password must contain an uppercase letter.")
                .Matches("[0-9]").WithMessage("Password must contain a number.");

            RuleFor(x => x.DisplayName)
                .NotEmpty()
                .MaximumLength(100);
        }
    }
}
