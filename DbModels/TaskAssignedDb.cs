namespace ModelsDb;

public class TaskAssignedDb
{
    public int TaskId { get; set; }

    public int UserId { get; set; }

    public TaskDb task { get; set; } = new();

    public UserDb user { get; set; } = new();
}