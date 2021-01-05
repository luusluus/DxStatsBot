FROM mcr.microsoft.com/dotnet/core/runtime:3.0-buster-slim AS base
WORKDIR /app

EXPOSE 8089

FROM mcr.microsoft.com/dotnet/core/sdk:3.0-buster AS build
WORKDIR /src
COPY ["DxStats/DxStats.csproj", "DxStats/"]
RUN dotnet restore "DxStats/DxStats.csproj"
COPY . .
WORKDIR "/src/DxStats"
RUN dotnet build "DxStats.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "DxStats.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DxStats.dll"]