namespace Domain.Entities;

public class Position
{
    public Position()
    {
        Guid = System.Guid.NewGuid().ToString();
    }

    private Position(string guid)
    {
        Guid = guid;
    }

    public override string ToString() => Name;

    public string Guid { get; } = null!;

    public string Title { get; set; } = null!;

    public virtual ICollection<Employee> Employees { get; set; } = new List<Employee>();
}