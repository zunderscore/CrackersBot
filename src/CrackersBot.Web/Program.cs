var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();

if (System.Diagnostics.Debugger.IsAttached)
{
    builder.Logging.AddDebug();
}
else
{
    builder.Logging.AddConsole();
}

if (!builder.Environment.IsDevelopment())
{
    builder.Configuration.AddAzureKeyVault(
        new Uri($"https://{builder.Configuration["KeyVaultName"]}.vault.azure.net/"),
        new Azure.Identity.DefaultAzureCredential()
    );
}

builder.Services.AddSingleton<IConfiguration>(builder.Configuration);
builder.Services.AddSingleton<CrackersBot.Core.IBotCore, CrackersBot.Web.Services.BotCore>();

builder.Services.AddHostedService<CrackersBot.Web.Services.BotService>();

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
else
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();
app.MapControllers();

app.Run();
