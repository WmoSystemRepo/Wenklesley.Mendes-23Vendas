FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["src/Api/Api.csproj", "src/Api/"]
COPY ["src/Application/Application.csproj", "src/Application/"]
COPY ["src/Domain/Domain.csproj", "src/Domain/"]
COPY ["src/Infra/Infra.csproj", "src/Infra/"]
COPY ["src/Infra.IoC/Infra.IoC.csproj", "src/Infra.IoC/"]
COPY ["tests/IntegrationTests/IntegrationTests.csproj", "tests/IntegrationTests/"]
COPY ["tests/BddTests/BddTests.csproj", "tests/BddTests/"]

RUN dotnet restore "tests/BddTests/BddTests.csproj"

COPY src/ src/
COPY tests/IntegrationTests/ tests/IntegrationTests/
COPY tests/BddTests/ tests/BddTests/

WORKDIR "/src/tests/BddTests"
RUN dotnet build "BddTests.csproj" -c Release -o /app/build

FROM build AS test
WORKDIR /src/tests/BddTests
ENTRYPOINT ["dotnet", "test", "BddTests.csproj", "--logger", "trx", "--results-directory", "/test-results"]

