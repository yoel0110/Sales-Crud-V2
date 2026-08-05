using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;

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

    private void ApplyBaseFilter()
    {
        var filterSelect = new SelectElement(_driver.FindElement(By.Id("filter")));
        filterSelect.SelectByValue("1");
        var priceInput = _driver.FindElement(By.Id("price"));
        priceInput.Clear();
        priceInput.SendKeys("0");
        _driver.FindElement(By.CssSelector(".filters button[type='submit']")).Click();
        LoginHelper.WaitUntilElementVisible(_driver, By.CssSelector(".product-table"), TimeSpan.FromSeconds(5));
    }

    private void Search()
    {
        var submitButton = _driver.FindElement(By.CssSelector(".filters button[type='submit']"));
        ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].click();", submitButton);
    }

    [Test]
    [Category("Camino feliz")]
    public void Filter_Success_ByCategory()
    {
        Test.Info("Cargando productos con filtro base");
        ApplyBaseFilter();

        Test.Info("Cambiando filtro de categoria");
        var categorySelect = new SelectElement(_driver.FindElement(By.Id("category")));
        categorySelect.SelectByText("Books");
        Search();

        Test.Info("Verificando que la UI se actualiza");
        var table = _driver.FindElement(By.CssSelector(".product-table"));
        Assert.That(table.Displayed, Is.True);
        Test.Pass("Filtro de categoria aplicado");
    }

    [Test]
    [Category("Prueba negativa")]
    public void Filter_Failure_InvalidPrice()
    {
        Test.Info("Cargando productos con filtro base");
        ApplyBaseFilter();

        Test.Info("Ingresando precio negativo en el filtro");
        var priceInput = _driver.FindElement(By.Id("price"));
        priceInput.Clear();
        priceInput.SendKeys("-50");
        Search();

        Test.Info("Verificando que la UI se muestra sin excepciones");
        var displayed = _driver.FindElement(By.CssSelector(".product-table, .empty")).Displayed;
        Assert.That(displayed, Is.True);
        Test.Pass("Filtro de precio negativo manejado");
    }

    [Test]
    [Category("Prueba de limites")]
    public void Filter_Boundary_ZeroResults()
    {
        Test.Info("Cargando productos con filtro base");
        ApplyBaseFilter();

        Test.Info("Filtrando con precio extremo sin resultados");
        var priceInput = _driver.FindElement(By.Id("price"));
        priceInput.Clear();
        priceInput.SendKeys("999999");
        Search();

        Test.Info("Verificando que se muestra lista vacia");
        var empty = _driver.FindElement(By.CssSelector(".empty"));
        Assert.That(empty.Text, Does.Contain("No se encontraron"));
        Test.Pass("Filtro sin resultados manejado");
    }
}
