Vusing ApiTestServer.Filters.TestBasic.Filters;
using AuthorizationServer.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace AuthorizationServer.Controllers
{
    [ApiAuthorize]
    [InitializeSimpleMembership]
    public class ValuesController : ApiController
    {
        public List<int> Get()
        {
            return new List<int>()
            {
                1, 2, 3, 4, 5, 6
            };
        }
    }
}
