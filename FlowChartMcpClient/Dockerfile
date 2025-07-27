FROM mcr.microsoft.com/dotnet/runtime:9.0 AS base
USER $APP_UID
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["FlowChartMcpClient/FlowChartMcpClient.csproj", "FlowChartMcpClient/"]
RUN dotnet restore "FlowChartMcpClient/FlowChartMcpClient.csproj"
COPY . .
WORKDIR "/src/FlowChartMcpClient"
RUN dotnet build "./FlowChartMcpClient.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./FlowChartMcpClient.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "FlowChartMcpClient.dll"]
