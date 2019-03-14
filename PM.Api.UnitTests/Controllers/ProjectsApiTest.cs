using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using PM.Api.Controllers;
using PM.BL.Common;
using PM.BL.Projects;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Http.ModelBinding;
using System.Web.Http.Results;

namespace PM.Api.UnitTests.Controllers
{
    [TestFixture]
    public class ProjectsApiTest
    {
        #region Test data for Setup
        private Mock<IProjectLogic> mockProjectsLogic;
        private Mock<ILogger<ProjectsController>> loggerInstance;
        private ProjectsController mockController;
        private List<Models.ViewModels.Project> mockProjectsList = new List<Models.ViewModels.Project>() { };
        #endregion

        #region SETUP
        [SetUp]
        public void SetupTesting()
        {
            mockProjectsLogic = new Mock<IProjectLogic>();
            loggerInstance = new Mock<ILogger<ProjectsController>>();
            mockController = new ProjectsController(mockProjectsLogic.Object, loggerInstance.Object);

            // Create mock users
            mockProjectsList = new List<Models.ViewModels.Project>();
            for (int iCounter = 0; iCounter < 10; iCounter++)
            {
                mockProjectsList.Add(
                (new Models.DataModel.Project
                {
                    ProjectId = iCounter +1, 
                    ProjectName = $"TestProject-{iCounter + 1}",
                    Priority = (iCounter + 1),
                    ProjectStart = DateTime.Now.AddMonths(iCounter - 5),
                    ProjectEnd = DateTime.Today.AddDays(iCounter % 2),
                    ManagerId = Guid.NewGuid()
                }).AsViewModel());
            }
        }
        #endregion

        #region Get All Projects
        [Test(Description = "Get All Projects")]
        [TestCase(TestName = "Test for Get All Projects Returns Valid Results")]
        public void Test_GetAll_Projects()
        {
            // Arrange
            mockProjectsLogic.Setup(u => u.GetAllProjects()).Returns(mockProjectsList);

            // Act
            var result = mockController.Get(); ;
            var actualResult = ((OkNegotiatedContentResult<IEnumerable<Models.ViewModels.Project>>)result).Content;

            // Assert
            Assert.IsNotNull(actualResult);
            Assert.IsNotEmpty(actualResult);
            Assert.That(actualResult.Count(), Is.EqualTo(mockProjectsList.Count()));
        }

        [Test(Description = "Get All Projects")]
        [TestCase(TestName = "Test for Get All Projects Throwing Exception")]
        public void Test_GetAllUsers_Throws_Exception()
        {
            // Arrange
            var expectedErrMsg = "Test for Exception";
            mockProjectsLogic.Setup(u => u.GetAllProjects()).Throws(new Exception(expectedErrMsg));

            // Act
            var result = mockController.Get();
            var actualResult = ((ExceptionResult)result).Exception;

            // Assert
            Assert.IsNotNull(actualResult);
            Assert.IsInstanceOf(typeof(Exception), actualResult);
            Assert.AreEqual(expectedErrMsg, actualResult.Message);
        }
        #endregion

        #region GET
        [Test(Description = "Get Project By Id")]
        [TestCase(1, TestName = "Test for Get method by Id - valid scenario")]
        public void Test_Get_ProjectById_validUsecase(int testProjectId)
        {
            // Arrange
            var validMockData = mockProjectsList.FirstOrDefault(p => p.ProjectId == testProjectId);
            mockProjectsLogic.Setup(u => u.GetProject(testProjectId, It.IsAny<string>())).Returns(validMockData);

            // Act
            var result = mockController.Get(testProjectId);
            var actualResult = ((OkNegotiatedContentResult<Models.ViewModels.Project>)result).Content;

            // Assert
            Assert.IsNotNull(actualResult);
            Assert.IsInstanceOf(typeof(Models.ViewModels.Project), actualResult);
            Assert.AreEqual(validMockData, actualResult);
        }

