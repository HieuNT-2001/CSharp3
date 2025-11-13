var builder = WebApplication.CreateBuilder(args);

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

app.UseStaticFiles();

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthorization();

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

//app.MapControllerRoute(
//    name: "default",
//    pattern: "{controller=Home}/{action=Index}/{id?}"
//);

//app.MapGet("/", () => "Hello, Thepv!");

app.Run();
