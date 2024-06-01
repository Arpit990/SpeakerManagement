using Microsoft.EntityFrameworkCore;
using SpeakerManagement.DatabaseContext;
using SpeakerManagement.Entities;
using SpeakerManagement.Helper;
using SpeakerManagement.ViewModels.Event;

namespace SpeakerManagement.Repository
{
    #region interface
    public interface IEventRepository : IBaseRepository<Events>
    {
        Task<MethodResponse<string>> AddEventTasks(EventTasksViewModel eventTasks);
        Task<List<EventViewModel>> GetEventList();
    }
    #endregion

    #region class
    public class EventRepository : BaseRepository<Events>, IEventRepository
    {
        #region Private
        private readonly IWebHostEnvironment _environment;
        private readonly IEventTaskRepository _eventTaskRepository;
        #endregion

        #region Constructor
        public EventRepository(
            DataContext dataContext,
            IWebHostEnvironment environment,
            IHttpContextAccessor httpContextAccessor,
            IEventTaskRepository eventTaskRepository
        ) : base(dataContext, httpContextAccessor)
        {
            _environment = environment;
            _eventTaskRepository = eventTaskRepository;
        }
        #endregion

        #region Public
        public async Task<List<EventViewModel>> GetEventList()
        {
            string? userId = GetCurrentUserId();

            var query = from events in _context.Events
                        join user in _context.Users on events.CreatedBy equals user.Id
                        let taskCount = (from eventTask in _context.EventTasks 
                                        where eventTask.EventId == events.Id && user.Id == userId
                                        select eventTask.Id).Count()
                        where user.Id == userId
                        select new EventViewModel
                        {
                            EventId = events.Id,
                            EventName = events.EventName,
                            EventLogo = events.Logo,
                            EventDate = events.EventDate,
                            NumberOfTask = taskCount
                        };

            return await query.AsNoTracking().ToListAsync();
        }

        public async Task<MethodResponse<string>> AddEventTasks(EventTasksViewModel eventTasks)
        {
            FileUpload fileUpload = new FileUpload(_environment);
            MethodResponse<object> result = await fileUpload.UploadFile(eventTasks.EventLogo);

            if (result.IsSuccess)
            {
                // Assuming eventTasks is an object where you want to set the EventLogoPath
                eventTasks.EventLogoPath = result.Data != null ? ((dynamic)result.Data).FilePath : null;

                Events events = new Events
                {
                    EventName = eventTasks.EventName,
                    EventDate = eventTasks.EventDate,
                    Logo = eventTasks.EventLogoPath ?? "",
                    CreatedBy = GetCurrentUserId() ?? ""
                };

                await Add(events);
               
                if(await SaveChangesAsync() > 0)
                {
                    List<EventTasks> eventTaskList = new List<EventTasks>();
                    foreach (var item in eventTasks.Tasks)
                    {
                        eventTaskList.Add(new EventTasks
                        {
                            EventId = events.Id,
                            TaskId = item.TaskId,
                            Deadline = item.Deadline
                        });
                    }

                    await _eventTaskRepository.AddRange(eventTaskList);
                    await SaveChangesAsync();

                    return MethodResponse<string>.Success("Data save successfully");
                }  
            }

            return MethodResponse<string>.Fail(result.Message);
        }
        #endregion
    }
    #endregion
}
