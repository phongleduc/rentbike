using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RentBike.Common
{
    public class Constants
    {
        public const string ACTION_LOGIN = "Đăng nhập";
        public const string ACTION_LOGOUT = "Đăng xuất";

        public const string ACTION_CREATE_STORE = "Tạo cửa hàng";
        public const string ACTION_UPDATE_STORE = "Cập nhật cửa hàng";

        public const string ACTION_CREATE_CONTRACT = "Làm hợp đồng";
        public const string ACTION_UPDATE_CONTRACT = "Cập nhật hợp đồng";
        public const string ACTION_CLOSE_CONTRACT = "Cập nhật hợp đồng";

        public const string ACTION_CREATE_ACCOUNT = "Tạo tài khoản";
        public const string ACTION_UPDATE_ACCOUNT = "Cập nhật tài khoản";

        public const string ACTION_CREATE_TYPE = "Tạo danh mục";

        public const string ACTION_CREATE_INOUT = "Thu/Chi";
        public const string ACTION_UPDATE_INOUT = "Cập nhật Thu/Chi";

        public const string ACTION_UPDATE_FEE = "Cập nhật phí";

        public const string DUMMY_INOUT = "This is a dummy record";
        public const string DUMMY_USER = "Dummy User";

        public const string LOW_RECOVERABILITY = "Khách hàng {0} đã bị liệt vào danh sách có khả năng thu hồi thấp và không thể được nhìn thấy ở màn hình Danh sách gọi phí và Theo dõi nợ phí";
        public const string REVERT_LOW_RECOVERABILITY = "Khách hàng {0} đã được bỏ khỏi danh sách có khả năng thu hồi thấp và có thể được nhìn thấy ở màn hình Danh sách gọi phí và Theo dõi nợ phí";

        public const string SINGLETON_FILE = "batch.sg";
    }
}