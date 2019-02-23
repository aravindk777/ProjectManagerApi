using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using PM.Api.Controllers;
using PM.BL.Common;
using PM.BL.Users;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http.Results;

namespace PM.Api.UnitTests.Controllers
{
    [TestFixture]
    public class UserApiTest
    {
        private Mock<IUserLogic> mockUserApiLogicLayer;
        private ILogger<UsersController> loggerInstance;
        private UsersController mockController;
        private List<Models.ViewModels.User> mockUsersList;


        [SetUp]
        public void SetupTesting()
        {
            mockUserApiLogicLayer = new Mock<IUserLogic>();
            loggerInstance = new Microsoft.Extensions.Logging.LoggerFactory().CreateLogger<UsersController>();
            mockController = new UsersController(mockUserApiLogicLayer.Object, loggerInstance);

            // Create mock users
            mockUsersList = new List<Models.ViewModels.User>();
            for (int iCounter = 0; iCounter < 10; iCounter++)
            {
                mockUsersList.Add(
                (new Models.DataModel.User
                {
                    FirstName = $"User{iCounter}First", LastName = $"User{iCounter}Last", UserId = $"TestUser{iCounter}",
                    Id = System.Guid.NewGuid(), Created = System.DateTime.Now.AddMonths(iCounter - 5), EndDate = System.DateTime.Today.AddDays(iCounter % 2)
                }).AsViewModel());
            }
        }

        [Test(Description = "Get All Users")]
        public void Test_GetAllUsers()
        {
            // Arrange
            mockUserApiLogicLayer.Setup(u => u.GetUsers(It.IsAny<bool>())).Returns(mockUsersList);

            // Act
            var result = mockController.GetAllUsers();
            var actualResult = ((OkNegotiatedContentResult<IEnumerable<PM.Models.ViewModels.User>>)result).Content;

            // Assert
            Assert.IsNotNull(actualResult);
            Assert.IsNotEmpty(actualResult);
            Assert.That(actualResult.Count(), Is.EqualTo(mockUsersList.Count()));
        }

        [Test(Description = "Get Only Active Users")]
        public void Test_GetActiveUsers()
        {
            // Arrange
            var activeUsersMockData = mockUsersList.Where(u => u.Active);
            mockUserApiLogicLayer.Setup(u => u.GetUsers(true)).Returns(activeUsersMockData);

            // Act
            var result = mockController.GetActiveUsers();
            var actualResult = ((OkNegotiatedContentResult<IEnumerable<PM.Models.ViewModels.User>>)result).Content;

            // Assert
            Assert.IsNotNull(actualResult);
            Assert.IsNotEmpty(actualResult);
            Assert.That(actualResult.Count(), Is.EqualTo(activeUsersMockData.Count()));
        }

        [Test(Description = "Create a new User")]
        [TestCase(Description = "Create a new User")]
        public void Test_CreateNewUser()
        {
            // Arrange
            var newTestUser = new Models.ViewModels.User() { FirstName = "TestUserAddFirst", LastName = "TestUserAddLastName", UserId = "TestUserN" };
            mockUserApiLogicLayer.Setup(api => api.AddUser(newTestUser)).Returns(newTestUser);

            // Act
            var addedUser = (mockController.Post(newTestUser) as OkNegotiatedContentResult<Models.ViewModels.User>).Content;

            // Assert
            Assert.IsNotNull(addedUser);
            Assert.AreEqual(newTestUser, addedUser);
        }
    }
}
