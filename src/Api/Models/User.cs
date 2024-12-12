using System.ComponentModel.DataAnnotations;

namespace UserService.Api.Models
{
    public class User : BaseModel
    {
        [StringLength(250)]
        public string Name { get; set; } = null!;

        [StringLength(250)]
        public string FirstLastName { get; set; } = null!;

        [StringLength(250)]
        public string SecondLastName { get; set; } = null!;

        [StringLength(250)]
        public string RUT { get; set; } = null!;

        [StringLength(250)]
        public string Email { get; set; } = null!;

        [StringLength(250)]
        public string HashedPassword { get; set; } = null!;
        public bool IsEnabled { get; set; } = true;
        public int CareerId { get; set; }
        public Career Career { get; set; } = null!;

        public int RoleId { get; set; }
        public Role Role { get; set; } = null!;
    }
}