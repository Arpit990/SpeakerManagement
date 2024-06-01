using Microsoft.AspNetCore.Mvc.Rendering;
using NuGet.ProjectModel;
using System.ComponentModel;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using static SpeakerManagement.Data.Enums;

namespace SpeakerManagement.Helper
{
    public static class Common
    {
        #region Private
        private const string SpecialCharacters = "!@#$%^&*()<>.?+";
        private const string Digits = "0123456789";
        private const string CapitalLetters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        private const string SmallLetters = "abcdefghijklmnopqrstuvwxyz";
        #endregion

        public static string GetEnumDescription(Enum value)
        {
            Type type = value.GetType();
            string name = Enum.GetName(type, value);

            if (name != null)
            {
                FieldInfo field = type.GetField(name);
                if (field != null)
                {
                    DescriptionAttribute attr = field.GetCustomAttribute<DescriptionAttribute>();
                    if (attr != null)
                    {
                        return attr.Description;
                    }
                }
            }

            // If no description attribute found, return the enum value's name
            return value.ToString();
        }

        public static List<SelectListItem> BuildSelectList<TEnum>()
        {
            var result = new List<SelectListItem>();

            foreach (var item in Enum.GetValues(typeof(TEnum)))
            {
                var text = GetEnumDescription(item as Enum);
                result.Add(new SelectListItem { Text = text, Value = text });
            }
            return result;
        }

        public static List<SelectListItem> BuildSelectListItem<TEnum>()
        {
            var result = new List<SelectListItem>();

            foreach (var item in Enum.GetValues(typeof(TEnum)))
            {
                var text = GetEnumDescription(item as Enum);
                result.Add(new SelectListItem { Text = text, Value = item.ToString() });
            }
            return result;
        }

        public static List<SelectListItem> InputTypeList() => BuildSelectList<InputTypes>();
        public static List<SelectListItem> ValidationList() => BuildSelectListItem<Validation>();
        public static List<SelectListItem> UserRoleList() => BuildSelectListItem<UserRoles>();

        #region Password generator
        public static string GeneratePassword()
        {
            Random random = new Random();

            // Initialize lists to hold characters for each category
            List<char> passwordChars = new List<char>();

            // Add 2 random special characters
            for (int i = 0; i < 2; i++)
            {
                passwordChars.Add(SpecialCharacters[random.Next(SpecialCharacters.Length)]);
            }

            // Add 2 random digits
            for (int i = 0; i < 2; i++)
            {
                passwordChars.Add(Digits[random.Next(Digits.Length)]);
            }

            // Add 6 random alphabetic characters (mix of capital and small letters)
            for (int i = 0; i < 6; i++)
            {
                if (i % 2 == 0)
                {
                    passwordChars.Add(CapitalLetters[random.Next(CapitalLetters.Length)]);
                }
                else
                {
                    passwordChars.Add(SmallLetters[random.Next(SmallLetters.Length)]);
                }
            }

            // Shuffle the password characters
            passwordChars = passwordChars.OrderBy(c => random.Next()).ToList();

            // Convert the list of characters to a string
            string password = new string(passwordChars.ToArray());

            return password;
        }

        public async static Task<string> FileUpload(IFormFile inputFile)
        {
            // Define a directory to store logo files
            var uploadDir = Path.Combine(Directory.GetCurrentDirectory(), "uploads", "logos");

            // Ensure the directory exists
            if (!Directory.Exists(uploadDir))
            {
                Directory.CreateDirectory(uploadDir);
            }

            // Define a unique file name for the logo
            var uniqueFileName = Path.Combine(uploadDir, Guid.NewGuid().ToString() + Path.GetExtension(inputFile.FileName));

            // Copy the logo file to the upload directory
            using (var fileStream = new FileStream(uniqueFileName, FileMode.Create))
            {
                await inputFile.CopyToAsync(fileStream);
            }

            // Return the unique file name
            return Path.GetFileName(uniqueFileName);
        }
        #endregion
    }
}
