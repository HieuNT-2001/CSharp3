using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using Serilog;
using System.Text;
using WebApi.Data;
using WebApi.Handlers;
using WebApi.Models.Entities;
using WebApi.Services.Implements;
using WebApi.Services.Interfaces;

// Tạo một builder cho ứng dụng web, dùng để cấu hình các dịch vụ và middleware
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// Thêm tệp cấu hình appsettings.json
//builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

// Đọc connection string từ appsettings.json
var connectionString = builder.Configuration.GetConnectionString("LocalDbConnection");

// Đăng ký DbContext
builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(connectionString));

// Đăng ký Identity với user tủy chỉnh và các tùy chọn cấu hình
builder.Services.AddIdentity<User, IdentityRole>(options =>
{
    // Cấu hình các tùy chọn về mật khẩu
    options.Password.RequiredLength = 8; // Độ dài tối thiểu của mật khẩu
    options.Password.RequireDigit = true; // Yêu cầu có chữ số
    options.Password.RequireNonAlphanumeric = false; // Yêu cầu có ký tự đặc biệt
    options.Password.RequireUppercase = false; // Yêu cầu có chữ hoa
    options.Password.RequireLowercase = false; // Yêu cầu có chữ thường
    options.Password.RequiredUniqueChars = 1; // Số ký tự duy nhất tối thiểu

    // Cấu hình các tùy chọn về khóa tài khoản
    //options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(30); // Thời gian khóa tài khoản
    //options.Lockout.MaxFailedAccessAttempts = 5; // Số lần đăng nhập thất bại tối đa trước khi khóa
    //options.Lockout.AllowedForNewUsers = true; // Cho phép khóa tài khoản cho người dùng mới

    // Cấu hình các tùy chọn về người dùng
    options.User.RequireUniqueEmail = true; // Yêu cầu email phải là duy nhất
    //options.SignIn.RequireConfirmedEmail = true; // Yêu cầu xác nhận email khi đăng nhập
})
.AddEntityFrameworkStores<AppDbContext>() // Sử dụng AppDbContext để lưu trữ thông tin Identity
.AddDefaultTokenProviders(); // Thêm các token mặc định (ví dụ: xác nhận email, đặt lại mật khẩu)

// Đăng ký cấu hình JWT từ appsettings.json
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));

// Cấu hình xác thực JWT
var jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JwtSettings>();
var key = Encoding.UTF8.GetBytes(jwtSettings!.Secret);

// Đăng ký dịch vụ xác thực với JWT Bearer
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = "JwtBearer";
    options.DefaultChallengeScheme = "JwtBearer";
})
.AddJwtBearer("JwtBearer", options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings.Issuer,
        ValidAudience = jwtSettings.Audience,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ClockSkew = TimeSpan.Zero
    };
});

// Đăng ký dịch vụ truy cập HttpContext
// Dịch vụ này cho phép các lớp khác trong ứng dụng truy cập thông tin về yêu cầu HTTP hiện tại
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

// Đăng ký dịch vụ ProductService cho IProductService
builder.Services.AddScoped<IProductService, ProductService>();

// Đăng ký dịch vụ CategoryService cho ICategoryService
builder.Services.AddScoped<ICategoryService, CategoryService>();

// Đăng ký dịch vụ TokenService cho ITokenService
builder.Services.AddScoped<ITokenService, TokenService>();

// Đăng ký dịch vụ controllers với cấu hình JSON
builder.Services.AddControllers().AddNewtonsoftJson(options =>
{
    // Giải quyết vấn đề vòng lặp (loop) tham chiếu khi serialize JSON
    options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
});

// Đăng ký dịch vụ CORS để cho phép chia sẻ tài nguyên giữa các nguồn khác nhau
builder.Services.AddCors();

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

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
// Đăng ký dịch vụ OpenAPI (Swagger) để tạo tài liệu API tự động
builder.Services.AddOpenApi();

// Cấu hình Swagger với thông tin chi tiết về API (tạo tài liệu API tự động)
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

    // Cấu hình Swagger để sử dụng JWT Bearer Authentication
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header
    });

    // Thêm yêu cầu bảo mật để sử dụng JWT Bearer Authentication trong Swagger UI
    c.AddSecurityRequirement(doc => new OpenApiSecurityRequirement
    {
        { new OpenApiSecuritySchemeReference("Bearer", doc), new List<string> { } }
    });
});

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

// Đăng ký Global Handler Exception 
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

// Cấu hình xử lý ngoại lệ toàn cục
// (Lưu ý đặt trước UseRouting hoặc bật kỳ middleware nào có thể ném exception nếu có)
app.UseExceptionHandler();

// Cấu hình CORS để cho phép tất cả các nguồn, phương thức và header
app.UseCors(builder =>
{
    builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
});

// Thiết lập hệ thống định tuyến, cho phép ánh xạ URL đến các controller hoặc endpoint tương ứng
app.UseRouting();

// Sử dụng rate limiting middleware
app.UseRateLimiter();

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
