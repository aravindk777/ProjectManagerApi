using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Http.ModelBinding;
using System.Web.Http.Results;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using PM.Api.Controllers;
using PM.BL.Common;
using PM.BL.Tasks;

namespace PM.Api.UnitTests.Controllers
{
    [TestFixture]
    public class TaskApiTest
    {
        #region Test data for Setup
        private Mock<ITaskLogic> mockTaskLogic;
        private TasksController mockController;
        private Mock<ILogger<TasksController>> mockLogger;
        private List<Models.ViewModels.Task> mockTasksList = new List<Models.ViewModels.Task>() { };
        #endregion

        #region SETUP
        [SetUp]
        public void SetupTesting()
        {
            mockTaskLogic = new Mock<ITaskLogic>();
            mockLogger = new Mock<ILogger<TasksController>>(MockBehavior.Loose);
            mockController = new TasksController(mockTaskLogic.Object, mockLogger.Object);

            //Create mock User Guids
            var mockUserGuids = new Guid[] { Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid() };

            // Create mock Tasks
            mockTasksList = new List<Models.ViewModels.Task>();
            for (int iCounter = 0; iCounter < 10; iCounter++)
            {
                mockTasksList.Add(
                (new Models.DataModel.Task
                {
                    TaskId = (iCounter + 1),
                    TaskName = $"TestTask-{iCounter + 1}",
                    Priority = (iCounter + 1),
                    StartDate = DateTime.Now.AddMonths(iCounter - 5),
                    EndDate = DateTime.Today.AddDays(iCounter % 2),
                    TaskOwnerId = mockUserGuids[iCounter % 3],
                    ProjectId = (iCounter * 10 + 1)
                }).AsViewModel());
            }
        }
        #endregion

        #region Get All Tasks
        [Test(Description = "Get All Tasks")]
        [TestCase(TestName = "Test for Get All Tasks Returns Valid Results")]
        public void Test_GetAll_Tasks()
        {
            // Arrange
            mockTaskLogic.Setup(u => u.GetTasks()).Returns(mockTasksList);

            // Act
            var result = mockController.Get();
            var actualResult = ((OkNegotiatedContentResult<IEnumerable<Models.ViewModels.Task>>)result).Content;

            // Assert
            Assert.IsNotNull(actualResult);
            Assert.IsNotEmpty(actualResult);
            Assert.That(actualResult.Count(), Is.EqualTo(mockTasksList.Count()));
        }

