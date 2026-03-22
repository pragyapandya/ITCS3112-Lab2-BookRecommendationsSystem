namespace Lab2BookRecommendationSystem.Domain;

public class Book
{
    public string Isbn { get; private set; }
    public string Author { get; set; }
    public string Title { get; set; }
    public int Year { get; set; }
    
    public Book(string isbn, string author, string title, int year)
    {
        Isbn = isbn;
        Author = author;
        Title = title;
        Year = year;
    }

    public void UpdateBook(string isbn, string author, string title, int year)
    {
        Isbn = isbn;
        Author = author;
        Title = title;
        Year = year;
    }
}