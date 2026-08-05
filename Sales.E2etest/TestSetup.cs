using AventStack.ExtentReports;
using AventStack.ExtentReports.Reporter;
using NUnit.Framework;

namespace Sales.E2etest;

[SetUpFixture]
public class TestSetup
{
    public static ExtentReports Extent = null!;

    [OneTimeSetUp]
    public void GlobalSetup()
    {
        var reportsDirectory = Path.Combine(
            Directory.GetCurrentDirectory(),
            "Reports");

        Directory.CreateDirectory(reportsDirectory);

        var reportPath = Path.Combine(
            reportsDirectory,
            "TestReport.html");

        var reporter = new ExtentSparkReporter(reportPath);

        Extent = new ExtentReports();
        Extent.AttachReporter(reporter);

        Console.WriteLine($"Report: {reportPath}");
    }

    [OneTimeTearDown]
    public void GlobalTearDown()
    {
        Extent.Flush();
    }
}
