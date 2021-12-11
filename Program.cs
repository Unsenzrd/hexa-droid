using hexa_droid;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors();
builder.Services.AddDbContext<ApiContext>(options => options.UseInMemoryDatabase("Users"));


var app = builder.Build();
DataGenerator.Initialize(app.Services);
// Configure the HTTP request pipeline.
if (builder.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

// Enable middleware to serve generated Swagger as a JSON endpoint.
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "API v1");
    c.RoutePrefix = string.Empty;
});

app.UseCors(p =>
{
    p.AllowAnyOrigin();
    p.AllowAnyMethod();
    p.AllowAnyHeader();
});

/* Users */
app.MapGet("/users", (ApiContext context) =>
{
    app.Logger.LogInformation("Returning users");

    var response = context.Users.ToListAsync();

    return Results.Ok(response.Result);
}).WithName("GetUsers");

app.MapPost("/users", async (http) =>
{
    var user = JsonSerializer.Deserialize<User>(http.Request.Body);
    app.Logger.LogInformation($"Creating user {user?.Name}");
    await using var context = app.Services.GetService<ApiContext>();
    await context.Users.AddAsync(user);
    await context.SaveChangesAsync();
    http.Response.StatusCode = 201;
}).WithName("CreateUser");

app.Run();
