FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY Frontend/Frontend.csproj Frontend/
RUN dotnet restore Frontend/Frontend.csproj

COPY Frontend/ Frontend/
RUN dotnet publish Frontend/Frontend.csproj -c Release -o /app/publish --no-restore

FROM nginx:alpine AS final
COPY --from=build /app/publish/wwwroot /usr/share/nginx/html
COPY Frontend/nginx.conf /etc/nginx/conf.d/default.conf

EXPOSE 80
