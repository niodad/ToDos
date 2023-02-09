# Clean Code minimal API using Mediatr and Cosmos DB

## Introduction

This is a  Clean Code minimal API  using Mediatr and Cosmos DB as the database. The API is designed to demonstrate how to write clean, maintainable and scalable code. You can also use the Inmemmory DB with EF 7.

## Requirements

- .NET 7
- Mediatr 11
- Cosmos DB
- AutoMapper 12

## Getting Started

1. Clone the repository:

         git clone https://github.com/niodad/ToDos.git
 
2. Change the directory to the cloned repository:

        cd ToDos

3. Restore the packages:

        dotnet restore

4. Set the Cosmos DB connection string in the appsettings.json file:

        "MongoDBSettings": {
            "ConnectionString": "***",
            "DatabaseName": "***"
        },

5. Run the API:

        dotnet run

6. Use EF Core In-Memory Database

        Make this change in the Extension.cs file
        //services.AddScoped(typeof(IRepository<,>), typeof(CosmosDbRepository<,>));
        //Use EF Core In-Memory Databas
        services.AddScoped(typeof(IRepository<,>), typeof(InmemoryRespository<,>));
        services.AddDbContext<ToDosDbContext>(opt => opt.UseInMemoryDatabase("TodoList"));

## Buy me a coffee or a beer

    If you found this repository helpful, you can buy me a coffee or a beer.

    Bitcoin address: 1795Eztk4vq6YSUA5LYboRH6NRTUNjWkMu

