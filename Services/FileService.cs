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
    // -------------------------------------------------------------------------
    // Paths
    // -------------------------------------------------------------------------
 
    /// <summary>Full path to the books data file.</summary>
    public string BookFile { get; set; }
 
    /// <summary>Full path to the ratings data file.</summary>
    public string RatingFile { get; set; }
 
    // -------------------------------------------------------------------------
    // Repositories — exposed so callers can wire them into other services
    // (e.g. RecommendationService needs IBookRepository and IMemberRepo directly)
    // -------------------------------------------------------------------------
 
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
 
    // -------------------------------------------------------------------------
    // Internal state
    // -------------------------------------------------------------------------
 
    /// <summary>
    /// Books in the exact order they were read from books.txt.
    /// CRITICAL: ratings.txt scores are positional — index 0 score → book 0,
    /// index 1 score → book 1, etc. This list preserves that insertion order,
    /// whereas a Dictionary (used by BookRepository) does not guarantee it.
    /// </summary>
    private readonly List<Book> _orderedBooks = new List<Book>();
 
    // -------------------------------------------------------------------------
    // Constructor
    // -------------------------------------------------------------------------
 
    public FileService(string bookFile, string ratingFile)
    {
        BookFile   = bookFile;
        RatingFile = ratingFile;
    }
 
    // -------------------------------------------------------------------------
    // Public methods
    // -------------------------------------------------------------------------
 
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
            _orderedBooks.Add(book); // preserve insertion order for rating mapping
        }
 
        return bookService;
    }
 
    /// <summary>
    /// Reads ratings.txt, creating one <see cref="Member"/> and their
    /// associated <see cref="Rating"/> records per two-line block.
    ///
    /// File format (alternating lines):
    ///   Line A: member name
    ///   Line B: space-separated integer scores, one per book, in the same
    ///           order as books.txt (positional mapping via <c>_orderedBooks</c>)
    ///
    /// Score semantics: 5=love, 3=liked, 1=ok, 0=unread, -3=disliked, -5=hated.
    /// Scores of 0 are NOT stored — absence of a rating record means "unread".
    /// </summary>
    /// <param name="ratingFile">Full path to ratings.txt.</param>
    /// <returns>
    /// A tuple of the populated <see cref="MemberService"/> and
    /// <see cref="RatingService"/>, ready for use by the rest of the system.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown if <see cref="PopulateBooks"/> has not been called first.
    /// </exception>
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
 
        // Read the entire file upfront to handle the two-line-per-member format.
        string[] lines = File.ReadAllLines(ratingFile);
 
        for (int i = 0; i + 1 < lines.Length; i += 2)
        {
            string memberName = lines[i].Trim();
            string scoreLine  = lines[i + 1].Trim();
 
            if (string.IsNullOrEmpty(memberName) || string.IsNullOrEmpty(scoreLine))
                continue;
 
            // CreateNewMember auto-generates the AccountId and adds to MemberRepo.
            Member member = memberService.CreateNewMember(memberName);
 
            string[] tokens = scoreLine.Split(' ', StringSplitOptions.RemoveEmptyEntries);
 
            for (int j = 0; j < tokens.Length && j < _orderedBooks.Count; j++)
            {
                if (!int.TryParse(tokens[j], out int score)) continue;
 
                // 0 means "haven't read" — do not store a rating record.
                // Its absence in the repo signals unread to HasMemberRatedBook.
                if (score == 0) continue;
 
                ratingService.RateBook(member, _orderedBooks[j], score);
            }
        }
 
        return (memberService, ratingService);
    }
    
}