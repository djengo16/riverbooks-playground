namespace RiverBooks.EmailSending.ListEmailsEndpoint;

public class ListEmailsResponse
{
  public int Count { get; set; }
  public List<EmailDto> Emails { get; internal set; } = new();
}

