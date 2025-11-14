using TechTalk.SpecFlow;
namespace BddTests.Hooks;
[Binding]
public class TestHooks
{
    [BeforeScenario]
    public void BeforeScenario()
    {
    }
    [AfterScenario]
    public void AfterScenario()
    {
    }
}
