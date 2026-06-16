namespace Library.Domain;

public class Catalog
{
    #region List
    public List<LibraryItem> _items = [];
    public int Count => _items.Count;
    #endregion List

    #region Stack
    public readonly Stack<LibraryItem> _returnCart = new();
    #endregion Stack

    #region Queue
    public readonly Queue<string> _holdQueue = new();
    #endregion Queue

    #region LinkedList
    public readonly LinkedList<LibraryItem> _readingList = new();
    #endregion LinkedList
}