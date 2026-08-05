using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace Sales.E2etest;

[TestFixture]
public class LoginE2ETest : TestBase
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
    public void Login_Success_ValidCredentials()
    {
        Test.Info("Navegando a la aplicacion");
        _driver.Navigate().GoToUrl("http://localhost:5173");

        Test.Info("Esperando formulario de login");
        LoginHelper.WaitUntilElementVisible(_driver, By.Id("username"), TimeSpan.FromSeconds(5));

        Test.Info("Ingresando credenciales validas");
        _driver.FindElement(By.Id("username")).SendKeys("admin");
        _driver.FindElement(By.Id("password")).SendKeys("admin123");
        _driver.FindElement(By.Id("login-button")).Click();

        Test.Info("Verificando que se muestra el usuario autenticado");
        LoginHelper.WaitUntilElementVisible(_driver, By.Id("logged-user"), TimeSpan.FromSeconds(5));

        var loggedUser = _driver.FindElement(By.Id("logged-user")).Text;
        Assert.That(loggedUser, Is.EqualTo("admin"));
        Test.Pass("Login exitoso");
    }

    [Test]
    [Category("Prueba negativa")]
    public void Login_Failure_InvalidCredentials()
    {
        Test.Info("Navegando a la aplicacion");
        _driver.Navigate().GoToUrl("http://localhost:5173");

        Test.Info("Esperando formulario de login");
        LoginHelper.WaitUntilElementVisible(_driver, By.Id("username"), TimeSpan.FromSeconds(5));

        Test.Info("Ingresando credenciales invalidas");
        _driver.FindElement(By.Id("username")).SendKeys("admin");
        _driver.FindElement(By.Id("password")).SendKeys("contraseñainvalida");
        _driver.FindElement(By.Id("login-button")).Click();

        Test.Info("Verificando mensaje de error");
        LoginHelper.WaitUntilElementVisible(_driver, By.Id("response-modal-message"), TimeSpan.FromSeconds(5));

        var errorMessage = _driver.FindElement(By.Id("response-modal-message")).Text;
        Assert.That(errorMessage.ToLowerInvariant(), Does.Contain("incorrectos"));
        Test.Pass("Login invalido rechazado correctamente");
    }

    [Test]
    [Category("Prueba de limites")]
    public void Login_Boundary_EmptyCredentials()
    {
        Test.Info("Navegando a la aplicacion");
        _driver.Navigate().GoToUrl("http://localhost:5173");

        Test.Info("Esperando formulario de login");
        LoginHelper.WaitUntilElementVisible(_driver, By.Id("username"), TimeSpan.FromSeconds(5));

        Test.Info("Intentando iniciar sesion con campos vacios");
        _driver.FindElement(By.Id("login-button")).Click();

        Test.Info("Verificando que permanece en el formulario de login");
        var loginButton = _driver.FindElement(By.Id("login-button"));
        Assert.That(loginButton.Displayed, Is.True);
        Test.Pass("Campos vacios mantienen al usuario en login");
    }
}
