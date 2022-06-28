using System;
namespace TheGetOutList.Data.Infrastructure.Attributes
{
	public class BsonCollectionAttribute : Attribute
	{
        public string CollectionName { get; }

        public BsonCollectionAttribute(string collectionName)
        {
            CollectionName = collectionName;
        }
    }
}

