using System.Net;
using System.Web;
using System.Web.Http.Controllers;

namespace UsExpress.Transport.Api
{
    //BinhNQ<22-11-2016> source: https://www.youtube.com/watch?v=rMA69bVv0U8
    public class AuthorizeAttribute : System.Web.Http.AuthorizeAttribute
    {
        protected override void HandleUnauthorizedRequest(HttpActionContext actionContext)
        {
            if (!HttpContext.Current.User.Identity.IsAuthenticated)
            {
                base.HandleUnauthorizedRequest(actionContext);
            }
            else
            {
                actionContext.Response = new System.Net.Http.HttpResponseMessage(HttpStatusCode.Forbidden);
            }


        }
    }
}