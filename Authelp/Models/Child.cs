using System.ComponentModel.DataAnnotations;

namespace Authelp.Models
{
    public class Child
    {
        public int Id { get; set; }

        public int ParentId { get; set; }
        public User? Parent { get; set; } 

        


        [Required]
        public string ChildName { get; set; } = null!;
        public string? ChildCondition { get; set; }


        public string? ChildStory { get; set; }
    }
}
