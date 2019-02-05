using PM.BL.User;
using PM.Models.ViewModels;
using System.Collections.Generic;
using System.Web.Http;

namespace PM.Api.Controllers
{
    public class UsersController : ApiController
    {
        private IUsersLogic userLogic;

        public UsersController(IUsersLogic _userlogicInstance)
        {
            userLogic = _userlogicInstance;
        }
        // GET: api/Users
        public IEnumerable<Users> Get()
        {
            return userLogic.GetUsers();
        }

        // GET: api/Users/5
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/Users
        public void Post([FromBody] Users value)
        {
            userLogic.AddUser(value);
        }

        // PUT: api/Users/5
        public void Put(int id, [FromBody]Users value)
        {
        }

        // DELETE: api/Users/5
        public void Delete(int id)
        {
        }
    }
}
