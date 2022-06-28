using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.IdGenerators;

namespace TheGetOutList.Data.Models
{
    public interface IDocument
    {
        public ObjectId? Id { get; set; }

        public DateTime CreatedAt { get; }

    }

    public abstract class Document : IDocument
    {
        public ObjectId? Id { get; set; }

        public DateTime CreatedAt { get; }
    }
}