# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj và restore
COPY Blog.API/*.csproj ./Blog.API/
RUN dotnet restore ./Blog.API/Blog.API.csproj

# Copy toàn bộ source và build
COPY . .
WORKDIR /src/Blog.API
RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

# Mở cổng 5000
EXPOSE 5000

# Đặt URL lắng nghe
ENV ASPNETCORE_URLS=http://+:5000

ENTRYPOINT ["dotnet", "Blog.API.dll"]
