using NLog;
using PM.Api.Extensions;
using PM.BL.Users;
using PM.Models.ViewModels;
using System;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Cors;

namespace PM.Api.Controllers
{
    [EnableCors("*", "*", "*")]
    [RoutePrefix("api/Users")]
    public class UsersController : ApiController
    {
        private IUserLogic _userOrchestrator;
        private readonly ILogger _logger;

        public UsersController(IUserLogic _userlogicInstance, ILogger logInstance)
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
                var result = _userOrchestrator.GetUsers();
                _logger.Debug("GetAllUsers invoked with count - " + result.Count());
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error during GetAllUsers", ex.InnerException, ex.StackTrace);
                return InternalServerError(ex);
            }
        }

        [HttpGet]
        [Route("active")]
        //[ActionName("GetAllUsers")]
        public IHttpActionResult GetActiveUsers()
        {
            try
            {
                return Ok(_userOrchestrator.GetUsers(true));
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error during GetAllUsers", ex.InnerException, ex.StackTrace);
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
            catch (Exception ex)
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
                    _logger.Error(ex, "Error during Creating a new user. Data attempted in JSON format: {0}", value.Stringify());
                    return InternalServerError(ex);
                }
            }
            else
            {
                _logger.Warn("Invalid/Incomplete User Information - {0}", value.Stringify());
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
                    if (result)
                        return Ok(result);
                    else
                        return NotFound();
                }
                catch (Exception ex)
                {
                    _logger.Error(ex, "Error during Update with the values supplied in JSON Format - {0}", value.Stringify());
                    return InternalServerError(ex);
                }
            }
            else
            {
                _logger.Warn("Invalid input during Update for the User - {1}. Check the model state information - {0}", ModelState.Values.Stringify(), id);
                return BadRequest("Invalid request information. Please verify the information entered.");
            }
        }

        // DELETE: api/Users/5
        [HttpDelete]
        public IHttpActionResult Delete(string id)
        {
            try
            {
                var result = _userOrchestrator.DeleteUser(id);
                _logger.Warn($"User {id} was attempted to be deleted and it status - {result}");
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error during Delete for UserId - {0}", id);
                return InternalServerError(ex);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="keyword"></param>
        /// <param name="matchExact"></param>
        /// <param name="fieldType"></param>
        /// <returns></returns>
        [HttpGet]
        [ActionName("Search")]
        [Route("Search")]
        public IHttpActionResult Search(string keyword, bool matchExact = false, string fieldType = "")
        {
            try
            {
                return Ok(_userOrchestrator.Search(keyword, matchExact, fieldType));
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"Error during Search by {keyword} with additional params exactMatch-{matchExact} and fieldType- {fieldType}");
                return InternalServerError(ex);
            }
        }
    }
}
