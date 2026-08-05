using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;

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

    private void LoadProducts()
    {
        Test.Info("Ajustando filtro para mostrar productos");
        var filterSelect = new SelectElement(_driver.FindElement(By.Id("filter")));
        filterSelect.SelectByValue("2");
        var priceInput = _driver.FindElement(By.CssSelector(".filters #price"));
        priceInput.Clear();
        priceInput.SendKeys("0");
        var submitButton = _driver.FindElement(By.CssSelector(".filters button[type='submit']"));
        ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].click();", submitButton);
        LoginHelper.WaitUntilElementVisible(_driver, By.CssSelector(".product-table"), TimeSpan.FromSeconds(5));
    }

    [Test]
    [Category("Camino feliz")]
    public void Read_Success_ListLoaded()
    {
        Test.Info("Esperando carga de productos");
        LoadProducts();

        Test.Info("Verificando que la tabla tiene filas");
        var rows = _driver.FindElements(By.CssSelector(".product-table tbody tr"));
        Assert.That(rows.Count, Is.GreaterThan(0));
        Test.Pass("Lista de productos cargada");
    }

    [Test]
    [Category("Prueba negativa")]
    public void Read_Failure_UnauthorizedAccess()
    {
        using var unauthenticatedDriver = new ChromeDriver();
        Test.Info("Navegando a la API sin autenticacion");
        unauthenticatedDriver.Navigate().GoToUrl("http://localhost:5138/api/v1/product/all?filter=1&minPrice=0&maxPrice=1000&price=0&length=10&category=Toys");

        Test.Info("Verificando respuesta de error");
        var body = unauthenticatedDriver.FindElement(By.TagName("body")).Text;
        Assert.That(body.ToLowerInvariant(), Does.Contain("401").Or.Contains("unauthorized"));
        Test.Pass("Acceso no autorizado a la API correctamente bloqueado");
    }

    [Test]
    [Category("Prueba de limites")]
    public void Read_Boundary_LargeLength()
    {
        Test.Info("Esperando carga de productos");
        LoadProducts();

        Test.Info("Solicitando cantidad limite de productos");
        _driver.FindElement(By.Id("length")).Clear();
        _driver.FindElement(By.Id("length")).SendKeys("1000");
        var submitButton = _driver.FindElement(By.CssSelector(".filters button[type='submit']"));
        ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].click();", submitButton);

        LoginHelper.WaitUntilElementVisible(_driver, By.CssSelector(".product-table, .empty"), TimeSpan.FromSeconds(5));

        Test.Info("Verificando que la tabla se muestra sin errores");
        var table = _driver.FindElement(By.CssSelector(".product-table"));
        Assert.That(table.Displayed, Is.True);
        Test.Pass("Cantidad limite procesada correctamente");
    }
}
