using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using UsExpress.Transport.Lib.Business.Interfaces;

namespace UsExpress.Transport.Api.Controllers
{
    public class AppController : ApiController
    {
        private readonly ITestService testService;

        public AppController(ITestService testService)
        {
            this.testService = testService;
        }

        [HttpGet]
        [Route("api/app/getliststore")]
        public IHttpActionResult GetListStore()
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
