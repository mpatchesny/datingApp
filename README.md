# datingApp

Tinder-like dating app created for university assingment and developed further for my portfolio.

## Note

I used DevMentors' [Solid WebAPI](https://platform.devmentors.io/courses/solid-web-api) course (recommend!) as a reference. Thus some code, as well as overall project structure, may be similar to what is presented in the course.

## How to run

### Using dotnet CLI

Navigate to the repository folder in terminal and execute the following command to run Postgres container:\
`docker compose -f ./deployments/development/docker-compose.yml -p datingapp up -d`

Next, execute the following command in terminal to restore project's dependencies:\
`dotnet restore`

Next, navigate to the ./src/datingApp.Api and execute the following command to run application:\
`dotnet run`

You may need to generate certificates, you can do so by running the following command:\
`dotnet dev-certs https`

Note: storage folder (application.json -> Storage -> StoragePath) must exist on the host machine, and the system user who runs the application must have read/write permissions on this folder.

### Using Visual Studio

Open datingApp.sln solution in Visual Studio and run solution from it.

Note: storage folder (application.json -> Storage -> StoragePath) must exist on the host machine, and the system user who runs the application must have read/write permissions on this folder.

### Using docker

Navigate to the repository folder in terminal and execute the following command to build docker image:\
`docker build -t datingapp-api .`

Next, execute the following command in terminal to run application:\
`docker compose -f ./deployments/docker/docker-compose.yml -p datingapp up -d`

## How to use

Please see [REST.http](REST.http) for examples of API calls. You must have [REST Client](https://marketplace.visualstudio.com/items?itemName=humao.rest-client) VS Code extension installed in order to run API calls from this file.

## What's interesting in this project?

- Entity Framework Core with relational database
- CQRS pattern
- clean/onion architecture: project divided into four layers: Core, Application, Infrastructure and Api. Layers communicate with each other using well defined interfaces. Layers can only use layers below them, eg. Application can use Core layer, Api can use Core, Application and Infrastructure layers, etc.
- JWT based authentication
- sign in with access code: request access code -> access code sent by email -> sign in with given access code

## What's left to do?

[See issues](https://github.com/mpatchesny/datingApp/issues)
