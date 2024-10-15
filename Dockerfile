FROM mcr.microsoft.com/dotnet/sdk:6.0 AS builder
WORKDIR /app

COPY ./src ./

RUN dotnet restore ./datingApp.Api/datingApp.Api.csproj
RUN dotnet build   ./datingApp.Api/datingApp.Api.csproj -c Release 
RUN dotnet publish ./datingApp.Api/datingApp.Api.csproj -c Release --no-build -o out

FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /app

COPY --from=builder /app/out .

# Make storage path
ARG STORAGE_PATH=/app/storage
RUN mkdir -p $STORAGE_PATH && chmod -R 666 $STORAGE_PATH

# Certificates
ARG CERTS_PATH=/app/https
ARG PASSWORD_ENV_SEED=test
ENV ASPNETCORE_Kestrel__Certificates__Default__Password=$PASSWORD_ENV_SEED
RUN mkdir -p $CERTS_PATH && chmod -R 666 $CERTS_PATH

## Generate self-signed certificate
RUN openssl req -x509 -nodes -days 365 -newkey rsa:2048 -keyout $CERTS_PATH/aspnetapp.key -out $CERTS_PATH/aspnetapp.crt -subj "/CN=localhost"
## Combine key and certificate to create .pfx file
RUN openssl pkcs12 -export -out $CERTS_PATH/aspnetapp.pfx -inkey $CERTS_PATH/aspnetapp.key -in $CERTS_PATH/aspnetapp.crt -password pass:$PASSWORD_ENV_SEED

ENV ASPNETCORE_URLS=http://+:5000;https://+:8443
ENV ASPNETCORE_ENVIRONMENT=Docker

EXPOSE 5000
EXPOSE 8443

ENTRYPOINT ["dotnet", "datingApp.Api.dll"]