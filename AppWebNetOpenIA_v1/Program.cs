using AppWebNetOpenIA_v1.Data;
using AppWebNetOpenIA_v1.Models.Views;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.AI;
using OpenAI;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// Agregar DbContext con PostgreSQL
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString));

// Add services to the container.
builder.Services.AddControllersWithViews();

//configuracion de apikey con OpenAI
builder.Services.AddSingleton<OpenAIClient>(provider =>
    new OpenAIClient(builder.Configuration["OpenAI:ApiKey"]));

//aca configuramos la version de gpt que se utilizara 
builder.Services.AddSingleton<IChatClient>(providerServicio =>
{
    var clienteOpenAi = new OpenAI.OpenAIClient(builder.Configuration["OpenAI:ApiKey"]);
    return clienteOpenAi.AsChatClient("gpt-4o-mini");
});
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=OpenAI}/{action=Index}/{id?}");

app.Run();
