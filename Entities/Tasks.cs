using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SpeakerManagement.Entities
{
    [Table("Task")]
    public class Tasks
    {
        [Key]
        public int Id { get; set; }
        public string TaskName { get; set; }
        public string InputType { get; set; }
        public string? Instructions { get; set; }
        public bool IsOptional { get; set; } = false;
    }
}
