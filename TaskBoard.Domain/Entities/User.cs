using System;
using System.Collections.Generic;
using System.Text;

namespace TaskBoard.Domain.Entities
{
	public class User : Common.BaseEntity
	{
		public string Email { get; private set; } = string.Empty;
		public string PasswordHash { get; private set; } = string.Empty;
		public string DisplayName { get; private set; } = string.Empty;
		public string? RefreshToken { get; private set; }
		public DateTime? RefreshTokenExpiry { get; private set; }

		private User() { }

		public static User Create(string email, string displayName, string passwordHash)
		{
			return new User
			{
				Email = email.ToLowerInvariant().Trim(),
				DisplayName = displayName.Trim(),
				PasswordHash = passwordHash
			};
		}

		public void SetRefreshToken(string token, DateTime expiry)
		{
			RefreshToken = token;
			RefreshTokenExpiry = expiry;
		}

		public void RevokeRefreshToken()
		{
			RefreshToken = null;
			RefreshTokenExpiry = null;
		}

		public bool IsRefreshTokenValid(string token)
		{
			return RefreshToken == token && RefreshTokenExpiry > DateTime.UtcNow;
		}
	}
}
