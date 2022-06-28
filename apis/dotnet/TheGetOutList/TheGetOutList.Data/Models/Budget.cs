using MongoDB.Bson;
using TheGetOutList.Data.Infrastructure.Attributes;

namespace TheGetOutList.Data.Models
{
    [BsonCollectionAttribute("budgets")]
    public class Budget : Document
    {
        public Guid UserId { get; set; }
    }
}