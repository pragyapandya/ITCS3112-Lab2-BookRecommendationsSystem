namespace Lab2BookRecommendationSystem.Domain;
 
/// <summary>
/// Represents a member's rating of a specific book.
/// Stores the link between a member (by AccountId) and a book (by ISBN),
/// along with the integer rating score.
/// </summary>
public class Rating
{
    /// <summary>
    /// The ISBN of the book being rated.
    /// </summary>
    public string BookIsbn { get; private set; }
 
    /// <summary>
    /// The AccountId of the member who submitted this rating.
    /// </summary>
    public int MemberId { get; private set; }
 
    /// <summary>
    /// The rating score. Must be one of the values defined in <see cref="ValidScores"/>.
    /// </summary>
    public int Score { get; private set; }
 
    /// <summary>
    /// The set of valid rating scores.
    /// -5 = Hated it, -3 = Didn't like it, 0 = Haven't read it,
    ///  1 = Ok,  3 = Liked it,  5 = Really liked it.
    /// </summary>
    public static readonly IReadOnlyList<int> ValidScores =
        new List<int> { -5, -3, 0, 1, 3, 5 }.AsReadOnly();
 
    /// <summary>
    /// Initializes a new Rating.
    /// </summary>
    /// <param name="bookIsbn">The ISBN of the book being rated.</param>
    /// <param name="memberId">The AccountId of the rating member.</param>
    /// <param name="score">The rating score. Must be a value in <see cref="ValidScores"/>.</param>
    public Rating(string bookIsbn, int memberId, int score)
    {
        BookIsbn = bookIsbn;
        MemberId = memberId;
        Score = score;
    }
 
    /// <summary>
    /// Updates the rating score.
    /// </summary>
    /// <param name="newScore">The new score. Must be a value in <see cref="ValidScores"/>.</param>
    public void UpdateScore(int newScore)
    {
        Score = newScore;
    }
}