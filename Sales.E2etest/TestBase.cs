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

        foreach (var category in TestContext.CurrentContext.Test.Properties["Category"])
        {
            if (category != null)
            {
                Test.AssignCategory(category.ToString()!);
            }
        }
    }

    [TearDown]
    public void BaseTearDown()
    {
        var result = TestContext.CurrentContext.Result;

        try
        {
            var screenshot = ((ITakesScreenshot)Driver).GetScreenshot();
            var path = SaveScreenshotToDisk(screenshot);
            var relativePath = GetRelativeScreenshotPath(path);

            Test.AddScreenCaptureFromPath(relativePath);

            Console.WriteLine($"Screenshot saved: {path}");
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

    private string SaveScreenshotToDisk(Screenshot screenshot)
    {
        var screenshotsDirectory = Path.Combine(
            ReportPathHelper.GetReportsDirectory(),
            "Screenshots");

        Directory.CreateDirectory(screenshotsDirectory);

        var fileName =
            $"{TestContext.CurrentContext.Test.Name}_{DateTime.Now:yyyyMMdd_HHmmss}.png";

        var path = Path.Combine(
            screenshotsDirectory,
            fileName);

        screenshot.SaveAsFile(path);

        return path;
    }

    private string GetRelativeScreenshotPath(string absolutePath)
    {
        var fileName = Path.GetFileName(absolutePath);
        return Path.Combine("Screenshots", fileName);
    }
}
