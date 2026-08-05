using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace Sales.E2etest;

[TestFixture]
public class ProductCreateE2ETest : TestBase
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
    public void Create_Success_ValidProduct()
    {
        Test.Info("Abriendo formulario de nuevo producto");
        _driver.FindElement(By.Id("new-product")).Click();
        LoginHelper.WaitUntilElementExists(_driver, By.Id("productName"), TimeSpan.FromSeconds(3));

        var productName = $"Producto E2E {DateTime.Now:yyyyMMddHHmmss}";
        Test.Info($"Creando producto: {productName}");
        _driver.FindElement(By.Id("productName")).SendKeys(productName);
        _driver.FindElement(By.Id("categoryId")).SendKeys("Toys");
        _driver.FindElement(By.Id("price")).SendKeys("99.99");
        _driver.FindElement(By.Id("stock")).SendKeys("50");
        _driver.FindElement(By.Id("add")).Click();

        Test.Info("Verificando mensaje de exito");
        LoginHelper.WaitUntilElementVisible(_driver, By.Id("response-modal-message"), TimeSpan.FromSeconds(5));

        var message = _driver.FindElement(By.Id("response-modal-message")).Text;
        Assert.That(message.ToLowerInvariant(), Does.Contain("creado"));
        Test.Pass("Producto creado exitosamente");
    }
}
