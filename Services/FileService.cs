using System;
using System.Collections.Generic;
using System.IO;
using Lab2BookRecommendationSystem.Domain;
using Lab2BookRecommendationSystem.Repositories;
using Lab2BookRecommendationSystem.Services;
 
/// <summary>
/// Responsible for reading books.txt and ratings.txt exactly once each,
/// parsing their contents into domain objects, and wiring up the
/// repositories and services that the rest of the system depends on.
///
/// NOTE: Call <see cref="PopulateBooks"/> before <see cref="PopulateRatings"/>.
/// PopulateRatings relies on the ordered book list built during PopulateBooks
/// to map positional rating scores to the correct book ISBNs.
/// </summary>
public class FileService
{

    /// <summary>Full path to the books data file.</summary>
    public string BookFile { get; set; }
 
    /// <summary>Full path to the ratings data file.</summary>
    public string RatingFile { get; set; }
 

    /// <summary>
    /// The book repository populated by <see cref="PopulateBooks"/>.
    /// Null until PopulateBooks is called.
    /// </summary>
    public BookRepository? BookRepo { get; private set; }
 
    /// <summary>
    /// The member repository populated by <see cref="PopulateRatings"/>.
    /// Null until PopulateRatings is called.
    /// </summary>
    public MemberRepo? MemberRepo { get; private set; }
 
    /// <summary>
    /// The rating repository populated by <see cref="PopulateRatings"/>.
    /// Null until PopulateRatings is called.
    /// </summary>
    public RatingRepo? RatingRepo { get; private set; }

    /// <summary>
    /// Books in the exact order they were read from books.txt.
    /// CRITICAL: ratings.txt scores are positional — index 0 score → book 0,
    /// index 1 score → book 1, etc. This list preserves that insertion order,
    /// whereas a Dictionary (used by BookRepository) does not guarantee it.
    /// </summary>
    private readonly List<Book> _orderedBooks = new List<Book>();

    public FileService(string bookFile, string ratingFile)
    {
        BookFile   = bookFile;
        RatingFile = ratingFile;
    }

    /// <summary>
    /// Reads books.txt, parses each line into a <see cref="Book"/> or
    /// <see cref="BookSeries"/>, and loads them into a new <see cref="BookRepository"/>.
    /// The repository and the ordered book list are stored as properties for
    /// use by <see cref="PopulateRatings"/> and the recommendation engine.
    /// </summary>
    /// <param name="bookFile">Full path to books.txt.</param>
    /// <returns>A fully populated <see cref="BookService"/>.</returns>
    public BookService PopulateBooks(string bookFile)
    {
        if (bookFile is null) throw new ArgumentNullException(nameof(bookFile));
 
        BookRepo = new BookRepository();
        var bookService = new BookService(BookRepo);
        _orderedBooks.Clear();
 
        int isbnCounter = 1;
 
        foreach (var rawLine in File.ReadLines(bookFile))
        {
            string line = rawLine.Trim();
            if (string.IsNullOrEmpty(line)) continue;
 
            // Split only on the first two commas to handle titles that contain commas.
            string[] parts = line.Split(',');
            if (parts.Length < 3) continue;
 
            string isbn    = $"B{isbnCounter++}";
            string author  = parts[0].Trim();
            string title   = parts[1].Trim();
            string yearStr = parts[2].Trim();
 
            Book book;
 
            if (yearStr.Contains('-'))
            {
                string[] yearParts = yearStr.Split('-');
                int.TryParse(yearParts[0], out int startYear);
                int endYear = yearParts[1].Trim().ToLower() == "present"
                    ? DateTime.Today.Year
                    : int.TryParse(yearParts[1], out int parsedEnd) ? parsedEnd : startYear;
 
                book = new BookSeries(isbn, author, title, startYear, endYear);
            }
            else
            {
                int.TryParse(yearStr, out int year);
                book = new Book(isbn, author, title, year);
            }
 
            bookService.NewBook(book);
            _orderedBooks.Add(book); 
        }
        return bookService;
    }
 
    /// <summary>
    /// Reads ratings.txt, creating one <see cref="Member"/> and their
    /// associated <see cref="Rating"/> records per two-line block.
    /// </summary>
    /// <param name="ratingFile">Full path to ratings.txt.</param>
    /// <returns>
    /// A tuple of the populated <see cref="MemberService"/> and
    /// <see cref="RatingService"/>, ready for use by the rest of the system.
    /// </returns>
    public (MemberService memberService, RatingService ratingService) PopulateRatings(string ratingFile)
    {
        if (ratingFile is null) throw new ArgumentNullException(nameof(ratingFile));
 
        if (_orderedBooks.Count == 0)
        {
            throw new InvalidOperationException(
                "PopulateBooks must be called before PopulateRatings so that " +
                "the positional book order is established.");
        }
 
        MemberRepo = new MemberRepo();
        RatingRepo = new RatingRepo();
 
        var memberService = new MemberService(MemberRepo);
        var ratingService = new RatingService(RatingRepo);
 
        // Read the entire file
        string[] lines = File.ReadAllLines(ratingFile);
 
        for (int i = 0; i + 1 < lines.Length; i += 2)
        {
            string memberName = lines[i].Trim();
            string scoreLine  = lines[i + 1].Trim();
 
            if (string.IsNullOrEmpty(memberName) || string.IsNullOrEmpty(scoreLine))
                continue;
            
            Member member = memberService.CreateNewMember(memberName);
            
            string[] tokens = scoreLine.Split(' ', StringSplitOptions.RemoveEmptyEntries);
 
            for (int j = 0; j < tokens.Length && j < _orderedBooks.Count; j++)
            {
                if (!int.TryParse(tokens[j], out int score)) continue;
                if (score == 0) continue;
                ratingService.RateBook(member, _orderedBooks[j], score);
            }
        }
        return (memberService, ratingService);
    }
}