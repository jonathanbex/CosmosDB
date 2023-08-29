using Microsoft.Azure.Cosmos;
using Newtonsoft.Json;
using System.Reflection;

namespace CosmosHelpers.Context
{
  public static class CosmosExtensions
  {
    public static async Task<Container> EnsureCollectionAsync(this Microsoft.Azure.Cosmos.Database database, object? collection = null, string? optionalKey = null, string? optionalName = null, Type? type = null)
    {
      string partitionKey = "";
      string collectionName = "";

      if (collection != null)
      {
        partitionKey = collection.GetType().GetProperties()
        .Where(p => p.GetCustomAttribute<JsonPropertyAttribute>()?.PropertyName == "partitionKey")
           .Select(p => p.Name)
           .FirstOrDefault() ?? "";

        collectionName = string.IsNullOrEmpty(optionalName) ? collection.GetType().Name : optionalName;
      }

      if (!string.IsNullOrEmpty(optionalKey)) partitionKey = optionalKey;

      if (type != null)
      {
        partitionKey = type.GetProperties()
        .Where(p => p.GetCustomAttribute<JsonPropertyAttribute>()?.PropertyName == "partitionKey")
           .Select(p => p.Name)
           .FirstOrDefault() ?? "";

        collectionName = string.IsNullOrEmpty(optionalName) ? type.Name : optionalName;
      }


      if (string.IsNullOrEmpty(partitionKey)) throw new Exception("Missing Partition Key consider adding JsonAttribute partitionKey to ur model");
      partitionKey = $"/" + "partitionKey";
      return await database.CreateContainerIfNotExistsAsync(collectionName, partitionKey);
    }

    public static async Task<List<T>> ToListAsync<T>(this Microsoft.Azure.Cosmos.Database database, QueryDefinition query, Container? container = null)
    {
      if (container == null) container = await EnsureCollectionAsync(database, null, null, null, typeof(T));
      using (var queryResultSetIterator = container.GetItemQueryIterator<T>(query))
      {
        var results = new List<T>();
        while (queryResultSetIterator.HasMoreResults)
        {
          FeedResponse<T> currentResultSet = await queryResultSetIterator.ReadNextAsync();
          foreach (T result in currentResultSet)
          {
            results.Add(result);
          }
        }

        return results;
      }
    }

    public static async Task<T?> FirstOrDefaultAsync<T>(this Microsoft.Azure.Cosmos.Database database, QueryDefinition query, Container? container = null)
    {
      if (container == null) container = await EnsureCollectionAsync(database, null, null, null, typeof(T));
      using (var queryResultSetIterator = container.GetItemQueryIterator<T>(query))
      {
        var results = new List<T>();
        while (queryResultSetIterator.HasMoreResults)
        {
          FeedResponse<T> currentResultSet = await queryResultSetIterator.ReadNextAsync();
          foreach (T result in currentResultSet)
          {
            return result;
          }
        }
        return default(T);
      }
    }

    public static async Task<T> CreateItemAsync<T>(this Microsoft.Azure.Cosmos.Database database, T item)
    {
      var container = await EnsureCollectionAsync(database, item);
      return await container.CreateItemAsync<T>((T)item);
    }
    public static async Task<T> CreateOrUpdateItemAsync<T>(this Microsoft.Azure.Cosmos.Database database, T item)
    {
      var container = await EnsureCollectionAsync(database, item);
      return await container.UpsertItemAsync<T>((T)item);
    }

    /// <summary>
    /// Delet Item Async
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="database"></param>
    /// <param name="item"></param>
    /// <param name="key"></param>
    /// <param name="id"> optional parameter, only really used if you use polymophism and have id on the parent or if you dont specify jsonattribute</param>
    /// <returns></returns>
    public static async Task<bool> DeleteItemAsync<T>(this Microsoft.Azure.Cosmos.Database database, T item, string key, string? id = null)
    {
      var container = await EnsureCollectionAsync(database, item);
      if (string.IsNullOrEmpty(id))
      {
        var idName = item.GetType().GetProperties()
                   .Select(p => p.GetCustomAttribute<JsonPropertyAttribute>())
                   .Where(x => x.PropertyName == "id")
                   .Select(x => x.PropertyName)
                   .FirstOrDefault() ?? "";


        var idProp = item.GetType().GetProperties().FirstOrDefault(x => x.Name == idName);
        if (idProp == null) throw new Exception("missing Id attribute");
        id = idProp.GetValue(item, null) as string;
      }
      var partitionKey = new PartitionKey(key);
      await container.DeleteItemAsync<T>(id, partitionKey);
      return true;
    }
  }
}