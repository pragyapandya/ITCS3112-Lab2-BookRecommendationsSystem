using System.Collections.Generic;
using Lab2BookRecommendationSystem.Domain;
 
namespace Lab2BookRecommendationSystem.Contracts;
 
/// <summary>
/// Defines the storage contract for <see cref="Rating"/> objects.
/// Handles raw persistence of ratings keyed on member ID and book ISBN.
/// </summary>
public interface IRatingRepo
{
    /// <summary>
    /// Adds a new rating to the repository.
    /// </summary>
    /// <param name="rating">The rating to add. Must not be null.</param>
    /// <remarks>
    /// Preconditions:
    /// <paramref name="rating"/> is not null.
    /// Postconditions:
    /// The rating is stored and retrievable via <see cref="GetRating"/> or
    /// <see cref="GetRatingsByMember"/>.
    /// </remarks>
    void AddRating(Rating rating);
 
    /// <summary>
    /// Replaces the score on an existing rating in the repository.
    /// If no matching rating exists, no change is made.
    /// </summary>
    /// <param name="rating">
    /// The rating whose updated score should be persisted.
    /// Must not be null. Matched by <see cref="Rating.MemberId"/> and
    /// <see cref="Rating.BookIsbn"/>.
    /// </param>
    /// <remarks>
    /// Preconditions:
    /// <paramref name="rating"/> is not null.
    /// Postconditions:
    /// If a matching rating exists, its score reflects <paramref name="rating"/>'s score.
    /// </remarks>
    void UpdateRating(Rating rating);
 
    /// <summary>
    /// Retrieves the rating submitted by a specific member for a specific book.
    /// </summary>
    /// <param name="memberId">The AccountId of the member.</param>
    /// <param name="bookIsbn">The ISBN of the book.</param>
    /// <returns>The matching <see cref="Rating"/>, or null if none exists.</returns>
    /// <remarks>
    /// Preconditions:
    /// <paramref name="bookIsbn"/> is not null.
    /// Postconditions:
    /// Returns the rating if found; otherwise null.
    /// </remarks>
    Rating? GetRating(int memberId, string bookIsbn);
 
    /// <summary>
    /// Retrieves all ratings submitted by a specific member.
    /// </summary>
    /// <param name="memberId">The AccountId of the member.</param>
    /// <returns>
    /// A list of all ratings for the member; an empty list if none exist.
    /// </returns>
    /// <remarks>
    /// Postconditions:
    /// Returns all ratings for the member. Never returns null.
    /// </remarks>
    List<Rating> GetRatingsByMember(int memberId);
 
    /// <summary>
    /// Retrieves every rating stored in the repository.
    /// Used by the recommendation algorithm to access all members' rating data.
    /// </summary>
    /// <returns>
    /// A list of all ratings; an empty list if none exist.
    /// </returns>
    /// <remarks>
    /// Postconditions:
    /// Returns all ratings. Never returns null.
    /// </remarks>
    List<Rating> GetAllRatings();
}