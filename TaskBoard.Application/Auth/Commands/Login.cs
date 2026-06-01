using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using TaskBoard.Application.Common.Interfaces;

namespace TaskBoard.Application.Auth.Commands
{
    public record LoginCommand(string Email, string Password) : IRequest<AuthResult>;
    public class LoginHandler : IRequestHandler<LoginCommand, AuthResult>
    {
        private readonly IUserRepository _users;
        private readonly ITokenService _tokens;
        private readonly IPasswordHasher _hasher;

        public LoginHandler(IUserRepository users, ITokenService tokens, IPasswordHasher hasher)
        {
            _users = users;
            _tokens = tokens;
            _hasher = hasher;
        }

        public async Task<AuthResult> Handle(LoginCommand request, CancellationToken ct)
        {
            var user = await _users.GetByEmailAsync(request.Email, ct);
            if(user is null)
            {
                throw new UnauthorizedAccessException("Invalid email or password.");
            }

            var passwordValid = _hasher.Verify(request.Password, user.PasswordHash);
            if(!passwordValid)
            {
                throw new UnauthorizedAccessException("Invalid email or password.");
            }

            var refreshToken = _tokens.GenerateRefreshToken();
            user.SetRefreshToken(refreshToken, DateTime.UtcNow.AddDays(7));
            await _users.UpdateAsync(user, ct);

            return new AuthResult(
                _tokens.GenerateAccessToken(user),
                refreshToken,
                user.Id,
                user.DisplayName
                );
        }
    }
}
