using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SpeakerManagement.Entities
{
    [Table("EventTask")]
    public class EventTasks
    {
        [Key]
        public int Id { get; set; }
        public int EventId { get; set; }
        public int TaskId { get; set; }
        public DateTime Deadline { get; set; }
    }
}
