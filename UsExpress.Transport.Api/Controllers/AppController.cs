using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Http;
using log4net;
using Newtonsoft.Json.Linq;
using UsExpress.Transport.Api.Models;
using UsExpress.Transport.Lib.Business.Interfaces;
using UsExpress.Transport.Lib.Business.Models.Extension;
using UsExpress.Transport.Lib.Business.Models.Extension.Constant;
using UsExpress.Transport.Lib.Utilities;

namespace UsExpress.Transport.Api.Controllers
{
    public class AppController : ApiController
    {
        private static readonly ILog _Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private readonly IStoreServices _iStoreServices;
        private readonly IOrderService _iOrderServices;
        private readonly IUserService _iUserServices;
        private readonly IAppService _iAppServices;
        private readonly IKerryService _iKerryServices;

        public AppController(IStoreServices iStoreServices, IOrderService iOrderServices, IUserService iUserServices, IAppService iAppServices,IKerryService iKerryServices)
        {
            this._iStoreServices = iStoreServices;
            _iOrderServices = iOrderServices;
            _iUserServices = iUserServices;
            _iAppServices = iAppServices;
            _iKerryServices = iKerryServices;
        }

        [Authorize]
        [HttpPost]
        [Route("api/app/store/getall")]
        public IHttpActionResult GetListStore(JObject jObject)
        {
            var response = new Response();
            try
            {
                _Log.Info("api/app/store/getall: " + jObject.ToJson());
                var packageSearchFromApp = jObject.ToObject<PackageSearchFromApp>();
                if (packageSearchFromApp == null || packageSearchFromApp.UserId < 1)
                {
                    return Ok(response.Failed(null));
                }
                if(packageSearchFromApp.WarehouseId == 0)
                {
                    var rolewarehouse = _iUserServices.CheckRoleUser(packageSearchFromApp.UserId, Lib.Business.Models.Extension.Constant.RoleUser.SupplierEmployee);
                    if (rolewarehouse)
                    {
                        var userInfo = _iUserServices.GetUserById(packageSearchFromApp.UserId);
                        if (userInfo != null)
                        {
                            packageSearchFromApp.WarehouseId = userInfo.WarehouseID ?? 0;
                        }
                    }
                }
                else
                {
                    // nv cskh co chon warehouseId
                    // khong phai nv cskh se ko dc lay
                    var roleCustomerService = _iUserServices.CheckRoleUser(packageSearchFromApp.UserId, Lib.Business.Models.Extension.Constant.RoleUser.UserVN);
                    if (!roleCustomerService)
                    {
                        packageSearchFromApp.WarehouseId = 0;
                    }
                }
                if(packageSearchFromApp.WarehouseId == 0)
                {
                    return Ok(response.Failed(packageSearchFromApp));
                }
                var result = _iOrderServices.App_SearchStoreWithCountPackage(packageSearchFromApp);
                return Ok(response.SetData(result));
            }
            catch (Exception ex)
            {
                return Ok(response.System(ex));
            }
        }


        [Authorize]
        [HttpPost]
        [Route("api/app/store/getpackage")]
        public IHttpActionResult GetLstPackageByStore(JObject jObject)
        {
            var response = new Response();
            try
            {
                _Log.Info("api/app/store/getpackage: " + jObject.ToJson());
                var packageSearchFromApp = jObject.ToObject<PackageSearchFromApp>();
                if (packageSearchFromApp == null || packageSearchFromApp.UserId < 1 || packageSearchFromApp.WarehouseId == 0)
                {
                    return Ok(response.Failed(null));
                }
                var rolewarehouse = _iUserServices.CheckRoleUser(packageSearchFromApp.UserId, Lib.Business.Models.Extension.Constant.RoleUser.SupplierEmployee);
                if (rolewarehouse)
                {
                    var userInfo = _iUserServices.GetUserById(packageSearchFromApp.UserId);
                    // nv kho phải lấy đúng kho mình xử lý
                    if (userInfo != null && packageSearchFromApp.WarehouseId != userInfo.WarehouseID)
                    {
                        return Ok(response.Failed(packageSearchFromApp));
                    }
                }
                else
                {
                    // nv cskh co chon warehouseId
                    // 
                    var roleCustomerService = _iUserServices.CheckRoleUser(packageSearchFromApp.UserId, Lib.Business.Models.Extension.Constant.RoleUser.UserVN);
                    if (!roleCustomerService)
                    {
                        return Ok(response.Failed(packageSearchFromApp));
                    }
                }
                packageSearchFromApp.PageIndex = 0;
                var result = _iOrderServices.App_SearchPackage(packageSearchFromApp);
                return Ok(response.SetData(result));
            }
            catch (Exception ex)
            {
                return Ok(response.System(ex));
            }
        }

