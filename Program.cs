using Lab2BookRecommendationSystem.Contracts;
using Lab2BookRecommendationSystem.Repositories;
using Lab2BookRecommendationSystem.Services;
using System.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using Lab2BookRecommendationSystem.Domain;
 
namespace Lab2BookRecommendationSystem;
 
public class Program
{
   
    private static BookService bookService = null!;
    private static MemberService memberService = null!;
    private static RatingService ratingService = null!;
    private static RecommendationService recommendationService = null!;
    private static FileService fileService = null!;
 
   
    public static void Main(string[] args)
    {
        Console.WriteLine("Welcome to the Book Recommendation Program.");
        Console.WriteLine();
 
        
        string baseDir      = AppContext.BaseDirectory;
        string defaultBooks = Path.GetFullPath(Path.Combine(baseDir, @"..\..\..\Files\books.txt"));
        string defaultRatings = Path.GetFullPath(Path.Combine(baseDir, @"..\..\..\Files\ratings.txt"));
 
        Console.Write("Enter books file: ");
        string booksInput = Console.ReadLine()?.Trim() ?? "";
        string booksPath  = string.IsNullOrEmpty(booksInput) ? defaultBooks : booksInput;
 
        Console.Write("Enter rating file: ");
        string ratingsInput = Console.ReadLine()?.Trim() ?? "";
        string ratingsPath  = string.IsNullOrEmpty(ratingsInput) ? defaultRatings : ratingsInput;
   
        fileService = new FileService(booksPath, ratingsPath);
 
        try
        {
            bookService = fileService.PopulateBooks(booksPath);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading books: {ex.Message}");
            return;
        }
 
        try
        {
            (memberService, ratingService) = fileService.PopulateRatings(ratingsPath);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading ratings: {ex.Message}");
            return;
        }
 
        try
        {
            recommendationService = new RecommendationService(
                fileService.BookRepo!,
                ratingService,
                fileService.MemberRepo!);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error initializing services: {ex.Message}");
            return;
        }
 
        Console.WriteLine();
        Console.WriteLine($"# of books: {bookService.ListAllBooks().Count}");
        Console.WriteLine($"# of memberList: {fileService.MemberRepo!.GetAllMembers().Count}");
 
        
        bool running = true;
        while (running)
        {
            if (!memberService.IsLoggedIn())
            {
                PrintLoggedOutMenu();
                string choice = Console.ReadLine()?.Trim() ?? "";
                switch (choice)
                {
                    case "1": HandleAddMember();  break;
                    case "2": HandleAddBook();    break;
                    case "3": HandleLogin();      break;
                    case "4": running = false;    break;
                    default:
                        Console.WriteLine("Invalid option. Please try again.");
                        break;
                }
            }
            else
            {
                PrintLoggedInMenu();
                string choice = Console.ReadLine()?.Trim() ?? "";
                switch (choice)
                {
                    case "1": HandleAddMember();       break;
                    case "2": HandleAddBook();         break;
                    case "3": HandleRateBook();        break;
                    case "4": HandleViewRatings();     break;
                    case "5": HandleRecommendations(); break;
                    case "6": HandleLogout();          break;
                    default:
                        Console.WriteLine("Invalid option. Please try again.");
                        break;
                }
            }
        }
 
        Console.WriteLine();
        Console.WriteLine("Thank you for using the Book Recommendation Program!");
    }
 
   
    private static void PrintLoggedOutMenu()
    {
        Console.WriteLine();
        Console.WriteLine("************** MENU **************");
        Console.WriteLine("* 1. Add a new member            *");
        Console.WriteLine("* 2. Add a new book              *");
        Console.WriteLine("* 3. Login                       *");
        Console.WriteLine("* 4. Quit                        *");
        Console.WriteLine("**********************************");
        Console.WriteLine();
        Console.Write("Enter a menu option: ");
    }
 
    private static void PrintLoggedInMenu()
    {
        Console.WriteLine();
        Console.WriteLine("************** MENU **************");
        Console.WriteLine("* 1. Add a new member            *");
        Console.WriteLine("* 2. Add a new book              *");
        Console.WriteLine("* 3. Rate book                   *");
        Console.WriteLine("* 4. View ratings                *");
        Console.WriteLine("* 5. See recommendations         *");
        Console.WriteLine("* 6. Logout                      *");
        Console.WriteLine("**********************************");
        Console.WriteLine();
        Console.Write("Enter a menu option: ");
    }
 
    /// <summary>
    /// Option 1 (both menus): creates a new member and confirms creation.
    /// </summary>
    private static void HandleAddMember()
    {
        Console.Write("Enter the name of the new member: ");
        string name = Console.ReadLine()?.Trim() ?? "";
 
        if (string.IsNullOrEmpty(name))
        {
            Console.WriteLine("Name cannot be empty.");
            return;
        }
 
        try
        {
            Member newMember = memberService.CreateNewMember(name);
            Console.WriteLine($"{newMember.Name} (account #: {newMember.AccountId}) was added.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Could not add member: {ex.Message}");
        }
    }
 
