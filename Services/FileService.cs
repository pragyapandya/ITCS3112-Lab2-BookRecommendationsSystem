using Lab2BookRecommendationSystem.Domain;
using Lab2BookRecommendationSystem.Repositories;
using Lab2BookRecommendationSystem.Services;

public class FileService
{
    public string BookFile { get; set; }
    public string MemberFile { get; set; }

    public FileService(string bookFile, string memberFile)
    {
        BookFile = bookFile;
        MemberFile = memberFile;
    }

    public BookService  PopulateBooks(string bookFile)
    {
        if (bookFile is null)
        {
            throw new ArgumentNullException(nameof(bookFile));
        }

        var books = new BookService(new BookRepository());
        int isbnCounter = 1;
        
        foreach (var line in File.ReadLines(bookFile))
        {
            string isbn = $"B{isbnCounter++}";
            string author = line.Split(",")[0].Trim();
            string title = line.Split(",")[1].Trim();
            string yearStr = line.Split(",")[2].Trim();

            if (yearStr.Contains('-'))
            {
                int.TryParse(yearStr.Split('-')[0], out int startYear);
                int endYear;
                
                if (yearStr.Split('-')[1].ToLower() == "present")
                {
                    endYear = DateTime.Today.Year;
                }
                else
                {
                    int.TryParse(yearStr.Split('-')[1], out endYear);
                }
                books.NewBook(new BookSeries(isbn, author, title, startYear, endYear));
            }
            else
            {
                int.TryParse(yearStr, out int year);
                books.NewBook(new Book(isbn, author, title, year));
            }
        }
        
        // Test to confirm books list exist
        /*
        Console.WriteLine(books.Count);
        Console.WriteLine(books[44].Title);
        */
        return books;
    }

    public void PopulateRatings(string ratingFile)
    {
        var members = new List<Member>();
        var ratings = new List<Rating>();
        int memberAccountId = 1;

        if (ratingFile is null)
        {
            throw new ArgumentNullException(nameof(ratingFile));
        }


        // Parsing of Member&Ratings File
        foreach (var line in File.ReadLines(ratingFile))
        {
            var lineSplit = line.Split(' ');
            string memberName = lineSplit[0].Trim();
            members.Add(new Member(memberAccountId, memberName));

            // Need Rating Logic to Complete
            // for (int i = 1; i < lineSplit.Length; i++)
            // {
            //
            // }
            memberAccountId++;
        }


        // Test to confirm members and ratings lists exist
        /*
        Console.WriteLine(members.Count);
        Console.WriteLine(members[4].AccountId);
        Console.WriteLine(ratings.Count);
        */
        
        
        
    }
    
}