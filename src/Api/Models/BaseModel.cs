namespace UserService.Api.Models
{
    public abstract class BaseModel
    {
        public int Id { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? DeletedAt { get; set; } = null;

        public int Version { get; set; } = 1;
    }
}