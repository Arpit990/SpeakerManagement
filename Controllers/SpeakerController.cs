using Microsoft.AspNetCore.Mvc;
using SpeakerManagement.Data;
using SpeakerManagement.Entities;
using SpeakerManagement.Helper;
using SpeakerManagement.Repository;
using System.IO;

namespace SpeakerManagement.Controllers
{
    public class SpeakerController : Controller
    {
        #region Private
        private readonly IEventTaskRepository _eventTaskRepository;
        private readonly ISpeakerTaskRepository _speakerTaskRepository;
        private readonly ITaskRepository _taskRepository;
        private readonly IWebHostEnvironment _environment;
        #endregion

        #region Constructor
        public SpeakerController(
            IEventTaskRepository eventTaskRepository,
            ISpeakerTaskRepository speakerTaskRepository,
            ITaskRepository taskRepository,
            IWebHostEnvironment environment
        )
        {
            _eventTaskRepository = eventTaskRepository;
            _speakerTaskRepository = speakerTaskRepository;
            _taskRepository = taskRepository;
            _environment = environment;
        }
        #endregion

        #region Public
        public async Task<IActionResult> Index()
        {
            var result = await _speakerTaskRepository.GetSpeakerTaskList();
            var data = _speakerTaskRepository.GetSpeakerTasksStatus();
            return View(result);
        }

        public async Task<IActionResult> SaveFile(IFormFile data, int speakerTaskId)
        {
            if(data != null && data.Length > 0)
            {
                var speakerTask = await _speakerTaskRepository.Get(x => x.Id == speakerTaskId);
                var eventTask = await _eventTaskRepository.Get(x => x.Id == speakerTask.EventTaskId);
                var task = await _taskRepository.Get(x => x.Id == eventTask.TaskId);

                if(!string.IsNullOrEmpty(task.Validation))
                {
                    if(task.Validation == Enums.Validation.UploadSize.ToString())
                    {
                        if(data.Length > 5 * 1024 * 1024)
                        {
                            return Json(FormResult.Success(false, "File Size Should Be Less Than 5MB"));
                        }
                    }
                    else if(task.Validation == Enums.Validation.PDFOnly.ToString())
                    {
                        if(!Utility.IsPdfFile(data))
                        {
                            return Json(FormResult.Success(false, "The Uploaded File Is Not a Valid PDF"));
                        }
                    }
                    else if (task.Validation == Enums.Validation.ImageOnly.ToString())
                    {
                        if (!Utility.IsImageFile(data))
                        {
                            return Json(FormResult.Success(false, "The Uploaded File Is Not a Valid Image File (jpg, jpeg, png)"));
                        }
                    }
                }
            }

            FileUpload fileUpload = new FileUpload(_environment);
            MethodResponse<object> result = await fileUpload.UploadFile(data);
            if (result.IsSuccess)
            {
                // Assuming eventTasks is an object where you want to set the EventLogoPath
                string path = result.Data != null ? ((dynamic)result.Data).FilePath : null;

                var speakerTask = await _speakerTaskRepository.Get(x => x.Id == speakerTaskId);
                if (speakerTask != null)
                {
                    speakerTask.Data = path;
                    speakerTask.IsCompleted = true;
                    speakerTask.CompletionDate = DateTime.Now;

                    await _speakerTaskRepository.SaveChangesAsync();
                    return Json(FormResult.Success(true, "Task Submitted Successfully"));
                }
            }
            return Json(FormResult.Success(false, result.Message));
        }

        public async Task<IActionResult> SaveData(string data, int speakerTaskId)
        {
            if (!string.IsNullOrEmpty(data))
            {
                var speakerTask = await _speakerTaskRepository.Get(x => x.Id == speakerTaskId);
                var eventTask = await _eventTaskRepository.Get(x => x.Id == speakerTask.EventTaskId);
                var task = await _taskRepository.Get(x => x.Id == eventTask.TaskId);

                if (string.IsNullOrEmpty(task.Validation))
                {
                    if (task.Validation == Enums.Validation.URLOnly.ToString())
                    {
                        if (!Utility.IsValidUrl(data))
                        {
                            return Json(FormResult.Success(false, "The Enter Valid URL Format"));
                        }
                    }
                }
            }

            if (speakerTaskId > 0 && !string.IsNullOrEmpty(data))
            {
                var speakerTask = await _speakerTaskRepository.Get(x => x.Id == speakerTaskId);
                if (speakerTask != null)
                {
                    speakerTask.Data = data;
                    speakerTask.IsCompleted = true;
                    speakerTask.CompletionDate = DateTime.Now;

                    await _speakerTaskRepository.SaveChangesAsync();
                    return Json(FormResult.Success(true, "Task Submitted Successfully"));
                }
            }
            return Json(FormResult.Success(false));
        }
        #endregion
    }
}
