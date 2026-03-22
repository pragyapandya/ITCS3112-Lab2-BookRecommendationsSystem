using System;
using System.Collections.Generic;
using System.Linq;
using Lab2BookRecommendationSystem.Contracts;
using Lab2BookRecommendationSystem.Domain;
 
namespace Lab2BookRecommendationSystem.Repositories;
 
/// <summary>
/// In-memory repository for <see cref="Rating"/> objects.
/// Ratings are stored in a flat list and looked up via LINQ.
/// </summary>
public class RatingRepo : IRatingRepo
{
    private readonly List<Rating> _ratings = new List<Rating>();
 
    /// <inheritdoc/>
    public void AddRating(Rating rating)
    {
        if (rating is null)
        {
            throw new ArgumentNullException(nameof(rating));
        }
 
        _ratings.Add(rating);
    }
 
    /// <inheritdoc/>
    public void UpdateRating(Rating rating)
    {
        if (rating is null)
        {
            throw new ArgumentNullException(nameof(rating));
        }
 
        var existing = GetRating(rating.MemberId, rating.BookIsbn);
        existing?.UpdateScore(rating.Score);
    }
 
    /// <inheritdoc/>
    public Rating? GetRating(int memberId, string bookIsbn)
    {
        if (bookIsbn is null)
        {
            throw new ArgumentNullException(nameof(bookIsbn));
        }
 
        return _ratings.FirstOrDefault(r =>
            r.MemberId == memberId && r.BookIsbn == bookIsbn);
    }
 
    /// <inheritdoc/>
    public List<Rating> GetRatingsByMember(int memberId)
    {
        return _ratings.Where(r => r.MemberId == memberId).ToList();
    }
 
    /// <inheritdoc/>
    public List<Rating> GetAllRatings()
    {
        return _ratings.ToList();
    }
}