FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["src/Domain/Domain.csproj", "src/Domain/"]
COPY ["src/Application/Application.csproj", "src/Application/"]
COPY ["tests/UnitTests/UnitTests.csproj", "tests/UnitTests/"]

RUN dotnet restore "tests/UnitTests/UnitTests.csproj"

COPY src/Domain/ src/Domain/
COPY src/Application/ src/Application/
COPY tests/UnitTests/ tests/UnitTests/

WORKDIR "/src/tests/UnitTests"
RUN dotnet build "UnitTests.csproj" -c Release -o /app/build

FROM build AS test
WORKDIR /src/tests/UnitTests
ENTRYPOINT ["dotnet", "test", "UnitTests.csproj", "--logger", "trx", "--results-directory", "/test-results"]