    /// <summary>
    /// Option 2 (both menus): prompts for book data, generates the next ISBN,
    /// and adds the book to the repository.
    /// </summary>
    private static void HandleAddBook()
    {
        Console.Write("Enter the author of the new book: ");
        string author = Console.ReadLine()?.Trim() ?? "";
 
        Console.Write("Enter the title of the new book: ");
        string title = Console.ReadLine()?.Trim() ?? "";
 
        Console.Write("Enter the year (or range of years) of the new book: ");
        string yearStr = Console.ReadLine()?.Trim() ?? "";
 
        if (string.IsNullOrEmpty(author) || string.IsNullOrEmpty(title) || string.IsNullOrEmpty(yearStr))
        {
            Console.WriteLine("All fields are required.");
            return;
        }
 
        try
        {
            int    nextNum = GetNextIsbnNumber();
            string isbn    = $"B{nextNum}";
            Book   book;
 
            if (yearStr.Contains('-'))
            {
                string[] parts = yearStr.Split('-');
                int.TryParse(parts[0].Trim(), out int startYear);
                int endYear = parts[1].Trim().ToLower() == "present"
                    ? DateTime.Today.Year
                    : int.TryParse(parts[1].Trim(), out int parsedEnd) ? parsedEnd : startYear;
 
                book = new BookSeries(isbn, author, title, startYear, endYear);
            }
            else
            {
                int.TryParse(yearStr, out int year);
                book = new Book(isbn, author, title, year);
            }
 
            bookService.NewBook(book);
 
            // Show original yearStr (preserves "present" or ranges as the user typed them)
            Console.WriteLine($"{nextNum}, {author}, {title}, {yearStr} was added.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Could not add book: {ex.Message}");
        }
    }
 
    /// <summary>
    /// Option 3 (logged-out menu): prompts for an account ID and logs in.
    /// </summary>
    private static void HandleLogin()
    {
        Console.Write("Enter member account: ");
        string input = Console.ReadLine()?.Trim() ?? "";
 
        if (!int.TryParse(input, out int accountId))
        {
            Console.WriteLine("Invalid account number.");
            return;
        }
 
        bool success = memberService.Login(accountId);
        if (success)
        {
            Console.WriteLine($"{memberService.GetLoggedInMember().Name}, you are logged in!");
        }
        else
        {
            Console.WriteLine("Account not found.");
        }
    }
 
    /// <summary>
    /// Option 3 (logged-in menu): prompts for a numeric ISBN and a score,
    /// handles re-rating if the book has already been rated.
    /// </summary>
    private static void HandleRateBook()
    {
        Member member = memberService.GetLoggedInMember();
 
        Console.Write("Enter the ISBN for the book you'd like to rate: ");
        string input = Console.ReadLine()?.Trim() ?? "";
 
        if (!int.TryParse(input, out int isbnNum))
        {
            Console.WriteLine("Invalid ISBN. Please enter a number (e.g. 12).");
            return;
        }
 
        Book? book = fileService.BookRepo!.GetBookByIsbn($"B{isbnNum}");
        if (book is null)
        {
            Console.WriteLine($"No book found with ISBN {isbnNum}.");
            return;
        }
 
        // If the member already rated this book, show the current rating and ask to update.
        if (ratingService.HasMemberRatedBook(member, book))
        {
            int currentScore = fileService.RatingRepo!
                .GetRating(member.AccountId, book.Isbn)?.Score ?? 0;
 
            Console.WriteLine($"Your current rating for {FormatBookRatingLine(book, currentScore)}");
            Console.Write("Would you like to re-rate this book (y/n)? ");
            string confirm = Console.ReadLine()?.Trim().ToLower() ?? "n";
 
            if (confirm != "y") return;
        }
 
        Console.Write("Enter your rating: ");
        string ratingInput = Console.ReadLine()?.Trim() ?? "";
 
        if (!int.TryParse(ratingInput, out int score))
        {
            Console.WriteLine("Invalid rating. Please enter a number.");
            return;
        }
 
        try
        {
            ratingService.RateBook(member, book, score);
            Console.WriteLine($"Your new rating for {FormatBookRatingLine(book, score)}");
        }
        catch (ArgumentException ex)
        {
            Console.WriteLine($"Invalid rating: {ex.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Could not save rating: {ex.Message}");
        }
    }
 
