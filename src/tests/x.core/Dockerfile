#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /
COPY ["NuGet.config", "."]
COPY ["src/tests/x.core/x.core.csproj", "src/tests/x.core/"]
COPY ["src/core/core.csproj", "src/core/"]
COPY ["src/extCore/*/*.csproj", "src/extCore/*/"]
COPY ["src/modules/*/*.csproj", "src/modules/*/"]
RUN dotnet restore "src/tests/x.core/x.core.csproj"
COPY . .
WORKDIR "/src/tests/x.core"
RUN dotnet build "x.core.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "x.core.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
COPY ["wait-for-it.sh", "."]
ENTRYPOINT ["dotnet", "x.core.dll"]