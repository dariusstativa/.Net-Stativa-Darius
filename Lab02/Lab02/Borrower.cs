namespace Lab02;

public record Borrower(int id, string Name, List<Book> BorrowedBooks)
{
    public override string ToString()
    {
        return (this.Name+" "+BorrowedBooks.Count);
    }
};
