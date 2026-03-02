using Football_Management_System.Models;
using System.Windows;

namespace Football_Management_System
{
    public partial class App : Application
    {
        public static User CurrentUser { get; set; }
        public static int? CurrentUserRole { get; set; }
    }
}
