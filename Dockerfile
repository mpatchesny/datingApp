FROM mcr.microsoft.com/dotnet/sdk:6.0 AS builder
WORKDIR /app

COPY ./src ./

RUN dotnet restore ./datingApp.Api/datingApp.Api.csproj
RUN dotnet build   ./datingApp.Api/datingApp.Api.csproj -c Release 
RUN dotnet publish ./datingApp.Api/datingApp.Api.csproj -c Release --no-build -o out

FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /app

COPY --from=builder /app/out .

# Set the environment variable for Storage Path & create the storage directory
ARG STORAGE_PATH=/app/storage
ENV ASPNETCORE_Storage__StoragePath=$STORAGE_PATH
RUN mkdir -p $STORAGE_PATH && chmod -R 666 $STORAGE_PATH

ENV ASPNETCORE_URLS https://*:8443, http://*:5000
ENV ASPNETCORE_ENVIRONMENT=Docker

EXPOSE 5000
EXPOSE 8443

ENTRYPOINT ["dotnet", "datingApp.Api.dll"]