using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SpeakerManagement.Entities
{
    [Table("SpeakerEvents")]
    public class SpeakerEvents
    {
        [Key]
        public int Id { get; set; }
        public int EventId { get; set; }
        public string SpeakerId { get; set; }
        public bool IsCompleted {  get; set; }
        public DateTime? CompletionDate { get; set; }
        public string AssignBy { get; set; }
        public DateTime AssignDate { get; set; }
    }
}
