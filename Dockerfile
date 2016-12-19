FROM microsoft/dotnet

WORKDIR /
COPY . .
RUN dotnet restore

WORKDIR /src/web
RUN dotnet publish -c Release -o out

ENV ASPNETCORE_SERVER.URLS http://*:80
EXPOSE 80

ENTRYPOINT ["dotnet", "out/web.dll"]
