using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SpeakerManagement.Entities
{
    [Table("SpeakerTasks")]
    public class SpeakerTasks
    {
        [Key]
        public int Id { get; set; }
        public string SpeakerId { get; set; }
        public int EventTaskId { get; set; }
        public string? Data {  get; set; }
        public bool IsCompleted { get; set; }
        public DateTime? CompletionDate { get; set; }
        public string? Status { get; set; }
    }
}
