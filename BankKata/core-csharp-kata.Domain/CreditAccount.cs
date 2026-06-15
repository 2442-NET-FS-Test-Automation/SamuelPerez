namespace CoreCsharpKata.Domain;

public class CreditAccount : Account
{
    #region Properties
    public decimal CreditLine {get; set;}
    #endregion Properties

    #region Constructor
    public CreditAccount(string client, decimal balance, uint numberAccount, decimal creditLine) : base(client, balance, numberAccount)
    {
        CreditLine = creditLine;
    }
    #endregion Constructor

    #region Methods
    public override void Withdraw(decimal amount)
    {
        if (amount > 0 && (Balance - amount) >= -CreditLine)
        {
            Balance -= amount;
            Console.WriteLine($"Withdrawal with credit applied. New balance: {Balance}");
        }
        else
        {
            Console.WriteLine("Exceeds the credit line limit.");
        }
    }

    public override string ListInformation()
    {
        return $"""

        ====== No.:{NumberAccount} ======
        Type: Credit Account.
        Client: {Client}
        Balanace: {Balance}
        Credit Line: {CreditLine}
        """;
    }

    #endregion Methods

}