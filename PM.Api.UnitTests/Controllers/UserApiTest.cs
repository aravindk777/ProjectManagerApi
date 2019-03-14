using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using PM.Api.Controllers;
using PM.BL.Common;
using PM.BL.Users;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Http.ModelBinding;
using System.Web.Http.Results;

namespace PM.Api.UnitTests.Controllers
{
    [TestFixture]
    public class UserApiTest
    {
        #region Test data for Setup
        private Mock<IUserLogic> mockUserApiLogicLayer;
        private Mock<ILogger<UsersController>> loggerInstance;
        private UsersController mockController;
        private List<Models.ViewModels.User> mockUsersList = new List<Models.ViewModels.User>() { };
        #endregion

        #region SETUP
        [SetUp]
        public void SetupTesting()
        {
            mockUserApiLogicLayer = new Mock<IUserLogic>();
            loggerInstance = new Mock<ILogger<UsersController>>(MockBehavior.Loose);
            mockController = new UsersController(mockUserApiLogicLayer.Object, loggerInstance.Object);

            // Create mock users
            mockUsersList = new List<Models.ViewModels.User>();
            for (int iCounter = 0; iCounter < 10; iCounter++)
            {
                mockUsersList.Add(
                (new Models.DataModel.User
                {
                    FirstName = $"User{iCounter}First",
                    LastName = $"User{iCounter}Last",
                    UserId = $"TestUser{iCounter}",
                    Id = System.Guid.NewGuid(),
                    Created = System.DateTime.Now.AddMonths(iCounter - 5),
                    EndDate = System.DateTime.Today.AddDays(iCounter % 2)
                }).AsViewModel());
            }
        }
        #endregion

        #region Get All Users
        [Test(Description = "Get All Users")]
        [TestCase(TestName = "Get All Users")]
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

        [Test(Description = "Get All Users")]
        [TestCase(TestName = "Test for Get All Throwing Exception")]
        public void Test_GetAllUsers_Throws_Exception()
        {
            // Arrange
            var expectedErrMsg = "Test for Exception";
            mockUserApiLogicLayer.Setup(u => u.GetUsers(It.IsAny<bool>())).Throws(new System.Exception(expectedErrMsg));

            // Act
            var result = mockController.GetAllUsers();
            var actualResult = ((ExceptionResult)result).Exception;

            // Assert
            Assert.IsNotNull(actualResult);
            Assert.IsInstanceOf(typeof(System.Exception), actualResult);
            Assert.AreEqual(expectedErrMsg, actualResult.Message);
        }
        #endregion

        #region Get Active Users
        [Test(Description = "Get Only Active Users")]
        [TestCase(TestName = "Get Only Active Users")]
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

        [Test(Description = "Get Only Active Users")]
        [TestCase(TestName = "Test for Get Active Users Throwing Exception")]
        public void Test_GetActiveUsers_ThrowsException()
        {
            // Arrange
            var activeUsersMockData = mockUsersList.Where(u => u.Active);
            var expectedErrMsg = "No data causing Null Reference during Where filter";
            mockUserApiLogicLayer.Setup(u => u.GetUsers(true)).Throws(new System.NullReferenceException(expectedErrMsg));

            // Act
            var result = mockController.GetActiveUsers();
            var actualResult = ((ExceptionResult)result).Exception;

            // Assert
            Assert.IsNotNull(actualResult);
            Assert.IsInstanceOf(typeof(System.Exception), actualResult);
            Assert.AreEqual(expectedErrMsg, actualResult.Message);
        }
        #endregion

        #region POST
        [Test]
        [TestCase(Description = "Create a new User", TestName = "Test Create valid User returns Ok result")]
        public void Test_Post_NewUser_Valid()
        {
            // Arrange
            var newTestUser = new Models.ViewModels.User() { FirstName = "TestUserAddFirst", LastName = "TestUserAddLastName", UserId = "TestUserNew1" };
            var expectedTestResult = new OkNegotiatedContentResult<Models.ViewModels.User>(newTestUser, mockController);
            mockUserApiLogicLayer.Setup(api => api.AddUser(newTestUser)).Returns(newTestUser);

            // Act
            var actualResult = mockController.Post(newTestUser);
            var actualUserData = ((OkNegotiatedContentResult<Models.ViewModels.User>)actualResult).Content;

            // Assert
            Assert.IsNotNull(actualResult);
            Assert.IsInstanceOf(typeof(OkNegotiatedContentResult<Models.ViewModels.User>), actualResult);
            Assert.AreEqual(newTestUser, actualUserData);
        }

