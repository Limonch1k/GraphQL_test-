namespace ModelsDb;

public class TaskStatusDb
{
    public int StatusId { get; set; }

    public string Description { get; set; }

    public List<TaskDb> tasks { get; set; } = new();
}