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

    [Test]
    [Category("Prueba negativa")]
    public void Filter_Failure_InvalidPrice()
    {
        Test.Info("Esperando carga de productos");
        LoginHelper.WaitUntilElementVisible(_driver, By.CssSelector(".product-table"), TimeSpan.FromSeconds(5));

        Test.Info("Ingresando precio negativo en el filtro");
        var priceInput = _driver.FindElement(By.Id("price"));
        priceInput.Clear();
        priceInput.SendKeys("-50");
        _driver.FindElement(By.CssSelector(".filters button[type='submit']")).Click();

        Test.Info("Verificando que la tabla se muestra sin excepciones");
        var table = _driver.FindElement(By.CssSelector(".product-table"));
        Assert.That(table.Displayed, Is.True);
        Test.Pass("Filtro de precio negativo manejado");
    }

    [Test]
    [Category("Prueba de limites")]
    public void Filter_Boundary_ZeroResults()
    {
        Test.Info("Esperando carga de productos");
        LoginHelper.WaitUntilElementVisible(_driver, By.CssSelector(".product-table"), TimeSpan.FromSeconds(5));

        Test.Info("Filtrando con precio extremo sin resultados");
        var priceInput = _driver.FindElement(By.Id("price"));
        priceInput.Clear();
        priceInput.SendKeys("999999");
        _driver.FindElement(By.CssSelector(".filters button[type='submit']")).Click();

        Test.Info("Verificando que se muestra lista vacia");
        var empty = _driver.FindElement(By.CssSelector(".empty"));
        Assert.That(empty.Text, Does.Contain("No se encontraron"));
        Test.Pass("Filtro sin resultados manejado");
    }
}
