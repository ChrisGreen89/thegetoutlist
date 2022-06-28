using TheGetOutList.Data.Infrastructure.Attributes;

namespace TheGetOutList.Data.Models
{
    [BsonCollectionAttribute("tasks")]
    public class Task : Document
    {
        public Guid UserId { get; set; }
    }
}