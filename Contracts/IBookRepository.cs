using Lab2BookRecommendationSystem.Domain;
namespace Lab2BookRecommendationSystem.Contracts;

/// <summary>
/// This interface represents a repository that stores and retrieves books 
/// for the book recommendation system. 
/// </summary>
public interface IBookRepository
{
    /// <summary>
    /// Adds a book to the repository.
    /// </summary>
    /// <param name="book">The book object being added.</param>
    /// <remarks>
    /// Preconditions:
    /// <paramref name="book"/> is not null.
    /// Postconditions:
    /// The book is added and can be retrived using <see cref="GetBookById(int)"/> or <see cref="GetAllBooks()"/>.
    /// </remarks>
    public void AddBook(Book book);

    /// <summary>
    /// Retrieves a book from the repository by its unique ISBN identifier.
    /// </summary>
    /// <param name="isbn">The Book's ISBN.</param>
    /// <returns>The book if found; otherwise, null.</returns>
    /// <remarks>
    /// Preconditions:
    /// <paramref name="isbn"/> is a valid ISBN number and not null.
    /// Postconditions:
    /// The book is returned if found; otherwise, null.
    /// </remarks>
    public Book? GetBookByIsbn(string isbn);

    /// <summary>
    /// Retrieves all books within the repository.
    /// </summary>
    /// <returns>A list of all books; otherwise, an empty list if no books are found.</returns>
    /// <remarks>
    /// Postconditions:
    /// Returns all books. If no books are found, an empty list is returned.
    /// </remarks>
    public List<Book> GetAllBooks();

    /// <summary>
    /// Removes a book from the repository by its unique ISBN identifier.
    /// </summary>
    /// <param name="isbn">The Book's ISBN.</param>
    /// <remarks>
    /// Preconditions:
    /// <paramref name="isbn"=> is a valid ISBN number and not null.
    /// Postconditions:
    /// The book is removed if found; otherwise, no changes are made to the repository.
    /// </remarks>
    public void RemoveBookByIsbn(string isbn);
}