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

        public enum Operation
        {
            Insert,
            Update, 
            Delete,
            Retrive
        }

        public enum InputTypes
        {
            [Description("Textarea")]
            TEXTAREA = 1,
            [Description("File")]
            FILE = 2,
            [Description("Checkbox")]
            CHECKBOX = 3,
            [Description("Url")]
            URL = 4
        };

        public enum UserRoles
        {
            [Description("Super Admin")]
            SuperAdmin,
            [Description("Admin")]
            Admin,
            [Description("Speaker")]
            Speaker
        }

        public enum Validation
        {
            [Description("Allow Only .pdf Files")]
            PDFOnly = 1,
            [Description("Allow Only .jpg, .jpeg, .png Files")]
            ImageOnly = 2,
            [Description("Allow Only URL")]
            URLOnly = 3,
            [Description("File Size Not More Than 5MB")]
            UploadSize = 4
        }
    }
}
