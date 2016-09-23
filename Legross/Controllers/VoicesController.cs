using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Legross.Controllers
{
    public class VoicesController : ApiController
    {
        // GET: api/Voices
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }
        // POST: api/Voices
        public void Post([FromBody]string value)
        {
        }

        // GET: api/Voices/5
        public string Get(int id)
        {
            return "value";
        }
        // PUT: api/Voices/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/Voices/5
        public void Delete(int id)
        {
        }
    }
}
