FROM microsoft/dotnet:runtime

WORKDIR /app
COPY out .

ENV ASPNETCORE_SERVER.URLS http://*:80
EXPOSE 80

ENTRYPOINT ["dotnet", "web.dll"]