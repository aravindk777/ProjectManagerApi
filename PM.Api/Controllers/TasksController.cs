using Microsoft.Extensions.Logging;
using PM.BL.Tasks;
using PM.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace PM.Api.Controllers
{
    public class TasksController : ApiController
    {
        private readonly ITaskLogic taskLogic;
        private readonly ILogger<TasksController> _logger;

        public TasksController(ITaskLogic _logic, ILogger<TasksController> logger)
        {
            taskLogic = _logic;
            _logger = logger;
        }

        // GET: api/Tasks
        /// <summary>
        /// Get all Tasks
        /// </summary>
        /// <returns>List of all Tasks</returns>
        public IHttpActionResult Get()
        {
            try
            {
                return Ok(taskLogic.GetTasks());
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Error during GET All Tasks");
                return InternalServerError(ex);
            }
        }

        // GET: api/Tasks/5
        public IHttpActionResult Get(int id)
        {
            try
            {
                var result = taskLogic.GetTask(id);
                if (result == null)
                {
                    _logger.LogWarning($"No data available for GET Task by ID - {id}.");
                    return NotFound();
                }
                return Ok(result);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, $"Error during GET Task by Id - {id}.");
                return InternalServerError(ex);
            }
        }

        // GET: api/Tasks/taskname
        //[HttpGet]
        //[ActionName("GetByName")]
        //public IHttpActionResult GetByName(string name)
        //{
        //    try
        //    {
        //        var result = taskLogic.GetTask(0, name);
        //        if (result == null)
        //        {
        //            _logger.LogWarning($"No data available for GET Task by Name: {name}");
        //            return NotFound();
        //        }
        //        return Ok(result);
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, $"Error during GET Task by Name: {name}");
        //        return InternalServerError(ex);
        //    }
        //}

        // POST: api/Tasks
        public IHttpActionResult Post([FromBody] Task value)
        {
            //return Created("", value);
            if (ModelState.IsValid)
            {
                try
                {
                    var result = taskLogic.CreateTask(value);
                    var createdUrl = string.Join("/", Request.RequestUri, result.TaskId);
                    return Created(createdUrl, result);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Error during Creating new Task with value - {Newtonsoft.Json.JsonConvert.SerializeObject(value)}");
                    return InternalServerError(ex);
                }
            }
            else
            {
                _logger.LogWarning("Invalid/Incomplete Task Information - {0}", Newtonsoft.Json.JsonConvert.SerializeObject(value));
                return BadRequest(ModelState); //"Invalid request information. Please verify the information entered.", 
            }
        }

        // PUT: api/Tasks/5
        public IHttpActionResult Put(int id, [FromBody]Task value)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    return Ok(taskLogic.UpdateTask(id, value));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Error during Updating Task by Id - {id} with new values: {Newtonsoft.Json.JsonConvert.SerializeObject(value)}");
                    return InternalServerError(ex);
                }
            }
            else
            {
                _logger.LogWarning("Invalid/Incomplete Task Information - {0}", Newtonsoft.Json.JsonConvert.SerializeObject(value));
                return BadRequest(ModelState); //"Invalid request information. Please verify the information entered.", 
            }
        }

        // DELETE: api/Tasks/5
        public IHttpActionResult Delete(int id)
        {
            try
            {
                return Ok(taskLogic.DeleteTask(id));
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, $"Error during Deleting Task Id - {id}");
                return InternalServerError(ex);
            }
        }

        // GET: api/Projects/{ProjId}/Tasks
        [Route("api/Projects/{ProjectId}/Tasks")]
        public IHttpActionResult GetAllTasksForProject(int projectId)
        {
            try
            {
                return Ok(taskLogic.GetAllTasksForProject(projectId));
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, $"Error during GET Tasks by ProjectId - {projectId}");
                return InternalServerError(ex);
            }
        }

        // GET: api/Users/{UserId}/Tasks
        [Route("api/Users/{UserId}/Tasks")]
        public IHttpActionResult GetAllTasksForUser(string UserId)
        {
            try
            {
                return Ok(taskLogic.GetAllTasksForUser(UserId));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error during GET Tasks by User Id - {UserId}");
                return InternalServerError(ex);
            }
        }

        // GET: api/Users/{UserId}/Projects/{ProjectId}/Tasks
        [Route("api/Users/{UserId}/Projects/{projId}/Tasks")]
        public IHttpActionResult GetAllTasksForUserByProject(string UserId, int projId)
        {
            try
            {
                return Ok(taskLogic.GetUserProjectTasks(UserId, projId));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error during GET Tasks by User Id - {UserId} by Project Id - {projId}");
                return InternalServerError(ex);
            }
        }

        [HttpPost]
        [Route("api/Tasks/{taskId}/End")]
        public IHttpActionResult EndTask(int taskId)
        {
            try
            {
                return Ok(taskLogic.EndTask(taskId));
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, $"Error during Ending the Task Id - {taskId}.");
                return InternalServerError(ex);
            }
        }
    }
}
