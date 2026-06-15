namespace CoreCsharpKata.Domain;

public abstract class Account
{
    #region Properties
    public string Client {get; set;}
    public decimal Balance {get; protected set;}
    public uint NumberAccount {get; set;}

    #endregion Properties

    #region Constructor
    public Account(string client, decimal balance, uint numberAccount)
    {
        Client = client;
        Balance = balance;
        NumberAccount = numberAccount;
    }
    #endregion Constructor

    #region Methods
    public virtual void Deposit(decimal amount)
    {
        if (amount > 0)
        {
            Balance += amount;
            Console.WriteLine($"Success! This is your new balance: {Balance}!");
        }
    }
    
    public virtual void Withdraw(decimal amount)
    {
        if (amount > 0 && Balance >= amount)
        {
            Balance -= amount;
            Console.WriteLine($"Success! This is your new balance: {Balance}!");
        }
        else
        {
            Console.WriteLine($"Not enough funds!");
        }
    }

    #endregion Methods

}
