namespace Library.Domain;

public interface ILendable
{
    bool CheckOut();
    void Return();
}