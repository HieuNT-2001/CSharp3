using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebAppMVC.Data;
using WebAppMVC.Models;
using WebAppMVC.Models.Entities;

namespace WebAppMVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        const String CookieUserId = "UserId";
        const String CookieUserName = "UserName";

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult GetComplexData()
        {
            ViewBag.data = HttpContext.Session.GetComplexData<User>("UserData");
            return View();
        }

        public IActionResult Index()
        {
            // CookieOptions để cấu hình cookie
            CookieOptions options = new CookieOptions()
            {
                Domain = "localhost", // Set domain cho cookie
                Path = "/", // Cookie sẽ có hiệu lực trên toàn bộ ứng dụng
                Expires = DateTime.Now.AddDays(7), // Thời gian hết hạn của cookie
                Secure = false, // Chỉ gửi cookie qua kết nối HTTPS (set false nếu dùng HTTP trong môi trường phát triển)
                HttpOnly = true, // Không cho phép truy cập cookie từ JavaScript (Các tập lệnh từ phía client)
                IsEssential = true, // Cookie này là cần thiết cho chức năng của ứng dụng
            };

            // Thiết lập giá trị cookie
            Response.Cookies.Append(CookieUserId, "001", options);
            Response.Cookies.Append(CookieUserName, "hieuntpp03096", options);

            // hiết lập giá trị session
            HttpContext.Session.SetString("SessionName", "This is session value");
            HttpContext.Session.SetString("Name", "Nguyen Trung Hieu");
            HttpContext.Session.SetString("Email", "hieunt2001@gmail.com");

            User user = new User
            {
                Name = "Nguyen Trung Hieu",
                Email = "hieuntpp03096@fpt.edu.vn"
            };

            HttpContext.Session.SetComplexData("UserData", user);

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult About() // SessionDemo
        {
            String? sessionValue = HttpContext.Session.GetString("SessionName");
            String? name = HttpContext.Session.GetString("Name");
            String? email = HttpContext.Session.GetString("Email");
            ViewBag.SessionValue = sessionValue;
            ViewBag.Name = name;
            ViewBag.Email = email;
            ViewData["Message"] = "Your about page, please refresh page after on minute";
            ViewData["PageTitle"] = "Demo session login";
            return View();
        }

        public String GetCookie() // CookieDemo
        {
            // Truy cập và lấy giá trị cookie
            String? UserName = Request.Cookies.ContainsKey(CookieUserName) ? Request.Cookies[CookieUserName] : "No UserName Cookie";
            int? UserId = null;
            if (Request.Cookies.ContainsKey(CookieUserId))
            {
                bool isValidInt = int.TryParse(Request.Cookies[CookieUserId], out int userIdValue);
                if (isValidInt) UserId = userIdValue;
            }

            // Tạo thông báo với giá trị cookie
            String Message = $"UserId: {UserId}, UserName: {UserName}";

            return Message;
        }

        public String DeleteCookie()
        {
            // CookieOptions để cấu hình cookie
            CookieOptions options = new CookieOptions()
            {
                Domain = "localhost", // Set domain cho cookie
                Path = "/", // Cookie sẽ có hiệu lực trên toàn bộ ứng dụng
            };

            // Xóa cookie
            Response.Cookies.Delete(CookieUserId, options);
            Response.Cookies.Delete(CookieUserName, options);

            return "Cookies are deleted";
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
