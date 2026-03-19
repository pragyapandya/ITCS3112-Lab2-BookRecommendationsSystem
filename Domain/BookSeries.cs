namespace Lab2BookRecommendationSystem.Domain;

public class BookSeries : Book
{
    public int EndYear { get; set; }

    public BookSeries(string isbn, string author, string title, int startYear, int endYear) 
        : base(isbn, author, title,  startYear)
    {
        EndYear = endYear;
    }

    public void UpdateBookSeries(string isbn, string author, string title, int startYear, int endYear)
    {
        UpdateBook(isbn, author, title, startYear);
        EndYear = EndYear;
    }
}