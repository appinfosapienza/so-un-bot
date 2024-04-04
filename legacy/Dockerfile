FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
WORKDIR /App

# Copy everything
COPY ./Bot ./
# Restore as distinct layers
RUN dotnet restore
# Build and publish a release
RUN dotnet publish -c Release -o out

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /App
# Copy the compiled binaries from the build stage container
COPY --from=build-env /App/out .
# Copy all the static data (questions, motd)
COPY ./Data /data
ENTRYPOINT ["dotnet", "SoUnBot.dll"]
