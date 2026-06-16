namespace CoreCsharpKata.Domain;

public interface ITransferable
{
    void Transfer(Account destinationAccount, decimal amount);
    void Transfer(Account destinationAccount, decimal amount, string message);
}