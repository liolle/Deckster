FROM node:20-alpine AS npm
COPY /Blazor/package.json .
RUN npm install

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG CACHE_BUSTER
WORKDIR /src

COPY ./Blazor ./Blazor
COPY ./Shared ./Shared
WORKDIR /src/Blazor
RUN dotnet publish -c Release -o /app

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
ARG CACHE_BUSTER
WORKDIR /app

COPY --from=build /app .
COPY --from=build /src/Blazor/wwwroot/Assets ./wwwroot
COPY --from=build /src/Blazor/wwwroot/Packages ./wwwroot
COPY --from=build /src/Blazor/wwwroot/Js ./wwwroot
COPY --from=build /src/Blazor/wwwroot/app.css ./wwwroot
COPY --from=npm node_modules/@microsoft/signalr/dist/browser/signalr.min.js ./wwwroot/Packages/signalr

RUN chmod -R 755 ./wwwroot

ENTRYPOINT ["dotnet", "Blazor.dll"]
