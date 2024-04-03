using CosmosDb.ChangeFeed.Template.Domain.Entities;
using CosmosDb.ChangeFeed.Template.Domain.Enums;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Document = Microsoft.Azure.Documents.Document;
using PartitionKey = Microsoft.Azure.Cosmos.PartitionKey;

namespace CosmosDb.ChangeFeed.Template.Functions;

/// <summary>
/// Represents the functions for syncing counts by status.
/// </summary>
public static class CosmosDbTriggerFunction
{
    private const string PropertyDelimiter = "/";

    private static CosmosClient Client { get; set; }

    static CosmosDbTriggerFunction()
    {
        var connectionString = Environment.GetEnvironmentVariable("CosmosDb");

        Client = new CosmosClient(connectionString);
    }

    /// <summary>
    /// Syncs the counts by status.
    /// </summary>
    /// <param name="input">The input documents.</param>
    /// <param name="log">The logger.</param>
    [FunctionName("SyncCountsByStatus")]
    public static async Task SyncCountsByStatus(
        [CosmosDBTrigger(
            databaseName: "Catalog",
            collectionName: "Products",
            ConnectionStringSetting = "CosmosDb",
            LeaseCollectionName = "leases",
            CreateLeaseCollectionIfNotExists = true)]
        IReadOnlyList<Document> input,
        ILogger log)
    {
        var container = Client.GetDatabase("Catalog").GetContainer("Products");

        foreach (var document in input)
        {
            // Check to ignore the rest changes that are not related to products.
            var type = document.GetPropertyValue<string>("Type");
            if (type != typeof(Product).AssemblyQualifiedName)
            {
                log.LogWarning("Skipping document with type {documentType}", type);

                continue;
            }

            var product = JsonConvert.DeserializeObject<Product>(document.ToString());

            // Dummy check to simulate a real-world rules where we only want to process document with certain conditions.
            if (product.Status is not ProductStatus.New and not ProductStatus.Open and not ProductStatus.Closed)
            {
                log.LogInformation("Skipping product with status {productStatus}", product.Status);

                continue;
            }

            try
            {
                var counterItemResponse = await container
                    .ReadItemAsync<Counter>(product.Status.ToString(), new PartitionKey(product.Status.ToString()));
                
                var counter = counterItemResponse.Resource;

                var incrementedCount = counter.Count + 1;

                log.LogInformation("Patching counter for status {productStatus}: {incrementedCount}", product.Status, incrementedCount);

                var patchResponse = await container.PatchItemAsync<Counter>(
                    counter.Id,
                    new PartitionKey(counter.Id),
                    patchOperations: new[] { PatchOperation.Set(PropertyDelimiter + nameof(Counter.Count), incrementedCount) });

                log.LogInformation("Patching counter: {requestCharge} RUs", patchResponse.RequestCharge);
            }
            catch (CosmosException ex) when (ex.StatusCode is HttpStatusCode.NotFound)
            {
                log.LogInformation("Creating counter for status {productStatus}", product.Status);

                var createResponse = await container.CreateItemAsync(new Counter
                {
                    Id = product.Status.ToString(),
                    Count = 1,
                    Type = typeof(Counter).AssemblyQualifiedName,
                });

                log.LogInformation("Creating counter: {requestCharge} RUs", createResponse.RequestCharge);
            }
            catch (Exception ex)
            {
                log.LogError(ex, "Exception during SyncCountsByStatus execution");
            }
        }
    }
}
