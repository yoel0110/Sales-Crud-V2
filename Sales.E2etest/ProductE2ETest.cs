using OpenQA.Selenium;
using NUnit.Framework;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support;
using OpenQA.Selenium.Support.UI;


namespace Sales.E2etest;

[TestFixture]
public class ProductE2ETest
{
    private IWebDriver _driver = null!;

    [SetUp]
    public void Setup()
    {
        _driver = new ChromeDriver();
        _driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);
    }

    [TearDown]
    public void TearDown()
    {
        _driver.Quit();
        _driver.Dispose();
    }

    [Test]
    public void Title()
    {
        _driver.Navigate().GoToUrl("http://localhost:5173/");
        Assert.That(_driver.Title, Is.EqualTo("sales-web"));
    }

    [Test]
    public void CreateProduct()
    {
        _driver.Navigate().GoToUrl("http://localhost:5173/");
        var newProductBtn = _driver.FindElement(By.Id("new-product"));
        newProductBtn.Click();
        if (_driver.FindElement(By.ClassName("product-form")).Displayed)
        {
            _driver.FindElement(By.Id("productName")).SendKeys("test");
            
            var select  = _driver.FindElement(By.Id("categoryId"));
            select.Click();
            select.FindElement(By.CssSelector("option[value='1']")).Click();
            _driver.FindElement(By.Id("nprice")).SendKeys("12.50");
            _driver.FindElement(By.Id("stock")).SendKeys("3");

            var button = _driver.FindElement(By.Id("add"));
            button.Submit();

            Thread.Sleep(5000);
        }
        Assert.That(_driver.FindElement(By.Id("categoryId")).FindElement(By.CssSelector("option[value='1']")).GetAttribute("value"), Is.EqualTo("1"));
        Assert.That(_driver.FindElement(By.Id("stock")).GetAttribute("value"), Is.EqualTo("3"));
        Assert.That(_driver.FindElement(By.Id("nprice")).GetAttribute("value"), Is.EqualTo("12.50"));
        Assert.That(_driver.FindElement(By.Id("productName")).GetAttribute("value"), Is.EqualTo("test"));
    }
}
