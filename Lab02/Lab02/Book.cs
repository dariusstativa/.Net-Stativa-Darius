namespace Lab02;

public record Book(string Title,string Author,int YearPublished)
{
    public override string ToString()
    {
        return (this.Title + " " + this.YearPublished);
    }
};