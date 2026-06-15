namespace CoreCsharpKata.Domain;

public class SavingsAccount : Account, ITransferable
{
    #region Properties
    public decimal InterestRate {get; set;}
    #endregion Properties

    #region Constructor
    public SavingsAccount(string client, decimal balance, uint numberAccount, decimal interestRate) : base(client, balance, numberAccount)
    {
        InterestRate = interestRate;
    }

    #endregion Constructor

    #region Methods
    public void ApplyInterest()
    {
        decimal interest = Balance * InterestRate;
        Deposit(interest);
        Console.WriteLine($"Applied interest. New balance is: {Balance:C}");
    }

    public void Transfer(Account destinationAccount, decimal amount)
    {
        if (amount > 0 && amount <= Balance)
        {
            this.Withdraw(amount);
            destinationAccount.Deposit(amount);
            Console.WriteLine($"Succesful transfer to {destinationAccount.Client}");
        }
        else
        {
            Console.WriteLine("Not enough funds!");
        }
    }

    public void Transfer(Account destinationAccount, decimal amount, string message)
    {
        if (amount > 0 && amount <= Balance)
        {
            this.Withdraw(amount);
            destinationAccount.Deposit(amount);
            Console.WriteLine($"""
            Succesful transfer to {destinationAccount.Client}
            Concept message: {message}
            """);
        }
        else
        {
            Console.WriteLine("Not enough funds!");
        }
    }

    public override string ListInformation()
    {
        return $"""

        ====== No.:{NumberAccount} ======
        Type: Savings Account.
        Client: {Client}
        Balanace: {Balance}
        Interest Rate: {InterestRate}
        """;
    }

    #endregion Methods
}