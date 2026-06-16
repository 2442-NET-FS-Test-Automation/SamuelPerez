namespace Library.Domain;
public class Shelf<T>
{
    private readonly T[] _slots;
    private int used;

    public Shelf(int capacity)
    {
        _slots = new T[capacity];
    }

    public int Capacity => _slots.Length;
    public int Count => used;

    #region Method
    public bool TryAdd(T item)
    {
        if (used == _slots.Length)
        {
            return false;
        }

        _slots[used++] = item;
        return true;
    }

    public T Get(int index)
    {
        return _slots[index];
    }
    #endregion Method
}