        [Test(Description = "Get Project By Id")]
        [TestCase(5, TestName = "Test for Get User Http method - throws Exception")]
        public void Test_GetProject_By_Id_ThrowsException(int testProjectId)
        {
            // Arrange
            var expectedErrMsg = "Db connection failure test";
            mockProjectsLogic.Setup(u => u.GetProject(testProjectId, It.IsAny<string>())).Throws(new Exception(expectedErrMsg));

            // Act
            var result = mockController.Get(testProjectId);
            var actualResult = ((ExceptionResult)result).Exception;

            // Assert
            Assert.IsNotNull(actualResult);
            Assert.IsInstanceOf(typeof(System.Exception), actualResult);
            Assert.AreEqual(expectedErrMsg, actualResult.Message);
        }
        #endregion

        #region POST
        [Test]
        [TestCase(Description = "Create a new Project", TestName = "Test for Create Project returns Valid result")]
        public void Test_Post_NewProject_Valid()
        {
            // Arrange
            var newTestProject = new Models.ViewModels.Project() { ProjectId = 20, ProjectName = "Test New Project", ManagerId = Guid.NewGuid(), Priority = 20, ProjectStart = DateTime.Today };
            var expectedUrlPart = "api/project";
            var supposedToBeBaseUri = "http://localhost/projectmanapi";
            var expectedCreatedUri = string.Join("/", supposedToBeBaseUri, expectedUrlPart, newTestProject.ProjectId);

            var mockRequest = mockController.Request ?? new System.Net.Http.HttpRequestMessage();
            mockRequest.RequestUri = new Uri(string.Join("/", supposedToBeBaseUri, expectedUrlPart));
            mockRequest.Method = System.Net.Http.HttpMethod.Post;
            mockController.Request = mockRequest;

            var expectedTestResult = new CreatedNegotiatedContentResult<Models.ViewModels.Project>(new Uri(expectedCreatedUri), newTestProject, mockController);
            mockProjectsLogic.Setup(api => api.CreateProject(newTestProject)).Returns(newTestProject);

            // Act
            var actualResult = mockController.Post(newTestProject);
            var actualProjData = ((CreatedNegotiatedContentResult<Models.ViewModels.Project>)actualResult).Content;
            var actualCreatedUri = ((CreatedNegotiatedContentResult<Models.ViewModels.Project>)actualResult).Location.ToString();

            // Assert
            Assert.IsNotNull(actualResult);
            Assert.IsInstanceOf(typeof(CreatedNegotiatedContentResult<Models.ViewModels.Project>), actualResult);
            Assert.AreEqual(newTestProject, actualProjData);
            Assert.IsNotEmpty(actualCreatedUri);
            Assert.AreEqual(expectedCreatedUri, actualCreatedUri);
            Assert.IsTrue(actualCreatedUri.Contains(expectedUrlPart));
        }

        [Test]
        [TestCase(Description = "Create a new Project", TestName = "Test Create Invalid Project returns BadRequest result")]
        public void Test_Post_New_Project_InValid()
        {
            // Arrange
            var newTestProject = new Models.ViewModels.Project() { ProjectId = 25, ProjectName = "Test New Project", Priority = 20 };
            var validationCxt = new ValidationContext(newTestProject);
            var expectedErrMsg = "Invalid request information. Please verify the information entered.";
            var fnMissingModelState = new ModelState();
            var mockModelErr = new ModelError(expectedErrMsg);
            fnMissingModelState.Errors.Add(mockModelErr);
            mockController.ModelState.Add("ManagerId", fnMissingModelState);
            mockProjectsLogic.Setup(api => api.CreateProject(newTestProject));

            // Act
            var actualResult = mockController.Post(newTestProject);
            var actualModelState = ((InvalidModelStateResult)actualResult).ModelState;

            // Assert
            Assert.IsNotNull(actualResult);
            Assert.Contains("ManagerId", actualModelState.Keys.ToList());
            Assert.AreEqual(fnMissingModelState, actualModelState["ManagerId"]);
            Assert.IsNotEmpty(actualModelState["ManagerId"].Errors[0].ErrorMessage);
            Assert.AreEqual(expectedErrMsg, actualModelState["ManagerId"].Errors[0].ErrorMessage);
        }

