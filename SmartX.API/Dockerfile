# ============================================================
# Build stage
# ============================================================
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["SmartX.API.csproj", "./"]
RUN dotnet restore "SmartX.API.csproj"

COPY . .
RUN dotnet publish "SmartX.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

# ============================================================
# Runtime stage
# ============================================================
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:5001
ENV ASPNETCORE_ENVIRONMENT=Production

EXPOSE 5001

ENTRYPOINT ["dotnet", "SmartX.API.dll"]