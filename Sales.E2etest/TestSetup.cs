using System.Text.RegularExpressions;
using AventStack.ExtentReports;
using AventStack.ExtentReports.Reporter;
using NUnit.Framework;

namespace Sales.E2etest;

[SetUpFixture]
public class TestSetup
{
    public static ExtentReports Extent = null!;
    private static string _reportPath = null!;

    [OneTimeSetUp]
    public void GlobalSetup()
    {
        var reportsDirectory = Path.Combine(
            Directory.GetCurrentDirectory(),
            "Reports");

        Directory.CreateDirectory(reportsDirectory);

        _reportPath = Path.Combine(
            reportsDirectory,
            "TestReport.html");

        var reporter = new ExtentSparkReporter(_reportPath);

        Extent = new ExtentReports();
        Extent.AttachReporter(reporter);

        Console.WriteLine($"Report: {_reportPath}");
    }

    [OneTimeTearDown]
    public void GlobalTearDown()
    {
        Extent.Flush();

        EmbedScreenshotsAsBase64(_reportPath);
    }

    private static void EmbedScreenshotsAsBase64(string reportPath)
    {
        try
        {
            if (!File.Exists(reportPath))
            {
                return;
            }

            var reportDirectory = Path.GetDirectoryName(reportPath)!;
            var html = File.ReadAllText(reportPath);

            var imgRegex = new Regex(
                @"<img([^>]*?)src=[""']([^""']*Screenshots/[^""']+)[""']([^>]*?)>",
                RegexOptions.IgnoreCase);

            var matches = imgRegex.Matches(html);
            if (matches.Count == 0)
            {
                return;
            }

            foreach (Match match in matches)
            {
                var src = match.Groups[2].Value;
                var imagePath = Path.Combine(reportDirectory, src.Replace('/', Path.DirectorySeparatorChar));

                if (!File.Exists(imagePath))
                {
                    continue;
                }

                var bytes = File.ReadAllBytes(imagePath);
                var base64 = Convert.ToBase64String(bytes);
                var dataUri = $"data:image/png;base64,{base64}";

                var replacement = $"<img{match.Groups[1].Value}src=\"{dataUri}\"{match.Groups[3].Value}>";
                html = html.Replace(match.Value, replacement);
            }

            File.WriteAllText(reportPath, html);
            Console.WriteLine($"Embedded {matches.Count} screenshots as base64 in report.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Could not embed screenshots as base64. {ex.Message}");
        }
    }
}
