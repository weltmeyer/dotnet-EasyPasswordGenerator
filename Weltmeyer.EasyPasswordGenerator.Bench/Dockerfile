FROM mcr.microsoft.com/dotnet/runtime:7.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["Weltmeyer.EasyPasswordGenerator.Bench/Weltmeyer.EasyPasswordGenerator.Bench.csproj", "Weltmeyer.EasyPasswordGenerator.Bench/"]
RUN dotnet restore "Weltmeyer.EasyPasswordGenerator.Bench/Weltmeyer.EasyPasswordGenerator.Bench.csproj"
COPY . .
WORKDIR "/src/Weltmeyer.EasyPasswordGenerator.Bench"
RUN dotnet build "Weltmeyer.EasyPasswordGenerator.Bench.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Weltmeyer.EasyPasswordGenerator.Bench.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Weltmeyer.EasyPasswordGenerator.Bench.dll"]
