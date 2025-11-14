using IntegrationTests.Fixtures;
namespace BddTests.Helpers;
public static class BddTestContext
{
    private static ApiWebApplicationFactory? _factory;
    private static HttpClient? _client;
    public static ApiWebApplicationFactory Factory
    {
        get
        {
            if (_factory == null)
            {
                _factory = new ApiWebApplicationFactory();
            }
            return _factory;
        }
    }
    public static HttpClient Client
    {
        get
        {
            if (_client == null)
            {
                _client = Factory.CreateClient();
            }
            return _client;
        }
    }
    public static void Reset()
    {
        _client?.Dispose();
        _factory?.Dispose();
        _client = null;
        _factory = null;
    }
}
