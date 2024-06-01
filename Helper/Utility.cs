using System.Globalization;
using System.Text.RegularExpressions;

namespace SpeakerManagement.Helper
{
    public class Utility
    {
        public static bool IsDate(string input)
        {
            string[] formats = { "M/d/yyyy", "M-d-yyyy" };
            return (DateTime.TryParseExact(input, formats, new CultureInfo("en-US"), DateTimeStyles.None, out _));
        }
        public static bool IsDecimal(string input)
        {
            return decimal.TryParse(input, out _);
        }
        public static bool IsInt32(string input)
        {
            return int.TryParse(input, out _);
        }
        public static bool IsInt64(string input)
        {
            return long.TryParse(input, out _);
        }
        public static bool IsDouble(string input)
        {
            return double.TryParse(input, out _);
        }
        public static bool IsNumeric(string value)
        {
            return value.All(char.IsNumber);
        }

        public static bool IsPdfFile(IFormFile file)
        {
            using (var reader = new BinaryReader(file.OpenReadStream()))
            {
                var buffer = reader.ReadBytes(4);
                return buffer[0] == 0x25 && buffer[1] == 0x50 && buffer[2] == 0x44 && buffer[3] == 0x46;
            }
        }
        public static bool IsImageFile(IFormFile file)
        {
            using (var reader = new BinaryReader(file.OpenReadStream()))
            {
                var buffer = reader.ReadBytes(8);
                // Check for JPEG
                if (buffer[0] == 0xFF && buffer[1] == 0xD8 && buffer[2] == 0xFF)
                {
                    return true;
                }
                // Check for PNG
                if (buffer[0] == 0x89 && buffer[1] == 0x50 && buffer[2] == 0x4E && buffer[3] == 0x47)
                {
                    return true;
                }
            }
            return false;
        }
        public static bool IsValidUrl(string url)
        {
            // Regular expression pattern for validating URL
            string pattern = @"^(https?://)?(www\.)?[a-zA-Z0-9\-\.]+\.[a-zA-Z]{2,}(\/\S*)?$";

            // Check if the URL matches the pattern
            return Regex.IsMatch(url, pattern);
        }
    }
}
