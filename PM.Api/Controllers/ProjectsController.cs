using Microsoft.Extensions.Logging;
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
        private ILogger<ProjectsController> logger;

        public ProjectsController(IProjectLogic projectOrhestrator, ILogger<ProjectsController> _logInstance)
        {
            _projectOrhestrator = projectOrhestrator;
            logger = _logInstance;
        }

        // GET: api/Projects
        public IHttpActionResult Get()
        {
            try
            {
                var result = _projectOrhestrator.GetAllProjects();
                logger.LogInformation("Get All - total records found: " + result.Count());
                return Ok(result);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error during Get All Projects", ex.StackTrace);
                return InternalServerError(ex);
            }
        }

        // GET: api/Projects/5
        public IHttpActionResult Get(int id)
        {
            try
            {
                var result = _projectOrhestrator.GetProject(id);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        // POST: api/Projects
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
                    logger.LogError(ex, "Error during POST for Projects with incoming Values: {0}", value.Stringify());
                    return InternalServerError(ex);
                }
            }
            else
            {
                logger.LogWarning("Invalid ModelState. See below for details.\nModelState: {0}\nData supplied:{1}", ModelState.Stringify(), value.Stringify());
                return BadRequest(ModelState);
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
                    logger.LogError(ex, $"Error during Update to Project Id {id} with values: {value.Stringify()}");
                    return InternalServerError(ex);
                }
            }
            else
            {
                logger.LogWarning("Model state is invalid .See below for details\nModelState: {0}\nIncoming changes: {1}", ModelState.Stringify(), value.Stringify());
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
                logger.LogError(ex, "Error during Deleting the Project with Id {0}", id);
                return InternalServerError(ex);
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
