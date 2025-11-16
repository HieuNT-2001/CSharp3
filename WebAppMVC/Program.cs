using Microsoft.EntityFrameworkCore;
using WebAppMVC.Data;

var builder = WebApplication.CreateBuilder(args);

// Đọc connection string từ appsettings.json
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// Đăng ký DbContext
builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(connectionString));

// Add services to the container.
builder.Services.AddControllersWithViews();

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

// Bật cơ chế xác thực và phân quyền (Authorization) trên các endpoint đã được định nghĩa
app.UseAuthorization();

// Bản đồ các tài nguyên tĩnh tùy chỉnh, ví dụ như các thư mục riêng hoặc file cụ thể (tương tự UseStaticFiles nhưng có thể cấu hình riêng)
app.MapStaticAssets();

//app.UseEndpoints(endpoints =>
//{
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
//});

app.MapControllerRoute(
   name: "default",
   pattern: "{controller=Home}/{action=Index}/{id?}"
);

//app.MapGet("/", () => "Hello, Thepv!");

app.Run();
