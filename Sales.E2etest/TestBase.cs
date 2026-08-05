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

        try
        {
            var screenshot = TakeScreenshot();

            Test.AddScreenCaptureFromPath(screenshot);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Could not capture screenshot. {ex.Message}");
            Test.Warning($"Could not capture screenshot. {ex.Message}");
        }

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
                break;
        }

        Driver.Quit();
        Driver.Dispose();
    }

    private string TakeScreenshot()
    {
        var screenshotsDirectory = Path.Combine(
            Directory.GetCurrentDirectory(),
            "Reports",
            "Screenshots");

        Directory.CreateDirectory(screenshotsDirectory);

        var fileName =
            $"{TestContext.CurrentContext.Test.Name}_{DateTime.Now:yyyyMMdd_HHmmss}.png";

        var absolutePath = Path.Combine(
            screenshotsDirectory,
            fileName);

        var screenshot = ((ITakesScreenshot)Driver).GetScreenshot();

        screenshot.SaveAsFile(absolutePath);

        var relativePath = Path.Combine(
            "Screenshots",
            fileName);

        Console.WriteLine($"Screenshot saved: {absolutePath}");

        return relativePath;
    }
}
