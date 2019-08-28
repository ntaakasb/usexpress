using System;

namespace AmazonProductAdvertising.Client
{
    [Serializable]
    public class Error
    {
        public static ErrorObject SUCCESS = new ErrorObject { Code = 0, Message = "Thành công" };
        public static ErrorObject FAILED = new ErrorObject { Code = 1, Message = "Thất bại" };
        public static ErrorObject SYSTEM = new ErrorObject { Code = 2, Message = "Lỗi hệ thống" };
        public static ErrorObject DATABASE = new ErrorObject { Code = 3, Message = "Lỗi database" };
        public static ErrorObject AUTH = new ErrorObject { Code = 4, Message = "Lỗi chứng thực" };
        public static ErrorObject INVALID_DATA = new ErrorObject { Code = 5, Message = "Dữ liệu vào không chính xác" };

        //Các lỗi về tài khoản
        public static ErrorObject ACCOUNT_INVALID = new ErrorObject { Code = 100, Message = "Tài khoản hoặc mật khẩu không chính xác." };
        public static ErrorObject ACCOUNT_LOCKED = new ErrorObject { Code = 101, Message = "Tài khoản đã bị khóa." };
        public static ErrorObject ACCOUNT_DELETED = new ErrorObject { Code = 102, Message = "Tài khoản đã bị xóa." };
        public static ErrorObject ACCOUNT_EXISTED = new ErrorObject { Code = 103, Message = "Tài khoản đã tồn tại." };
        public static ErrorObject ACCOUNT_INACTIVE = new ErrorObject { Code = 104, Message = "Tài khoản chưa được phép sử dụng." };
        public static ErrorObject ACCOUNT_LICENSE_EXPIRED = new ErrorObject { Code = 105, Message = "Tài khoản đã hết hạn sử dụng phần mềm." };
        public static ErrorObject ACCOUNT_NOT_EXISTED = new ErrorObject { Code = 106, Message = "Tài khoản không tồn tại." };

        //Các lỗi về license
        public static ErrorObject LICENSE_VALID = new ErrorObject { Code = 120, Message = "Serial Number hợp lệ." };
        public static ErrorObject LICENSE_INVALID = new ErrorObject { Code = 121, Message = "Serial Number Không hợp lệ." };


        //vì các biến trên là static nên dẫn đến bị setData chồng lên nhau
        //=> tạo ra các function khác vùng nhớ
        //Dùng khi có kết quả trả về 
        public static ErrorObject Get(ErrorObject ErrorObj, object Data = null)
        {
            return new ErrorObject(ErrorObj, Data);
        }

    }

    [Serializable]
    public class ErrorObject
    {
        public int Code = 0;
        public string Message = "Thành công";
        public object Data;
        public ErrorObject() { }
        public ErrorObject(int Code, string Message, object Data = null)
        {
            this.Code = Code;
            this.Message = Message;
            this.Data = Data;
        }
        public ErrorObject(ErrorObject Obj)
        {
            this.Code = Obj.Code;
            this.Message = Obj.Message;
            this.Data = Obj.Data;
        }

        public ErrorObject(ErrorObject Obj, object Data)
        {
            this.Code = Obj.Code;
            this.Message = Obj.Message;
            this.Data = Data;
        }

        public ErrorObject SetCode(ErrorObject Obj)
        {
            this.Code = Obj.Code;
            this.Message = Obj.Message;
            return this;
        }

        public ErrorObject SetData(object Data)
        {
            this.Data = Data;
            return this;
        }

        public object GetData()
        {
            return this.Data;
        }
    }
}
