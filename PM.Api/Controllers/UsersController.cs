using Microsoft.Extensions.Logging;
using PM.BL.Users;
using PM.Models.ViewModels;
using System;
using System.Web.Http;
using System.Web.Http.Cors;

namespace PM.Api.Controllers
{
    [EnableCors("*", "*", "*")]
    public class UsersController : ApiController
    {
        private IUserLogic _userOrchestrator;
        private readonly ILogger<UsersController> _logger;

        public UsersController(IUserLogic _userlogicInstance, ILogger<UsersController> logInstance)
        {
            _userOrchestrator = _userlogicInstance;
            _logger = logInstance;
        }

        // GET: api/Users
        [HttpGet]
        //[ActionName("GetAllUsers")]
        public IHttpActionResult GetAllUsers()
        {
            try
            {
                return Ok(_userOrchestrator.GetUsers());
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Error during GetAllUsers", ex.InnerException, ex.StackTrace);
                return InternalServerError(ex);
            }
        }

        // GET: api/Users/5
        [HttpGet]
        //[Route("api/users/{UserId:alpha}")]
        //[ActionName("GetById")]
        public IHttpActionResult Get(string id)
        {
            try
            {
                return Ok(_userOrchestrator.GetUserById(id));
            }
            catch(Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        // POST: api/Users
        [HttpPost]
        public IHttpActionResult Post([FromBody] User value)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var result = _userOrchestrator.AddUser(value);
                    return Ok(result);
                }
                catch (Exception ex)
                {
                    return InternalServerError(ex);
                }
            }
            else
            {
                _logger.LogWarning("Invalid/Incomplete User Information - {0}", Newtonsoft.Json.JsonConvert.SerializeObject(value));
                return BadRequest("Invalid request information. Please verify the information entered.");
            }
        }

        // PUT: api/Users/5
        [HttpPut]
        public IHttpActionResult Put(string id, [FromBody]User value)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var result = _userOrchestrator.EditUser(id, value);
                    return Ok(result);
                }
                catch (Exception ex)
                {
                    return InternalServerError(ex);
                }
            }
            else
                return BadRequest("Invalid request information. Please verify the information entered.");
        }

        // DELETE: api/Users/5
        [HttpDelete]
        public IHttpActionResult Delete(string id)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var result = _userOrchestrator.DeleteUser(id);
                    return Ok(result);
                }
                catch (Exception ex)
                {
                    return InternalServerError(ex);
                }
            }
            else
                return BadRequest("Invalid request information. Please verify the information entered.");
        }
    }
}
