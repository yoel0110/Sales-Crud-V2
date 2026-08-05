using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;

namespace Sales.E2etest;

[TestFixture]
public class ProductUpdateE2ETest : TestBase
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
        filterSelect.SelectByValue("1");
        var priceInput = _driver.FindElement(By.Id("price"));
        priceInput.Clear();
        priceInput.SendKeys("0");
        var submitButton = _driver.FindElement(By.CssSelector(".filters button[type='submit']"));
        ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].click();", submitButton);
        LoginHelper.WaitUntilElementVisible(_driver, By.CssSelector(".product-table"), TimeSpan.FromSeconds(5));
    }

    private void SubmitForm()
    {
        var addButton = _driver.FindElement(By.Id("add"));
        ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].click();", addButton);
    }

    [Test]
    [Category("Camino feliz")]
    public void Update_Success_ChangePrice()
    {
        Test.Info("Cargando productos");
        LoadProducts();

        Test.Info("Seleccionando primer producto para editar");
        var editButton = _driver.FindElement(By.CssSelector("button[id^='edit-product-']"));
        editButton.Click();
        LoginHelper.WaitUntilElementVisible(_driver, By.Id("productName"), TimeSpan.FromSeconds(3));

        Test.Info("Actualizando precio del producto");
        var priceInput = _driver.FindElement(By.Id("price"));
        priceInput.Clear();
        priceInput.SendKeys("999.99");
        SubmitForm();

        Test.Info("Verificando mensaje de exito");
        LoginHelper.WaitUntilElementVisible(_driver, By.Id("response-modal-message"), TimeSpan.FromSeconds(5));
        var message = _driver.FindElement(By.Id("response-modal-message")).Text;
        Assert.That(message.ToLowerInvariant(), Does.Contain("actualizado").Or.Contain("creado"));
        Test.Pass("Producto actualizado exitosamente");
    }

    [Test]
    [Category("Prueba negativa")]
    public void Update_Failure_ClearName()
    {
        Test.Info("Cargando productos");
        LoadProducts();

        Test.Info("Seleccionando primer producto para editar");
        var editButton = _driver.FindElement(By.CssSelector("button[id^='edit-product-']"));
        editButton.Click();
        LoginHelper.WaitUntilElementVisible(_driver, By.Id("productName"), TimeSpan.FromSeconds(3));

        Test.Info("Borrando nombre del producto");
        var nameInput = _driver.FindElement(By.Id("productName"));
        nameInput.Clear();
        SubmitForm();

        Test.Info("Verificando que el formulario no se envio");
        var form = _driver.FindElement(By.CssSelector(".product-form"));
        Assert.That(form.Displayed, Is.True);
        Test.Pass("Actualizacion con nombre vacio rechazada");
    }

    [Test]
    [Category("Prueba de limites")]
    public void Update_Boundary_MaxPrice()
    {
        Test.Info("Cargando productos");
        LoadProducts();

        Test.Info("Seleccionando primer producto para editar");
        var editButton = _driver.FindElement(By.CssSelector("button[id^='edit-product-']"));
        editButton.Click();
        LoginHelper.WaitUntilElementVisible(_driver, By.Id("productName"), TimeSpan.FromSeconds(3));

        Test.Info("Actualizando con precio maximo");
        var priceInput = _driver.FindElement(By.Id("price"));
        priceInput.Clear();
        priceInput.SendKeys("999999.99");
        SubmitForm();

        Test.Info("Verificando mensaje de respuesta");
        LoginHelper.WaitUntilElementVisible(_driver, By.Id("response-modal-message"), TimeSpan.FromSeconds(5));
        var message = _driver.FindElement(By.Id("response-modal-message")).Text;
        Assert.That(message, Is.Not.Empty);
        Test.Pass("Precio maximo procesado");
    }
}
