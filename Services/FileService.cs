using Lab2BookRecommendationSystem.Domain;
namespace Lab2BookRecommendationSystem.Services;

public class FileService
{
    public string BookFile { get; set; }
    public string MemberFile { get; set; }

    public FileService(string bookFile, string memberFile)
    {
        BookFile = bookFile;
        MemberFile = memberFile;
    }

    public void PopulateBooks(string bookFile)
    {
        if (bookFile is null)
        {
            throw new ArgumentNullException(nameof(bookFile));
        }

        var books = new List<Book>();
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
                books.Add(new BookSeries(isbn, author, title, startYear, endYear));
            }
            else
            {
                int.TryParse(yearStr, out int year);
                books.Add(new Book(isbn, author, title, year));
            }
        }
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
        }

        memberAccountId++;
    }
}