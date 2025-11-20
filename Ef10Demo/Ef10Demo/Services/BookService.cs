using Ef10Demo.Data;
using Ef10Demo.Models;
using Microsoft.EntityFrameworkCore;

namespace Ef10Demo.Services;

public class BookService
{
    private readonly BookContext _context;

    public BookService(BookContext context)
    {
        _context = context;
    }

    public async Task InitializeDatabaseAsync()
    {
        await _context.Database.EnsureDeletedAsync();
        await _context.Database.EnsureCreatedAsync();
    }

    public async Task<Book> AddBookAsync(string title, int year, string authorName, string authorEmail, string authorCountry)
    {
        var book = new Book
        {
            Title = title,
            PublishedYear = year,
            Author = new Author
            {
                Name = authorName,
                Email = authorEmail,
                Country = authorCountry
            }
        };

        _context.Books.Add(book);
        await _context.SaveChangesAsync();
        
        Console.WriteLine($"kitap eklendi: {book.Title} ({book.Id})");
        return book;
    }

    public async Task<List<Book>> GetAllBooksAsync()
    {
        return await _context.Books.ToListAsync();
    }

    public async Task<Book?> GetBookByIdAsync(int id)
    {
        return await _context.Books.FindAsync(id);
    }

    public async Task<List<Book>> SearchBooksByAuthorCountryAsync(string country)
    {
        return await _context.Books
            .Where(b => b.Author.Country == country)
            .ToListAsync();
    }
}
