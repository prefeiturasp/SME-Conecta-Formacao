FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["SME.Conecta.Worker.Rabbbit/SME.Conecta.Worker.Rabbbit.csproj", "SME.Conecta.Worker.Rabbbit/"]
RUN dotnet restore "SME.Conecta.Worker.Rabbbit/SME.Conecta.Worker.Rabbbit.csproj"

COPY . .
WORKDIR "/src/SME.Conecta.Worker.Rabbbit"
RUN dotnet build "SME.Conecta.Worker.Rabbbit.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "SME.Conecta.Worker.Rabbbit.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
ENV TZ=America/Sao_Paulo
RUN apt-get update \
    && apt-get install -yq tzdata locales -y \
    && dpkg-reconfigure --frontend noninteractive tzdata \
	  && locale-gen en_US.UTF-8
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SME.Conecta.Worker.Rabbbit.dll"]
