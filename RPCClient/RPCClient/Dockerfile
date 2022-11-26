FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["RPCClient/RPCClient.csproj", "RPCClient/"]
RUN dotnet restore "RPCClient/RPCClient.csproj"
COPY . .
WORKDIR "/src/RPCClient"
RUN dotnet build "RPCClient.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "RPCClient.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "RPCClient.dll"]
