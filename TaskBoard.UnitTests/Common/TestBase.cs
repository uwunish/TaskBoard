using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using TaskBoard.Application.Common.Interfaces;

namespace TaskBoard.UnitTests.Common
{
    // every test class will inherit this to get prebuilt mocks
    public abstract class TestBase
    {
        protected readonly Mock<IBoardRepository> MockBoardRepo = new();
        protected readonly Mock<ICardRepository> MockCardRepo = new();
        protected readonly Mock<IUserRepository> MockUserRepo = new();
        protected readonly Mock<IBoardHubService> MockHub = new();
        protected readonly Mock<ITokenService> MockTokens = new();
        protected readonly Mock<IPasswordHasher> MockHasher = new();
    }
}
