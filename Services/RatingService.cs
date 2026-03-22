using System;
using System.Collections.Generic;
using System.Linq;
using Lab2BookRecommendationSystem.Contracts;
using Lab2BookRecommendationSystem.Domain;
 
namespace Lab2BookRecommendationSystem.Services;
 
/// <summary>
/// Handles the business logic for rating operations.
/// Delegates storage to an <see cref="IRatingRepo"/> and enforces
/// validation rules (valid scores, non-null inputs) before persistence.
/// </summary>
public class RatingService : IRatingService
{
    private readonly IRatingRepo _ratingRepo;
 
    /// <summary>
    /// Initializes a new RatingService with the given rating repository.
    /// </summary>
    /// <param name="ratingRepo">The repository used to persist ratings. Must not be null.</param>
    public RatingService(IRatingRepo ratingRepo)
    {
        _ratingRepo = ratingRepo ?? throw new ArgumentNullException(nameof(ratingRepo));
    }
 
    /// <inheritdoc/>
    public void RateBook(Member member, Book book, int score)
    {
        if (member is null)
        {
            throw new ArgumentNullException(nameof(member));
        }
 
        if (book is null)
        {
            throw new ArgumentNullException(nameof(book));
        }
 
        if (!Rating.ValidScores.Contains(score))
        {
            throw new ArgumentException(
                $"Score {score} is invalid. Valid values are: {string.Join(", ", Rating.ValidScores)}",
                nameof(score));
        }
 
        var existing = _ratingRepo.GetRating(member.AccountId, book.Isbn);
 
        if (existing is null)
        {
            _ratingRepo.AddRating(new Rating(book.Isbn, member.AccountId, score));
        }
        else
        {
            existing.UpdateScore(score);
            _ratingRepo.UpdateRating(existing);
        }
    }
 
    /// <inheritdoc/>
    public List<Rating> GetMemberRatings(Member member)
    {
        if (member is null)
        {
            throw new ArgumentNullException(nameof(member));
        }
 
        return _ratingRepo.GetRatingsByMember(member.AccountId);
    }
 
    /// <inheritdoc/>
    public bool HasMemberRatedBook(Member member, Book book)
    {
        if (member is null)
        {
            throw new ArgumentNullException(nameof(member));
        }
 
        if (book is null)
        {
            throw new ArgumentNullException(nameof(book));
        }
 
        return _ratingRepo.GetRating(member.AccountId, book.Isbn) is not null;
    }
 
    /// <inheritdoc/>
    public List<int> GetRatingVector(Member member, List<Book> allBooks)
    {
        if (member is null)
        {
            throw new ArgumentNullException(nameof(member));
        }
 
        if (allBooks is null)
        {
            throw new ArgumentNullException(nameof(allBooks));
        }
 
        return allBooks.Select(book =>
        {
            var rating = _ratingRepo.GetRating(member.AccountId, book.Isbn);
            return rating?.Score ?? 0;
        }).ToList();
    }
}