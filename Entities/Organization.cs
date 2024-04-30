using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SpeakerManagement.Entities
{
    [Table("Organization")]
    public class Organization
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "Organization Name")]
        public required string OrganizationName { get; set; }
    }
}
