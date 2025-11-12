FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY ["SME.ConectaFormacao.Webapi/SME.ConectaFormacao.Webapi.csproj", "SME.ConectaFormacao.Webapi/"]
RUN dotnet restore "SME.ConectaFormacao.Webapi/SME.ConectaFormacao.Webapi.csproj"

COPY . .
WORKDIR "/src/SME.ConectaFormacao.Webapi"
RUN dotnet build "SME.ConectaFormacao.Webapi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "SME.ConectaFormacao.Webapi.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
ENV TZ=America/Sao_Paulo
RUN apt-get update \
    && apt-get install -yq tzdata locales -y \
    && dpkg-reconfigure --frontend noninteractive tzdata \
	  && locale-gen en_US.UTF-8
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SME.ConectaFormacao.Webapi.dll"]
