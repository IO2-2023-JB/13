namespace MyWideIO.API.Exceptions
{
    public class BadRequestException : Exception
    {
        public BadRequestException(string message) : base(message)
        {

        }

    }

    public class NotCreatorException : BadRequestException 
    {
        public NotCreatorException() : base("User is not a creator") 
        {
        
        }
    }
}
