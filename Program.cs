using Lab2BookRecommendationSystem.Contracts;
using Lab2BookRecommendationSystem.Repositories;
using Lab2BookRecommendationSystem.Services;
using System.IO;
using System;
using System.Collections.Generic;
using Lab2BookRecommendationSystem.Domain;

namespace Lab2BookRecommendationSystem;

public class Program
{
    public static void Main(String[] args)
    {

        
       
        
        
        
        
        // File Pathing
        string baseDir = AppContext.BaseDirectory;
        
        string booksPath = Path.GetFullPath(Path.Combine(baseDir, @"..\..\..\Files\books.txt"));
        string ratingsPath = Path.GetFullPath(Path.Combine(baseDir, @"..\..\..\Files\ratings.txt"));
        //
        
        
        FileService fileservice = new FileService(booksPath, ratingsPath);
        BookService bservice = fileservice.PopulateBooks(booksPath);
        fileservice.PopulateRatings(ratingsPath);

        
        // Listed Books in the populated Books Repository
        List<Book> bookList = bservice.ListAllBooks();

        foreach (Book book in bookList)
        {
            Console.WriteLine("############################################################################");
            Console.WriteLine(book.Title);
            Console.WriteLine(book.Author);
            Console.WriteLine(book.Isbn);
            Console.WriteLine(book.Year);
            Console.WriteLine("############################################################################");

        }


    }
}