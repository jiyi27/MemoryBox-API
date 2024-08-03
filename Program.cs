using MemoryBox_API.Config;
using MemoryBox_API.Middlewares;

var builder = WebApplication.CreateBuilder(args);

var appConfiguration = new AppConfiguration(builder.Configuration);
appConfiguration.ConfigureServices(builder.Services);

var app = builder.Build();

app.UseHttpsRedirection()
    .UseCors(b => b.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader())
    .UseAuthorization()
    .UseGlobalExceptionHandling();
app.MapControllers();

app.Run();