        [Authorize]
        [HttpPost]
        [Route("api/app/store/order/updatestatus")]
        public async Task<IHttpActionResult> UpdateStoreStatus(JObject jObject)
        {
            //_Log.Info("api/app/store/updatestatus: " + jObject.ToJson());
            Libraries.Log.Write("api/app/store/updatestatus: " + jObject.ToJson());
            var response = new Response();
            try
            {
                JToken jToken;
                if (!jObject.TryGetValue("PackageId", out jToken)) return Ok(response.Failed("PackageId invalid !"));
                var packageId = jToken.Value<int>();
                
                if (!jObject.TryGetValue("UpdateStatus", out jToken)) return Ok(response.Failed("UpdateStatus invalid !"));
                var updateStatus = jToken.Value<int>();
                var appTime = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
                if (jObject.TryGetValue("AppTime", out jToken))
                {
                    appTime = jToken.Value<string>();
                } 
                   
                var result = _iOrderServices.App_UpdateStatusPackage(packageId, updateStatus);

                if (result.Result == 1 && updateStatus == (int) OrderStatusInfo.SendToVN)
                {
                    //tạo vận đơn bên kerry
                   await Task.Run(() => _iKerryServices.PostNewOrder(packageId));
                }

                return Ok(result.Result == 1 ? response.SetData(result.Message) : response.Failed(result.Message));
            }
            catch (Exception ex)
            {
                return Ok(response.System(ex));
            }
        }

        [Authorize]
        [HttpPost]
        [Route("api/app/store/order/getbycode")]
        public IHttpActionResult GetOrderBycode(JObject jObject)
        {
            _Log.Info("api/app/store/order/getbycode: " + jObject.ToJson());
            var response = new Response();
            try
            {
                JToken jToken;
                if (!jObject.TryGetValue("Code", out jToken)) return Ok(response.Failed("Code invalid !"));
                var code = jToken.Value<string>().Trim().Replace(" ", string.Empty);
                //us([\d]{1,}) => regex usexpress code
                //([a-zA-Z]{2,})([\d]{12})-([\d]{4}) => regex ustranport code
                //code = Regex.Match(code, @"us([\d]{1,})|([a-zA-Z]{2,})([\d]{12})-([\d]{2})").Value;

                if (string.IsNullOrEmpty(code))
                {
                    return Ok(response.Failed("Code invalid !"));
                }
                var result = _iOrderServices.App_GetDetailPackageHasItemByCode(code);

                return Ok(response.SetData(result));
            }
            catch (Exception ex)
            {
                return Ok(response.System(ex));
            }
        }

        [Authorize]
        [HttpPost]
        [Route("api/app/login")]
        public IHttpActionResult Login(JObject jObject)
        {
            _Log.Info("api/app/login: " + jObject.ToJson());
            var response = new Response();
            try
            {
                JToken jToken;
                if (!jObject.TryGetValue("Email", out jToken)) return Ok(response.Failed("Username invalid!"));
                var email = jToken.Value<string>();

                if (!jObject.TryGetValue("Password", out jToken)) return Ok(response.Failed("Password invalid!"));
                var password = jToken.Value<string>();

                var user = _iAppServices.Login(email, password);
                if (user != null)
                {
                    if (!user.IsActive) return Ok(response.Failed("Tài khoản đã bị khóa!"));
                    var roleMenu = _iAppServices.GetAppUserRoleMenu((int) user.Id);
                    dynamic userResponse = new ExpandoObject();
                    userResponse.UserId = user.Id;
                    userResponse.Email = user.Username;
                    userResponse.Fullname = user.FullName;
                    userResponse.RoleMenus = new List<dynamic>();
                    roleMenu.ForEach(x=>userResponse.RoleMenus.Add(x.AppMenuId));
                    return Ok(response.SetData(userResponse));
                }
                else
                {
                    return Ok(response.Failed("Tài khoản không tồn tại!"));
                }

                
            }
            catch (Exception ex)
            {
                _Log.Error(ex.Message, ex);
                return Ok(response.System(ex));
            }
            finally
            {
                response = null;
            }

        }
    }
}
