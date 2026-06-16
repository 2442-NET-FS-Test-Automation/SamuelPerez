namespace Library.Domain;

public class ItemNotFoundException : LibraryException
{
    public int Id {get;}

    public ItemNotFoundException(int id) 
        : base($"No library item with id {id}")
    {
        Id = id;
    }
}