        [Test]
        [TestCase(Description = "Create a new User", TestName = "Test Create Invalid User returns BadRequest result")]
        public void Test_Post_New_User_InValid()
        {
            // Arrange
            var newTestUser = new Models.ViewModels.User() { LastName = "TestUserAddLastName", UserId = "TestUserNew1" };
            var validationCxt = new ValidationContext(newTestUser);
            var expectedUserData = "Invalid request information. Please verify the information entered.";
            var fnMissingModelState = new ModelState();
            fnMissingModelState.Errors.Add(new ModelError(expectedUserData));
            mockController.ModelState.Add("FirstName", fnMissingModelState);
            mockUserApiLogicLayer.Setup(api => api.AddUser(newTestUser));

            // Act
            var actualResult = mockController.Post(newTestUser);
            var actualUserData = ((BadRequestErrorMessageResult)actualResult).Message;

            // Assert
            Assert.IsNotNull(actualResult);
            Assert.AreEqual(actualUserData, expectedUserData);
            //Assert.AreEqual(newTestUser, actualUserData);
        }

        [Test]
        [TestCase(Description = "Create a new User", TestName = "Test Create User throws Exception")]
        public void Test_Post_New_User_For_Exception()
        {
            // Arrange
            var newTestUser = new Models.ViewModels.User() { FirstName = "TestAddFirstNameNew", LastName = "TestUserAddLastName", UserId = "TestUserNew1" };
            var expectedErrMsg = "Test Exception raised";
            mockUserApiLogicLayer.Setup(api => api.AddUser(newTestUser)).Throws(new System.Exception(expectedErrMsg));

            // Act
            var actualResult = mockController.Post(newTestUser);
            var actualUserData = ((ExceptionResult)actualResult).Exception;

            // Assert
            Assert.IsNotNull(actualResult);
            Assert.IsInstanceOf(typeof(System.Exception), actualUserData);
            Assert.AreEqual(actualUserData.Message, expectedErrMsg);
        }
        #endregion

        #region GET
        [Test(Description = "Get User")]
        [TestCase(0, TestName = "Test for Get Http method - valid scenario")]
        public void Test_GetUser(int index)
        {
            // Arrange
            mockUserApiLogicLayer.Setup(u => u.GetUserById(mockUsersList[index].UserId)).Returns(mockUsersList[index]);

            // Act
            var result = mockController.Get(mockUsersList[index].UserId);
            var actualResult = ((OkNegotiatedContentResult<Models.ViewModels.User>)result).Content;

            // Assert
            Assert.IsNotNull(actualResult);
            Assert.IsNotEmpty(actualResult.UserId);
            Assert.IsInstanceOf(typeof(Models.ViewModels.User), actualResult);
            Assert.AreEqual(mockUsersList[index], actualResult);
        }

        [Test(Description = "Get User")]
        [TestCase(1, TestName = "Test for Get User Http method - throws Exception")]
        public void Test_GetUser_ThrowsException(int index)
        {
            // Arrange
            var expectedErrMsg = "Db connection failure test";
            mockUserApiLogicLayer.Setup(u => u.GetUserById(mockUsersList[index].UserId)).Throws(new System.Exception(expectedErrMsg));

            // Act
            var result = mockController.Get(mockUsersList[index].UserId);
            var actualResult = ((ExceptionResult)result).Exception;

            // Assert
            Assert.IsNotNull(actualResult);
            Assert.IsInstanceOf(typeof(System.Exception), actualResult);
            Assert.AreEqual(expectedErrMsg, actualResult.Message);
        }
        #endregion

