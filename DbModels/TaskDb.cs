namespace ModelsDb;

public class TaskDb
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }

    public int Status { get; set; }

    public DateTime CreatedAt { get; set; }
    public int CreatedBy { get; set; }

    public UserDb? user { get; set; }

    public TaskStatusDb taskStatus { get; set; } = new();

    public List<TaskAssignedDb> taskAssigned { get; set; } = new();
}