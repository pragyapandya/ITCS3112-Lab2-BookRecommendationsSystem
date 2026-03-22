using System.Collections.Generic;
using Lab2BookRecommendationSystem.Domain;
 
namespace Lab2BookRecommendationSystem.Contracts;
 
/// <summary>
/// Defines the business logic contract for rating operations.
/// Acts as the bridge between UI-level rating actions and the
/// <see cref="IRatingRepo"/> storage layer.
/// </summary>
public interface IRatingService
{
    /// <summary>
    /// Submits a rating for a book on behalf of a member.
    /// If the member has already rated the book, the existing score is updated.
    /// If not, a new rating is created.
    /// </summary>
    /// <param name="member">The member submitting the rating. Must not be null.</param>
    /// <param name="book">The book being rated. Must not be null.</param>
    /// <param name="score">
    /// The score to assign. Must be one of the values in
    /// <see cref="Rating.ValidScores"/>: -5, -3, 0, 1, 3, or 5.
    /// </param>
    /// <remarks>
    /// Preconditions:
    /// <paramref name="member"/> and <paramref name="book"/> are not null.
    /// <paramref name="score"/> is a value in <see cref="Rating.ValidScores"/>.
    /// Postconditions:
    /// A rating for this member/book pair is stored and reflects <paramref name="score"/>.
    /// </remarks>
    void RateBook(Member member, Book book, int score);
 
    /// <summary>
    /// Retrieves all ratings the given member has submitted.
    /// </summary>
    /// <param name="member">The member whose ratings to retrieve. Must not be null.</param>
    /// <returns>
    /// A list of the member's ratings; an empty list if they have rated nothing.
    /// </returns>
    /// <remarks>
    /// Preconditions:
    /// <paramref name="member"/> is not null.
    /// Postconditions:
    /// Returns all ratings for the member. Never returns null.
    /// </remarks>
    List<Rating> GetMemberRatings(Member member);
 
    /// <summary>
    /// Determines whether a member has already submitted a rating for a specific book.
    /// Used by the recommendation algorithm to exclude already-rated books.
    /// </summary>
    /// <param name="member">The member to check. Must not be null.</param>
    /// <param name="book">The book to check. Must not be null.</param>
    /// <returns>
    /// True if the member has rated the book (any score including 0); otherwise false.
    /// </returns>
    /// <remarks>
    /// Preconditions:
    /// <paramref name="member"/> and <paramref name="book"/> are not null.
    /// Postconditions:
    /// Returns true if a rating record exists for this member/book pair.
    /// </remarks>
    bool HasMemberRatedBook(Member member, Book book);
 
    /// <summary>
    /// Returns an ordered integer vector of a member's scores across all books.
    /// Books the member has not rated are represented as 0.
    /// The order mirrors the order of <paramref name="allBooks"/>.
    /// This vector is used directly in dot-product similarity calculations.
    /// </summary>
    /// <param name="member">The member whose rating vector to build. Must not be null.</param>
    /// <param name="allBooks">
    /// The full ordered list of books in the system. Must not be null.
    /// </param>
    /// <returns>
    /// A list of integers with one entry per book in <paramref name="allBooks"/>.
    /// </returns>
    /// <remarks>
    /// Preconditions:
    /// <paramref name="member"/> and <paramref name="allBooks"/> are not null.
    /// Postconditions:
    /// Returns a list of length equal to <paramref name="allBooks"/>.Count.
    /// Unrated books contribute a 0 to the vector.
    /// </remarks>
    List<int> GetRatingVector(Member member, List<Book> allBooks);
}