        #region DELETE
        [Test]
        [TestCase(2, Description = "Delete User", TestName = "Test for Delete a valid User returns Ok result")]
        public void Test_Delete_User_Valid(int index)
        {
            // Arrange
            var existingUser = mockUsersList[index];
            var expectedTestResult = new OkNegotiatedContentResult<bool>(true, mockController);
            mockUserApiLogicLayer.Setup(api => api.DeleteUser(existingUser.UserId)).Returns(true);

            // Act
            var actualResult = mockController.Delete(existingUser.UserId);
            var actualUserData = ((OkNegotiatedContentResult<bool>)actualResult).Content;

            // Assert
            Assert.IsNotNull(actualResult);
            Assert.IsInstanceOf(typeof(OkNegotiatedContentResult<bool>), actualResult);
            Assert.IsTrue(actualUserData);
        }

        [Test]
        [TestCase(1, Description = "Delete User", TestName = "Test for Delete User throws Exception")]
        public void Test_Delete_User_For_Exception(int index)
        {
            // Arrange
            var existingUser = mockUsersList[index];
            var expectedErrMsg = "Test Exception raised";
            mockUserApiLogicLayer.Setup(api => api.DeleteUser(existingUser.UserId)).Throws(new System.Exception(expectedErrMsg));

            // Act
            var actualResult = mockController.Delete(existingUser.UserId);
            var actualUserData = ((ExceptionResult)actualResult).Exception;

            // Assert
            Assert.IsNotNull(actualResult);
            Assert.IsInstanceOf(typeof(System.Exception), actualUserData);
            Assert.AreEqual(actualUserData.Message, expectedErrMsg);
        }
        #endregion

        #region PUT
        [Test]
        [TestCase(1, "Updated FirstName", "Updated LastName", Description = "Updates existing User", TestName = "Test for Update User returns Ok result")]
        public void Test_Put_User_Valid(int index, string FirstNameToUpdate, string LastNameToUpdate)
        {
            // Arrange
            var userToUpdate = mockUsersList[index];
            userToUpdate.FirstName = FirstNameToUpdate;
            userToUpdate.LastName = LastNameToUpdate;
            var expectedTestResult = new OkNegotiatedContentResult<bool>(true, mockController);
            mockUserApiLogicLayer.Setup(api => api.EditUser(userToUpdate.UserId, userToUpdate)).Returns(true);

            // Act
            var actualResult = mockController.Put(userToUpdate.UserId, userToUpdate);
            var actualUserData = ((OkNegotiatedContentResult<bool>)actualResult).Content;

            // Assert
            Assert.IsNotNull(actualResult);
            Assert.IsInstanceOf(typeof(OkNegotiatedContentResult<bool>), actualResult);
            Assert.IsTrue(actualUserData);
        }

        [Test]
        [TestCase(1, "", "Updated LastName", Description = "Updates existing User", TestName = "Test for Update User returns BadRequest result")]
        public void Test_Put_User_InValid(int index, string FirstNameToUpdate, string LastNameToUpdate)
        {
            // Arrange
            var userToUpdate = mockUsersList[index];
            userToUpdate.FirstName = FirstNameToUpdate;
            userToUpdate.LastName = LastNameToUpdate;

            var validationCxt = new ValidationContext(userToUpdate);
            var expectedUserData = "Invalid request information. Please verify the information entered.";
            var fnMissingModelState = new ModelState();
            fnMissingModelState.Errors.Add(new ModelError(expectedUserData));
            mockController.ModelState.Add("FirstName", fnMissingModelState);
            mockUserApiLogicLayer.Setup(api => api.EditUser(userToUpdate.UserId, userToUpdate));

            // Act
            var actualResult = mockController.Put(userToUpdate.UserId, userToUpdate);
            var actualUserData = ((BadRequestErrorMessageResult)actualResult).Message;

            // Assert
            Assert.IsNotNull(actualResult);
            Assert.AreEqual(actualUserData, expectedUserData);
            //Assert.AreEqual(newTestUser, actualUserData);
        }

