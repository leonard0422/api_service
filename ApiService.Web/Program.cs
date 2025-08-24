using ApiService.Application;
using ApiService.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Services (API only)
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(); // ← Swagger UI
builder.Services.AddSingleton<IUserRepository, UserRepository>();
builder.Services.AddSingleton<IEmailSender, EmailSender>();
builder.Services.AddScoped<IAuthService, AuthService>();

var app = builder.Build();

// Pipeline
if (app.Environment.IsDevelopment() || app.Configuration.GetValue<bool>("Swagger:Enabled"))
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "API v1");
        c.RoutePrefix = string.Empty; // ← Swagger UI en "/"
    });
}

app.UseHttpsRedirection();
app.UseAuthorization();

app.MapControllers();

app.Run();
