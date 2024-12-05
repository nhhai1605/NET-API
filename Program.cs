using NET_API.Configurations;
using NET_API.Controllers;
using NET_API.Middlewares;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
// builder.Services.AddOpenApi();

//NHH: Add controllers
builder.Services.AddControllers();
builder.Services.Configure<AppConfig>(builder.Configuration.GetSection(nameof(AppConfig)));
var app = builder.Build();

//Only allow access to the AuthController
app.UseWhen(context => !context.Request.Path.StartsWithSegments("/Auth"), appBuilder =>
{
    appBuilder.UseMiddleware<AuthorizationMiddleware>();
});
app.UseMiddleware<HandleExceptionMiddleware>();

// Configure the HTTP request pipeline.
// if (app.Environment.IsDevelopment())
// {
//     app.MapOpenApi();
// }
 
app.UseHttpsRedirection();

//NHH: Need this for the controllers to work
app.MapControllers();

app.Run();
