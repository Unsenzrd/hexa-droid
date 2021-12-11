using Microsoft.EntityFrameworkCore;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors();
builder.Services.AddScoped<ApiContext, ApiContext>();
builder.Services.AddDbContext<ApiContext>(options => options.UseInMemoryDatabase("Users"));


var app = builder.Build();
//DataGenerator.Initialize(app.Services);
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
    if (!context.Users.Any())
    {
        context.Users.AddRange(
        new User
        {
            Id = 1,
            Name = "Josh",
            Email = "joshh@email.com",
        //Attributes = new Dictionary<string, string> { { "age", "29.5" }, { "eyes", "yes" } },
    },
        new User
        {
            Id = 2,
            Name = "Toni",
            Email = "tonii@email.com",
        //Attributes = new Dictionary<string, string> { { "age", "29" }, { "eyes", "yes" } },
    });

        context.SaveChanges();
    }

    var response = context.Users.ToListAsync();

    return Results.Ok(response.Result);
}).WithName("GetUsers");

app.MapPost("/users", async (HttpContext http, User u, ApiContext context) =>
{
    app.Logger.LogInformation($"Creating user {u?.Name}");
    await context.Users.AddAsync(u);
    await context.SaveChangesAsync();
    return Results.Created("/users", u);
}).WithName("CreateUser");

app.Run();
