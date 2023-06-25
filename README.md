# datingApp

Tinder-like dating app created for university assingment.

## Note

I used DevMentors' [Solid WebAPI](https://platform.devmentors.io/courses/solid-web-api) course (recommend!) as a reference. Thus some code, as well as overall project structure, may be similar to what is presented in the course.

## How to run

Clone repository. Navigate to repository folder in console and run the following command to run Postgres container:
`docker compose up -d`

Next, navigate to ./src/datingApp.Api and run the following command to run application:
`dotner run`

If no success, try to run the follwing command to restore project's dependencies:
`dotnet restore`

Alternatively, open datingApp.sln solution in Visual Studio and run solution from there.

## How to use

TODO

## What's interesting in this project?

- Entity Framework Core with relational database
- CQRS pattern
- onion architecture: project divided into four layers: Core, Application, Infrastructure and Api. Layers communicate with each other using well defined interfaces. Layers can only use layers below them, eg. Application can use Core layer, Api can use Core, Application and Infrastructure layers, etc.
- JWT token based authentication
- sign in with access code: request access code -> access code sent by email -> sign in with given access code

## What's left to do?

- [ ] authorization
- [ ] more & better tests (API tests, etc.)
- [ ] use Redis to store JWT tokens and access codes
- [ ] fix endpoints, eg. split PATCH users into three endpoints: change user profile info, user settings and user location
