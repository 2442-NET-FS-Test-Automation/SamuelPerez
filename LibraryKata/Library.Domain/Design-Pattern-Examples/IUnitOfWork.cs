namespace Library.Domain;

public interface IUnitOfWork
{
    ILabraryRepository Items {get;}
    void Stage(string change);
    int Commit();
}