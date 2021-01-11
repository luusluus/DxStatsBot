FROM mcr.microsoft.com/dotnet/core/aspnet:3.0-buster-slim AS base
WORKDIR /app

EXPOSE 8090

FROM mcr.microsoft.com/dotnet/core/sdk:3.0-buster AS build
WORKDIR /src
COPY ["DXStats/DXStats.csproj", "DXStats/"]
RUN dotnet restore "DXStats/DXStats.csproj"
COPY . .
WORKDIR "/src/DXStats"
RUN dotnet build "DXStats.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "DXStats.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DXStats.dll"]