FROM microsoft/dotnet

COPY . /app

WORKDIR /app/samples/web
RUN dotnet restore
RUN dotnet publish -c Release -o out

ENV ASPNETCORE_URLS http://*:5080
EXPOSE 5080

ENTRYPOINT ["dotnet", "out/web.dll"]
