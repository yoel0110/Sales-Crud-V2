using OpenQA.Selenium;

namespace Sales.E2etest;

public static class LoginHelper
{
    public static void Login(IWebDriver driver, string username = "admin", string password = "admin123")
    {
        driver.Navigate().GoToUrl("http://localhost:5173");

        var usernameInput = driver.FindElement(By.Id("username"));
        usernameInput.Clear();
        usernameInput.SendKeys(username);

        var passwordInput = driver.FindElement(By.Id("password"));
        passwordInput.Clear();
        passwordInput.SendKeys(password);

        driver.FindElement(By.Id("login-button")).Click();

        WaitUntilElementVisible(driver, By.Id("logged-user"), TimeSpan.FromSeconds(5));
    }

    public static void WaitUntilElementVisible(IWebDriver driver, By by, TimeSpan timeout)
    {
        var deadline = DateTime.Now.Add(timeout);
        while (DateTime.Now < deadline)
        {
            try
            {
                var element = driver.FindElement(by);
                if (element.Displayed)
                {
                    return;
                }
            }
            catch
            {
            }

            Thread.Sleep(200);
        }

        throw new TimeoutException($"Element {by} was not visible within {timeout}.");
    }

    public static void WaitUntilElementExists(IWebDriver driver, By by, TimeSpan timeout)
    {
        var deadline = DateTime.Now.Add(timeout);
        while (DateTime.Now < deadline)
        {
            try
            {
                var element = driver.FindElement(by);
                if (element != null)
                {
                    return;
                }
            }
            catch
            {
            }

            Thread.Sleep(200);
        }

        throw new TimeoutException($"Element {by} was not found within {timeout}.");
    }

    public static void CloseResponseModal(IWebDriver driver)
    {
        try
        {
            var closeButton = driver.FindElement(By.Id("response-modal-close"));
            if (closeButton.Displayed)
            {
                closeButton.Click();
                Thread.Sleep(200);
            }
        }
        catch
        {
        }
    }
}
