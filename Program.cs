using RetailProject.Options;
using RetailProject.Services;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllersWithViews();


builder.Services.Configure<AzureStorageOptions>(options =>
{
    options.ConnectionString =
        builder.Configuration.GetConnectionString("AzureStorage");

    options.CustomersTable =
        builder.Configuration["AzureStorage:CustomersTable"]
        ?? "Customers";

    options.ProductsTable =
        builder.Configuration["AzureStorage:ProductsTable"]
        ?? "Products";

    options.BlobContainer =
        builder.Configuration["AzureStorage:BlobContainer"]
        ?? "retailmedia";

    options.OrderQueue =
        builder.Configuration["AzureStorage:OrderQueue"]
        ?? "order-processing";

    options.InventoryQueue =
        builder.Configuration["AzureStorage:InventoryQueue"]
        ?? "inventory-processing";

    options.ImageQueue =
        builder.Configuration["AzureStorage:ImageQueue"]
        ?? "image-processing";

    options.LogFileShare =
        builder.Configuration["AzureStorage:LogFileShare"]
        ?? "application-logs";
});


builder.Services.AddScoped<TableStorageService>();
builder.Services.AddScoped<QueueStorageService>();
builder.Services.AddScoped<RetailProject.Services.BlobStorageService>();
builder.Services.AddScoped<FileShareService>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}

app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");


using (var scope = app.Services.CreateScope())
{
    var tableStorage = scope.ServiceProvider.GetRequiredService<TableStorageService>();
    var queueStorage = scope.ServiceProvider.GetRequiredService<QueueStorageService>();

    await tableStorage.InitializeAsync();
    await queueStorage.InitializeAsync();
}


app.Run();