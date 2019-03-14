using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using PM.Api.Controllers;
using PM.Data.Repos.Users;
using System;
using System.Linq;
using System.Web.Http.Results;

namespace PM.Api.UnitTests.Controllers
{
    [TestFixture]
    public class HealthChkApiTest
    {
        #region Test Elements
        private Mock<IUserRepository> mockUserRepo;
        private HealthController testHlthCtrler;
        private Mock<ILogger<HealthController>> mockLogger;
        #endregion

        [SetUp]
        public void SetupTesting()
        {
            mockUserRepo = new Mock<IUserRepository>();
            mockLogger = new Mock<ILogger<HealthController>>();
            testHlthCtrler = new HealthController(mockUserRepo.Object, mockLogger.Object);
        }

        [Test(Description = "Test GET - API Status")]
        [TestCase(TestName = "Test for Get API Status - returns True")]
        public void Test_For_Api_Status_GET()
        {
            // Arrange
                    // -- nothing to arrange for this test

            // Act
            var actualResultType = testHlthCtrler.ServiceStatus();
            var actualResult = ((OkNegotiatedContentResult<bool>)actualResultType).Content;

            // Assert
            Assert.IsNotNull(actualResultType);
            Assert.IsInstanceOf(typeof(OkNegotiatedContentResult<bool>), actualResultType);
            Assert.IsTrue(actualResult);
        }

        [Test(Description = "Test GET - Database Status")]
        [TestCase(10, TestName = "Test for Get DB Status - returns Valid Result")]
        public void Test_For_DB_Status_GET_Valid(int expectedCount)
        {
            // Arrange
            var testData = new Models.DataModel.User[expectedCount];
            mockUserRepo.Setup(d => d.GetAll()).Returns(testData);
            // Act
            var actualResultType = testHlthCtrler.DbStatus();
            var actualResultCount = ((OkNegotiatedContentResult<int>)actualResultType).Content;
            // Assert
            Assert.IsNotNull(actualResultType);
            Assert.IsNotNull(actualResultCount);
            Assert.IsInstanceOf(typeof(OkNegotiatedContentResult<int>), actualResultType);
            Assert.AreEqual(expectedCount, actualResultCount);
        }

        [Test(Description = "Test GET - Database Status")]
        [TestCase(TestName = "Test for Get DB Status - throws Exception")]
        public void Test_For_DB_Status_Throws_Exception()
        {
            // Arrange
            var expectedErrMsg = "Db health status failed";
            mockUserRepo.Setup(u => u.GetAll()).Throws(new Exception(expectedErrMsg));

            // Act
            var result = testHlthCtrler.DbStatus();
            var actualResult = ((ExceptionResult)result).Exception;

            // Assert
            Assert.IsNotNull(actualResult);
            Assert.IsInstanceOf(typeof(Exception), actualResult);
            Assert.AreEqual(expectedErrMsg, actualResult.Message);
        }
    }
}
