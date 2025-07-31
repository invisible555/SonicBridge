# 1. Budowanie aplikacji
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY . .
RUN dotnet restore
RUN dotnet publish -c Release -o /app

# 2. Uruchamianie aplikacji
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
COPY --from=build /app ./
# Domyœlnie ASP.NET s³ucha na 80
EXPOSE 80
ENTRYPOINT ["dotnet", "Backend.dll"]