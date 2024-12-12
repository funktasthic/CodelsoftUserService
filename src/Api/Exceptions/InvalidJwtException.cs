namespace Cubitwelve.Src.Exceptions
{
    public class InvalidJwtException : Exception
    {
        public InvalidJwtException() { }

        public InvalidJwtException(string? message)
            : base(message) { }

        public InvalidJwtException(string? message, Exception? innerException)
            : base(message, innerException) { }
    }
}