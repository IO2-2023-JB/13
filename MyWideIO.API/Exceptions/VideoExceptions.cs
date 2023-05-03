namespace MyWideIO.API.Exceptions
{
    public class VideoException: CustomException
    {
        public VideoException(string? message) : base(message) { }
    }

    public class VideoNotFoundException : VideoException
    {
        public VideoNotFoundException() : base("Video not found") { }
    }
    public class VideoIsPrivateException : VideoException
    {
        public VideoIsPrivateException() : base("Video is private") { }
    }

}
