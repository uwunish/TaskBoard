using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using TaskBoard.Application.Common.Interfaces;

namespace TaskBoard.Application.Auth.Commands
{
    public record RefreshTokenCommand(Guid UserId, string RefreshToken) : IRequest<AuthResult>;
    public class RefreshTokenHandler : IRequestHandler<RefreshTokenCommand, AuthResult>
    {
        private readonly IUserRepository _users;
        private readonly ITokenService _tokens;

        public RefreshTokenHandler(IUserRepository users, ITokenService tokens)
        {
            _users = users;
            _tokens = tokens;

        }

        public async Task<AuthResult> Handle(RefreshTokenCommand request, CancellationToken ct)
        {
            var user = await _users.GetByIdAsync(request.UserId, ct);
            if(user is null)
            {
                throw new UnauthorizedAccessException("Invalid token.");
            }

            if(!user.IsRefreshTokenValid(request.RefreshToken))
            {
                throw new UnauthorizedAccessException("Refresh token is expired or invalid.");
            }

            // rotate the refresh token - old one is invalidated immediately
            var newRefreshToken = _tokens.GenerateRefreshToken();
            user.SetRefreshToken(newRefreshToken, DateTime.UtcNow.AddDays(7));
            await _users.UpdateAsync(user, ct);

            return new AuthResult(
                _tokens.GenerateAccessToken(user),
                newRefreshToken,
                user.Id,
                user.DisplayName
                );
        }
    }
}
