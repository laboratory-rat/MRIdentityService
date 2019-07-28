FROM mcr.microsoft.com/dotnet/core/sdk:2.2 AS build-env
WORKDIR /app
EXPOSE 80
EXPOSE 443

# Copy csproj and restore as distinct layers
COPY *.sln ./
COPY Src/Api/Api.csproj Api/
RUN dotnet restore
COPY . .

# Copy everything else and build
WORKDIR Api
RUN dotnet publish -c Release -o ./out 

# Build runtime image
FROM mcr.microsoft.com/dotnet/core/aspnet:2.2
WORKDIR /app
COPY --from=build-env /app/Api/out .
ENTRYPOINT ["dotnet", "Api.dll"]
