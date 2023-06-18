namespace MyWideIO.API.Exceptions
{
    public class TicketException : CustomException
    {
        public TicketException(string? message) : base(message)
        {

        }
    }
    public class TicketNotFoundException : TicketException
    {
        public TicketNotFoundException() : base("Ticket not found")
        {

        }
    }
}
