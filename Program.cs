using Ardalis.GuardClauses;
using hexa_droid.Services;
using hexa_droid.Services.Interface;
using hexa_droid.Utility;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.Net.Http.Headers;
using MinimalApis.Extensions.Results;
using System.Diagnostics;

var builder = WebApplication.CreateBuilder(args);

/* Middleware */
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors();
builder.Services.AddProblemDetailsDeveloperPageExceptionFilter();

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

    var result = await _userService.GetAllUsers();

    return Results.Ok(result);

}).WithName("GetAllUsers");

app.MapGet("/users/{id}", async (int id, IUserService _userService) =>
{
    Guard.Against.Negative(id, nameof(id));

    app.Logger.LogInformation($"Returning user by id: {id}");

    var result = await _userService.GetUserById(id);

    return result is null ? Results.NotFound($"No user found for id: {id}") : Results.Ok(result);

}).WithName("GetUserById");

app.MapPost("/users", async (User user, IUserService _userService) =>
{
    Guard.Against.Null(user, nameof(user));

    app.Logger.LogInformation($"Creating user {user?.Name}");

    var result = _userService.CreateUser(user);

    return Results.Created("/users", user);

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

/* Error Handler */
var problemJsonMediaType = new MediaTypeHeaderValue("application/problem+json");
app.MapGet("/error", Results<Problem, StatusCode> (HttpContext context) =>
{
    var error = context.Features.Get<IExceptionHandlerFeature>()?.Error;
    var badRequestEx = error as BadHttpRequestException;
    var statusCode = badRequestEx?.StatusCode ?? StatusCodes.Status500InternalServerError;

    if (context.Request.GetTypedHeaders().Accept?.Any(h => problemJsonMediaType.IsSubsetOf(h)) == true)
    {
        var extensions = new Dictionary<string, object> { { "requestId", Activity.Current?.Id ?? context.TraceIdentifier } };

        // JSON Problem Details
        return error switch
        {
            BadHttpRequestException ex => Results.Extensions.Problem(detail: ex.Message, statusCode: ex.StatusCode, extensions: extensions),
            _ => Results.Extensions.Problem(extensions: extensions)
        };
    }

    // Plain text
    return Results.Extensions.StatusCode(statusCode, badRequestEx?.Message ?? "An unhandled exception occurred while processing the request.");
})
   .ExcludeFromDescription();

app.Run();
