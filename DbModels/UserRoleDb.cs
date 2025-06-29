namespace ModelsDb;

public class UserRoleDb
{
    public int Id { get; set; }

    public string Description { get; set; }

    public List<UserDb> users { get; set; }  = new();
}