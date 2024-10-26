# Use official .NET image as a build stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Development

# Build Stage
FROM mcr.microsoft.com/dotnet/sdk:8.0@sha256:35792ea4ad1db051981f62b313f1be3b46b1f45cadbaa3c288cd0d3056eefb83 AS build-env
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
# Copy everything
COPY . ./IMC
# Restore as distinct layers
RUN dotnet restore "IMC/IMC_CC_App.csproj"
WORKDIR /src/IMC
# Build as distinct layer
RUN dotnet build "IMC_CC_App.csproj" -c Release -o /app/build

# Publish a release
FROM build-env AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "IMC_CC_App.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Build runtime image
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENV ASPNETCORE_FORWARDEDHEADERS_ENABLED=true
ENTRYPOINT ["dotnet", "IMC_CC_App.dll"]