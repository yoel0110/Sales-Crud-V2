using System.Reflection;

namespace Sales.E2etest;

public static class ReportPathHelper
{
    public static string GetReportsDirectory()
    {
        var projectDirectory = GetProjectDirectory();
        var reportsDirectory = Path.Combine(projectDirectory, "Reports");
        Directory.CreateDirectory(reportsDirectory);
        return reportsDirectory;
    }

    private static string GetProjectDirectory()
    {
        var assemblyLocation = Assembly.GetExecutingAssembly().Location;
        var directory = new FileInfo(assemblyLocation).Directory;

        while (directory != null && !directory.GetFiles("*.csproj").Any())
        {
            directory = directory.Parent;
        }

        return directory?.FullName ?? Directory.GetCurrentDirectory();
    }
}
