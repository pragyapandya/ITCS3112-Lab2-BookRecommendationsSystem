using System;
using System.Collections.Generic;
using System.Linq;
using Lab2BookRecommendationSystem.Contracts;
using Lab2BookRecommendationSystem.Domain;
 
namespace Lab2BookRecommendationSystem.Services;
 
/// <summary>
/// Generates book recommendations using dot-product similarity.
/// Compares a target member's rating vector against every other member's
/// to identify the most similar member, then surfaces that member's
/// highest-rated books that the target has not yet rated.
/// </summary>
public class RecommendationService : IRecommendationService
{
    private readonly IBookRepository _bookRepository;
    private readonly IRatingService _ratingService;
    private readonly IMemberRepo _memberRepo;
 
    /// <summary>
    /// Initializes a new RecommendationService with its required dependencies.
    /// </summary>
    /// <param name="bookRepository">Source of all books in the system.</param>
    /// <param name="ratingService">Provides rating vectors for similarity calculations.</param>
    /// <param name="memberRepo">Source of all members to compare against.</param>
    public RecommendationService(
        IBookRepository bookRepository,
        IRatingService ratingService,
        IMemberRepo memberRepo)
    {
        _bookRepository = bookRepository ?? throw new ArgumentNullException(nameof(bookRepository));
        _ratingService  = ratingService  ?? throw new ArgumentNullException(nameof(ratingService));
        _memberRepo     = memberRepo     ?? throw new ArgumentNullException(nameof(memberRepo));
    }
 
    
    /// <remarks>
    /// The target member's rating vector is built once and reused across
    /// all candidate comparisons to avoid redundant work.
    /// </remarks>
    public Member? FindMostSimilarMember(Member member)
    {
        if (member is null) throw new ArgumentNullException(nameof(member));
 
        // Snapshot the book list once 
        var allBooks = _bookRepository.GetAllBooks();
 
        var targetVector = _ratingService.GetRatingVector(member, allBooks);
 
        var otherMembers = _memberRepo.GetAllMembers()
            .Where(m => m.AccountId != member.AccountId)
            .ToList();
 
        if (!otherMembers.Any()) return null;
 
        Member? mostSimilar = null;
        int highestScore = int.MinValue;
 
        foreach (var candidate in otherMembers)
        {
            var candidateVector = _ratingService.GetRatingVector(candidate, allBooks);

            int score = DotProduct(targetVector, candidateVector);
            Console.WriteLine("DotProduct: "+ score);
            if (score > highestScore)
            {
                highestScore = score;
                mostSimilar  = candidate;
            }
        }
 
        return mostSimilar;
    }



    
    /// <remarks>
    /// Finds the most similar member, then recommends their positively-rated
    /// books that the target member has not yet rated, ordered highest score first.
    /// Returns an empty list if no similar member exists or no qualifying books remain.
    /// </remarks>
    public List<Book> GenerateRecommendations(Member member)
    {
        if (member is null) throw new ArgumentNullException(nameof(member));
 
        var similarMember = FindMostSimilarMember(member);
        if (similarMember is null) return new List<Book>();
 
        var allBooks      = _bookRepository.GetAllBooks();
        var similarVector = _ratingService.GetRatingVector(similarMember, allBooks);
 
        var candidates = new List<(Book book, int score)>();
 
        for (int i = 0; i < allBooks.Count; i++)
        {
            var book  = allBooks[i];
            int score = similarVector[i];
 
            // Only recommend books the similar member rated positively
            // that the target member has not yet read (has no rating record for).
            if (score > 0 && !_ratingService.HasMemberRatedBook(member, book))
            {
                candidates.Add((book, score));
            }
        }
 
        return candidates
            .OrderByDescending(r => r.score)
            .Select(r => r.book)
            .ToList();
    }
 
   
    public int CalculateSimilarity(Member member1, Member member2)
    {
        if (member1 is null) throw new ArgumentNullException(nameof(member1));
        if (member2 is null) throw new ArgumentNullException(nameof(member2));
 
        var allBooks = _bookRepository.GetAllBooks();
        var vector1  = _ratingService.GetRatingVector(member1, allBooks);
        var vector2  = _ratingService.GetRatingVector(member2, allBooks);
 
        return DotProduct(vector1, vector2);
    }
 
   
    /// <summary>
    /// Computes the dot product of two integer vectors.
    /// Stops at the shorter vector's length if sizes differ.
    /// </summary>
    private static int DotProduct(List<int> v1, List<int> v2)
    {
        int sum    = 0;
        int length = Math.Min(v1.Count, v2.Count);
 
        for (int i = 0; i < length; i++)
        {
            sum += v1[i] * v2[i];
        }
 
        return sum;
    }
}