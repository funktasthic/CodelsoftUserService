namespace UserService.Api.Models
{
    public class UserProgress : BaseModel
    {
        public int UserId { get; set; }
        public User User { get; set; } = null!;

        public int SubjectId { get; set; }
        public Subject Subject { get; set; } = null!;
    }
}