        [Test(Description = "Get All Tasks")]
        [TestCase(TestName = "Test for Get All Tasks Throwing Exception")]
        public void Test_GetAllUsers_Throws_Exception()
        {
            // Arrange
            var expectedErrMsg = "Test for Exception";
            mockTaskLogic.Setup(u => u.GetTasks()).Throws(new Exception(expectedErrMsg));

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
        [Test(Description = "Get Task By Id")]
        [TestCase(1, TestName = "Test for Get method by Id - valid scenario")]
        public void Test_Get_TaskById_validUsecase(int testTaskId)
        {
            // Arrange
            var validMockData = mockTasksList.FirstOrDefault(p => p.TaskId == testTaskId);
            mockTaskLogic.Setup(u => u.GetTask(testTaskId)).Returns(validMockData);

            // Act
            var result = mockController.Get(testTaskId);
            var actualResult = ((OkNegotiatedContentResult<Models.ViewModels.Task>)result).Content;

            // Assert
            Assert.IsNotNull(actualResult);
            Assert.IsInstanceOf(typeof(Models.ViewModels.Task), actualResult);
            Assert.AreEqual(validMockData, actualResult);
        }

        [Test(Description = "Get Task By Id")]
        [TestCase(5, TestName = "Test for Get User Http method - throws Exception")]
        public void Test_GetTask_By_Id_ThrowsException(int testTaskId)
        {
            // Arrange
            var expectedErrMsg = "Db connection failure test";
            mockTaskLogic.Setup(u => u.GetTask(testTaskId)).Throws(new Exception(expectedErrMsg));

            // Act
            var result = mockController.Get(testTaskId);
            var actualResult = ((ExceptionResult)result).Exception;

            // Assert
            Assert.IsNotNull(actualResult);
            Assert.IsInstanceOf(typeof(System.Exception), actualResult);
            Assert.AreEqual(expectedErrMsg, actualResult.Message);
        }

        [Test(Description = "Get Task By Id")]
        [TestCase(40, Description = "Get Task By Id", TestName = "Test for Get User Http method - Throws NotFound")]
        public void Test_GetTask_By_Id_Throws_Not_Found(int index)
        {
            // Arrange
            mockTaskLogic.Setup(api => api.GetTask(index)).Returns(It.Is<Models.ViewModels.Task>(null));

            // Act
            var actualResult = mockController.Get(index);
            var actualUserData = (NotFoundResult)actualResult;

            // Assert
            Assert.IsNotNull(actualResult);
            Assert.IsInstanceOf(typeof(NotFoundResult), actualUserData);
        }
        #endregion
        
        #region POST
        [Test]
        [TestCase(Description = "Create a new Task", TestName = "Test for Create Task returns Valid result")]
        public void Test_Post_NewTask_Valid()
        {
            // Arrange
            var newTestTask = new Models.ViewModels.Task() { TaskId = 20, TaskName = "Test New Task", TaskOwnerId = Guid.NewGuid(), Priority = 20, StartDate = DateTime.Today, ProjectId = 100 };
            var expectedUrlPart = "api/Task";
            var supposedToBeBaseUri = "http://localhost/projectmanapi";
            var expectedCreatedUri = string.Join("/", supposedToBeBaseUri, expectedUrlPart, newTestTask.TaskId);

            var mockRequest = mockController.Request ?? new System.Net.Http.HttpRequestMessage();
            mockRequest.RequestUri = new Uri(string.Join("/", supposedToBeBaseUri, expectedUrlPart));
            mockRequest.Method = System.Net.Http.HttpMethod.Post;
            mockController.Request = mockRequest;

            var expectedTestResult = new CreatedNegotiatedContentResult<Models.ViewModels.Task>(new Uri(expectedCreatedUri), newTestTask, mockController);
            mockTaskLogic.Setup(api => api.CreateTask(newTestTask)).Returns(newTestTask);

            // Act
            var actualResult = mockController.Post(newTestTask);
            var actualProjData = ((CreatedNegotiatedContentResult<Models.ViewModels.Task>)actualResult).Content;
            var actualCreatedUri = ((CreatedNegotiatedContentResult<Models.ViewModels.Task>)actualResult).Location.ToString();

            // Assert
            Assert.IsNotNull(actualResult);
            Assert.IsInstanceOf(typeof(CreatedNegotiatedContentResult<Models.ViewModels.Task>), actualResult);
            Assert.AreEqual(newTestTask, actualProjData);
            Assert.IsNotEmpty(actualCreatedUri);
            Assert.AreEqual(expectedCreatedUri, actualCreatedUri);
            Assert.IsTrue(actualCreatedUri.Contains(expectedUrlPart));
        }

        [Test]
        [TestCase(Description = "Create a new Task", TestName = "Test Create Invalid Task returns BadRequest result")]
        public void Test_Post_New_Task_InValid()
        {
            // Arrange
            var newTestTask = new Models.ViewModels.Task() { TaskId = 25, TaskName = "Test New Task", Priority = 20 };
            var validationCxt = new ValidationContext(newTestTask);
            var expectedErrMsg = "Invalid request information. Please verify the information entered.";
            var fnMissingModelState = new ModelState();
            var mockModelErr = new ModelError(expectedErrMsg);
            fnMissingModelState.Errors.Add(mockModelErr);
            mockController.ModelState.Add("ProjectId", fnMissingModelState);
            mockTaskLogic.Setup(api => api.CreateTask(newTestTask));

            // Act
            var actualResult = mockController.Post(newTestTask);
            var actualModelState = ((InvalidModelStateResult)actualResult).ModelState;

            // Assert
            Assert.IsNotNull(actualResult);
            Assert.Contains("ProjectId", actualModelState.Keys.ToList());
            Assert.AreEqual(fnMissingModelState, actualModelState["ProjectId"]);
            Assert.IsNotEmpty(actualModelState["ProjectId"].Errors[0].ErrorMessage);
            Assert.AreEqual(expectedErrMsg, actualModelState["ProjectId"].Errors[0].ErrorMessage);
        }

        [Test]
        [TestCase(Description = "Create a new Task", TestName = "Test Create Task throws Exception")]
        public void Test_Post_New_Task_For_Exception()
        {
            // Arrange
            var newTestTask = new Models.ViewModels.Task() { TaskId = 5, TaskName = "Test New Task", TaskOwnerId = Guid.NewGuid(), Priority = 20, StartDate = DateTime.Today };
            var expectedErrMsg = "Test Exception raised missing Project Id";
            mockTaskLogic.Setup(api => api.CreateTask(newTestTask)).Throws(new Exception(expectedErrMsg));

            // Act
            var actualResult = mockController.Post(newTestTask);
            var actualData = ((ExceptionResult)actualResult).Exception;

            // Assert
            Assert.IsNotNull(actualResult);
            Assert.IsInstanceOf(typeof(Exception), actualData);
            Assert.AreEqual(actualData.Message, expectedErrMsg);
        }

        [Test]
        [TestCase(2, true, Description = "End a Task", TestName = "Test for Ending a Task returns Valid result with Success")]
        [TestCase(4, false, Description = "End a Task", TestName = "Test for Ending a Task returns Valid result with failure")]
        public void Test_Post_EndTask_Valid(int taskIdx, bool expectedStatus)
        {
            // Arrange
            var testTaskToEnd = mockTasksList[taskIdx];
            testTaskToEnd.EndDate = DateTime.Now;
            var expectedTestResult = new OkNegotiatedContentResult<bool>(expectedStatus, mockController);
            mockTaskLogic.Setup(api => api.EndTask(testTaskToEnd.TaskId)).Returns(expectedStatus);

            // Act
            var actualTestResult = mockController.EndTask(testTaskToEnd.TaskId);
            var actualResult = ((OkNegotiatedContentResult<bool>)actualTestResult).Content;

            // Assert
            Assert.IsNotNull(actualTestResult);
            Assert.IsInstanceOf(typeof(OkNegotiatedContentResult<bool>), actualTestResult);
            Assert.AreEqual(expectedStatus, actualResult);
        }

        [Test]
        [TestCase(2, true, Description = "End a Task", TestName = "Test for Ending a Task throws DB Exception")]
        public void Test_Post_EndTask_ThrowsException(int taskIdx, bool expectedStatus)
        {
            // Arrange
            var testTaskToEnd = mockTasksList[taskIdx];
            testTaskToEnd.EndDate = DateTime.Now;
            var expectedErrMsg = "Test Database Exception raised";
            mockTaskLogic.Setup(api => api.EndTask(testTaskToEnd.TaskId)).Throws(new Exception(expectedErrMsg));

            // Act
            var actualTestResult = mockController.EndTask(testTaskToEnd.TaskId);
            var actualResult = ((ExceptionResult)actualTestResult).Exception;

            // Assert
            Assert.IsNotNull(actualResult);
            Assert.IsInstanceOf(typeof(Exception), actualResult);
            Assert.AreEqual(actualResult.Message, expectedErrMsg);
        }
        #endregion

        #region DELETE
        [Test]
        [TestCase(2, Description = "Delete Task", TestName = "Test for Delete a valid Task returns Ok result")]
        public void Test_Delete_Task_Valid(int index)
        {
            // Arrange
            var existingTask = mockTasksList[index];
            var expectedTestResult = new OkNegotiatedContentResult<bool>(true, mockController);
            mockTaskLogic.Setup(api => api.DeleteTask(existingTask.TaskId)).Returns(true);

            // Act
            var actualResult = mockController.Delete(existingTask.TaskId);
            var actualData = ((OkNegotiatedContentResult<bool>)actualResult).Content;

            // Assert
            Assert.IsNotNull(actualResult);
            Assert.IsInstanceOf(typeof(OkNegotiatedContentResult<bool>), actualResult);
            Assert.IsTrue(actualData);
        }

        [Test]
        [TestCase(1, Description = "Delete Task", TestName = "Test for Delete Task throws Exception")]
        public void Test_Delete_Task_For_Exception(int index)
        {
            // Arrange
            var existingTask = mockTasksList[index];
            var expectedErrMsg = "Test Exception raised";
            mockTaskLogic.Setup(api => api.DeleteTask(existingTask.TaskId)).Throws(new Exception(expectedErrMsg));

            // Act
            var actualResult = mockController.Delete(existingTask.TaskId);
            var actualData = ((ExceptionResult)actualResult).Exception;

            // Assert
            Assert.IsNotNull(actualResult);
            Assert.IsInstanceOf(typeof(Exception), actualData);
            Assert.AreEqual(actualData.Message, expectedErrMsg);
        }
        #endregion
        
        #region PUT
        [Test]
        [TestCase(1, "Updated New Name", Description = "Updates existing Task", TestName = "Test for Update Task returns Ok result")]
        public void Test_Put_Task_Valid(int index, string ProjNameToUpdate)
        {
            // Arrange
            var projToUpdate = mockTasksList[index];
            projToUpdate.TaskName = ProjNameToUpdate;

            var expectedTestResult = new OkNegotiatedContentResult<bool>(true, mockController);
            mockTaskLogic.Setup(api => api.UpdateTask(projToUpdate.TaskId, projToUpdate)).Returns(true);

            // Act
            var actualResult = mockController.Put(projToUpdate.TaskId, projToUpdate);
            var actualUserData = ((OkNegotiatedContentResult<bool>)actualResult).Content;

            // Assert
            Assert.IsNotNull(actualResult);
            Assert.IsInstanceOf(typeof(OkNegotiatedContentResult<bool>), actualResult);
            Assert.IsTrue(actualUserData);
        }

        [Test]
        [TestCase(1, "", Description = "Updates existing Task", TestName = "Test for Update Task returns BadRequest result")]
        public void Test_Put_Task_InValid(int index, string ProjNameToUpdate)
        {
            // Arrange
            var projToUpdate = mockTasksList[index];
            projToUpdate.TaskName = ProjNameToUpdate;

            var validationCxt = new ValidationContext(projToUpdate);
            var expectedErrMsg = "Invalid request information. Please verify the information entered.";
            var mockModelState = new ModelState();
            var mockModelErr = new ModelError(expectedErrMsg);
            mockModelState.Errors.Add(mockModelErr);
            mockController.ModelState.Add("TaskName", mockModelState);
            mockTaskLogic.Setup(api => api.UpdateTask(projToUpdate.TaskId, projToUpdate));

            // Act
            var actualResult = mockController.Put(projToUpdate.TaskId, projToUpdate);
            var actualModelState = ((InvalidModelStateResult)actualResult).ModelState;

            // Assert
            Assert.IsNotNull(actualResult);
            Assert.Contains("TaskName", actualModelState.Keys.ToList());
            Assert.AreEqual(mockModelState, actualModelState["TaskName"]);
            Assert.IsNotEmpty(actualModelState["TaskName"].Errors[0].ErrorMessage);
            Assert.AreEqual(expectedErrMsg, actualModelState["TaskName"].Errors[0].ErrorMessage);
        }

        [Test]
        [TestCase(1, "Updated New Name", Description = "Updates existing Task", TestName = "Test for Update Task throws Exception")]
        public void Test_Put_Task_Throws_Exception(int index, string TaskNameToUpdate)
        {
            // Arrange
            var taskToUpdate = mockTasksList[index];
            taskToUpdate.TaskName = TaskNameToUpdate;

            var expectedErrMsg = "Test Exception raised";
            mockTaskLogic.Setup(api => api.UpdateTask(taskToUpdate.TaskId, taskToUpdate)).Throws(new System.Exception(expectedErrMsg));

            // Act
            var actualResult = mockController.Put(taskToUpdate.TaskId, taskToUpdate);
            var actualTaskData = ((ExceptionResult)actualResult).Exception;

            // Assert
            Assert.IsNotNull(actualResult);
            Assert.IsInstanceOf(typeof(Exception), actualTaskData);
            Assert.AreEqual(actualTaskData.Message, expectedErrMsg);
        }

        //[Test]
        //[TestCase(55, "Updated New Name", Description = "Updates existing Task", TestName = "Test for Update Task returns Not Found status")]
        //public void Test_Put_Task_Not_Found(int index, string TasktNameToUpdate)
        //{
        //    // Arrange
        //    var taskToUpdate = new Models.ViewModels.Task { TaskId = index, TaskName = TasktNameToUpdate, TaskOwnerId = Guid.NewGuid(), Priority = 5 };
        //    taskToUpdate.TaskName = TasktNameToUpdate;
        //    mockTaskLogic.Setup(api => api.UpdateTask(taskToUpdate.TaskId, taskToUpdate)).Returns(false);

        //    // Act
        //    var actualResult = mockController.Put(taskToUpdate.TaskId, taskToUpdate);
        //    var actualTaskData = (NotFoundResult)actualResult;

        //    // Assert
        //    Assert.IsNotNull(actualResult);
        //    Assert.IsInstanceOf(typeof(NotFoundResult), actualTaskData);
        //}
        #endregion
        
        #region Get Tasks By Groups
        [Test]
        [TestCase("User1First", 0, Description = "Test Get Tasks For Group", TestName = "Test for Get User's All Tasks returns Ok result")]
        public void Test_Get_User_Tasks(string mockUserId, int ownerGuidIdx)
        {
            // Arrange
            var filterGuid = mockTasksList[ownerGuidIdx].TaskOwnerId;
            var expectedTasks = mockTasksList.Where(ownerGuid => ownerGuid.TaskOwnerId == filterGuid);
            var expectedTestResult = new OkNegotiatedContentResult<IEnumerable<Models.ViewModels.Task>>(expectedTasks, mockController);
            mockTaskLogic.Setup(api => api.GetAllTasksForUser(mockUserId)).Returns(expectedTasks);

            // Act
            var actualResult = mockController.GetAllTasksForUser(mockUserId);
            var actualUserTasks = ((OkNegotiatedContentResult<IEnumerable<Models.ViewModels.Task>>)actualResult).Content;

            // Assert
            Assert.IsNotNull(actualResult);
            Assert.IsInstanceOf(typeof(OkNegotiatedContentResult<IEnumerable<Models.ViewModels.Task>>), actualResult);
            Assert.AreEqual(expectedTasks, actualUserTasks);
        }

        [Test(Description = "Test Get Tasks For Group")]
        [TestCase("User1First", 1, TestName = "Test for Get User's All Tasks - throws Exception")]
        public void Test_Get_User_Tasks_ThrowsException(string mockUserId, int ownerGuidIdx)
        {
            // Arrange
            var expectedErrMsg = "Db connection failure test";
            var filterGuid = mockTasksList[ownerGuidIdx].TaskOwnerId;
            var expectedTasks = mockTasksList.Where(ownerGuid => ownerGuid.TaskOwnerId == filterGuid);
            mockTaskLogic.Setup(u => u.GetAllTasksForUser(mockUserId)).Throws(new Exception(expectedErrMsg));

            // Act
            var result = mockController.GetAllTasksForUser(mockUserId);
            var actualResult = ((ExceptionResult)result).Exception;

            // Assert
            Assert.IsNotNull(actualResult);
            Assert.IsInstanceOf(typeof(Exception), actualResult);
            Assert.AreEqual(expectedErrMsg, actualResult.Message);
        }

        [Test]
        [TestCase(1, Description = "Test Get Tasks For Group", TestName = "Test for Get Project's All Tasks returns Ok result")]
        public void Test_Get_AllTasks_For_ProjectById(int projId)
        {
            // Arrange
            var expectedTasks = mockTasksList.Where(p => p.ProjectId == projId);
            var expectedTestResult = new OkNegotiatedContentResult<IEnumerable<Models.ViewModels.Task>>(expectedTasks, mockController);
            mockTaskLogic.Setup(api => api.GetAllTasksForProject(projId)).Returns(expectedTasks);

            // Act
            var actualResult = mockController.GetAllTasksForProject(projId);
            var actualTasksList = ((OkNegotiatedContentResult<IEnumerable<Models.ViewModels.Task>>)actualResult).Content;

            // Assert
            Assert.IsNotNull(actualResult);
            Assert.IsInstanceOf(typeof(OkNegotiatedContentResult<IEnumerable<Models.ViewModels.Task>>), actualResult);
            Assert.AreEqual(expectedTasks, actualTasksList);
        }

        [Test(Description = "Test Get Tasks For Group")]
        [TestCase(1, TestName = "Test for Get Project's All Tasks - throws Exception")]
        public void Test_Get_AllTasks_For_ProjectById_ThrowsException(int projId)
        {
            // Arrange
            var expectedErrMsg = "Db connection failure test";
            var expectedTasks = mockTasksList.Where(p => p.ProjectId == projId);
            mockTaskLogic.Setup(u => u.GetAllTasksForProject(projId)).Throws(new Exception(expectedErrMsg));

            // Act
            var result = mockController.GetAllTasksForProject(projId);
            var actualResult = ((ExceptionResult)result).Exception;

            // Assert
            Assert.IsNotNull(actualResult);
            Assert.IsInstanceOf(typeof(Exception), actualResult);
            Assert.AreEqual(expectedErrMsg, actualResult.Message);
        }

        [Test]
        [TestCase(1, "User1First", Description = "Test Get Tasks For Group", TestName = "Test for Get User's Project's Tasks returns Ok result")]
        public void Test_Get_AllTasks_For_User_And_ProjectId(int projId, string mockUserId)
        {
            // Arrange
            var filterGuid = mockTasksList[projId].TaskOwnerId;
            var expectedTasks = mockTasksList.Where(p => p.ProjectId == projId && p.TaskOwnerId == filterGuid);
            var expectedTestResult = new OkNegotiatedContentResult<IEnumerable<Models.ViewModels.Task>>(expectedTasks, mockController);
            mockTaskLogic.Setup(api => api.GetAllTasksForProject(projId)).Returns(expectedTasks);

            // Act
            var actualResult = mockController.GetAllTasksForUserByProject(mockUserId, projId);
            var actualTasksList = ((OkNegotiatedContentResult<IEnumerable<Models.ViewModels.Task>>)actualResult).Content;

            // Assert
            Assert.IsNotNull(actualResult);
            Assert.IsInstanceOf(typeof(OkNegotiatedContentResult<IEnumerable<Models.ViewModels.Task>>), actualResult);
            Assert.AreEqual(expectedTasks, actualTasksList);
        }

        [Test(Description = "Test Get Tasks For Group")]
        [TestCase(2, "User1First", TestName = "Test for Get User's Project's Tasks - throws Exception")]
        public void Test_Get_AllTasks_For_User_And_ProjectId_ThrowsException(int projId, string mockUserId)
        {
            // Arrange
            var expectedErrMsg = "Db connection failure test";
            var filterGuid = mockTasksList[projId].TaskOwnerId;
            var expectedTasks = mockTasksList.Where(p => p.ProjectId == projId && p.TaskOwnerId == filterGuid);
            mockTaskLogic.Setup(u => u.GetUserProjectTasks(mockUserId, projId)).Throws(new Exception(expectedErrMsg));

            // Act
            var result = mockController.GetAllTasksForUserByProject(mockUserId, projId);
            var actualResult = ((ExceptionResult)result).Exception;

            // Assert
            Assert.IsNotNull(actualResult);
            Assert.IsInstanceOf(typeof(Exception), actualResult);
            Assert.AreEqual(expectedErrMsg, actualResult.Message);
        }
        #endregion
    }
}
