FROM node:20-alpine AS npm
COPY /Blazor/package.json .
RUN npm install

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG CACHE_BUSTER
RUN echo "${CACHE_BUSTER}"
WORKDIR src

COPY ./Blazor ./Blazor
COPY ./Shared ./Shared
WORKDIR Blazor
RUN dotnet publish -c Release -o app Blazor.csproj

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
ARG CACHE_BUSTER
RUN echo "${CACHE_BUSTER}"
WORKDIR app
COPY --from=build ./src/Blazor/app .
COPY --from=npm node_modules/@microsoft/signalr/dist/browser/signalr.min.js ./wwwroot/Packages/signalr/signalr.min.js
COPY --from=npm node_modules/pixi.js/dist/pixi.min.js ./wwwroot/Packages/pixijs/pixi.min.js

RUN chmod -R 755 ./wwwroot

ENTRYPOINT ["dotnet", "Blazor.dll"]
