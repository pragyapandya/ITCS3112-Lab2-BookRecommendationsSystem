using System;
using System.Collections.Generic;
using Lab2BookRecommendationSystem.Contracts;
using Lab2BookRecommendationSystem.Domain;
namespace Lab2BookRecommendationSystem.Services;

public class RecommendationService
{
    private readonly IBookRepository _bookRepository;
    // private readonly IRatingRepository _ratingRepository;

    public int UserRatingScore { get; set; }

    // public RecommendationService(IBookRepository bookRepository, IRatingRepository ratingRepository, int userRatingScore)
    public RecommendationService(IBookRepository bookRepository, int userRatingScore)
    {
        _bookRepository = bookRepository;
        // _ratingRepository = ratingRepository;
        UserRatingScore = userRatingScore;
    }

    public Member? FindRecommendation(int accountId)
    {
        throw new NotImplementedException();
    }

    public List<Book> GenerateRecommendations(Member member)
    {
        throw new NotImplementedException();
    }
}