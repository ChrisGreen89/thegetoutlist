using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using TheGetOutList.Data.Infrastructure.Attributes;

namespace TheGetOutList.Data.Models
{
    [BsonCollectionAttribute("users")]
	public class User : Document
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string EmailAddress { get; set; }
        public string AuthId { get; set; }
    }
}

