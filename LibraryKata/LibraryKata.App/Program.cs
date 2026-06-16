using System.Runtime.CompilerServices;
using Library.Domain;
using LibraryKata;

namespace LibraryKata.App;
public class Program
{
    public static void Main()
    {
        DataTypeAndOperators();
        ClassesExample();
        OopDemo();
        CollectionsDemo();
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

    public static void ClassesExample()
    {
        Console.WriteLine("Using our domain Book Class.");

        OldBook dune = new OldBook("Dune", "Luis", 3);
        OldBook littlePrince = new OldBook("Little Prince", "Antoine de Saint-Exupéry", 0);

        Console.WriteLine(dune);

        Console.WriteLine($"Checking out Dune: {Convert.ToString(dune.CheckOut)}");
        Console.WriteLine($"Checking out Little Prince: {littlePrince.CheckOut}");
    } 

    public static void OopDemo()
    {
        Console .WriteLine("\n\n == OOP Demo ==");

        LibraryItem[] catalog =
        {
            new Book("Dune", "Frank", 2),
            new ReferenceBook("C# Language Standards", "Microsoft", "Technology"),
            new Magazine("Sports Illustrated", "Conde Naste", 5)
        };

        foreach (LibraryItem item in catalog)
        {
            Console.WriteLine(item.Describe());
        }

        foreach (LibraryItem item in catalog)
        {
            if (item is ILendable lendable)
            {
                Console.WriteLine($"{item.Title}: checkout -> {lendable.CheckOut()}");
            } else
            {
                Console.WriteLine($"{item.Title} is Reference only.");
            }
        }

        Magazine wired = new Magazine("Wired", "Conde Nast", 3);
        LibraryItem baseMag = wired;

        Console.WriteLine("== Override vs new on the same object, different ref type");
        Console.WriteLine($"Magazine reference -> {wired.Describe()}");
        Console.WriteLine($"LibraryItem reference -> {baseMag.Describe}");

    }

    #region CollectionsDemo
    private static void CollectionsDemo()
    {
        Console.WriteLine("==== Demo Stuff ====");

        Catalog catalog = new();

        Book dune = new Book("Dune", "Frank Herbert", 3);
        catalog._items.Add(dune);

        catalog._items.Add(new ReferenceBook("C# Language Specs", "Microsoft", "Technology"));
        catalog._items.Add(new Magazine("Nat Geo", "Charlie", 4));

        Console.WriteLine($"Catalog holds {catalog._items.Count}, first is {catalog._items[0].Title}");

        ItemKind kind = ItemKind.Magazine;

        ShelfLocation location = new ShelfLocation(3, 12);

        Console.WriteLine($"{kind} sits at {location}");

        Shelf<LibraryItem> shelf = new Shelf<LibraryItem>(2);
        Shelf<int> intShelf = new Shelf<int>(200);

        shelf.TryAdd(catalog._items[0]);
        shelf.TryAdd(catalog._items[1]);

        Console.WriteLine($"Trying to add a third thing in our catalog: {shelf.TryAdd(catalog._items[2])}");





    }
    #endregion CollectionsDemo
}