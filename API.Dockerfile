FROM mcr.microsoft.com/dotnet/sdk:8.0 AS sdk

FROM sdk AS build
WORKDIR src
COPY ./Domain ./Domain
ARG CACHE_BUSTER
COPY ./API ./API

WORKDIR API
RUN dotnet publish -c release -o app API.csproj 

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
ARG CACHE_BUSTER
WORKDIR /
COPY --from=build ./src/API/app .
ENTRYPOINT ["dotnet", "API.dll"]
