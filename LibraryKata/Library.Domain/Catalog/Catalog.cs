namespace Library.Domain;

public partial class Catalog
{
    #region List
    public List<LibraryItem> _items = [];
    #endregion List

    #region Stack
    private readonly Stack<LibraryItem> _returnCart = new();
    #endregion Stack

    #region Queue
    private readonly Queue<string> _holdQueue = new();
    #endregion Queue

    #region LinkedList
    private readonly LinkedList<LibraryItem> _readingList = new();
    #endregion LinkedList

    #region HashSet
    private readonly HashSet<string> _authors = new();
    #endregion HashSet

    public IReadOnlyCollection<string> Authors => _authors;




    // --- List surface ---
    // Wrapping Add/Remove/index is the whole point of encapsulation: callers state intent,
    // the Catalog decides how. Count was already exposed this way; now the rest is too.
    public int Count => _items.Count;
    public LibraryItem this[int index] => _items[index]; // indexer: read catalog[0] like an array, but read-only
    public void Add(LibraryItem item){
        _items.Add(item);
        _authors.Add(item.Author);
    }
    public bool Remove(LibraryItem item) => _items.Remove(item);

    public bool IsEmpty => _items.Count == 0;

    // --- Stack surface (return cart) ---
    public void DropInReturnCart(LibraryItem item) => _returnCart.Push(item);
    public LibraryItem Reshelve() => _returnCart.Pop();   // most-recently-returned first (LIFO)
    public int CartCount => _returnCart.Count;

    // --- Queue surface (holds line) ---
    public void PlaceHold(string member) => _holdQueue.Enqueue(member);
    public string ServeNextHold() => _holdQueue.Dequeue(); // earliest request first (FIFO)
    public int HoldsWaiting => _holdQueue.Count;

    // --- LinkedList surface (reading list) ---
    public void AddToReadingList(LibraryItem item) => _readingList.AddLast(item);
    public void AddNextUp(LibraryItem item) => _readingList.AddFirst(item); // jump to the front of the list
    // Expose as IEnumerable so callers can foreach over it but cannot mutate the linked list directly.
    public IEnumerable<LibraryItem> ReadingList => _readingList;
}