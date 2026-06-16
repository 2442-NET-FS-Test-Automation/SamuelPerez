using System.Collections;
using System.ComponentModel.Design;
using CoreCsharpKata.Domain;
#pragma warning disable CS8604
#pragma warning disable CS8600


namespace CoreCsharpKata.App;

public class Program
{
    #region Main
    static void Main()
    {
        bool active = true;
        while (active)
        {
            Menu();
            int choice = int.Parse(Console.ReadLine());
            switch (choice)
            {
                case 1:  
                    Console.Clear();
                    AccountSelectionMenu();
                    int type = int.Parse(Console.ReadLine());
                    Console.WriteLine("What is your name?");
                    string name = Console.ReadLine();
                    OpenAccount(type, name);
                    break;
                case 2:
                    MakeWithdraw();
                    break;

                case 3:
                    MakeTransfer();
                    break;
                case 4:
                    MakeDeposit();
                    break;
                case 5:
                    ListAccounts();
                    break;
                case 0:
                    active = false;
                    break;
            }

        }
    }
    #endregion Main

    #region Entities
    public static List<Account> accounts = new()
    {
        new SavingsAccount("Samuel Pérez", 300.56m, 1, 0.05m),
        new CreditAccount("Samuel Pérez", 800.76m, 2, 1345.00m)
    };
    #endregion Entities

    #region Methods
    private static void Menu() => Console.WriteLine("""
        
        ===== MENU =====
        1: Open a new account.
        2: Make a withdraw.
        3: Make a transfer.
        4: Make a deposit.
        5: List all accounts.
        0: Exit.
        
        """);

    private static void AccountSelectionMenu() => Console.WriteLine("""
        1: Savings account.
        2: Credit account.
        """);
    private static string OpenAccount(int choice, string name)
    {
        switch (choice)
        {
            case 1:
                SavingsAccount sAccount = new SavingsAccount(name, 0.00m, (uint)Random.Shared.Next(1,100000000), 0.0625m);
                accounts.Add(sAccount);
                return sAccount.ToString();
            case 2:
                CreditAccount cAccount = new CreditAccount(name, 0.00m, (uint)Random.Shared.Next(1,100000000), 1500.00m);
                accounts.Add(cAccount);
                return cAccount.ToString();
            default:
                return "Invalid choice";
        }
    }

    private static void MakeWithdraw(){
        Console.Clear();
        Console.WriteLine("Enter you account number.");
        uint number = uint.Parse(Console.ReadLine());
        Console.WriteLine("Enter amount to withdraw.");
        decimal amount = decimal.Parse(Console.ReadLine());

        Account origin = null;

        
        foreach(Account account in accounts){
            if (account.NumberAccount == number)
            {
                origin = account;
                break;
            }
        }
        origin?.Withdraw(amount);
    }

    private static void MakeTransfer(){
        Console.Clear();
        Console.WriteLine("Enter your account number.");
        uint originNumber = uint.Parse(Console.ReadLine());
        Console.WriteLine("Enter the destination account number.");
        uint destinationNumber = uint.Parse(Console.ReadLine());
        Console.WriteLine("Enter amount to withdraw.");
        decimal amount = decimal.Parse(Console.ReadLine());
        Console.WriteLine("Enter transfer message.");
        string? message = Console.ReadLine();
        Account origin = null;
        Account destination = null;


        foreach(Account account in accounts){
            if (account.NumberAccount == originNumber)
                origin = account;
            if(account.NumberAccount == destinationNumber)
                destination = account;
        }

        if(origin==null || destination==null)
        {
            Console.WriteLine("An account was not found.");
            return;
        }

        if(origin is ITransferable transferable){
            if(string.IsNullOrWhiteSpace(message)){
                transferable.Transfer(destination, amount);
            }
            else
            {
                transferable.Transfer(destination, amount, message);
            }
        } else{
            Console.WriteLine("The account doesn't allow transfers.");
        }
    }

    private static void MakeDeposit(){
        Console.Clear();
        Console.WriteLine("Enter the destination account number.");
        uint destinationNumber = uint.Parse(Console.ReadLine());
        Console.WriteLine("Enter amount to deposit.");
        decimal amount = decimal.Parse(Console.ReadLine());

        Account destination = null;
        foreach(Account account in accounts)
        {
            if(account.NumberAccount == destinationNumber)
            {
                destination = account;
                break;
            }

        }
        if (destination == null){
            Console.WriteLine("Account not found.");
            return;
        }
        if (destination is SavingsAccount savingsAccount){
            savingsAccount.ApplyInterest(amount);
        }else{
            destination.Deposit(amount);
        }

    }

    private static void ListAccounts()
    {
        Console.Clear();
        foreach (var acc in accounts)
        {
            Console.WriteLine(acc);
        }
    }

    #endregion Methods
}