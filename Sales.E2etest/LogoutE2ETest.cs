using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace Sales.E2etest;

[TestFixture]
public class LogoutE2ETest : TestBase
{
    private IWebDriver _driver = null!;

    protected override IWebDriver Driver => _driver;

    [SetUp]
    public void Setup()
    {
        _driver = new ChromeDriver();
    }

    [Test]
    [Category("Camino feliz")]
    public void Logout_Success_ReturnsToLogin()
    {
        Test.Info("Iniciando sesion");
        LoginHelper.Login(_driver);

        Test.Info("Haciendo clic en cerrar sesion");
        _driver.FindElement(By.Id("logout-button")).Click();

        Test.Info("Verificando que se muestra el formulario de login");
        LoginHelper.WaitUntilElementVisible(_driver, By.Id("login-button"), TimeSpan.FromSeconds(5));

        var loginButton = _driver.FindElement(By.Id("login-button"));
        Assert.That(loginButton.Displayed, Is.True);
        Test.Pass("Cierre de sesion exitoso");
    }
}
