using Lab2BookRecommendationSystem.Contracts;
using Lab2BookRecommendationSystem.Domain;
namespace Lab2BookRecommendationSystem.Services;

public class BookService : IBookService
{
    private readonly IBookRepository _bookRepository;

    public BookService(IBookRepository bookRepository)
    {
        _bookRepository = bookRepository;
    }

    public void NewBook(Book book)
    {
        _bookRepository.AddBook(book);
    }

    public Book? GetBook(Book book)
    {
        if (book is null)
        {
            throw new ArgumentNullException(nameof(book));
        }

        return _bookRepository.GetBookByIsbn(book.Isbn);
    }

    public List<Book> ListAllBooks()
    {
        return _bookRepository.GetAllBooks();
    }

    public void UpdateBook(Book book)
    {
        if (book is null)
        {
            throw new ArgumentNullException(nameof(book));
        }
        
        var bookUpdate = _bookRepository.GetBookByIsbn(book.Isbn);

        if (bookUpdate is null)
        {
            throw new ArgumentNullException(nameof(book));
        }

        bookUpdate.UpdateBook(book.Isbn, book.Author, book.Title, book.Year);
    }

    public void RemoveBook(string isbn)
    {
        if (isbn is null)
        {
            throw new ArgumentNullException(nameof(isbn));
        }

        _bookRepository.RemoveBookByIsbn(isbn);
    }
}