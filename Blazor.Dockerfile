FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
#ARG CACHE_BUSTER
WORKDIR /src

COPY ./Blazor ./Blazor
WORKDIR /src/Blazor
RUN dotnet publish -c Release -o /app

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
#ARG CACHE_BUSTER
WORKDIR /app

COPY --from=build /app .
COPY --from=build /src/Blazor/wwwroot ./wwwroot/

RUN chmod -R 755 ./wwwroot

ENTRYPOINT ["dotnet", "Blazor.dll"]
