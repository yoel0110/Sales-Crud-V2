using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

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

    [Test]
    [Category("Camino feliz")]
    public void Delete_Success_FirstProduct()
    {
        Test.Info("Esperando lista de productos");
        LoginHelper.WaitUntilElementVisible(_driver, By.CssSelector(".product-table"), TimeSpan.FromSeconds(5));

        Test.Info("Seleccionando primer producto para eliminar");
        var deleteButton = _driver.FindElement(By.CssSelector("button[id^='delete-product-']"));
        deleteButton.Click();

        Test.Info("Confirmando eliminacion");
        LoginHelper.WaitUntilElementExists(_driver, By.Id("confirm-accept"), TimeSpan.FromSeconds(3));
        _driver.FindElement(By.Id("confirm-accept")).Click();

        Test.Info("Verificando mensaje de exito");
        LoginHelper.WaitUntilElementVisible(_driver, By.Id("response-modal-message"), TimeSpan.FromSeconds(5));
        var message = _driver.FindElement(By.Id("response-modal-message")).Text;
        Assert.That(message.ToLowerInvariant(), Does.Contain("eliminado"));
        Test.Pass("Producto eliminado exitosamente");
    }
}
