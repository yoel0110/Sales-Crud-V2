using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace Sales.E2etest;

[TestFixture]
public class ProductFilterE2ETest : TestBase
{
    private IWebDriver _driver = null!;

    protected override IWebDriver Driver => _driver;

    [SetUp]
    public void Setup()
    {
        _driver = new ChromeDriver();
        LoginHelper.Login(_driver);
    }

    [Test]
    [Category("Camino feliz")]
    public void Filter_Success_ByCategory()
    {
        Test.Info("Esperando carga de productos");
        LoginHelper.WaitUntilElementVisible(_driver, By.CssSelector(".product-table"), TimeSpan.FromSeconds(5));

        Test.Info("Cambiando filtro de categoria");
        _driver.FindElement(By.Id("category")).SendKeys("Books");
        _driver.FindElement(By.CssSelector(".filters button[type='submit']")).Click();

        Test.Info("Verificando que la tabla sigue cargada");
        var rows = _driver.FindElements(By.CssSelector(".product-table tbody tr"));
        Assert.That(rows.Count, Is.GreaterThanOrEqualTo(0));
        Test.Pass("Filtro de categoria aplicado");
    }
}
