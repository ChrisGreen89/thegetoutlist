using System;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace TheGetOutList.Data.Models
{
    public class Goal
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public Guid Id { get; set; }
        public Guid? DreamId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public List<Note> Notes { get; set; }
    }
}