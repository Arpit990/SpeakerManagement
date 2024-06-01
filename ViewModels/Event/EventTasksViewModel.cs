namespace SpeakerManagement.ViewModels.Event
{
    public class EventTasksViewModel
    {
        public string EventName { get; set; }
        public string? EventLogoPath { get; set; }
        public IFormFile EventLogo { get; set; }
        public DateTime EventDate { get; set; }
        public List<TaskViewModel> Tasks { get; set; }
    }

    public class TaskViewModel
    {
        public int TaskId { get; set; }
        public DateTime Deadline { get; set; }
    }
}