        [Test]
        [TestCase(Description = "Create a new Project", TestName = "Test Create Project throws Exception")]
        public void Test_Post_New_Project_For_Exception()
        {
            // Arrange
            var newTestProject = new Models.ViewModels.Project() { ProjectId = 5, ProjectName = "Test New Project", ManagerId = Guid.NewGuid(), Priority = 20, ProjectStart = DateTime.Today };
            var expectedErrMsg = "Test Exception raised";
            mockProjectsLogic.Setup(api => api.CreateProject(newTestProject)).Throws(new Exception(expectedErrMsg));

            // Act
            var actualResult = mockController.Post(newTestProject);
            var actualData = ((ExceptionResult)actualResult).Exception;

            // Assert
            Assert.IsNotNull(actualResult);
            Assert.IsInstanceOf(typeof(Exception), actualData);
            Assert.AreEqual(actualData.Message, expectedErrMsg);
        }
        #endregion        
        
        #region DELETE
        [Test]
        [TestCase(2, Description = "Delete Project", TestName = "Test for Delete a valid Project returns Ok result")]
        public void Test_Delete_Project_Valid(int index)
        {
            // Arrange
            var existingProject = mockProjectsList[index];
            var expectedTestResult = new OkNegotiatedContentResult<bool>(true, mockController);
            mockProjectsLogic.Setup(api => api.Remove(existingProject.ProjectId)).Returns(true);

            // Act
            var actualResult = mockController.Delete(existingProject.ProjectId);
            var actualData = ((OkNegotiatedContentResult<bool>)actualResult).Content;

            // Assert
            Assert.IsNotNull(actualResult);
            Assert.IsInstanceOf(typeof(OkNegotiatedContentResult<bool>), actualResult);
            Assert.IsTrue(actualData);
        }

        [Test]
        [TestCase(1, Description = "Delete Project", TestName = "Test for Delete Project throws Exception")]
        public void Test_Delete_Project_For_Exception(int index)
        {
            // Arrange
            var existingProject = mockProjectsList[index];
            var expectedErrMsg = "Test Exception raised";
            mockProjectsLogic.Setup(api => api.Remove(existingProject.ProjectId)).Throws(new Exception(expectedErrMsg));

            // Act
            var actualResult = mockController.Delete(existingProject.ProjectId);
            var actualData = ((ExceptionResult)actualResult).Exception;

            // Assert
            Assert.IsNotNull(actualResult);
            Assert.IsInstanceOf(typeof(Exception), actualData);
            Assert.AreEqual(actualData.Message, expectedErrMsg);
        }
        #endregion
        
        #region PUT
        [Test]
        [TestCase(1, "Updated New Name",Description = "Updates existing Project", TestName = "Test for Update Project returns Ok result")]
        public void Test_Put_Project_Valid(int index, string ProjNameToUpdate)
        {
            // Arrange
            var projToUpdate = mockProjectsList[index];
            projToUpdate.ProjectName = ProjNameToUpdate;

            var expectedTestResult = new OkNegotiatedContentResult<bool>(true, mockController);
            mockProjectsLogic.Setup(api => api.Modify(projToUpdate.ProjectId, projToUpdate)).Returns(true);

            // Act
            var actualResult = mockController.Put(projToUpdate.ProjectId, projToUpdate);
            var actualUserData = ((OkNegotiatedContentResult<bool>)actualResult).Content;

            // Assert
            Assert.IsNotNull(actualResult);
            Assert.IsInstanceOf(typeof(OkNegotiatedContentResult<bool>), actualResult);
            Assert.IsTrue(actualUserData);
        }

