FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["EmitLogTopic/EmitLogTopic.csproj", "EmitLogTopic/"]
RUN dotnet restore "EmitLogTopic/EmitLogTopic.csproj"
COPY . .
WORKDIR "/src/EmitLogTopic"
RUN dotnet build "EmitLogTopic.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "EmitLogTopic.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "EmitLogTopic.dll"]
