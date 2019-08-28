using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using UsExpress.Transport.Lib.Business.Interfaces;

namespace UsExpress.Transport.Api.Controllers
{
    public class TestController : ApiController
    {

        private readonly ITestService testService;

        public TestController(ITestService testService)
        {
            this.testService = testService;
        }

        [HttpGet]
        [Route("api/test/getAll")]
        public IHttpActionResult test()
        {

            try
            {

                var listTest = testService.GetAll();
                return Ok(listTest);
            }
            catch (Exception ex)
            {

                return Ok(ex);
            }
            finally
            {
            }

        }
    }
}
