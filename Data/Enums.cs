using System.ComponentModel;

namespace SpeakerManagement.Data
{
    public class Enums
    {
        public enum AjaxResponse
        {
            Success,
            Error,
            Info,
            Warning,
            Processing
        }

        public enum InputTypes
        {
            [Description("Textarea")]
            TEXTAREA = 1,
            [Description("File")]
            FILE = 2,
            [Description("Checkbox")]
            CHECKBOX = 3,
        };
    }
    
}
