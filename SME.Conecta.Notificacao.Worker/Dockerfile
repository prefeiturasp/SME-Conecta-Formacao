FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY ["SME.Conecta.Notificacao.Worker/SME.Conecta.Notificacao.Worker.csproj", "SME.Conecta.Notificacao.Worker/"]
COPY ["SME.ConectaFormacao.Infra.Servicos/SME.ConectaFormacao.Infra.Servicos.csproj", "SME.ConectaFormacao.Infra.Servicos/"]
COPY ["SME.ConectaFormacao.Dominio/SME.ConectaFormacao.Dominio.csproj", "SME.ConectaFormacao.Dominio/"]
RUN dotnet restore "SME.Conecta.Notificacao.Worker/SME.Conecta.Notificacao.Worker.csproj"
COPY . .
WORKDIR "/src/SME.Conecta.Notificacao.Worker"
RUN dotnet build "SME.Conecta.Notificacao.Worker.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "SME.Conecta.Notificacao.Worker.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
ENV TZ=America/Sao_Paulo
RUN apt-get update \
    && apt-get install -yq tzdata locales -y \
    && dpkg-reconfigure --frontend noninteractive tzdata \
	  && locale-gen en_US.UTF-8
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SME.Conecta.Notificacao.Worker.dll"]