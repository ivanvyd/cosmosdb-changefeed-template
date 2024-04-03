# CosmosDb Change Feed Template

This solution contains a set of Azure Functions that interact with Cosmos DB's Change Feed to sync counts by status. It's designed to demonstrate how to use Azure Functions and Cosmos DB's Change Feed together to process data in real-time.

## Getting Started

### Prerequisites

- .NET 5.0 or later
- Azure Functions Core Tools
- An Azure Cosmos DB account

### Installation

1. Clone the repository to your local machine using `git clone <repository-url>`.
2. Open the solution in Visual Studio.
3. Set up your Cosmos DB connection string in your `local.settings.json` file under the key "CosmosDb" OR put it in the User Secrets on WebApi level.

## Project Structure

The solution is divided into next projects:

- `CosmosDb.ChangeFeed.Template.Application`: The main business logic including implementations of `ICounter` is presented in this project.
- `CosmosDb.ChangeFeed.Template.Domain`: This project contains the domain entities and enums used in the solution. It includes the `Product` and `Counter` entities, as well as the `ProductStatus` enum.
- `CosmosDb.ChangeFeed.Template.Persistence`: The project is responsible for configuration and interaction with Database.
- `CosmosDb.ChangeFeed.Template.WebApi`: The controllers and models for communication are located in this project.

- `CosmosDb.ChangeFeed.Template.Functions`: This project contains the Azure Functions that interact with Cosmos DB's Change Feed. The main function is `SyncCountsByStatus`, which is triggered by changes in the Cosmos DB Change Feed.

## Key Files

- `CosmosDbTriggerFunction.cs`: This file contains the `SyncCountsByStatus` function. This function is triggered by changes in the Cosmos DB Change Feed. For each document in the feed, it checks the product status and updates the corresponding counter in the Cosmos DB.

## Running the Solution

1. Set up the Cosmos DB connection string in `local.settings.json` file under the key "CosmosDb" (or put it in the User Secrets) on `CosmosDb.ChangeFeed.Template.WebApi` level.
2. Set up the Cosmos DB connection string in `local.settings.json` file under the key "CosmosDb" on `CosmosDb.ChangeFeed.Template.Functions` level.
3. Start `CosmosDb.ChangeFeed.Template.WebApi`.
4. Start `CosmosDb.ChangeFeed.Template.Functions`.
5. Press F5 to start debugging. The Azure Functions runtime will start and your function will be ready to process changes from the Cosmos DB Change Feed.
