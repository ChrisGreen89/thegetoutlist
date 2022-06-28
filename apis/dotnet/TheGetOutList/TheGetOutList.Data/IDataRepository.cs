using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using TheGetOutList.Data.Infrastructure.Attributes;
using TheGetOutList.Data.Models;
using Task = System.Threading.Tasks.Task;

namespace TheGetOutList.Data
{
    public interface IDataRepository
    {
        public IDataRepository Connect(string connectionString);
        public IDataRepository SetDatabase(string databaseName);
        public Task<T> GetItem<T>(Expression<Func<T, bool>> predicate) where T : Document;
        public Task<List<T>> GetItems<T>(Expression<Func<T, bool>> predicate) where T : Document;
        Task<T> UpsertItem<T>(T item) where T : Document;
        Task DeleteItem<T>(T item) where T : Document;
        Task SeedDatabase();
    }

    public class MongoRepository : IDataRepository
    {
        private IMongoDatabase? _db;
        private MongoClient? _client;

        /// <summary>
        /// Fluent method to connect the repository to a MongoDB using a connection string.
        /// </summary>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        public IDataRepository Connect(string connectionString)
        {
            _client = new MongoClient(connectionString);
            return this;
        }

        /// <summary>
        /// Fluent method to set the working database using a database name string
        /// </summary>
        /// <param name="databaseName"></param>
        /// <returns></returns>
        public IDataRepository SetDatabase(string databaseName)
        {
            _db = _client.GetDatabase(databaseName);
            return this;
        }

        /// <summary>
        /// Returns a single item from the database
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public async Task<T> GetItem<T>(Expression<Func<T, bool>> predicate) where T : Document
        {
            var collection = GetCollection<T>(typeof(T).GetType());
            var payload = await collection.FindAsync(predicate);

            return payload.FirstOrDefault();
        }

        /// <summary>
        /// Returns a list of items from the database
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public async Task<List<T>> GetItems<T>(Expression<Func<T, bool>> predicate) where T : Document
        {
            var collection = GetCollection<T>(typeof(T).GetType());
            var items = await collection.FindAsync(predicate);
            return items.ToList();
        }


        /// <summary>
        /// Inserts or updates an item into the database, into the collection specified by the class
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item"></param>
        /// <returns></returns>
        public async Task<T> UpsertItem<T>(T item) where T : Document
        {
            var filter = new FilterDefinitionBuilder<T>().Where(x => x.Id == item.Id);

            var updateoptions = new ReplaceOptions()
            {
                IsUpsert = true
            };

            var collection = GetCollection<T>(item.GetType());

            var itemUpdate = await collection.ReplaceOneAsync(filter, item, updateoptions);

            if (itemUpdate.UpsertedId.IsObjectId)
            {
                item.Id = ((ObjectId?)itemUpdate.UpsertedId);
            }

            return item;
        }

        /// <summary>
        /// Deletes a single item from the database
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item"></param>
        /// <returns></returns>
        public async Task DeleteItem<T>(T item) where T : Document
        {
            var collection = GetCollection<T>(item.GetType());
            await collection.FindOneAndDeleteAsync(x => x.Id == item.Id);
            return;
        }

        /// <summary>
        /// Given a Document child class, returns the specified collection based on the collection name attribute
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        private IMongoCollection<T> GetCollection<T>(Type type) where T : Document
        {
            var documentType = typeof(T).GetType();
            var attributes = type.GetCustomAttributes(
                typeof(BsonCollectionAttribute),
                true);

            var collectionName = attributes.FirstOrDefault() as BsonCollectionAttribute;

            return _db.GetCollection<T>(collectionName?.CollectionName);
        }

        public async Task SeedDatabase()
        {
            var newDream = new Dream()
            {
                Description = "A seeded dream",
                Title = "Seed Dream"
            };

            await UpsertItem(newDream);

        }
    }
}