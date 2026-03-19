using Lab2BookRecommendationSystem.Domain;
namespace Lab2BookRecommendationSystem.Contracts;

/// <summary>
/// This interface represents a book service that adds, retrieves, updates, and removes books  
/// for the book recommendation system. 
/// </summary>
public interface IBookService
{
    /// <summary>
    /// Creates a new book and adds it to the <see cref="IBookRepository"/>.
    /// </summary>
    /// <param name="book">The book object being added.</param>
    /// <remarks>
    /// Preconditions:
    /// <paramref name="book"/> is not null.
    /// Postconditions:
    /// The book is added and can be retrived using <see cref="GetBook(Book)"/> or <see cref="ListAllBooks()"/>.
    /// </remarks>
    public void NewBook(Book book);

    /// <summary>
    /// Retrieves a book from the <see cref="IBookRepository"/> by its unique ISBN identifier. 
    /// </summary>
    /// <param name="book">The book object being retrieved.</param>
    /// <returns>The book if found; otherwise, null.</returns>
    /// <remarks>
    /// Preconditions:
    /// <paramref name="book"/> is not null and has a valid ISBN number that is also not null.
    /// Postconditions:
    /// The book is returned if found; otherwise, null.
    public Book? GetBook(Book book);

    /// <summary>
    /// Lists all books in the <see cref="IBookRepository"/>.
    /// </summary>
    /// <returns>A list of all books; otherwise, an empty list if no books are found.</returns>
    /// <remarks>
    /// Postconditons:
    /// Returns all books. If no books are found, an empty list is returned.
    /// </remarks>
    public List<Book> ListAllBooks();

    /// <summary>
    /// Updates an existing book in the <see cref="IBookRepository"/> by its unique ISBN identifier.
    /// </summary>
    /// <param name="book">The book object being updated.</param>
    /// <remarks>
    /// Preconditions:
    /// <paramref name="book"/> is not null and has a valid ISBN number that is also not null.
    /// Postconditions:
    /// The book is updated if found; otherwise, no changes are made to the repository.
    /// </remarks>
    public void UpdateBook(Book book);

    /// <summary>
    /// Removes a book from the <see cref="IBookRepository"/> by its unique ISBN identifier.
    /// </summary>
    /// <param name="isbn">The ISBN of the book being removed.</param>
    /// <remarks>
    /// Preconditions:
    /// <paramref name="isbn"/> is a valid ISBN number and not null.
    /// Postconditions:
    /// The book is removed if found; otherwise, no changes are made to the repository.
    /// </remarks>
    public void RemoveBook(string isbn);
}