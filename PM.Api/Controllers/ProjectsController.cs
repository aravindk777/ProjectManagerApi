using NLog;
using PM.Api.Extensions;
using PM.BL.Projects;
using PM.Models.ViewModels;
using System;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Cors;

namespace PM.Api.Controllers
{
    [EnableCors("*", "*", "*")]
    public class ProjectsController : ApiController
    {
        private IProjectLogic _projectOrhestrator;
        //private ILogger<ProjectsController> logger;
        private ILogger logger;

        public ProjectsController(IProjectLogic projectOrhestrator, ILogger _logInstance)
        {
            _projectOrhestrator = projectOrhestrator;
            logger = _logInstance;
        }

        // GET: api/Projects
        [HttpGet]
        public IHttpActionResult Get()
        {
            try
            {
                var result = _projectOrhestrator.GetAllProjects();
                logger.Info("Get All - total records found: " + result.Count());
                return Ok(result);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Error during Get All Projects", ex.StackTrace);
                return InternalServerError();
            }
        }

        // GET: api/Projects/5
        [HttpGet]
        public IHttpActionResult Get(int id)
        {
            try
            {
                var result = _projectOrhestrator.GetProject(id);
                return Ok(result);
            }
            catch (Exception ex)
            {
                logger.Error(ex, $"Error during GET Project for Project ID: {id}");
                return InternalServerError();
            }
        }

        // POST: api/Projects
        [HttpPost]
        public IHttpActionResult Post([FromBody]Project value)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var result = _projectOrhestrator.CreateProject(value);
                    var createdUrl = string.Join("/", Request.RequestUri, result.ProjectId);
                    return Created(createdUrl, result);
                }
                catch (Exception ex)
                {
                    logger.Error(ex, "Error during POST for Projects with incoming Values: {0}", value.Stringify());
                    return InternalServerError();
                }
            }
            else
            {
                logger.Warn("Invalid ModelState. See below for details.\nModelState: {0}\nData supplied:{1}", ModelState.Stringify(), value.Stringify());
                return BadRequest(ModelState);
            }
        }

        // POST to End a project
        [HttpPost]
        [Route("api/Projects/{id}/End")]
        public IHttpActionResult EndProject(int id)
        {
            try
            {
                var status = _projectOrhestrator.EndProject(id);
                return Ok(status);
            }
            catch(Exception ex)
            {
                logger.Error(ex, "Error during Ending a project for " + id);
                return InternalServerError();
            }
        }

        // PUT: api/Projects/5
        public IHttpActionResult Put(int id, [FromBody]Project value)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var result = _projectOrhestrator.Modify(id, value);
                    if (result)
                        return Ok(result);
                    else
                        return NotFound();
                }
                catch (Exception ex)
                {
                    logger.Error(ex, $"Error during Update to Project Id {id} with values: {value.Stringify()}");
                    return InternalServerError();
                }
            }
            else
            {
                logger.Warn("Model state is invalid .See below for details\nModelState: {0}\nIncoming changes: {1}", ModelState.Stringify(), value.Stringify());
                return BadRequest(ModelState);
            }
        }

        // DELETE: api/Projects/5
        public IHttpActionResult Delete(int id)
        {
            try
            {
                var result = _projectOrhestrator.Remove(id);
                return Ok(result);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Error during Deleting the Project with Id {0}", id);
                return InternalServerError();
            }
        }

        /// <summary>
        /// Returns list of projects associated to the manager by UserId
        /// </summary>
        /// <param name="userId">UserId of the manager</param>
        /// <returns>List of Projects belonging to the User</returns>
        /// <example>api/Users/user1/Projects</example>
        [HttpGet]
        [Route("api/Users/{userId}/Projects")]
        public IHttpActionResult GetUserProjects(string userId)
        {
            return Ok(_projectOrhestrator.GetUserProjects(userId));
        }
    }
}
