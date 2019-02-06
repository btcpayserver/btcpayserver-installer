FROM microsoft/dotnet:sdk AS build-env

RUN apt-get update -yq && apt-get upgrade -yq && apt-get install -yq curl git nano
RUN curl -sL https://deb.nodesource.com/setup_8.x | bash - && apt-get install -yq nodejs build-essential
RUN npm install -g npm

WORKDIR /app

# Copy csproj and restore as distinct layers
COPY BTCPayServer.Installer/*.csproj ./
RUN dotnet restore
# Copy everything else and build
COPY ./BTCPayServer.Installer ./

RUN cd ClientApp && npm i && npm rebuild node-sass && npm run build
RUN dotnet publish -c Release -o out

# Build runtime image
FROM microsoft/dotnet:aspnetcore-runtime
WORKDIR /app
COPY --from=build-env /app/out .
COPY --from=build-env /app/ClientApp/dist ./ClientApp/dist
ENTRYPOINT ["dotnet", "BTCPayServer.Installer.dll"]