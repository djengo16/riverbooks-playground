namespace RiverBooks.EmailSending;

public record EmailDto(Guid Id, string From, string To, string Subject, DateTime? DateTimeUtcProcessed);
