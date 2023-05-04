namespace MyWideIO.API.Exceptions
{
    public class PlaylistException : CustomException
    {
        public PlaylistException(string? message) : base(message)
        {

        }
    }
    public class PlaylistNotFoundException : PlaylistException
    {
        public PlaylistNotFoundException() : base("Playlist not found")
        {

        }
    }
    public class PlaylistIsPrivateException : PlaylistException
    {
        public PlaylistIsPrivateException() : base("Playlist is private")
        {

        }
    }
}
