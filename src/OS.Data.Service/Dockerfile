FROM microsoft/dotnet:2.0.0-runtime-jessie
ARG source
WORKDIR /app
COPY ${source:-obj/Docker/publish} .
ENTRYPOINT ["dotnet", "OS.Data.Service.dll"]
