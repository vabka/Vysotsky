FROM mcr.microsoft.com/dotnet/aspnet:5.0-alpine3.12
WORKDIR /app
COPY bin/Debug/net5.0/ ./

ENV COMPlus_EnableDiagnostics=0
ENV DOTNET_ENVIRONMENT="Production"
ENV PG_CONNECTION_STRING="User ID=postgres;Password=postgres;Host=192.168.101.98;Port=5432;Database=vysotsky"
ENV SALT="ZWFzeVNhbHQ="
ENV SECRET="FZXKVt98zxy4VYMVI5uEBIV3lDNKWp4yxesZnKxHEKX8ZYwtB0ss6bdPpEFCCcWnNwWShmuoJH4O76ByZ1k4XdIsW22yDeDmaSKPtGKyVcCieNjXhVh47MzTg0pPNCEo"
ENV ASPNETCORE_URLS="http://+:8080"
EXPOSE 8080
ENTRYPOINT ["dotnet", "Vysotsky.API.dll"]
