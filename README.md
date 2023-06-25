# datingApp
Tinder-like dating app created for university assingment

## Note
I used DevMentors' [Solid WebAPI](https://platform.devmentors.io/courses/solid-web-api) course (recommend!) as a reference. Thus some code, as well as overall project structure, may be similar to what is presented in the course.

## How to run
Clone repository. Navigate to repository folder in console and run the following command to run Postgres container:
`docker compose up -d`

Next, navigate to ./src/datingApp.Api and run the following command to run application:
`dotner run`

If no success, try to run follwing command to restore project's dependencies.
`dotnet restore`

Alternatively, open datingApp.sln solution in Visual Studio and run solution from there.
