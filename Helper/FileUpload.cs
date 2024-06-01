namespace SpeakerManagement.Helper
{
    public class FileUpload
    {
        #region Private
        private readonly IWebHostEnvironment _environment;
        #endregion

        #region Constructor
        public FileUpload(IWebHostEnvironment environment)
        {
            _environment = environment;
        }
        #endregion

        #region Public
        public async Task<MethodResponse<object>> UploadFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return MethodResponse<object>.Fail("");
            }

            try
            {
                var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads\\EventLogo"); // Path to save uploads
                var fileName = Guid.NewGuid().ToString() + "_" + file.FileName; // Generate unique file name
                var filePath = Path.Combine(uploadsFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream); // Save file to disk
                }

                return MethodResponse<object>.Success(new { FilePath = "uploads\\EventLogo" + fileName });
            }
            catch (Exception ex)
            {
                return MethodResponse<object>.Fail(ex.Message);
            }
        }
        #endregion
    }
}
