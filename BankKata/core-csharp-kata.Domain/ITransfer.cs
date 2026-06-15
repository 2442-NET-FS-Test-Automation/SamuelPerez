using CoreCsharpKata.Domain.Account;

namespace CoreCsharpKata.Domain;

public interface ITransferible
{
    void Transfer(Account account, decimal amount);
}