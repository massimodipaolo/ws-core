FROM microsoft/dotnet

WORKDIR /app
COPY . .

RUN dotnet restore
RUN dotnet publish -c Release -o out

ENV ASPNETCORE_SERVER.URLS http://*:80
EXPOSE 80

ENTRYPOINT ["dotnet", "out/web.dll"]
