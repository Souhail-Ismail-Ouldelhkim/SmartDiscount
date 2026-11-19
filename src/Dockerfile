ARG DOTNET_VERSION=10.0

FROM mcr.microsoft.com/dotnet/sdk:${DOTNET_VERSION} AS build
ARG SERVICE
WORKDIR /src
COPY . .
RUN dotnet publish "${SERVICE}/${SERVICE}.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:${DOTNET_VERSION} AS final
ARG SERVICE
ENV APP_DLL=${SERVICE}.dll
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 8080
ENTRYPOINT ["/bin/sh", "-c", "exec dotnet \"$APP_DLL\""]