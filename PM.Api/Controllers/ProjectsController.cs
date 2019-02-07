using PM.BL.Projects;
using PM.Models.ViewModels;
using System;
using System.Web.Http;
using System.Web.Http.Cors;

namespace PM.Api.Controllers
{
    [EnableCors("*", "*", "*")]
    public class ProjectsController : ApiController
    {
        private IProjectLogic _projectOrhestrator;

        public ProjectsController(IProjectLogic projectOrhestrator)
        {
            _projectOrhestrator = projectOrhestrator;
        }


        // GET: api/Projects
        public IHttpActionResult Get()
        {
            try
            {
                var result = _projectOrhestrator.GetAllProjects();
                return Ok(result);
            }
            catch (Exception ex)
            {
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
    }
}
