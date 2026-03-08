# Build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copia a solução e todos os projetos
COPY ["PaymentsAPI.sln", "."]
COPY ["src/PaymentsAPI.Api/PaymentsAPI.Api.csproj", "src/PaymentsAPI.Api/"]
COPY ["src/PaymentsAPI.Application/PaymentsAPI.Application.csproj", "src/PaymentsAPI.Application/"]
COPY ["src/PaymentsAPI.Authentication/PaymentsAPI.Authentication.csproj", "src/PaymentsAPI.Authentication/"]
COPY ["src/PaymentsAPI.Data/PaymentsAPI.Data.csproj", "src/PaymentsAPI.Data/"]
COPY ["src/PaymentsAPI.Domain/PaymentsAPI.Domain.csproj", "src/PaymentsAPI.Domain/"]
COPY ["src/PaymentsAPI.IoC/PaymentsAPI.IoC.csproj", "src/PaymentsAPI.IoC/"]
COPY ["src/PaymentsAPI.Messaging/PaymentsAPI.Messaging.csproj", "src/PaymentsAPI.Messaging/"]

# Restaura as dependências
RUN dotnet restore "PaymentsAPI.sln"

# Copia o restante do código
COPY . .

# Publica apenas o projeto principal
RUN dotnet publish "src/PaymentsAPI.Api/PaymentsAPI.Api.csproj" -c Release -o /app/publish

# Imagem final
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "PaymentsAPI.Api.dll"]