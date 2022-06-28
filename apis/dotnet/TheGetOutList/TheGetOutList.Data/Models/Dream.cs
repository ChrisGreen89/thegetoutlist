using System;
using TheGetOutList.Data.Infrastructure.Attributes;

namespace TheGetOutList.Data.Models
{
    [BsonCollectionAttribute("dreams")]
    public class Dream : Document
    {
        public Guid UserId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
    }
}