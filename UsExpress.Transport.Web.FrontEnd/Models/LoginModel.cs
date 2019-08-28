using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace UsExpress.Transport.Web.FrontEnd.Models
{
    public class LoginModel
    {
        [Required(ErrorMessage = "Username required!")]

        public string LoginUsername { get; set; }

        [Required(ErrorMessage = "Password required!")]
        [DataType(DataType.Password)]
        public string LoginPassword { get; set; }

        public bool RememberMe { get; set; }
    }

    public class RegisterModel
    {
        [Required(ErrorMessage = "Username required")]
        [StringLength(100, ErrorMessage = "Username must be {2} digit", MinimumLength = 6)]
        public string Username { get; set; }

        [Required(ErrorMessage = "Password required")]
        [DataType(DataType.Password)]
        [StringLength(100, ErrorMessage = "Password must be {2} digit!", MinimumLength = 6)]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Confirm password required")]
        public string ConfirmPassword { get; set; }
        [Required(ErrorMessage = "Fullname required")]
        public string FullName { get; set; }
    }

    public class ChangePasswordModel
    {
        [Required(ErrorMessage = "Old password required!")]
        [DataType(DataType.Password)]
        [StringLength(100, ErrorMessage = "Password must be {2} digit!", MinimumLength = 6)]
        public string CurrentPassword { get; set; }

        [Required(ErrorMessage = "Password required!")]
        [DataType(DataType.Password)]
        [StringLength(100, ErrorMessage = "Password must be {2} digit!", MinimumLength = 6)]
        public string Password { get; set; }
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Confirm password required")]
        public string ConfirmPassword { get; set; }

    }

    public class RetsetPassModel
    {
        [Required(ErrorMessage = "Mật khẩu không được để trống")]
        [DataType(DataType.Password)]
        [StringLength(100, ErrorMessage = "Mật khẩu phải ít nhất 6 ký tự.", MinimumLength = 6)]
        public string Password { get; set; }


        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Mật khẩu không khớp.")]
        public string ConfirmPassword { get; set; }
    }
}