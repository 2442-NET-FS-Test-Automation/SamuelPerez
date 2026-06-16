using Serilog;

namespace Library.Domain;

public class LibraryUnitOfWork : IUnitOfWork
{
    public ILabraryRepository Items {get;}

    private readonly List<string> _staged = new();

    public LibraryUnitOfWork(ILabraryRepository items)
    {
        Items = items;
    }

    public int Commit()
    {
        int count = _staged.Count;
        Log.Information("LibraryUnitOfWork commited {Count} staged change(s)", count);

        _staged.Clear();

        return count;
    }

    public void Stage(string change)
    {
        _staged.Add(change);
    }
}