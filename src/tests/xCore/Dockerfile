#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["NuGet.config", "."]
COPY ["src/tests/xCore/xCore.csproj", "src/tests/xCore/"]
COPY ["src/modules/StaticFiles/StaticFiles.csproj", "src/modules/StaticFiles/"]
COPY ["src/modules/ExtensionBase/ExtensionBase.csproj", "src/modules/ExtensionBase/"]
COPY ["src/extCore/Infrastructure/Infrastructure.csproj", "src/extCore/Infrastructure/"]
COPY ["src/modules/Data.FileSystem/Data.FileSystem.csproj", "src/modules/Data.FileSystem/"]
COPY ["src/modules/Data/Data.csproj", "src/modules/Data/"]
COPY ["src/modules/HealthCheck/HealthCheck.csproj", "src/modules/HealthCheck/"]
COPY ["src/modules/Data.EF.SqlServer/Data.EF.SqlServer.csproj", "src/modules/Data.EF.SqlServer/"]
COPY ["src/modules/Data.EF/Data.EF.csproj", "src/modules/Data.EF/"]
COPY ["src/modules/Gateway/Gateway.csproj", "src/modules/Gateway/"]
COPY ["src/modules/Message/Message.csproj", "src/modules/Message/"]
COPY ["src/modules/Diagnostic/Diagnostic.csproj", "src/modules/Diagnostic/"]
COPY ["src/modules/Data.Cache/Data.Cache.csproj", "src/modules/Data.Cache/"]
COPY ["src/core/core.csproj", "src/core/"]
COPY ["src/extCore/Application/Application.csproj", "src/extCore/Application/"]
COPY ["src/modules/Data.EF.MySql/Data.EF.MySql.csproj", "src/modules/Data.EF.MySql/"]
COPY ["src/modules/Api/Api.csproj", "src/modules/Api/"]
COPY ["src/modules/Data.EF.SQLite/Data.EF.SQLite.csproj", "src/modules/Data.EF.SQLite/"]
RUN dotnet restore "src/tests/xCore/xCore.csproj"
COPY . .
WORKDIR "/src/src/tests/xCore"
RUN dotnet build "xCore.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "xCore.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "xCore.dll"]