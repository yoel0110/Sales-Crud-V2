using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace Sales.E2etest;

[TestFixture]
public class ProductReadE2ETest : TestBase
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
    public void Read_Success_ListLoaded()
    {
        Test.Info("Esperando carga de productos");
        LoginHelper.WaitUntilElementVisible(_driver, By.CssSelector(".product-table"), TimeSpan.FromSeconds(5));

        Test.Info("Verificando que la tabla tiene filas");
        var rows = _driver.FindElements(By.CssSelector(".product-table tbody tr"));
        Assert.That(rows.Count, Is.GreaterThan(0));
        Test.Pass("Lista de productos cargada");
    }
}
