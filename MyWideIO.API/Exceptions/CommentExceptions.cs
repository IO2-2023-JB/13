namespace MyWideIO.API.Exceptions
{
    public class CommentException : CustomException
    {
        public CommentException(string message) : base(message)
        { }
    }
    public class CommentNotFoundException : CommentException
    {
        public CommentNotFoundException() : base("Comment not found")
        { }
    }
}
