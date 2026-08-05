using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

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

    [Test]
    [Category("Camino feliz")]
    public void Update_Success_ChangePrice()
    {
        Test.Info("Esperando lista de productos");
        LoginHelper.WaitUntilElementVisible(_driver, By.CssSelector(".product-table"), TimeSpan.FromSeconds(5));

        Test.Info("Seleccionando primer producto para editar");
        var editButton = _driver.FindElement(By.CssSelector("button[id^='edit-product-']"));
        editButton.Click();
        LoginHelper.WaitUntilElementExists(_driver, By.Id("productName"), TimeSpan.FromSeconds(3));

        Test.Info("Actualizando precio del producto");
        var priceInput = _driver.FindElement(By.Id("price"));
        priceInput.Clear();
        priceInput.SendKeys("999.99");
        _driver.FindElement(By.Id("add")).Click();

        Test.Info("Verificando mensaje de exito");
        LoginHelper.WaitUntilElementVisible(_driver, By.Id("response-modal-message"), TimeSpan.FromSeconds(5));
        var message = _driver.FindElement(By.Id("response-modal-message")).Text;
        Assert.That(message.ToLowerInvariant(), Does.Contain("actualizado").Or.Contain("creado"));
        Test.Pass("Producto actualizado exitosamente");
    }
}
