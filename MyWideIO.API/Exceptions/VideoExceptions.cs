namespace MyWideIO.API.Exceptions
{
    public class VideoException: CustomException
    {
        public VideoException(string? message) : base(message) { }
    }

    public class VideoNotFoundException : VideoException
    {
        public VideoNotFoundException() : base("Nie znaleziono podanego video") { }
    }

}
