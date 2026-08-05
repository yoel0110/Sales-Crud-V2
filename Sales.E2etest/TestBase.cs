using AventStack.ExtentReports;
using NUnit.Framework;
using NUnit.Framework.Interfaces;
using OpenQA.Selenium;

namespace Sales.E2etest;

public abstract class TestBase
{
    protected ExtentTest Test = null!;
    protected abstract IWebDriver Driver { get; }

    [SetUp]
    public void BaseSetUp()
    {
        Test = TestSetup.Extent.CreateTest(
            TestContext.CurrentContext.Test.Name);
    }

    [TearDown]
    public void BaseTearDown()
    {
        var result = TestContext.CurrentContext.Result;

        switch (result.Outcome.Status)
        {
            case TestStatus.Passed:
                Test.Pass("Test Passed");
                break;

            case TestStatus.Skipped:
                Test.Skip(result.Message);
                break;

            case TestStatus.Failed:

                Test.Fail(result.Message);

                try
                {
                    var screenshot = TakeScreenshot();

                    Test.AddScreenCaptureFromPath(screenshot);
                }
                catch (Exception ex)
                {
                    Test.Warning($"Could not capture screenshot. {ex.Message}");
                }

                break;
        }
    }

    private string TakeScreenshot()
    {
        Directory.CreateDirectory("Reports/Screenshots");

        var fileName =
            $"{TestContext.CurrentContext.Test.Name}_{DateTime.Now:yyyyMMdd_HHmmss}.png";

        var path = Path.Combine(
            "Reports",
            "Screenshots",
            fileName);

        var screenshot = ((ITakesScreenshot)Driver).GetScreenshot();

        screenshot.SaveAsFile(path);

        return path;
    }
}