using AppWebNetOpenIA_v1.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.AI;
using OpenAI;

var builder = WebApplication.CreateBuilder(args);

var port = Environment.GetEnvironmentVariable("PORT") ?? "10000";
builder.WebHost.UseUrls($"http://0.0.0.0:{port}");

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
if (string.IsNullOrWhiteSpace(connectionString))
{
    throw new Exception("No se encontró la cadena de conexión 'DefaultConnection'.");
}

var openAiApiKey = builder.Configuration["OpenAI:ApiKey"];
if (string.IsNullOrWhiteSpace(openAiApiKey))
{
    throw new Exception("No se encontró la API key de OpenAI.");
}

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddControllersWithViews();

builder.Services.AddSingleton<OpenAIClient>(_ =>
    new OpenAIClient(openAiApiKey));

builder.Services.AddSingleton<IChatClient>(_ =>
{
    var clienteOpenAi = new OpenAIClient(openAiApiKey);
    return clienteOpenAi.AsChatClient("gpt-4o-mini");
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
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