using TheGetOutList.Data.Infrastructure.Attributes;

namespace TheGetOutList.Data.Models
{
    [BsonCollectionAttribute("projects")]
    public class Project
    {
        public Guid UserId { get; set; }

    }
}