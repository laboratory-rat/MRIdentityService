FROM mcr.microsoft.com/dotnet/core/aspnet:2.2-stretch-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:2.2-stretch AS build
WORKDIR /src
COPY ["Src/Api/Api.csproj", "Src/Api/"]
COPY ["Service/EmailService/EmailService.csproj", "Service/EmailService/"]
COPY ["Shared/MRPackage/MRPackage.csproj", "Shared/MRPackage/"]
COPY ["Service/TemplateService/TemplateService.csproj", "Service/TemplateService/"]
COPY ["Src/Manager/BLL.csproj", "Src/Manager/"]
COPY ["Src/Repository/DL.csproj", "Src/Repository/"]
COPY ["Src/Infrastructure/Infrastructure.csproj", "Src/Infrastructure/"]
COPY ["Src/Tools/Tools.csproj", "Src/Tools/"]
RUN dotnet restore "Src/Api/Api.csproj"
COPY . .
WORKDIR "/src/Src/Api"
RUN dotnet build "Api.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "Api.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Api.dll"]