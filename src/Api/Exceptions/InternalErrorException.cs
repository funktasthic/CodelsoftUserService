namespace Cubitwelve.Src.Exceptions
{
    public class InternalErrorException : Exception
    {
        public InternalErrorException() { }

        public InternalErrorException(string? message)
            : base(message) { }

        public InternalErrorException(string? message, Exception? innerException)
            : base(message, innerException) { }
    }
}