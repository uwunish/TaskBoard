using FluentValidation;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace TaskBoard.Application.Common.Behaviours
{
    public class ValidationBehaviour<TRequest, TResponse>
        : IPipelineBehavior<TRequest, TResponse>
        where TRequest : notnull
    {
        private readonly IEnumerable<IValidator<TRequest>> _validators;

        public ValidationBehaviour(IEnumerable<IValidator<TRequest>> validators)
        {
            _validators = validators;
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken ct)
        {
            if(!_validators.Any())
            {
                return await next();
            }

            var context = new ValidationContext<TRequest>(request);

            var failures = _validators
                .Select(x => x.Validate(context))
                .SelectMany(y => y.Errors)
                .Where(z => z is not null)
                .ToList();

            if(failures.Any())
            {
                throw new ValidationException(failures);
            }

            return await next();

        }
    }
}