        [Test]
        [TestCase(1, "Updated FirstName", "Updated LastName", Description = "Updates existing User", TestName = "Test for Update User throws Exception")]
        public void Test_Put_User_Throws_Exception(int index, string FirstNameToUpdate, string LastNameToUpdate)
        {
            // Arrange
            var userToUpdate = mockUsersList[index];
            userToUpdate.FirstName = FirstNameToUpdate;
            userToUpdate.LastName = LastNameToUpdate;

            var expectedErrMsg = "Test Exception raised";
            mockUserApiLogicLayer.Setup(api => api.EditUser(userToUpdate.UserId, userToUpdate)).Throws(new System.Exception(expectedErrMsg));

            // Act
            var actualResult = mockController.Put(userToUpdate.UserId, userToUpdate);
            var actualUserData = ((ExceptionResult)actualResult).Exception;

            // Assert
            Assert.IsNotNull(actualResult);
            Assert.IsInstanceOf(typeof(System.Exception), actualUserData);
            Assert.AreEqual(actualUserData.Message, expectedErrMsg);
        }

        [Test]
        [TestCase(5, "Updated FirstName", "Updated LastName", Description = "Updates existing User", TestName = "Test for Update User returns Not Found status")]
        public void Test_Put_User_Not_Found(int index, string FirstNameToUpdate, string LastNameToUpdate)
        {
            // Arrange
            var userToUpdate = mockUsersList[index];
            userToUpdate.FirstName = FirstNameToUpdate;
            userToUpdate.LastName = LastNameToUpdate;
            mockUserApiLogicLayer.Setup(api => api.EditUser(userToUpdate.UserId, userToUpdate)).Returns(false);

            // Act
            var actualResult = mockController.Put(userToUpdate.UserId, userToUpdate);
            var actualUserData = (NotFoundResult)actualResult;

            // Assert
            Assert.IsNotNull(actualResult);
            Assert.IsInstanceOf(typeof(NotFoundResult), actualUserData);
        }
        #endregion

        #region Search
        [Test]
        [TestCase("User1First", Description = "Search User", TestName = "Test for Search a valid User returns Ok result")]
        public void Test_Search_User_Valid(string testSearchKeyword)
        {
            // Arrange
            var validSearchResult = mockUsersList.Where(u => u.FirstName.ToLower().Contains(testSearchKeyword.ToLower()));
            var expectedTestResult = new OkNegotiatedContentResult<IEnumerable<Models.ViewModels.User>>(validSearchResult, mockController);
            mockUserApiLogicLayer.Setup(api => api.Search(testSearchKeyword, It.IsAny<bool>(), It.IsAny<string>())).Returns(validSearchResult);

            // Act
            var actualResult = mockController.Search(testSearchKeyword);
            var actualUserData = ((OkNegotiatedContentResult<IEnumerable<Models.ViewModels.User>>)actualResult).Content;

            // Assert
            Assert.IsNotNull(actualResult);
            Assert.IsInstanceOf(typeof(OkNegotiatedContentResult<IEnumerable<Models.ViewModels.User>>), actualResult);
        }

        [Test]
        [TestCase("User1First", Description = "Search User", TestName = "Test for Search User throws Exception")]
        public void Test_Search_User_For_Exception(string testSearchKeyword)
        {
            // Arrange
            var searchResult = mockUsersList.Where(u => u.FirstName.ToLower().Contains(testSearchKeyword.ToLower()));
            var expectedErrMsg = "Test Exception raised";
            mockUserApiLogicLayer.Setup(api => api.Search(testSearchKeyword, It.IsAny<bool>(), It.IsAny<string>())).Throws(new System.Exception(expectedErrMsg));

            // Act
            var actualResult = mockController.Search(testSearchKeyword);
            var actualUserData = ((ExceptionResult)actualResult).Exception;

            // Assert
            Assert.IsNotNull(actualResult);
            Assert.IsInstanceOf(typeof(System.Exception), actualUserData);
            Assert.AreEqual(actualUserData.Message, expectedErrMsg);
        }
        #endregion
    }
}
