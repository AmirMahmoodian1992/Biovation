#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["Biovation.Data.Commands/Biovation.Data.Commands.csproj", "Biovation.Data.Commands/"]
RUN dotnet restore "Biovation.Data.Commands/Biovation.Data.Commands.csproj"
COPY . .
WORKDIR "/src/Biovation.Data.Commands"
RUN dotnet build "Biovation.Data.Commands.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Biovation.Data.Commands.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Biovation.Data.Commands.dll"]