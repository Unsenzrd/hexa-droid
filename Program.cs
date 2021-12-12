using hexa_droid.Services;
using hexa_droid.Services.Interface;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

/* Middleware */
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors();

/* Context */
builder.Services.AddScoped<ApiContext, ApiContext>();
builder.Services.AddDbContext<ApiContext>(options => options.UseInMemoryDatabase("Users"));

/* Register Services */
builder.Services.AddTransient<IUserService, UserService>();


var app = builder.Build();
if (builder.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "API v1");
    c.RoutePrefix = string.Empty;
});

app.UseExceptionHandler("/error");

app.UseCors(p =>
{
    p.AllowAnyOrigin();
    p.AllowAnyMethod();
    p.AllowAnyHeader();
});

/* Users */
app.MapGet("/users", async (IUserService _userService) =>
{
    app.Logger.LogInformation("Returning users");

    var response = await _userService.GetAllUsers();

    return Results.Ok(response);

}).WithName("GetAllUsers");

app.MapGet("/users/{id}", async (int id, IUserService _userService) =>
{
    app.Logger.LogInformation($"Returning user by id: {id}");

    var response = await _userService.GetUserById(id);

    return response is null ? Results.NotFound($"No user found for id: {id}") : Results.Ok(response);

}).WithName("GetUserById");

app.MapPost("/users", async (HttpContext http, User u, ApiContext context) =>
{
    app.Logger.LogInformation($"Creating user {u?.Name}");

    await context.Users.AddAsync(u);
    await context.SaveChangesAsync();

    return Results.Created("/users", u);

}).WithName("CreateUser");

app.MapPut("/users/{id}", async (HttpContext http, User u, ApiContext context, int id) =>
{
    app.Logger.LogInformation($"Creating user {u?.Name}");

    var user = context.Users.FirstOrDefault(u => u.Id == id);
    user.Email = u.Email;
    context.Users.Update(user);    
    await context.SaveChangesAsync();

    return Results.Ok(u);

}).WithName("UpdateUserById");

/* Misc */
app.MapGet("/error", () => Results.Problem(statusCode: 500));

app.Run();
