namespace Library.Domain;

public abstract class LibraryItem
{
    public string Title {get; private set;}
    public string Author {get; private set;}
    private static int _nextId = 1;
    public int Id{ get;}

    protected LibraryItem(string title, string author)
    {
        Id = _nextId++;
        Title = title;
        Author = author;
    }

    public abstract string Describe();

    public override string ToString() => Describe();

    public virtual string ShelfLabel()
    {
        return $"{Id}: {Title}";
    }
}