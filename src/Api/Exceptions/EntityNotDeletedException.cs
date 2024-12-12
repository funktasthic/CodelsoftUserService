namespace Cubitwelve.Src.Exceptions
{
    public class EntityDeletedException : Exception
    {
        public EntityDeletedException() { }

        public EntityDeletedException(string? message)
            : base(message) { }

        public EntityDeletedException(string? message, Exception? innerException)
            : base(message, innerException) { }
    }
}