using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.Owin.Security.OAuth;

namespace UsExpress.Transport.Api
{
    //BinhNQ<22-11-2016> source: https://www.youtube.com/watch?v=rMA69bVv0U8
    public class MyAuthorizationServerProvider : OAuthAuthorizationServerProvider
    {
        public override async Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            context.Validated();
        }

        public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            var identity = new ClaimsIdentity(context.Options.AuthenticationType);
            var userIdentity = System.Configuration.ConfigurationManager.AppSettings["USER_IDENTITY"];
            var userAndPass = context.UserName + "/" + context.Password;
            if (userIdentity.Contains(userAndPass))
            {
                identity.AddClaim(new Claim(ClaimTypes.Name, context.UserName));
                context.Validated(identity);
            }
            else
            {
                context.SetError("user not authorize");
                return;
            }
            /*using (var db = new CalcLDEntities())
            {

                var user = db.tblUsers.FirstOrDefault(x=> x.Username == context.UserName && x.Password == context.Password);
                if (user != null)
                {

                    switch (user.Status)
                    {
                        case (int)EStatus.DELETED:
                            context.SetError(Error.ACCOUNT_DELETED.Message, Error.ACCOUNT_DELETED.Message);
                            break;
                        case (int)EStatus.INACTIVE:
                            context.SetError(Error.ACCOUNT_INACTIVE.Message, Error.ACCOUNT_INACTIVE.Message);
                            break;
                        case (int)EStatus.LOCKED:
                            context.SetError(Error.ACCOUNT_LOCKED.Message, Error.ACCOUNT_LOCKED.Message);
                            break;
                        case (int)EStatus.ACTIVED:
                            identity.AddClaim(new Claim(ClaimTypes.Role, db.tblRoles.Find(user.RoleId).Name));
                            identity.AddClaim(new Claim("roleid", user.RoleId.ToString()));
                            identity.AddClaim(new Claim("username", user.Username));
                            identity.AddClaim(new Claim("userid", user.Id.ToString()));
                            identity.AddClaim(new Claim("licenseid", user.LicenseId.ToString()));
                            context.Validated(identity);
                            //update last login
                            user.LastLogin = DateTime.Now;
                            db.SaveChanges();
                            break;
                    }
                }
                else
                {
                    context.SetError(Error.ACCOUNT_INVALID.Message, Error.ACCOUNT_INVALID.Message);
                    return;
                }
            }*/


            /* if (context.UserName == "admin" && context.Password == "admin")
             {
                 identity.AddClaim(new Claim(ClaimTypes.Role,"admin"));
                 identity.AddClaim(new Claim("username","admin"));
                 identity.AddClaim(new Claim(ClaimTypes.Name,"BinhNQ"));
                 context.Validated(identity);
             }
             else if(context.UserName == "user" && context.Password == "user")
             {
                 identity.AddClaim(new Claim(ClaimTypes.Role, "user"));
                 identity.AddClaim(new Claim("username", "user"));
                 identity.AddClaim(new Claim(ClaimTypes.Name, "TrangNT"));
                 //add info return for client
                 var props = new AuthenticationProperties(new Dictionary<string, string>
                 {
                     {
                         "userName", context.UserName
                     }
                 });
                 var ticket = new AuthenticationTicket(identity, props);
                 context.Validated(ticket);
             }
             else
             {
                 context.SetError("invalid_grant","Provided username and password incorrect");
                 return;
             }*/
        }

        public override Task TokenEndpoint(OAuthTokenEndpointContext context)
        {
            foreach (KeyValuePair<string, string> property in context.Properties.Dictionary)
            {
                context.AdditionalResponseParameters.Add(property.Key, property.Value);
            }

            return Task.FromResult<object>(null);
        }
    }
}