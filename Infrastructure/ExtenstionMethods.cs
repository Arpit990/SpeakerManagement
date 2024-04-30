using Microsoft.EntityFrameworkCore.Metadata.Internal;
using SpeakerManagement.Data;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using System.Linq.Expressions;

namespace SpeakerManagement.Infrastructure
{
    public static class ExtenstionMethods
    {
        public static GridResult SearchGrid<T>(this List<T> list, GridSearch data)
        {
            var totalCount = list.Count();

            list = list.Skip(data.start).Take(data.length).ToList();
            var gridResult = new GridResult()
            {
                draw = data.draw,
                recordsTotal = totalCount,
                recordsFiltered = totalCount,
                data = list
            };
            return gridResult;
        }
    }
}
