using CosmosHelpers.CosmosRegions;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using Microsoft.Win32.SafeHandles;
using System.Runtime.InteropServices;

namespace CosmosHelpers.Context
{
  public class CosmosContext : IDisposable
  {
    private string _endpoint;
    private string _key;
    CosmosClient _cosmosClient;
    private SafeHandle _safeHandle = new SafeFileHandle(IntPtr.Zero, true);
    private bool _disposed = false;
    private string? _region;
    public CosmosContext(IConfiguration config)
    {
      _endpoint = config.GetValue<string>("cosmosEndpoint");
      _key = config.GetValue<string>("cosmosKey");
      _region = Environment.GetEnvironmentVariable("REGION_NAME");
      var options = new CosmosClientOptions()
      {
        ConnectionMode = ConnectionMode.Gateway,
        ApplicationRegion = AzureRegionToCosmosTranslator.Translate(_region)
      };
      _cosmosClient = new CosmosClient(_endpoint, _key, options);

    }

    public async Task<Database> GetDatabase(string dbName)
    {
      var database = await _cosmosClient.CreateDatabaseIfNotExistsAsync(dbName);
      return database;
    }

    public void Dispose()
    {
      _cosmosClient.Dispose();
      Dispose(true);
    }
    // Protected implementation of Dispose pattern.
    protected virtual void Dispose(bool disposing)
    {
      if (_disposed)
      {
        return;
      }

      if (disposing)
      {
        // Dispose managed state (managed objects).
        _safeHandle?.Dispose();
      }

      _disposed = true;
    }
  }
}
