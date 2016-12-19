FROM microsoft/dotnet

WORKDIR /src/web
COPY project.json
RUN dotnet restore

COPY . .
RUN dotnet publish -c Release -o out

ENV ASPNETCORE_SERVER.URLS http://*:80
EXPOSE 80

ENTRYPOINT ["dotnet", "out/web.dll"]