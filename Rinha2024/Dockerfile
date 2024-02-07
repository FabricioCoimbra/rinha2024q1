#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
# Install clang/zlib1g-dev dependencies for publishing to native
RUN apt-get remove
RUN wget https://packages.microsoft.com/config/ubuntu/22.04/packages-microsoft-prod.deb -O packages-microsoft-prod.deb
RUN dpkg -i packages-microsoft-prod.deb
RUN rm packages-microsoft-prod.deb

RUN apt-get update \
    && apt-get install -y --no-install-recommends \
    clang zlib1g-dev

RUN apt-get install -y dotnet-host
RUN apt-get install -y dotnet-sdk-8.0

ARG BUILD_CONFIGURATION=Release

# Install clang/zlib1g-dev dependencies for publishing to native
RUN apt-get remove
RUN wget https://packages.microsoft.com/config/ubuntu/22.04/packages-microsoft-prod.deb -O packages-microsoft-prod.deb
RUN dpkg -i packages-microsoft-prod.deb
RUN rm packages-microsoft-prod.deb

RUN apt-get update \
    && apt-get install -y --no-install-recommends \
    clang zlib1g-dev

RUN apt-get install -y dotnet-host
RUN apt-get install -y dotnet-sdk-8.0
WORKDIR /src
COPY ["Rinha2024.csproj", "."]
RUN dotnet restore "./././Rinha2024.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "./Rinha2024.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Rinha2024.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=true

FROM mcr.microsoft.com/dotnet/runtime-deps:8.0 AS final
WORKDIR /app
EXPOSE 8080
COPY --from=publish /app/publish .
ENTRYPOINT ["./Rinha2024"]