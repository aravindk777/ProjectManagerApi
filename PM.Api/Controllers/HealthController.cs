using PM.Data.Repos.Users;
using System;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Cors;

namespace PM.Api.Controllers
{
    [EnableCors("*", "*", "*")]
    public class HealthController : ApiController
    {
        private readonly IUserRepository userRepo;

        public HealthController(IUserRepository _userRepo)
        {
            userRepo = _userRepo;
        }

        [HttpGet]
        [Route("api/health/service")]
        [ActionName("Service")]
        public IHttpActionResult ServiceStatus()
        {
            return Ok(true);
        }

        [HttpGet]
        [Route("api/health/db")]
        [ActionName("Db")]
        public IHttpActionResult DbStatus()
        {
            try
            {
                return Ok(userRepo.GetAll().Count());
            }
            catch (Exception ex)
            {
                return InternalServerError(new Exception("Db health status failed", ex));
            }
        }
    }
}
