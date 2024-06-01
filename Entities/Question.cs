using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SpeakerManagement.Entities
{
    [Table("Question")]
    public class Question
    {
        [Key]
        public int Id { get; set; }
        public int SpeaketEventId { get; set; }
        public string Description { get; set; }
        public DateTime CreatedDate { get; set; }
        public short Status { get; set; }
    }
}
