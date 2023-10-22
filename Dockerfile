FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build_server
COPY ./Server/ .
ARG COMMIT_SHA=none
ARG COMMIT_REF_NAME=none
RUN dotnet restore
RUN dotnet build -c Release --no-restore
RUN dotnet publish -c Release --no-build -o /out

FROM node:18 AS build_client
WORKDIR /build
ARG COMMIT_SHA=none
ARG COMMIT_REF_NAME=none
COPY Client16/package.json .
COPY Client16/package-lock.json .
RUN npm install
COPY Client16/ .
RUN npx nx run sd-hub:build:production --output-path /out

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS app
RUN apt update && apt install -y wget unzip cabextract wget xfonts-utils curl \
    && curl -s -o ttf-mscorefonts-installer_3.7_all.deb http://ftp.us.debian.org/debian/pool/contrib/m/msttcorefonts/ttf-mscorefonts-installer_3.7_all.deb \
    && sh -c "echo ttf-mscorefonts-installer msttcorefonts/accepted-mscorefonts-eula select true | debconf-set-selections" \
    && dpkg -i ttf-mscorefonts-installer_3.7_all.deb
WORKDIR /app
ARG COMMIT_SHA=none
ARG COMMIT_REF_NAME=none
ENV ApplicationSettings__AppInfo__GitCommitSha=${COMMIT_SHA}
ENV ApplicationSettings__AppInfo__GitRefName=${COMMIT_REF_NAME}
COPY --from=build_server /out /app
COPY --from=build_client /out /app/spa_dist
EXPOSE 80
ENTRYPOINT ["dotnet", "SdHub.dll"]
