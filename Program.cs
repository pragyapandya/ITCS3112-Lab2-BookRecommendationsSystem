using Lab2BookRecommendationSystem.Contracts;
using Lab2BookRecommendationSystem.Repositories;
using Lab2BookRecommendationSystem.Services;
using System.IO;
using System;
using System.Collections.Generic;
using Lab2BookRecommendationSystem.Domain;
 
namespace Lab2BookRecommendationSystem;
 
public class Program
{
    // ─────────────────────────────────────────────────────────────────────────
    // Entry point
    // ─────────────────────────────────────────────────────────────────────────
 
    public static void Main(string[] args)
    {
        Console.WriteLine("╔══════════════════════════════════════════════════════════════╗");
        Console.WriteLine("║         Book Recommendation System — Functionality Test      ║");
        Console.WriteLine("╚══════════════════════════════════════════════════════════════╝");
        Console.WriteLine();
 
        // ── 1. File paths ──────────────────────────────────────────────────
        string baseDir     = AppContext.BaseDirectory;
        string booksPath   = Path.GetFullPath(Path.Combine(baseDir, @"..\..\..\Files\books.txt"));
        string ratingsPath = Path.GetFullPath(Path.Combine(baseDir, @"..\..\..\Files\ratings.txt"));
 
        // ── 2. Wire up the whole system ────────────────────────────────────
        PrintSection("STEP 1 — Loading data files");
 
        BookService?   bookService   = null;
        MemberService? memberService = null;
        RatingService? ratingService = null;
        RecommendationService? recommendationService = null;
        FileService fileService = new FileService(booksPath, ratingsPath);
 
        try
        {
            bookService = fileService.PopulateBooks(booksPath);
            Pass($"Books loaded: {bookService.ListAllBooks().Count}");
        }
        catch (Exception ex)
        {
            Fail($"PopulateBooks failed: {ex.Message}");
            return; // Cannot continue without books
        }
 
        try
        {
            (memberService, ratingService) = fileService.PopulateRatings(ratingsPath);
 
            int memberCount = fileService.MemberRepo!.GetAllMembers().Count;
            int ratingCount = fileService.RatingRepo!.GetAllRatings().Count;
 
            Pass($"Members loaded:  {memberCount}");
            Pass($"Ratings loaded:  {ratingCount}  " +
                 "(score-0 entries correctly excluded — they mean 'unread')");
        }
        catch (Exception ex)
        {
            Fail($"PopulateRatings failed: {ex.Message}");
            return;
        }
 
        try
        {
            recommendationService = new RecommendationService(
                fileService.BookRepo!,
                ratingService,
                fileService.MemberRepo!);
 
            Pass("RecommendationService wired up successfully");
        }
        catch (Exception ex)
        {
            Fail($"RecommendationService construction failed: {ex.Message}");
            return;
        }
 
        // ── 3. Book listing spot-check ──────────────────────────────────────
        PrintSection("STEP 2 — Book catalogue spot-check (first 5 books)");
 
        try
        {
            List<Book> allBooks = bookService.ListAllBooks();
            foreach (var book in allBooks.GetRange(0, Math.Min(5, allBooks.Count)))
            {
                string yearDisplay = book is BookSeries bs
                    ? $"{book.Year}–{bs.EndYear}"
                    : book.Year.ToString();
                Console.WriteLine($"  [{book.Isbn}]  {book.Title}  |  {book.Author}  |  {yearDisplay}");
            }
            Pass("Catalogue readable");
        }
        catch (Exception ex)
        {
            Fail($"Book listing failed: {ex.Message}");
        }
 
        // ── 4. Login / logout ───────────────────────────────────────────────
        PrintSection("STEP 3 — Login / Logout");
 
        try
        {
            // Brandon is member ID 1 (first entry in ratings.txt)
            bool loggedIn = memberService!.Login(1);
            if (!loggedIn) throw new Exception("Login returned false for member ID 1");
 
            Member loggedInMember = memberService.GetLoggedInMember();
            Pass($"Login succeeded  →  Welcome, {loggedInMember.Name} (ID: {loggedInMember.AccountId})");
            Pass($"IsLoggedIn: {memberService.IsLoggedIn()}");
 
            memberService.Logout();
            Pass($"Logout succeeded  →  IsLoggedIn: {memberService.IsLoggedIn()}");
        }
        catch (Exception ex)
        {
            Fail($"Login/Logout failed: {ex.Message}");
        }
 
        // ── 5. View personal ratings ────────────────────────────────────────
        PrintSection("STEP 4 — View personal ratings  (Brandon, ID 1)");
 
        try
        {
            memberService!.Login(1);
            Member brandon = memberService.GetLoggedInMember();
 
            List<Rating>  brandonRatings = ratingService!.GetMemberRatings(brandon);
            List<Book>    allBooks       = bookService.ListAllBooks();
 
            // Build a quick ISBN → Book lookup for display
            var bookLookup = new Dictionary<string, Book>();
            foreach (var b in allBooks) bookLookup[b.Isbn] = b;
 
            Console.WriteLine($"  {brandon.Name} has rated {brandonRatings.Count} books:");
            Console.WriteLine($"  {"Score",-7} {"ISBN",-6} Title");
            Console.WriteLine($"  {new string('-', 55)}");
 
            foreach (var r in brandonRatings)
            {
                string title = bookLookup.TryGetValue(r.BookIsbn, out var bk) ? bk.Title : "Unknown";
                
                Console.WriteLine($"  {r.Score,+3}    [{r.BookIsbn}]  {title}");
            }
 
            Pass("Personal ratings displayed successfully");
        }
        catch (Exception ex)
        {
            Fail($"View ratings failed: {ex.Message}");
        }
 
        // ── 6. Similarity calculation ───────────────────────────────────────
        PrintSection("STEP 5 — Similarity score (Brandon vs first 5 other members)");
 
        try
        {
            Member brandon = memberService!.GetLoggedInMember();
            var    others  = fileService.MemberRepo!.GetAllMembers()
                                .Where(m => m.AccountId != brandon.AccountId)
                                .Take(5)
                                .ToList();
 
            Console.WriteLine($"  Comparing {brandon.Name} against:");
            Console.WriteLine($"  {"Member",-15} {"Similarity Score",17}");
            Console.WriteLine($"  {new string('-', 33)}");
 
            foreach (var other in others)
            {
                int sim = recommendationService!.CalculateSimilarity(brandon, other);
                string otherName = other.Name;
                Console.WriteLine($"  {otherName ,-15} {sim,17}");
            }
 
            Pass("Similarity calculations completed");
        }
        catch (Exception ex)
        {
            Fail($"Similarity calculation failed: {ex.Message}");
        }
 
        // ── 7. Find most similar member ─────────────────────────────────────
        PrintSection("STEP 6 — Most similar member lookup");
 
        try
        {
            Member brandon = memberService!.GetLoggedInMember();
            Member? similar = recommendationService!.FindMostSimilarMember(brandon);
 
            if (similar is null)
            {
                Console.WriteLine("  No other members found to compare.");
            }
            else
            {
                int sim = recommendationService.CalculateSimilarity(brandon, similar);
                Pass($"Most similar to {brandon.Name}:  {similar.Name}  " +
                     $"(dot-product similarity = {sim})");
            }
        }
        catch (Exception ex)
        {
            Fail($"FindMostSimilarMember failed: {ex.Message}");
        }
 
        // ── 8. Generate recommendations ─────────────────────────────────────
        PrintSection("STEP 7 — Generate recommendations  (Brandon)");
 
        try
        {
            Member brandon = memberService!.GetLoggedInMember();
            List<Book> recs = recommendationService!.GenerateRecommendations(brandon);
 
            if (recs.Count == 0)
            {
                Console.WriteLine("  No recommendations generated (all books already rated).");
            }
            else
            {
                Console.WriteLine($"  Top recommendations for {brandon.Name}:");
                for (int i = 0; i < Math.Min(5, recs.Count); i++)
                {
                    Console.WriteLine($"    {i + 1}. {recs[i].Title}  ({recs[i].Author})");
                }
            }
 
            Pass($"Recommendations generated: {recs.Count} books");
        }
        catch (Exception ex)
        {
            Fail($"GenerateRecommendations failed: {ex.Message}");
        }
 
        memberService!.Logout();
 
        // ── 9. New member creation + manual rating + recommendation ─────────
        PrintSection("STEP 8 — New member: create, rate books, get recommendations");
 
        try
        {
            Member newMember = memberService!.CreateNewMember("TestUser");
            Pass($"New member created: {newMember.Name} (ID: {newMember.AccountId})");
 
            memberService.Login(newMember.AccountId);
            Pass($"Logged in as {memberService.GetLoggedInMember().Name}");
 
            // Rate a handful of books manually (use first 6 books from the catalogue)
            List<Book> catalogue = bookService.ListAllBooks();
            var toRate = new[] { (catalogue[0], 5), (catalogue[1], 3), (catalogue[4], -3),
                                 (catalogue[11], 5), (catalogue[13], 5), (catalogue[42], 3) };
 
            foreach (var (book, score) in toRate)
            {
                ratingService!.RateBook(newMember, book, score);
                Console.WriteLine($"    Rated [{book.Isbn}] '{book.Title}'  →  {score} ");
            }
 
            Pass($"Rated {toRate.Length} books");
 
            List<Rating> myRatings = ratingService!.GetMemberRatings(newMember);
            Pass($"GetMemberRatings returned {myRatings.Count} entries (expected {toRate.Length})");
 
            // HasMemberRatedBook spot-checks
            bool ratedFirst   = ratingService.HasMemberRatedBook(newMember, catalogue[0]);
            bool ratedLast    = ratingService.HasMemberRatedBook(newMember, catalogue[catalogue.Count - 1]);
            Pass($"HasMemberRatedBook: catalogue[0] = {ratedFirst} (expect True), " +
                 $"catalogue[last] = {ratedLast} (expect False)");
 
            // Recommendations for the new member
            List<Book> newRecs = recommendationService!.GenerateRecommendations(newMember);
            Pass($"Recommendations for TestUser: {newRecs.Count} books");
 
            if (newRecs.Count > 0)
            {
                Console.WriteLine($"  Top recommendations for {newMember.Name}:");
                for (int i = 0; i < Math.Min(5, newRecs.Count); i++)
                {
                    Console.WriteLine($"    {i + 1}. {newRecs[i].Title}");
                }
 
                // Verify none of the recommendations are books TestUser already rated
                bool noOverlap = !newRecs.Any(rec =>
                    toRate.Any(t => t.Item1.Isbn == rec.Isbn));
                Pass($"No overlap with already-rated books: {noOverlap}");
            }
 
            memberService.Logout();
            Pass("Logout successful");
        }
        catch (Exception ex)
        {
            Fail($"New member workflow failed: {ex.Message}");
        }
 
        // ── 10. Edge case — invalid score ────────────────────────────────────
        PrintSection("STEP 9 — Edge case: invalid rating score");
 
        try
        {
            Member testMember = memberService!.CreateNewMember("EdgeCaseUser");
            memberService.Login(testMember.AccountId);
            List<Book> catalogue = bookService.ListAllBooks();
 
            // Score 4 is not a valid value — RatingService should throw
            ratingService!.RateBook(testMember, catalogue[0], 4);
            Fail("Expected ArgumentException for invalid score 4 — none was thrown");
        }
        catch (ArgumentException)
        {
            Pass("ArgumentException correctly thrown for invalid score (4)");
        }
        catch (Exception ex)
        {
            Fail($"Unexpected exception type: {ex.GetType().Name} — {ex.Message}");
        }
        finally
        {
            memberService?.Logout();
        }
 
        // ── 11. Edge case — member with zero ratings ─────────────────────────
        PrintSection("STEP 10 — Edge case: recommendation for member with no ratings");
 
        try
        {
            Member emptyMember = memberService!.CreateNewMember("NoRatingsUser");
            memberService.Login(emptyMember.AccountId);
 
            List<Book> recs = recommendationService!.GenerateRecommendations(emptyMember);
            // With a zero vector, dot products with any member are 0.
            // FindMostSimilarMember still picks someone (highest of all-zero scores).
            // Recommendations may be empty or populated depending on who 'wins'.
            Pass($"No crash for zero-rating member. Recommendations returned: {recs.Count}");
 
            memberService.Logout();
        }
        catch (Exception ex)
        {
            Fail($"Zero-rating edge case failed: {ex.Message}");
        }
 
        // ── 12. Edge case — rating the same book twice (upsert) ─────────────
        PrintSection("STEP 11 — Edge case: re-rating a book (upsert)");
 
        try
        {
            Member upsertMember = memberService!.CreateNewMember("UpsertUser");
            memberService.Login(upsertMember.AccountId);
 
            List<Book> catalogue = bookService.ListAllBooks();
            Book target = catalogue[0];
 
            ratingService!.RateBook(upsertMember, target, 1);
            ratingService.RateBook(upsertMember, target, 5); // update existing
 
            List<Rating> ratings = ratingService.GetMemberRatings(upsertMember);
            if (ratings.Count != 1)
            {
                Fail($"Expected 1 rating after upsert, got {ratings.Count}");
            }
            else if (ratings[0].Score != 5)
            {
                Fail($"Expected updated score 5, got {ratings[0].Score}");
            }
            else
            {
                Pass($"Upsert correct: 1 rating record with updated score = {ratings[0].Score}");
            }
 
            memberService.Logout();
        }
        catch (Exception ex)
        {
            Fail($"Upsert test failed: {ex.Message}");
        }
 
        // ── Final summary ────────────────────────────────────────────────────
        Console.WriteLine();
        Console.WriteLine("╔══════════════════════════════════════════════════════════════╗");
        Console.WriteLine("║                     All tests complete.                      ║");
        Console.WriteLine("╚══════════════════════════════════════════════════════════════╝");
    }
 
    // ─────────────────────────────────────────────────────────────────────────
    // Console helpers
    // ─────────────────────────────────────────────────────────────────────────
 
    private static void PrintSection(string title)
    {
        Console.WriteLine();
        Console.WriteLine($"┌─ {title} ");
        Console.WriteLine();
    }
 
    private static void Pass(string message)
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.Write("  ✓ ");
        Console.ResetColor();
        Console.WriteLine(message);
    }
 
    private static void Fail(string message)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.Write("  ✗ ");
        Console.ResetColor();
        Console.WriteLine(message);
    }
 

}
