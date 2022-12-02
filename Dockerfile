FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build_server
COPY ./Server/ .
RUN dotnet restore
RUN dotnet build -c Release --no-restore
RUN dotnet publish -c Release --no-build -o /out

FROM node:14 AS build_client
WORKDIR /build
ARG COMMIT_SHA=none
ARG COMMIT_REF_NAME=none
COPY Client/package.json .
COPY Client/package-lock.json .
RUN npm install -D
COPY ./Client/ .
RUN sed -i -e "s|clientBranch: '.*'|clientBranch: '$COMMIT_REF_NAME'|1" -e "s|clientSha: '.*'|clientSha: '$COMMIT_SHA'|1" apps/SdHub/src/environments/environment*.ts
RUN cat apps/SdHub/src/environments/environment*.ts
RUN npm run-script build -- --output-path /out

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS app
RUN apt update && apt install -y wget unzip
WORKDIR /app
COPY --from=build_server /out /app
COPY --from=build_client /out /app/spa_dist
EXPOSE 80
ENTRYPOINT ["dotnet", "SdHub.dll"]
