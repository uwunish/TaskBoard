using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using TaskBoard.Application.Common.Interfaces;
using TaskBoard.Domain.Entities;

namespace TaskBoard.Application.Auth.Commands
{
    public record RegisterCommand(
        string Email,
        string Password,
        string DisplayName
        ) : IRequest<AuthResult>;

    public record AuthResult(
        string AccessToken,
        string RefreshToken,
        Guid UserId,
        string DisplayName
        );

    public class RegisterHandler : IRequestHandler<RegisterCommand, AuthResult>
    {
        private readonly IUserRepository _users;
        private readonly ITokenService _tokens;
        private readonly IPasswordHasher _hasher;

        public RegisterHandler(IUserRepository users, ITokenService tokens, IPasswordHasher hasher)
        {
            _users = users;
            _tokens = tokens;
            _hasher = hasher;
        }

        public async Task<AuthResult> Handle(RegisterCommand request, CancellationToken ct)
        {
            // Check email isn't already taken
            var existing = await _users.GetByEmailAsync(request.Email, ct);
            if (existing is not null)
            {
                throw new InvalidOperationException("Email is already registered");
            }

            var passwordHash = _hasher.Hash(request.Password);
            var user = User.Create(request.Email, request.DisplayName,  passwordHash);

            var refreshToken = _tokens.GenerateRefreshToken();
            user.SetRefreshToken(refreshToken, DateTime.UtcNow.AddDays(7));

            await _users.AddAsync(user, ct);

            return new AuthResult(
                _tokens.GenerateAccessToken(user),
                refreshToken,
                user.Id,
                user.DisplayName
                );

        }
    }
}
