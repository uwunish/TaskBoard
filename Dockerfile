FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy csproj files first for layer caching
COPY TaskBoard.API/TaskBoard.API.csproj TaskBoard.API/
COPY TaskBoard.Application/TaskBoard.Application.csproj TaskBoard.Application/
COPY TaskBoard.Infrastructure/TaskBoard.Infrastructure.csproj TaskBoard.Infrastructure/
COPY TaskBoard.Domain/TaskBoard.Domain.csproj TaskBoard.Domain/

RUN dotnet restore "src/TaskBoard.API/TaskBoard.API.csproj"

# Copy everything else
COPY TaskBoard.API/ TaskBoard.API/
COPY TaskBoard.Application/ TaskBoard.Application/
COPY TaskBoard.Infrastructure/ TaskBoard.Infrastructure/
COPY TaskBoard.Domain/ TaskBoard.Domain/

RUN dotnet publish "TaskBoard.API/TaskBoard.API.csproj" \
    -c Release \
    -o /app/publish \
    --no-restore

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "TaskBoard.API.dll"]