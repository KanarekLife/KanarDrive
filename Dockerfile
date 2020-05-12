FROM mcr.microsoft.com/dotnet/core/sdk:3.1 as build-env
WORKDIR /app
COPY . ./
RUN dotnet restore KanarSite.sln
RUN cd KanarSite.App && curl -sL https://deb.nodesource.com/setup_12.x | bash - && apt-get install nodejs -y && npm install && npm install --global gulp-cli && gulp
RUN dotnet publish -c Release -o out KanarSite.sln

# Build runtime image
FROM mcr.microsoft.com/dotnet/core/aspnet:3.1
WORKDIR /app
COPY --from=build-env /app/out .
ENTRYPOINT ["dotnet", "KanarSite.App.dll"]