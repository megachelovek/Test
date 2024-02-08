using MongoDB.Bson.Serialization.Attributes;

namespace Test.Common
{
    [BsonIgnoreExtraElements]
    public class Task
    {
        public Task(Guid id)
        {
            this.Id = id;
        }

        public Guid Id { get; }

        public TimeSpan CurrentTime { get; set; }

        public Status Status { get; set; }
    }

    public enum Status
    {
        Created = 1,
        Running = 2,
        Finished = 3
    };
}