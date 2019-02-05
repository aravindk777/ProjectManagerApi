using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace PM.Api.Controllers
{
    public class ProjectsController : ApiController
    {
        
        // GET: api/Projects
        public IHttpActionResult Get()
        {
            return Ok();
        }

        // GET: api/Projects/5
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/Projects
        public void Post([FromBody]string value)
        {
        }

        // PUT: api/Projects/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/Projects/5
        public void Delete(int id)
        {
        }
    }
}
