FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["src/Api/Api.csproj", "src/Api/"]
COPY ["src/Application/Application.csproj", "src/Application/"]
COPY ["src/Domain/Domain.csproj", "src/Domain/"]
COPY ["src/Infra/Infra.csproj", "src/Infra/"]
COPY ["src/Infra.IoC/Infra.IoC.csproj", "src/Infra.IoC/"]
COPY ["tests/IntegrationTests/IntegrationTests.csproj", "tests/IntegrationTests/"]

RUN dotnet restore "tests/IntegrationTests/IntegrationTests.csproj"

COPY src/ src/
COPY tests/IntegrationTests/ tests/IntegrationTests/

WORKDIR "/src/tests/IntegrationTests"
RUN dotnet build "IntegrationTests.csproj" -c Release -o /app/build

FROM build AS test
WORKDIR /src/tests/IntegrationTests
ENTRYPOINT ["dotnet", "test", "IntegrationTests.csproj", "--logger", "trx", "--results-directory", "/test-results"]

