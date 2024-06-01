namespace SpeakerManagement.ViewModels.Speaker
{
    public class AssignTaskViewModel
    {
        public int SpeakerTaskId { get; set; }
        public string TaskName { get; set; }
        public string InputType { get; set; }
        public string? Instruction { get; set; }
        public string? Data { get; set; }
        public DateTime Deadline { get; set; }
        public bool IsCompleted { get; set; }
    }
}
