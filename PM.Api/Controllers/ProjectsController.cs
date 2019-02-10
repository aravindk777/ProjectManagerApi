using Microsoft.Extensions.Logging;
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
                    logger.LogError(ex, "Error during POST for Projects", ex.StackTrace, ex.InnerException);
                    return InternalServerError(ex);
                }
            }
            else
                return BadRequest("Invalid Project Information entered. Please check the information");
        }

        // PUT: api/Projects/5
        public IHttpActionResult Put(int id, [FromBody]Project value)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var result = _projectOrhestrator.Modify(id, value);
                    return Ok(result);
                }
                catch (Exception ex)
                {
                    return InternalServerError(ex);
                }
            }
            else
                return BadRequest("Invalid Project Information entered. Please check the information");
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
                return InternalServerError(ex);
            }
        }

        [HttpGet]
        [Route("api/Users/{userId}/Projects")]
        public IHttpActionResult GetUserProjects(string userId)
        {
            return Ok(_projectOrhestrator.GetUserProjects(userId));
        }
    }
}
