using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Content.Server.Database;

public sealed class L5Model
{
    /// <summary>
    /// Defines a collection of in-game books
    /// </summary>
    [Table(name: "L5_book_collections")]
    public class BookCollection
    {
        public int Id { get; set; }

        /// <summary>
        /// The name of the collection
        /// </summary>
        [Required, MaxLength(256)]
        public required string CollectionName { get; set; }

        /// <summary>
        /// The player who created the collection
        /// </summary>
        [Required, ForeignKey("CreatedBy")] public Guid? CreatedById { get; set; }
        public Player? CreatedBy { get; set; }

        /// <summary>
        /// The books in the collection
        /// </summary>
        public List<Book> Books { get; set; } = null!;
    }
    /// <summary>
    /// Defines an in-game book that is persisted across rounds
    /// </summary>
    [Table(name: "L5_books")]
    public class Book
    {
        public int Id { get; set; }

        /// <summary>
        /// The title of the book.
        /// </summary>
        public required string Title { get; set; }

        /// <summary>
        /// An optional subtitle for the book.
        /// </summary>
        public string Subtitle { get; set; } = "";

        /// <summary>
        /// A short summary for the book to be shown on the catalog listing.
        /// </summary>
        [MaxLength(512)]
        public string Summary { get; set; } = "";

        /// <summary>
        /// An optional collection that this book belongs to; usually a longer work split into volumes.
        /// </summary>
        [ForeignKey("Collection")] public int? CollectionId { get; set; }
        public BookCollection? Collection { get; set; }

        /// <summary>
        /// Since books often need to be split across multiple volumes, store which volume in the collections this book is.
        /// </summary>
        public int? Volume { get; set; }

        /// <summary>
        /// The author of the book (just a text field, not the creator, in case someone is uploading e.g: Poe's "The Raven").
        /// </summary>
        [Required, MaxLength(256)]
        public required string Author { get; set; }

        /// <summary>
        /// The player who clicked submit.
        /// </summary>
        [Required, ForeignKey("CreatedBy")] public Guid? CreatorUserId { get; set; }
        public Player CreatedBy { get; set; } = default!;

        /// <summary>
        /// The librarian who approved the book.
        /// </summary>
        [Required, ForeignKey("ApprovedBy")] public Guid? ApprovedById { get; set; }
        public Player ApprovedBy { get; set; } = default!;

        /// <summary>
        /// Whether the book was approved or not.
        /// </summary>
        public bool Approved { get; set; }

        /// <summary>
        /// The round during which the book was approved.
        /// </summary>
        [Required, ForeignKey("RoundCreated")] public int RoundCreatedId { get; set; }
        public Round RoundCreated { get; set; } = default!;

        /// <summary>
        /// The text of the book itself.
        /// </summary>
        public required string Text { get; set; }
    }
}
