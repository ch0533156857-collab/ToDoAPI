namespace TodoApi;

public class TaskItem
{
    public int Id { get; set; }
    public string ?Name { get; set; } = string.Empty;
    public bool IsComplete { get; set; }
}
