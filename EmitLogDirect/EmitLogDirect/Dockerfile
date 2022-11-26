FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["EmitLogDirect/EmitLogDirect.csproj", "EmitLogDirect/"]
RUN dotnet restore "EmitLogDirect/EmitLogDirect.csproj"
COPY . .
WORKDIR "/src/EmitLogDirect"
RUN dotnet build "EmitLogDirect.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "EmitLogDirect.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "EmitLogDirect.dll"]
