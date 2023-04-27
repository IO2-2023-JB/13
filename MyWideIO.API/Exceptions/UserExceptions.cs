namespace MyWideIO.API.Exceptions
{
    /// <summary>
    /// Wyjatek zwiazany z kontrolerem usera.
    /// </summary>
    public class UserException : CustomException
    {
        public UserException(string? message) : base(message)
        {

        }
    }
    public class ForbiddenException : UserException
    {
        public ForbiddenException() : base("Forbidden")
        {

        }
    }
    public class DuplicateEmailException : UserException
    {
        public DuplicateEmailException() : base("A user with this e-mail address already exists")
        {

        }
    }
    public class UserNotFoundException : UserException
    {
        public UserNotFoundException() : base("User not found")
        {

        }
    }
    public class UserNotFoundExceptionDelete : UserException
    {
        public UserNotFoundExceptionDelete() : base("User not found ale 400 blad")
        {

        }
    }
    public class IncorrectPasswordException : UserException
    {
        public IncorrectPasswordException() : base("Password was incorrect")
        {

        }
    }

}
