FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# copy csproj files first to leverage layer caching
COPY Origami.API/Origami.API.csproj Origami.API/
COPY Origami.BusinessTier/Origami.BusinessTier.csproj Origami.BusinessTier/
COPY Origami.DataTier/Origami.DataTier.csproj Origami.DataTier/
RUN dotnet restore Origami.API/Origami.API.csproj

# copy the rest of the repository and publish
COPY . .
WORKDIR /src/Origami.API
RUN dotnet publish Origami.API.csproj -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
COPY render-start.sh .
RUN chmod +x render-start.sh
ENTRYPOINT ["./render-start.sh"]

