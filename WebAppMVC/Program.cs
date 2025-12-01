using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using WebAppMVC.Data;
using WebAppMVC.Models;
using WebAppMVC.Services.Implements;
using WebAppMVC.Services.Interfaces;

// Tạo một builder cho ứng dụng web, dùng để cấu hình các dịch vụ và middleware
var builder = WebApplication.CreateBuilder(args);

// Đọc connection string từ appsettings.json
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// Đăng ký DbContext
builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(connectionString));

// Đăng ký Identity với User mặc định và yêu cầu xác nhận tài khoản qua email
builder.Services.AddDefaultIdentity<IdentityUser>(options =>
    options.SignIn.RequireConfirmedAccount = true)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>(); // Sử dụng AppDbContext để lưu trữ thông tin Identity

// Đăng ký Identity với user tủy chỉnh và các tùy chọn cấu hình
//builder.Services.AddIdentity<User, IdentityRole>(options =>
//{
//    // Cấu hình các tùy chọn về mật khẩu
//    options.Password.RequireDigit = true; // Yêu cầu có chữ số
//    options.Password.RequiredLength = 8; // Độ dài tối thiểu của mật khẩu
//    options.Password.RequireNonAlphanumeric = true; // Yêu cầu có ký tự đặc biệt
//    options.Password.RequireUppercase = true; // Yêu cầu có chữ hoa
//    options.Password.RequireLowercase = true; // Yêu cầu có chữ thường
//    //options.Password.RequiredUniqueChars = 1; // Số ký tự duy nhất tối thiểu

//    // Cấu hình các tùy chọn về khóa tài khoản
//    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15); // Thời gian khóa tài khoản
//    options.Lockout.MaxFailedAccessAttempts = 5; // Số lần đăng nhập thất bại tối đa trước khi khóa
//    options.Lockout.AllowedForNewUsers = true; // Cho phép khóa tài khoản cho người dùng mới

//    // Cấu hình các tùy chọn về người dùng
//    options.User.RequireUniqueEmail = true; // Yêu cầu email phải là duy nhất
//    options.SignIn.RequireConfirmedEmail = true; // Yêu cầu xác nhận email khi đăng nhập
//})
//.AddEntityFrameworkStores<AppDbContext>() // Sử dụng AppDbContext để lưu trữ thông tin Identity
//.AddDefaultTokenProviders(); // Thêm các token mặc định (ví dụ: xác nhận email, đặt lại mật khẩu)

// Cấu hình chính sách phân quyền
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("RequireAdminRole", policy => policy.RequireRole("Admin")); // Chỉ cho phép vai trò Admin
    options.AddPolicy("RequireUserRole", policy => policy.RequireRole("User")); // Chỉ cho phép vai trò User
    //options.AddPolicy("RequireUserRole", policy => policy.RequireClaim(ClaimTypes.Role, "User")); // Chỉ cho phép vai trò Use
    options.AddPolicy("EditRolePolicy", policy => policy.RequireClaim("Edit Role")); // Yêu cầu có claim "Edit Role"
});

// Đăng ký dịch vụ ProductService cho IProductService
builder.Services.AddScoped<IProductService, ProductService>();

// Add services to the container.
// Đăng ký dịch vụ MVC với hỗ trợ controller và views
builder.Services.AddControllersWithViews();

// Đăng ký dịch vụ bộ nhớ phân tán (Distributed Memory Cache)
// Dịch vụ này cần thiết để sử dụng session trong ASP.NET Core]
// Nó lưu trữ dữ liệu session trong bộ nhớ của máy chủ
// Lưu ý: Dữ liệu session sẽ bị mất khi ứng dụng khởi động lại
// Nếu cần lưu trữ lâu dài hơn, có thể sử dụng các nhà cung cấp khác như Redis hoặc SQL Server
builder.Services.AddDistributedMemoryCache();

// Đăng ký dịch vụ session
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Thời gian hết hạn session
    options.Cookie.HttpOnly = true; // Chỉ cho phép truy cập cookie qua HTTP, không qua JavaScript
    options.Cookie.IsEssential = true; // Cookie này là cần thiết cho ứng dụng
});

// Đăng ký dịch vụ truy cập HttpContext
// Dịch vụ này cho phép các lớp khác trong ứng dụng truy cập thông tin về yêu cầu HTTP hiện tại
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

// Xây dựng (build) ứng dụng từ builder
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

// Cho phép ứng dụng phục vụ các tệp tĩnh như CSS, JS, hình ảnh từ wwwroot hoặc thư mục tĩnh khác
app.UseStaticFiles();

// Chuyển hướng tất cả các yêu cầu HTTP sang HTTPS để bảo mật
app.UseHttpsRedirection();

// Thiết lập hệ thống định tuyến, cho phép ánh xạ URL đến các controller hoặc endpoint tương ứng
app.UseRouting();

// Bật cơ chế xác thực (Authentication) để xác định người dùng
app.UseAuthentication();

// Bật cơ chế xác thực và phân quyền (Authorization) trên các endpoint đã được định nghĩa
app.UseAuthorization();

// Bản đồ các trang Razor Pages (nếu có sử dụng Razor Pages trong ứng dụng)
app.MapRazorPages();

// Bản đồ các tài nguyên tĩnh tùy chỉnh, ví dụ như các thư mục riêng hoặc file cụ thể (tương tự UseStaticFiles nhưng có thể cấu hình riêng)
app.MapStaticAssets();

// Kích hoạt sử dụng session trong ứng dụng
app.UseSession();

// Cấu hình các endpoint cho ứng dụng (các route của MVC)
// app.UseEndpoints(endpoints =>
// {
//     // Route mặc định: nếu không chỉ định controller/action, sẽ mặc định dùng HomeController và Index action
//    endpoints.MapControllerRoute(
//        name: "default",
//        pattern: "{controller=Home}/{action=Index}/{id?}");

//    endpoints.MapControllerRoute(
//        name: "customers",
//        pattern: "Customer/{action=Index}",
//        defaults: new {controller = "Customer"});

//    endpoints.MapControllerRoute(
//        name: "customer-details",
//        pattern: "Customer/chi-tiet/{id}",
//        defaults: new { controller = "Customer", action = "Details"});

//    endpoints.MapControllerRoute(
//        name: "new-customer",
//        pattern: "Customer/them-moi",
//        defaults: new { controller = "Customer", action = "Create" });

//    endpoints.MapControllerRoute(
//        name: "new-customer",
//        pattern: "Customer/cap-nhat/{id}",
//        defaults: new { controller = "Customer", action = "Edit" });

//    endpoints.MapControllerRoute(
//        name: "new-customer",
//        pattern: "Customer/xoa/{id}",
//        defaults: new { controller = "Customer", action = "Delete" });
// });

// Route mặc định khác (có thể không cần vì đã khai báo trong UseEndpoints)
app.MapControllerRoute(
   name: "default",
   pattern: "{controller=Home}/{action=Index}/{id?}"
);

// Route đơn giản trả về chuỗi "Hello, Thepv!" khi truy cập vào root "/"
//app.MapGet("/", () => "Hello, Thepv!");

// Chạy ứng dụng web
app.Run();
