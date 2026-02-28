using System.Windows;
using Football_Management_System.Models;

namespace Football_Management_System
{
    public partial class App : Application
    {
        // Lưu thông tin user hiện tại sau khi đăng nhập
        public static User CurrentUser { get; set; }
        public static int CurrentUserRole { get; set; }
    }
}
