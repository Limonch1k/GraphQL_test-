namespace ModelsDb;

public class UserDb
{
    public int Id { get; set; }
    public string Username { get; set; }
    public string Email { get; set; }

    public int Role { get; set; }

    public UserRoleDb userRole { get; set; } = new();

    public List<TaskAssignedDb> taskAssigneds { get; set; }  = new();

    public List<TaskDb> tasks { get; set; }  = new();
}