namespace ViewModel;
public class Task
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }

    public string Status { get; set; }

    public DateTime CreatedAt { get; set; }
    public string CreatedBy { get; set; }
}