using System.ComponentModel.DataAnnotations.Schema;

namespace SpeakerManagement.Entities
{
    [Table("Event")]
    public class Events
    {
        public int Id { get; set; }
        public string EventName { get; set; }
        public string Logo { get; set; }
        public DateTime EventDate { get; set; }
        public string CreatedBy { get; set; }
    }
}
