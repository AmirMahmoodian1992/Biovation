#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["Biovation.Brands.PFK/Biovation.Brands.PFK.csproj", "Biovation.Brands.PFK/"]
RUN dotnet restore "Biovation.Brands.PFK/Biovation.Brands.PFK.csproj"
COPY . .
WORKDIR "/src/Biovation.Brands.PFK"
RUN dotnet build "Biovation.Brands.PFK.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Biovation.Brands.PFK.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Biovation.Brands.PFK.dll"]