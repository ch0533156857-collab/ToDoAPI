using Microsoft.EntityFrameworkCore;
using TodoApi;
using Microsoft.OpenApi.Models;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

// -----------------------------
// 1️⃣ הוספת DbContext ל‑services
// -----------------------------
builder.Services.AddDbContext<ToDoDbContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("ToDoDB"),
        ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("ToDoDB"))
    )
);

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

builder.Services.AddEndpointsApiExplorer(); // מאפשר ל-Swagger לסרוק את ה-endpoints
builder.Services.AddSwaggerGen();           // מוסיף את Swashbuckle

var app = builder.Build();

// הפעלת Swagger
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();           // יוצר את הקובץ JSON של ה-API
    app.UseSwaggerUI();         // מציג את הממשק הגרפי ב-UI
}

app.UseCors();

// -----------------------------
// Route ל־root "/" רק כדי לבדוק שה‑API רץ
// -----------------------------
app.MapGet("/", () => "API is running! Go to /tasks to see the tasks.");

// -----------------------------
// 2️⃣ Routes ל‑tasks
// -----------------------------

// שליפת כל המשימות
app.MapGet("/tasks", async (ToDoDbContext context) =>
{
    return await context.Tasks.ToListAsync();
});

// הוספת משימה חדשה
app.MapPost("/tasks", async (TaskItem item, ToDoDbContext context) =>
{
    context.Tasks.Add(item);
    await context.SaveChangesAsync();
    return Results.Created($"/tasks/{item.Id}", item);
});

// עדכון משימה
app.MapPut("/tasks/{id}", async (int id, TaskItem updatedItem, ToDoDbContext context) =>
{
    var item = await context.Tasks.FindAsync(id);
    if (item == null) return Results.NotFound();

    item.Name = updatedItem.Name;
    item.IsComplete = updatedItem.IsComplete;

    await context.SaveChangesAsync();
    return Results.Ok(item);
});

// מחיקת משימה
app.MapDelete("/tasks/{id}", async (int id, ToDoDbContext context) =>
{
    var item = await context.Tasks.FindAsync(id);
    if (item == null) return Results.NotFound();

    context.Tasks.Remove(item);
    await context.SaveChangesAsync();
    return Results.NoContent();
});


using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ToDoDbContext>();

    if (!context.Tasks.Any())
    {
        context.Tasks.AddRange(
            new TaskItem { Name = "Learn React", IsComplete = false },
            new TaskItem { Name = "Build ToDo App", IsComplete = false },
            new TaskItem { Name = "Test API", IsComplete = false }
        );
        context.SaveChanges();
    }
}


// -----------------------------
// 3️⃣ הרצת האפליקציה
// -----------------------------
app.Run();



