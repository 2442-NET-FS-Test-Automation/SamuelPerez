using Serilog;

namespace Library.Domain;

public class InMemoryLibraryRepository : ILabraryRepository
{

    private readonly List<LibraryItem> _items = new();


    public void Add(LibraryItem item)
    {
        _items.Add(item);
        Log.Information("Added {Title} - Id:{id}", item.Title, item.Id);
    }

    public IReadOnlyList<LibraryItem> GetAll()
    {
        return _items.ToList();
    }

    public LibraryItem GetById(int id)
    {
        foreach (LibraryItem item in _items)
        {
            if(item.Id == id)
            {
                return item;
            }
        }
        Log.Warning("Lookup fail for id {Id}", id);
        throw new ItemNotFoundException(id);
    }

    public bool Remove(int id)
    {
        foreach (LibraryItem item in _items)
        {
            if (item.Id == id)
            {
                _items.Remove(item);
                Log.Information("Remove item with id {Id}", id);
                return true;
            }
        }
        Log.Information("Removal failed for item with id {Id}", id);
        return false;

    }
}