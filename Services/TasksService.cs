using MongoDB.Driver;
using Test.Common;
using Test.Controllers;
using Task = System.Threading.Tasks.Task;

namespace Test.Services
{
    public class TasksService
    {
        private readonly BackgroundTask _backgroundTask;
        private readonly IMongoCollection<Common.Task> _taskCollection;

        public TasksService(BackgroundTask backgroundTask)
        {
            _backgroundTask = backgroundTask;
        }

        public async Task<Guid> CreateAndRunTask(CancellationToken ct)
        {
            var newTask = new Common.Task(new Guid())
            {
                CurrentTime = new TimeSpan(),
                Status = Status.Created
            };

            //Не дожидаясь выполнения задачи, возвращаем её GUID. Если будет ошибка она отобразится в логгере.
            _backgroundTask.RunTask(newTask, ct);

            return newTask.Id;
        }

        public async Task<Common.Task> GetTaskStatus(string id, CancellationToken ct)
        {
            var guid = Guid.Parse(id);
            var filter = Builders<Common.Task>.Filter;
            var filterBuilder = filter.Eq(x => x.Id, guid);

            var result = _taskCollection.FindAsync(filterBuilder, null, ct).Result.FirstOrDefault();

            return result;
        }
    }
}
