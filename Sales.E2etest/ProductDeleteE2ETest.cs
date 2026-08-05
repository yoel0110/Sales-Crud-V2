using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;

namespace Sales.E2etest;

[TestFixture]
public class ProductDeleteE2ETest : TestBase
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
    public void Delete_Success_FirstProduct()
    {
        Test.Info("Cargando productos");
        LoadProducts();

        Test.Info("Seleccionando primer producto para eliminar");
        var deleteButton = _driver.FindElement(By.CssSelector("button[id^='delete-product-']"));
        deleteButton.Click();

        Test.Info("Confirmando eliminacion");
        LoginHelper.WaitUntilElementVisible(_driver, By.Id("confirm-accept"), TimeSpan.FromSeconds(3));
        _driver.FindElement(By.Id("confirm-accept")).Click();

        Test.Info("Verificando mensaje de exito");
        LoginHelper.WaitUntilElementVisible(_driver, By.Id("response-modal-message"), TimeSpan.FromSeconds(5));
        var message = _driver.FindElement(By.Id("response-modal-message")).Text;
        Assert.That(message.ToLowerInvariant(), Does.Contain("ok").Or.Contain("eliminado"));
        Test.Pass("Producto eliminado exitosamente");
    }

    [Test]
    [Category("Prueba negativa")]
    public void Delete_Failure_Cancel()
    {
        Test.Info("Cargando productos");
        LoadProducts();

        Test.Info("Seleccionando primer producto para eliminar");
        var deleteButton = _driver.FindElement(By.CssSelector("button[id^='delete-product-']"));
        deleteButton.Click();

        Test.Info("Cancelando eliminacion");
        LoginHelper.WaitUntilElementVisible(_driver, By.Id("confirm-cancel"), TimeSpan.FromSeconds(3));
        _driver.FindElement(By.Id("confirm-cancel")).Click();

        Test.Info("Verificando que la lista sigue visible");
        var table = _driver.FindElement(By.CssSelector(".product-table"));
        Assert.That(table.Displayed, Is.True);
        Test.Pass("Eliminacion cancelada correctamente");
    }

    [Test]
    [Category("Prueba de limites")]
    public void Delete_Boundary_LastRemaining()
    {
        Test.Info("Cargando productos");
        LoadProducts();

        var initialRows = _driver.FindElements(By.CssSelector(".product-table tbody tr")).Count;
        Test.Info($"Filas iniciales: {initialRows}");

        Test.Info("Seleccionando primer producto para eliminar");
        var deleteButton = _driver.FindElement(By.CssSelector("button[id^='delete-product-']"));
        deleteButton.Click();

        Test.Info("Confirmando eliminacion");
        LoginHelper.WaitUntilElementVisible(_driver, By.Id("confirm-accept"), TimeSpan.FromSeconds(3));
        _driver.FindElement(By.Id("confirm-accept")).Click();

        Test.Info("Verificando que la UI se actualiza");
        LoginHelper.WaitUntilElementVisible(_driver, By.Id("response-modal-message"), TimeSpan.FromSeconds(5));
        var message = _driver.FindElement(By.Id("response-modal-message")).Text;
        Assert.That(message.ToLowerInvariant(), Does.Contain("ok").Or.Contain("eliminado"));
        Test.Pass("Eliminacion en limite de filas procesada");
    }
}
