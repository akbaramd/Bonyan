namespace BonyanTemplate.Domain.Books;

/// <summary>
/// Availability status of a book.
/// </summary>
public enum BookStatus
{
    /// <summary>Book is available.</summary>
    Available,

    /// <summary>Book is out of stock.</summary>
    OutOfStock,
}