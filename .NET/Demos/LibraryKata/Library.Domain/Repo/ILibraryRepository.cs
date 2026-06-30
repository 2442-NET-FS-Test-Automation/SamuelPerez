namespace Library.Domain;

public interface ILabraryRepository
{
    #region CREATE
    void Add(LibraryItem item);
    #endregion CREATE


    #region READ
    LibraryItem GetById(int id);
    IReadOnlyList<LibraryItem> GetAll();
    #endregion READ


    #region UPDATE

    #endregion UPDATE


    #region DELETE
    bool Remove(int id);
    #endregion DELETE
}