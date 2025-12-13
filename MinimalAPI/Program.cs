using System.Threading.RateLimiting;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi;
using MinimalAPI.Data;
using MinimalAPI.Endpoints;
using MinimalAPI.Handlers;
using MinimalAPI.Services.Implements;
using MinimalAPI.Services.Interfaces;
using Serilog;

// Tạo builder cho ứng dụng web
var builder = WebApplication.CreateBuilder(args);

// Đọc connection string từ appsettings.json
var connectionString = builder.Configuration.GetConnectionString("LocalDbConnection");

// Đăng ký DbContext
builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(connectionString));

// Đăng ký dịch vụ ProductService cho IProductService
builder.Services.AddScoped<IProductService, ProductService>();

// Đăng ký dịch vụ CategoryService cho ICategoryService
builder.Services.AddScoped<ICategoryService, CategoryService>();

// Đăng ký dịch vụ Rate Limiting (Giới hạn số request gửi đến trong 1 khoảng thời gian)
builder.Services.AddRateLimiter(options =>
{
    // Cấu hình rate limit kiểu Fixed Window
    options.AddFixedWindowLimiter(policyName: "fixed", opt =>
    {
        opt.Window = TimeSpan.FromMinutes(1); // Khoảng thời gian giới hạn
        opt.PermitLimit = 10; // Giới hạn số request
        opt.QueueLimit = 0; // Số request tối đa trong hàng đợi
        // opt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
    });

    // Cấu hình rate limit kiểu Sliding Window
    options.AddSlidingWindowLimiter("sliding", opt =>
    {
        opt.Window = TimeSpan.FromMinutes(1); // Khoảng thời gian giới hạn
        opt.PermitLimit = 10; // Giới hạn số request
        opt.SegmentsPerWindow = 6; // Chia cửa sổ thời gian thành 6 phần
    });

    // Cấu hình rate limit kiểu Token Bucket
    options.AddTokenBucketLimiter("token", opt =>
    {
        opt.ReplenishmentPeriod = TimeSpan.FromSeconds(10);
        opt.TokenLimit = 100;
        opt.TokensPerPeriod = 20;
    });

    // Cấu hình rate limit kiểu Concurency
    options.AddConcurrencyLimiter("concurrent", opt =>
    {
        opt.PermitLimit = 5;
    });

    // Cấu hình phản hồi khi vượt quá giới hạn
    options.OnRejected = async (context, cancellationToken) =>
    {
        context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
        await context.HttpContext.Response.WriteAsync(
            "Too many requests. Please try again later!", cancellationToken
        );
    };
});

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
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

    // Cấu hình Swagger để đọc XML comments từ file tài liệu XML
    // Thiết lập đường dẫn tới file XML chứa chú thích (comments) để Swagger có thể hiển thị chúng
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath);
});

// Đăng ký Global Exception 
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

// Đăng ký Problem Details middleware
builder.Services.AddProblemDetails();

// Cấu hình Serilog làm hệ thống logging
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console() // Ghi log ra console
    .WriteTo.File("logs/app.log", rollingInterval: RollingInterval.Day) // Ghi log ra file với phân đoạn theo ngày
    .CreateLogger(); // Tạo logger

// Sử dụng Serilog trong ứng dụng
builder.Host.UseSerilog();

// Xây dựng ứng dụng web
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

// Cấu hình xử lý ngoại lệ toàn cục
app.UseExceptionHandler();

// Sử dụng rate limiting middleware
app.UseRateLimiter();

// Chuyển hướng tất cả các yêu cầu HTTP sang HTTPS để bảo mật
app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast").RequireRateLimiting("fixed");

// Map endpoints
app.MapProductEndpoints();
app.MapCategoryEndpoints();

// Chạy ứng dụng web
app.Run();

internal record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
