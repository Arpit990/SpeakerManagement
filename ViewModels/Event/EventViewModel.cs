namespace SpeakerManagement.ViewModels.Event
{
    public class EventViewModel
    {
        public int EventId { get; set; }
        public string? EventName { get; set; }
        public string EventLogo { get; set; }
        public DateTime EventDate { get; set; }
        public int NumberOfTask { get; set; }
    }
}
