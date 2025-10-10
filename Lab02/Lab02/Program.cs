using Lab02;
using System.Linq;
var b1=new Book("Tom Sawyer","idk",1997);
var b2=new Book("Great Expectations","Dickens",1998);
var b3=new Book("Heidi","darius",2004);
var bookList=new List<Book>();
bookList.Add(b1);
bookList.Add(b2);
bookList.Add(b3);
var borrower=new Borrower(1,"Tom",bookList);
var b4 = new Book("Enigma", "Calinescu", 1980);
var borrower2 = borrower with
{
     BorrowedBooks = new List<Book>(borrower.BorrowedBooks) { b4 }
};
List<Borrower> Borrowers = new List<Borrower>();
Borrowers.Add(borrower);
Borrowers.Add(borrower2);
List<string> Books = new List<string>();
void patternMatcher(object obj)
{
     if (obj is Book b)
     {
          Console.WriteLine(b.ToString());
     }
     else if (obj is Borrower bo)
     {
          Console.WriteLine(bo.ToString());
     }
     else
     {
          Console.WriteLine("Unknown type");
     }
}


while (true)
{
    Console.WriteLine("Pick the desired method: ");
    Console.WriteLine("1.Add Book");
    Console.WriteLine("2.Pattern Matching");
    Console.WriteLine("3.Quit");
    Console.WriteLine("4.Filter books after 2010");   // <- new option
    string input = Console.ReadLine();

    if (input == "1")
    {
        while (true)
        {
            Console.WriteLine("1.Add book");
            Console.WriteLine("2.Quit");
            string newInput = Console.ReadLine();
            if (newInput == "2")
                break;
            else if (newInput == "1")
            {
                Console.WriteLine("Write the title of the Book:");
                string name = Console.ReadLine();
                Books.Add(name);
            }
        }

        for (int i = 0; i < Books.Count; i++)
            Console.WriteLine($"{i}. {Books[i]}");
    }

    else if (input == "2")
    {
        Console.WriteLine("Pick the object : ");
        Console.WriteLine("1.A Book from bookList");
        Console.WriteLine("2.A Borrower from Borrowers");
        Console.WriteLine("3.Something else");
        string pick = Console.ReadLine();

        object obj = null;

        if (pick == "1")
        {
            if (bookList.Count == 0)
            {
                Console.WriteLine("No books found");
                continue;
            }

            Console.WriteLine("Pick book index:");
            for (int i = 0; i < bookList.Count; i++)
                Console.WriteLine($"{i}. {bookList[i]}");

            int index = Convert.ToInt32(Console.ReadLine());
            if (index < 0 || index >= bookList.Count)
            {
                Console.WriteLine("Invalid index");
                continue;
            }

            obj = bookList[index];
        }
        else if (pick == "2")
        {
            if (Borrowers.Count == 0)
            {
                Console.WriteLine("No borrowers found");
                continue;
            }

            Console.WriteLine("Pick borrower index:");
            for (int i = 0; i < Borrowers.Count; i++)
                Console.WriteLine($"{i}. {Borrowers[i].Name}");

            int index = Convert.ToInt32(Console.ReadLine());
            if (index < 0 || index >= Borrowers.Count)
            {
                Console.WriteLine("Invalid index");
                continue;
            }

            obj = Borrowers[index];
        }
        else
        {
            obj = 123;
        }

        patternMatcher(obj);
    }

    else if (input == "4") 
    {
        var recentBooks = bookList.Where(static b => b.YearPublished > 2010).ToList();

        if (recentBooks.Count == 0)
        {
            Console.WriteLine("No books published after 2010.");
        }
        else
        {
            Console.WriteLine("Books published after 2010:");
            for (int i = 0; i < recentBooks.Count; i++)
                Console.WriteLine($"{i}. {recentBooks[i].Title} ({recentBooks[i].YearPublished})");
        }
    }

    else if (input == "3")
    {
        break;
    }

    else
    {
        Console.WriteLine("Optiune invalida.");
    }
}



     
    
