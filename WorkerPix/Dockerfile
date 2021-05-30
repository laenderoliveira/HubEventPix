#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:5.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["WorkerPix/WorkerPix.csproj", "WorkerPix/"]
RUN dotnet restore "WorkerPix/WorkerPix.csproj"
COPY . .
WORKDIR "/src/WorkerPix"
RUN dotnet build "WorkerPix.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "WorkerPix.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "WorkerPix.dll"]