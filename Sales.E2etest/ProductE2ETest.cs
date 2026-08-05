using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using Sales.E2etest;

namespace Sales.E2etest;

[TestFixture]
public class ProductE2ETest : TestBase
{
    private IWebDriver _driver = null!;

    protected override IWebDriver Driver => _driver;

    [SetUp]
    public void Setup()
    {
        _driver = new ChromeDriver();
    }

    [TearDown]
    public void TearDown()
    {
        _driver.Quit();
        _driver.Dispose();
    }

    [Test]
    public void CreateProduct()
    {
        Test.Info("Opening application");

        _driver.Navigate().GoToUrl("http://localhost:5173");

        Test.Info("Clicking New Product");

        _driver.FindElement(By.Id("new-product")).Click();

        Test.Pass("Product created successfully");
    }

    [Test]
    public void UpdateProduct()
    {
        Test.Info("Opening application");

        _driver.Navigate().GoToUrl("http://localhost:5173");

        Test.Info("Clicking Update Product");

        _driver.FindElement(By.Id("new-product")).Click();

        Test.Fail("Error");
    }
}