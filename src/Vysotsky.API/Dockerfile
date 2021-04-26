FROM mcr.microsoft.com/dotnet/sdk:5.0 as build

COPY *.sln .

WORKDIR /src

COPY */*.csproj ./**
COPY */*.csproj ./**
RUN dotnet restore

# copy everything else and build app
COPY */** ./**
WORKDIR /src/Vysotsky.API
RUN dotnet publish -c Release -o /app --no-restore

# final stage/image
FROM mcr.microsoft.com/dotnet/aspnet:5.0
WORKDIR /app
COPY --from=build /app ./

ENV DOTNET_ENVIRONMENT="Production"
ENV PG_CONNECTION_STRING="User ID=postgres;Password=postgres;Host=localhost;Port=5432;Database=vysotsky"
ENV SALT="ZWFzeVNhbHQ="
ENV SECRET="FZXKVt98zxy4VYMVI5uEBIV3lDNKWp4yxesZnKxHEKX8ZYwtB0ss6bdPpEFCCcWnNwWShmuoJH4O76ByZ1k4XdIsW22yDeDmaSKPtGKyVcCieNjXhVh47MzTg0pPNCEo"


ENTRYPOINT ["dotnet", "Vysotsky.API.dll"]
