var builder = WebApplication.CreateBuilder(args);

// ============================================
// Registro de servicios
// ============================================
builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Gestor de Tareas API",
        Version = "v1",
        Description = "API REST para la gestión de tareas con autenticación JWT"
    });
});

var app = builder.Build();

// ============================================
// Pipeline HTTP
// ============================================
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Gestor de Tareas API v1");
        options.RoutePrefix = "swagger";
    });
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();