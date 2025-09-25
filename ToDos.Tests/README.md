# ToDos.Tests

Comprehensive test suite for the ToDos API application.

## Test Structure

### Unit Tests
- **Handlers**: Tests for CQRS command and query handlers
- **Repositories**: Tests for repository layer with mocked dependencies
- **Middleware**: Tests for custom middleware components
- **Services**: Tests for application services

### Integration Tests
- **API Endpoints**: End-to-end tests for all API endpoints
- **Authentication**: Tests for API key authentication
- **Error Handling**: Tests for error scenarios and responses

## Test Categories

### Handler Tests
- `SaveToDoCommandHandlerTests` - Tests for creating/updating todos
- `DeleteToDoCommandHandlerTests` - Tests for deleting todos
- `GetToDoByIdQueryHandlerTests` - Tests for retrieving single todo
- `GetToDosQueryHandlerTests` - Tests for retrieving multiple todos

### Middleware Tests
- `ExceptionHandlingMiddlewareTests` - Tests for global exception handling
- `ApiKeyAuthenticationMiddlewareTests` - Tests for API key authentication

### Integration Tests
- `ToDosApiIntegrationTests` - End-to-end API tests

## Running Tests

### Run All Tests
```bash
dotnet test
```

### Run Tests with Coverage
```bash
dotnet test --collect:"XPlat Code Coverage" --settings coverlet.runsettings
```

### Run Specific Test Category
```bash
# Run only unit tests
dotnet test --filter Category=Unit

# Run only integration tests
dotnet test --filter Category=Integration
```

## Test Coverage

The test suite aims for:
- **Line Coverage**: 80%+
- **Branch Coverage**: 80%+
- **Method Coverage**: 80%+

Coverage reports are generated in multiple formats:
- Cobertura (for CI/CD)
- JSON (for tools)
- LCOV (for IDEs)

## Test Data

Test data is provided by `TestDataHelper` class:
- `CreateTestToDo()` - Creates a single test todo
- `CreateTestToDoList()` - Creates a list of test todos

## Mocking

Tests use Moq for mocking dependencies:
- Repository interfaces
- Logger interfaces
- Configuration services
- Authentication services

## Assertions

Tests use FluentAssertions for readable assertions:
- `Should().NotBeNull()`
- `Should().BeEquivalentTo()`
- `Should().HaveCount()`
- `Should().Contain()`

## Best Practices

1. **Arrange-Act-Assert** pattern
2. **Descriptive test names** that explain the scenario
3. **One assertion per test** when possible
4. **Mock external dependencies** completely
5. **Test both success and failure scenarios**
6. **Use meaningful test data**

## Continuous Integration

Tests are designed to run in CI/CD pipelines:
- No external dependencies
- Deterministic results
- Fast execution
- Clear failure messages

## Troubleshooting

### Common Issues

1. **File Lock Errors**: Stop any running API processes before running tests
2. **Missing Dependencies**: Run `dotnet restore` before tests
3. **Configuration Issues**: Ensure test configuration is properly set up

### Debugging Tests

```bash
# Run tests with detailed output
dotnet test --verbosity detailed

# Run specific test with debugger
dotnet test --filter "TestName" --logger "console;verbosity=detailed"
```

## Adding New Tests

When adding new functionality:

1. **Create unit tests** for business logic
2. **Create integration tests** for API endpoints
3. **Update test data helpers** if needed
4. **Ensure test coverage** meets requirements
5. **Document test scenarios** in test names

## Test Environment

Tests run in isolation with:
- In-memory test host
- Mocked external dependencies
- Test-specific configuration
- Clean state for each test
