namespace Lab2BookRecommendationSystem.Contracts;

/// <summary>
/// This interface represents a file service that reads data from files 
/// for the book recommendation system.
/// </summary>
public interface IFileService
{
    /// <summary>
    /// Populates the book's data from the specified file path.
    /// </summary>
    /// <param name="bookFile">The file path for the book's file.</param>
    /// <remarks>
    /// Preconditions:
    /// <paramref name="bookFile"/> is a valid file path and not null.
    /// Postconditions:
    /// The book's data is populated from the specified file path.
    /// </remarks>
    public void PopulateBooks(string bookFile);

    /// <summary>
    /// Populates the member's rating's data from the specified file path. 
    /// </summary>
    /// <param name="ratingFile">The file path for the rating's file.</param>
    /// <remarks>
    /// Preconditions:
    /// <paramref name="ratingFile"/> is a valid file path and not null.
    /// Postconditions:
    /// The member's rating's data is populated from the specified file path.
    /// </remarks>
    public void PopulateRatings(string ratingFile);
}