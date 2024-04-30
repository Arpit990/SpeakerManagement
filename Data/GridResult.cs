namespace SpeakerManagement.Data
{
    public class GridResult
    {
        public object data { get; set; }
        public int draw { get; set; }
        public int recordsTotal { get; set; }
        public int recordsFiltered { get; set; }
        public string query { get; set; }
        public object additionalData { get; set; }
    }
}
