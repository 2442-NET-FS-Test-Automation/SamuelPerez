using System.Runtime.CompilerServices;

namespace LibraryKata.App;
public class Program
{
    public static void Main()
    {        
    }

    private static void DataTypeAndOperators()
    {
        Console.WriteLine("=== Data types and operators ===");

        int copies = 3;
        double lateFee = 0.1;
        bool isMember = true;
        char shelf = 'A';
        string title = "Clean Code";

        int total = copies * 2;
        bool isEnough = total > 4;
        
    }
    private static void ControlFlow()
    {
        Console.WriteLine("\n== Control Flow ==");
        int copiesAvailable = 0;
        bool isMember = true;


        //if - else if - else
        if(copiesAvailable > 1)
            Console.WriteLine("Many available for chekout");
        else if (copiesAvailable == 1)
            Console.WriteLine("Last Copy!");
        else
            Console.WriteLine("Out of stock!");

        //swtitch
        string genre = "Mystery";

        switch (genre)
        {
            case "Mystery":
                Console.WriteLine("Check Action A!");
                break;
            case "Science-Fiction":
                Console.WriteLine("Check Action B!");
                break;
            default:
                Console.WriteLine("Default.");
                break;
        }

        int number = 3;
        string day = number switch
        {
            1 => "Lunes",
            2 => "Martes",
            3 => "Wednesday",
            _ => "Other day." 
        };
    }

    private static void Loops()
    {
        // For, While, do-while, etc

        for (int day = 1; day <= 3; day++)
        {
            Console.WriteLine($"Reminder day{day}: fee so far {CalculateLateFee(day)}");
        }

        int onShelf = 3;
        while (onShelf > 0)
        {
            Console.WriteLine($"{onShelf} copies on the shelf");
            onShelf--;
        }
        Console.WriteLine("NO copies on the shelf!");
    }

    private static decimal CalculateLateFee(int daysLate) => daysLate * 2;

    private static void ArraysWork()
    {
        string[] books = ["Dune", "Harry Potter", "Percy Jackson", "Lord of the rings"];

        Console.WriteLine(books[2]);
        
        foreach (string book in books)
        {
            Console.WriteLine(book);
        }
    }
}