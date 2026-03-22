using System.Collections.Generic;
using Lab2BookRecommendationSystem.Domain;

namespace Lab2BookRecommendationSystem.Contracts;

/// <summary>
/// This interface represents a recommendation service that generates book recommendations 
/// for members based on their ratings and preferences in the book recommendation system.
/// </summary>
public interface IRecommendationService
{


    /// <summary>
    /// Generates a list of book recommendations for a member 
    /// based on their ratings and book preferences.
    /// </summary>
    /// <param name="member">The member to generate recommendations for.</param>
    /// <returns>A list of recommended books; otherwise, an empty list if no books found.</returns>
    /// <remarks>
    /// Preconditions:
    /// <paramref name="member"/> is a valid member and not null.
    /// Postconditions:
    /// A list of recommended books is returned; otherwise, an empty list if no books found.
    /// </remarks>
    public List<Book> GenerateRecommendations(Member member);
}