using MongoDB.Driver;
using Test.Common;
using Test.Controllers;
using Task = System.Threading.Tasks.Task;

namespace Test.Services
{
    public class BackgroundTask
    {
        private readonly IMongoCollection<Common.Task> _taskCollection;
        private readonly ILogger<TasksController> _logger;

        public BackgroundTask(ILogger<TasksController> logger, IMongoCollection<Common.Task> taskCollection)
        {
            _taskCollection = taskCollection;
            _logger = logger;
        }

        public async Task RunTask(Common.Task newTask, CancellationToken ct)
        {
            try
            {
                if (_taskCollection == null) throw new Exception("Пустая задача");

                await _taskCollection.InsertOneAsync(newTask, ct);

                Thread.Sleep(1000 * 60 * 120);

                var filterBuilder = Builders<Common.Task>.Filter; 
                var filterRunning = filterBuilder.Eq(x => x.Id, newTask.Id);

                var updateBuilder = Builders<Common.Task>.Update; 
                var updateRunning = updateBuilder.Set(x => x.Status, Status.Running);

                await _taskCollection.UpdateOneAsync(filterRunning, updateRunning, null,ct);

                Thread.Sleep(1000 * 60 * 120);

                var filterFinish = filterBuilder.Eq(x => x.Id, newTask.Id);
                var updateFinish = updateBuilder.Set(x => x.Status, Status.Running);

                await _taskCollection.UpdateOneAsync(filterFinish, updateFinish, null, ct);
            }
            catch (Exception ex)
            {
                _logger.LogError("Ошибка во время работы BackgroundTask: "+ex.Message );
            }
        }
    }
}
