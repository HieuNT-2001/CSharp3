using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi;
using WebApi.Data;
using WebApi.Services.Implements;
using WebApi.Services.Interfaces;

// Tạo một builder cho ứng dụng web, dùng để cấu hình các dịch vụ và middleware
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// Đọc connection string từ appsettings.json
var connectionString = builder.Configuration.GetConnectionString("LocalDbConnection");

// Đăng ký DbContext
builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(connectionString));

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

// Đăng ký dịch vụ ProductService cho IProductService
builder.Services.AddScoped<IProductService, ProductService>();

// Đăng ký dịch vụ CategoryService cho ICategoryService
builder.Services.AddScoped<ICategoryService, CategoryService>();

// Đăng ký dịch vụ controllers với cấu hình JSON
builder.Services.AddControllers().AddNewtonsoftJson(options =>
{
    // Giải quyết vấn đề vòng lặp (loop) tham chiếu khi serialize JSON
    options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
});

// Đăng ký dịch vụ CORS để cho phép chia sẻ tài nguyên giữa các nguồn khác nhau
builder.Services.AddCors();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
// Đăng ký dịch vụ OpenAPI (Swagger) để tạo tài liệu API tự động
builder.Services.AddOpenApi();

// Cấu hình Swagger với thông tin chi tiết về API
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "My API",
        Version = "v1",
        Description = "This is my API description",
        TermsOfService = new Uri("https://example.com/terms"),
        Contact = new OpenApiContact
        {
            Name = "Your Name",
            Email = "your.email@example.com",
            Url = new Uri("https://example.com"),
        },
        License = new OpenApiLicense
        {
            Name = "Use under LICX",
            Url = new Uri("https://example.com/license"),
        }
    });

    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath);
});

// Xây dựng (build) ứng dụng từ builder
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    // Sử dụng Swagger và giao diện người dùng Swagger trong môi trường phát triển
    app.MapOpenApi();

    // Bật Swagger middleware để phục vụ tài liệu API ở định dạng JSON
    app.UseSwagger();

    // Bật Swagger UI middleware để cung cấp giao diện người dùng cho tài liệu API
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
        c.RoutePrefix = "swagger"; // Đặt Swagger UI tại gốc của ứng dụng
    });
}

// Cấu hình CORS để cho phép tất cả các nguồn, phương thức và header
app.UseCors(builder =>
{
    builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
});

// Thiết lập hệ thống định tuyến, cho phép ánh xạ URL đến các controller hoặc endpoint tương ứng
app.UseRouting();

// Chuyển hướng tất cả các yêu cầu HTTP sang HTTPS để bảo mật
app.UseHttpsRedirection();

// Bật cơ chế xác thực (Authentication) để xác định người dùng
app.UseAuthentication();

// Bật cơ chế xác thực và phân quyền (Authorization) trên các endpoint đã được định nghĩa
app.UseAuthorization();

// Kích hoạt sử dụng session trong ứng dụng
app.UseSession();

// Ánh xạ các controller để xử lý các yêu cầu HTTP
app.MapControllers();

// Chạy ứng dụng web
app.Run();
