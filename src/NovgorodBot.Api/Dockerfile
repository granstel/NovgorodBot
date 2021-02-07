#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["NovgorodBot.Api/NovgorodBot.Api.csproj", "NovgorodBot.Api/"]
COPY ["NovgorodBot.Messengers.Yandex/NovgorodBot.Messengers.Yandex.csproj", "NovgorodBot.Messengers.Yandex/"]
COPY ["NovgorodBot.Messengers/NovgorodBot.Messengers.csproj", "NovgorodBot.Messengers/"]
COPY ["NovgorodBot.Services/NovgorodBot.Services.csproj", "NovgorodBot.Services/"]
COPY ["NovgorodBot.Models/NovgorodBot.Models.csproj", "NovgorodBot.Models/"]
RUN dotnet restore "NovgorodBot.Api/NovgorodBot.Api.csproj"
COPY . .
WORKDIR "/src/NovgorodBot.Api"
RUN dotnet build "NovgorodBot.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "NovgorodBot.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "NovgorodBot.Api.dll"]