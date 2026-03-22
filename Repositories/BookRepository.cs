using System;
using System.Collections.Generic;
using System.Linq;
using Lab2BookRecommendationSystem.Contracts;
using Lab2BookRecommendationSystem.Domain;
namespace Lab2BookRecommendationSystem.Repositories;

public class BookRepository : IBookRepository
{
    private readonly Dictionary<string, Book> _books;

    public BookRepository()
    {
        _books = new Dictionary<string, Book>();
    }
    
    public void AddBook(Book book)
    {
        if (book is null)
        {
            throw new ArgumentNullException(nameof(book));
        } else if (_books.ContainsKey(book.Isbn))
        {
            throw new ArgumentException($"A book with the same ISBN already exists: {book.Isbn}");
        }

        _books.Add(book.Isbn, book);
    }

    public Book? GetBookByIsbn(string isbn)
    {
        if (isbn is null)
        {
            throw new ArgumentNullException(nameof(isbn));
        }

        _books.TryGetValue(isbn, out var book);
        return book;
    }

    public List<Book> GetAllBooks()
    {
        return _books.Values.ToList();
    }

    public void RemoveBookByIsbn(string isbn)
    {
        if (isbn is null)
        {
            throw new ArgumentNullException(nameof(isbn));
        }

        _books.Remove(isbn);
    }
}