        [Test]
        [TestCase(1, "", Description = "Updates existing Project", TestName = "Test for Update Project returns BadRequest result")]
        public void Test_Put_Project_InValid(int index, string ProjNameToUpdate)
        {
            // Arrange
            var projToUpdate = mockProjectsList[index];
            projToUpdate.ProjectName = ProjNameToUpdate;

            var validationCxt = new ValidationContext(projToUpdate);
            var expectedErrMsg = "Invalid request information. Please verify the information entered.";
            var mockModelState = new ModelState();
            var mockModelErr = new ModelError(expectedErrMsg);
            mockModelState.Errors.Add(mockModelErr);
            mockController.ModelState.Add("ProjectName", mockModelState);
            mockProjectsLogic.Setup(api => api.Modify(projToUpdate.ProjectId, projToUpdate));

            // Act
            var actualResult = mockController.Put(projToUpdate.ProjectId, projToUpdate);
            var actualModelState = ((InvalidModelStateResult)actualResult).ModelState;

            // Assert
            Assert.IsNotNull(actualResult);
            Assert.Contains("ProjectName", actualModelState.Keys.ToList());
            Assert.AreEqual(mockModelState, actualModelState["ProjectName"]);
            Assert.IsNotEmpty(actualModelState["ProjectName"].Errors[0].ErrorMessage);
            Assert.AreEqual(expectedErrMsg, actualModelState["ProjectName"].Errors[0].ErrorMessage);
        }

        [Test]
        [TestCase(1, "Updated New Name", Description = "Updates existing Project", TestName = "Test for Update Project throws Exception")]
        public void Test_Put_Project_Throws_Exception(int index, string ProjectNameToUpdate)
        {
            // Arrange
            var projToUpdate = mockProjectsList[index];
            projToUpdate.ProjectName = ProjectNameToUpdate;

            var expectedErrMsg = "Test Exception raised";
            mockProjectsLogic.Setup(api => api.Modify(projToUpdate.ProjectId, projToUpdate)).Throws(new System.Exception(expectedErrMsg));

            // Act
            var actualResult = mockController.Put(projToUpdate.ProjectId, projToUpdate);
            var actualUserData = ((ExceptionResult)actualResult).Exception;

            // Assert
            Assert.IsNotNull(actualResult);
            Assert.IsInstanceOf(typeof(Exception), actualUserData);
            Assert.AreEqual(actualUserData.Message, expectedErrMsg);
        }

        [Test]
        [TestCase(55, "Updated New Name", Description = "Updates existing Project", TestName = "Test for Update Project returns Not Found status")]
        public void Test_Put_Project_Not_Found(int index, string ProjecttNameToUpdate)
        {
            // Arrange
            var projToUpdate = new Models.ViewModels.Project { ProjectId = index, ProjectName = ProjecttNameToUpdate, ManagerId = Guid.NewGuid(), Priority = 5};
            projToUpdate.ProjectName = ProjecttNameToUpdate;
            mockProjectsLogic.Setup(api => api.Modify(projToUpdate.ProjectId, projToUpdate)).Returns(false);

            // Act
            var actualResult = mockController.Put(projToUpdate.ProjectId, projToUpdate);
            var actualUserData = (NotFoundResult)actualResult;

            // Assert
            Assert.IsNotNull(actualResult);
            Assert.IsInstanceOf(typeof(NotFoundResult), actualUserData);
        }
        #endregion
        
        #region Get User Projects
        [Test]
        [TestCase("User1First", Description = "Test Get User Projects", TestName = "Test for Get User Projects returns Ok result")]
        public void Test_Get_User_Projects(string managerId)
        {
            // Arrange
            var expectedTestResult = new OkNegotiatedContentResult<IEnumerable<Models.ViewModels.Project>>(mockProjectsList, mockController);
            mockProjectsLogic.Setup(api => api.GetUserProjects(managerId)).Returns(mockProjectsList);

            // Act
            var actualResult = mockController.GetUserProjects(managerId);
            var actualUserProjList = ((OkNegotiatedContentResult<IEnumerable<Models.ViewModels.Project>>)actualResult).Content;

            // Assert
            Assert.IsNotNull(actualResult);
            Assert.IsInstanceOf(typeof(OkNegotiatedContentResult<IEnumerable<Models.ViewModels.Project>>), actualResult);
            Assert.AreEqual(mockProjectsList, actualUserProjList);
        }
        #endregion
    }
}