    /// <summary>
    /// Option 4 (logged-in menu): displays all books in order with the
    /// logged-in member's rating (0 = unread / not yet rated).
    /// </summary>
    private static void HandleViewRatings()
    {
        Member     member       = memberService.GetLoggedInMember();
        List<Book> orderedBooks = GetOrderedBooks();
 
        // GetRatingVector returns a 0 for every book that has no rating record,
        // so ALL books are shown — including unread ones — as in the sample output.
        List<int> vector = ratingService.GetRatingVector(member, orderedBooks);
 
        Console.WriteLine();
        Console.WriteLine($"{member.Name}'s ratings...");
 
        for (int i = 0; i < orderedBooks.Count; i++)
        {
            Console.WriteLine(FormatBookRatingLine(orderedBooks[i], vector[i]));
        }
    }
 
    /// <summary>
    /// Option 5 (logged-in menu): finds the most similar member, then displays
    /// their positively-rated books that the logged-in member has not yet read.
    /// Split into "really liked" (score 5) and "liked" (score 3).
    /// Books with score 1 are excluded from display, consistent with the sample output.
    /// </summary>
    private static void HandleRecommendations()
    {
        Member member = memberService.GetLoggedInMember();
 
        Member? similar = recommendationService.FindMostSimilarMember(member);
        if (similar is null)
        {
            Console.WriteLine("You have no ratings to compare with anyone else!");
            return;
        }
 
        Console.WriteLine();
        Console.WriteLine($"You have similar taste in books as {similar.Name}!");
 
        // Build a score lookup for the similar member so we can split the output.
        List<Book> orderedBooks   = GetOrderedBooks();
        List<int>  similarVector  = ratingService.GetRatingVector(similar, orderedBooks);
        var bookScoreMap = orderedBooks
            .Select((b, i) => (b.Isbn, Score: similarVector[i]))
            .ToDictionary(x => x.Isbn, x => x.Score);
 
        // GenerateRecommendations already filters: score>0 AND member hasn't rated the book.
        List<Book> recs = recommendationService.GenerateRecommendations(member);
 
      var reallyLiked = recs
            .Where(b => bookScoreMap.TryGetValue(b.Isbn, out int s) && s == 5)
            .OrderBy(b => GetIsbnNum(b.Isbn))
            .ToList();
 
        var liked = recs
            .Where(b => bookScoreMap.TryGetValue(b.Isbn, out int s) && s == 3)
            .OrderBy(b => GetIsbnNum(b.Isbn))
            .ToList();
 
        if (reallyLiked.Count > 0)
        {
            Console.WriteLine();
            Console.WriteLine("Here are the books they really liked:");
            foreach (var book in reallyLiked)
                Console.WriteLine(FormatBookLine(book));
        }
 
        if (liked.Count > 0)
        {
            Console.WriteLine();
            Console.WriteLine("And here are the books they liked:");
            foreach (var book in liked)
                Console.WriteLine(FormatBookLine(book));
        }
 
        if (reallyLiked.Count == 0 && liked.Count == 0)
        {
            Console.WriteLine("No recommendations available at this time.");
        }
    }
 
    // Helper Functions
    
    /// <summary>
    /// Option 6 (logged-in menu): logs the current member out.
    /// The next loop iteration will show the logged-out menu.
    /// </summary>
    private static void HandleLogout()
    {
        memberService.Logout();
    }
 
    /// <summary>
    /// Parses the numeric portion of an ISBN string ("B41" → 41).
    /// </summary>
    private static int GetIsbnNum(string isbn) =>
        int.TryParse(isbn.AsSpan(1), out int n) ? n : 0;
 
    /// <summary>
    /// Returns the next sequential ISBN number: max existing + 1.
    /// Safe even if books were added at runtime.
    /// </summary>
    private static int GetNextIsbnNumber() =>
        bookService.ListAllBooks()
            .Select(b => GetIsbnNum(b.Isbn))
            .DefaultIfEmpty(0)
            .Max() + 1;
 
    /// <summary>
    /// Returns all books sorted by their numeric ISBN (insertion order for display).
    /// This guarantees books are always listed 1, 2, 3 … N regardless of
    /// how the backing Dictionary enumerates.
    /// </summary>
    private static List<Book> GetOrderedBooks() =>
        bookService.ListAllBooks()
            .OrderBy(b => GetIsbnNum(b.Isbn))
            .ToList();
 
    /// <summary>
    /// Formats the year field of a book for display.
    /// Single books show just the year; series show "startYear-endYear".
    /// </summary>
    private static string GetYearDisplay(Book book) =>
        book is BookSeries bs ? $"{book.Year}-{bs.EndYear}" : book.Year.ToString();
 
    /// <summary>
    /// Formats a book as "N, Author, Title, Year" (no rating suffix).
    /// </summary>
    private static string FormatBookLine(Book book) =>
        $"{GetIsbnNum(book.Isbn)}, {book.Author}, {book.Title}, {GetYearDisplay(book)}";
 
    /// <summary>
    /// Formats a book line with a rating suffix: "N, Author, Title, Year => rating: X"
    /// </summary>
    private static string FormatBookRatingLine(Book book, int score) =>
        $"{FormatBookLine(book)} => rating: {score}";
}