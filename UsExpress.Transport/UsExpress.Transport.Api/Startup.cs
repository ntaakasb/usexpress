using System;
using System.Web.Http;
using Microsoft.Owin;
using Microsoft.Owin.Security.OAuth;
using Owin;
using UsExpress.Transport.Api;
using UsExpress.Transport.Logger;

[assembly: OwinStartup(typeof(Startup))]

namespace UsExpress.Transport.Api
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            // For more information on how to configure your application, visit http://go.microsoft.com/fwlink/?LinkID=316888
            //enable cors origin requests

            //BinhNQ<22-11-2016> source: https://www.youtube.com/watch?v=rMA69bVv0U8

            app.UseCors(Microsoft.Owin.Cors.CorsOptions.AllowAll);

            var myProvider = new MyAuthorizationServerProvider();
            OAuthAuthorizationServerOptions options = new OAuthAuthorizationServerOptions
            {
                AllowInsecureHttp = true,
                TokenEndpointPath = new PathString("/token"),
                AccessTokenExpireTimeSpan = TimeSpan.FromDays(30),
                Provider = myProvider,
                RefreshTokenProvider = new SimpleRefreshTokenProvider()
            };
            app.UseOAuthAuthorizationServer(options);
            app.UseOAuthBearerAuthentication(new OAuthBearerAuthenticationOptions());

            HttpConfiguration config = new HttpConfiguration();
            WebApiConfig.Register(config);

            // register log4net
            log4net.Config.XmlConfigurator.Configure();
            GhhLog.Instance();
        }
    }
}
