using System.ComponentModel.DataAnnotations;

namespace UserService.Api.Models
{
    public class Career : BaseModel
    {
        [StringLength(250)]
        public string Name { get; set; } = null!;
    }
}