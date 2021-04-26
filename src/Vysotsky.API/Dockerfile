FROM mcr.microsoft.com/dotnet/sdk:5.0 as build
WORKDIR /source
COPY *.sln .
COPY ./src ./src
WORKDIR
CMD ls
#RUN dotnet publish -c Release -o /app
#
## final stage/image
#FROM mcr.microsoft.com/dotnet/aspnet:5.0-alpine3.12
#WORKDIR /app
#COPY --from=build /app ./
#
#ENV DOTNET_ENVIRONMENT="Production"
#ENV PG_CONNECTION_STRING="User ID=postgres;Password=postgres;Host=192.168.101.98;Port=5432;Database=vysotsky"
#ENV SALT="ZWFzeVNhbHQ="
#ENV SECRET="FZXKVt98zxy4VYMVI5uEBIV3lDNKWp4yxesZnKxHEKX8ZYwtB0ss6bdPpEFCCcWnNwWShmuoJH4O76ByZ1k4XdIsW22yDeDmaSKPtGKyVcCieNjXhVh47MzTg0pPNCEo"
#
#EXPOSE 3001
#ENTRYPOINT ["dotnet", "Vysotsky.API.dll"]
