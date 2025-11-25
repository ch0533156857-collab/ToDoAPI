using Microsoft.EntityFrameworkCore;
using TodoApi;
using Microsoft.OpenApi.Models;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ToDoDbContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        new MySqlServerVersion(new Version(8, 0, 21)),
        mySqlOptions => mySqlOptions.EnableRetryOnFailure(
            maxRetryCount: 5,
            maxRetryDelay: TimeSpan.FromSeconds(10),
            errorNumbersToAdd: null
        )
    )
);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();
app.UseCors();

app.MapGet("/", () => "API is running! Go to /tasks to see the tasks.");

// אתחול הדאטאבייס
app.MapGet("/init", async (ToDoDbContext context) =>
{
    if (!await context.Tasks.AnyAsync())
    {
        context.Tasks.AddRange(
            new TaskItem { Name = "Learn React", IsComplete = false },
            new TaskItem { Name = "Build ToDo App", IsComplete = false },
            new TaskItem { Name = "Test API", IsComplete = false }
        );
        await context.SaveChangesAsync();
        return Results.Ok("Database initialized!");
    }
    return Results.Ok("Database already has data");
});

app.MapGet("/tasks", async (ToDoDbContext context) =>
{
    return await context.Tasks.ToListAsync();
});

app.MapPost("/tasks", async (TaskItem item, ToDoDbContext context) =>
{
    context.Tasks.Add(item);
    await context.SaveChangesAsync();
    return Results.Created($"/tasks/{item.Id}", item);
});

app.MapPut("/tasks/{id}", async (int id, TaskItem updatedItem, ToDoDbContext context) =>
{
    var item = await context.Tasks.FindAsync(id);
    if (item == null) return Results.NotFound();

    item.Name = updatedItem.Name;
    item.IsComplete = updatedItem.IsComplete;
    await context.SaveChangesAsync();
    return Results.Ok(item);
});

app.MapDelete("/tasks/{id}", async (int id, ToDoDbContext context) =>
{
    var item = await context.Tasks.FindAsync(id);
    if (item == null) return Results.NotFound();

    context.Tasks.Remove(item);
    await context.SaveChangesAsync();
    return Results.NoContent();
});

app.UseCors("AllowAll");

app.Run();