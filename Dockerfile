# -----------------------------------------------------
# STAGE 1: Build
# -----------------------------------------------------
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /src

COPY *.csproj ./
RUN dotnet restore

COPY . .
RUN dotnet publish -c Release -o /app/publish



# -----------------------------------------------------
# STAGE 2: Runtime + wkhtmltopdf STATIC
# -----------------------------------------------------
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final

WORKDIR /app

# Dependencias necesarias + wget
RUN apt-get update && apt-get install -y \
    wget \
    xfonts-base \
    xfonts-75dpi \
    fontconfig \
    libxrender1 \
    libxext6 \
    libssl3 \
    && rm -rf /var/lib/apt/lists/*

# Instalar wkhtmltopdf 0.12.6 para Debian Bookworm (base de aspnet:8.0)
RUN wget https://github.com/wkhtmltopdf/packaging/releases/download/0.12.6.1-3/wkhtmltox_0.12.6.1-3.bookworm_amd64.deb \
    && apt-get update \
    && apt install -y ./wkhtmltox_0.12.6.1-3.bookworm_amd64.deb \
    && rm wkhtmltox_0.12.6.1-3.bookworm_amd64.deb \
    && rm -rf /var/lib/apt/lists/*
COPY --from=build /app/publish .

EXPOSE 8080

ENTRYPOINT ["dotnet", "AnimeWeb_App.dll"]
