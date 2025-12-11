using OpenQA.Selenium;
using NUnit.Framework;

namespace RimansaRealEstate.Tests
{
    [TestFixture]
    [Category("Authentication")]
    public class LoginTests : BaseTest
    {
        [Test]
        [Order(1)]
        public void Test01_LoginExitoso()
        {
            TestContext.WriteLine("Iniciando Test01_LoginExitoso");

            driver.Navigate().GoToUrl($"{baseUrl}/Admin/Login");
            Wait(1000);

            var usernameField = WaitForElement(By.Name("Username"));
            usernameField.Clear();
            usernameField.SendKeys("admin");

            var passwordField = driver.FindElement(By.Name("Password"));
            passwordField.Clear();
            passwordField.SendKeys("Admin123!");

            var submitButton = driver.FindElement(By.CssSelector("button[type='submit']"));
            submitButton.Click();

            WaitForUrl("/Admin/Dashboard", 10);

            Assert.That(driver.Url, Does.Contain("/Admin/Dashboard"),
                "El usuario debería ser redirigido al dashboard después del login exitoso");

            TestContext.WriteLine("Test01_LoginExitoso - PASÓ");
        }

        [Test]
        [Order(2)]
        public void Test02_LoginCredencialesIncorrectas()
        {
            TestContext.WriteLine("Iniciando Test02_LoginCredencialesIncorrectas");

            driver.Navigate().GoToUrl($"{baseUrl}/Admin/Login");
            Wait(1000);

            var usernameField = WaitForElement(By.Name("Username"));
            usernameField.SendKeys("usuarioincorrecto");

            var passwordField = driver.FindElement(By.Name("Password"));
            passwordField.SendKeys("ContraseñaIncorrecta123");

            var submitButton = driver.FindElement(By.CssSelector("button[type='submit']"));
            submitButton.Click();

            Wait(2000);

            Assert.That(driver.Url, Does.Contain("/Admin/Login"),
                "El usuario debería permanecer en la página de login con credenciales incorrectas");

            TestContext.WriteLine("Test02_LoginCredencialesIncorrectas - PASÓ");
        }

        [Test]
        [Order(3)]
        public void Test03_AccesoNoAutorizadoAlAdmin()
        {
            TestContext.WriteLine("Iniciando Test03_AccesoNoAutorizadoAlAdmin");

            driver.Navigate().GoToUrl($"{baseUrl}/Admin/Dashboard");
            Wait(2000);

            bool isProtected = driver.Url.Contains("/Admin/Login") ||
                             driver.Url.Contains("/Admin/AccessDenied") ||
                             driver.Url.Contains("/Account/Login");

            Assert.That(isProtected, Is.True,
                "Las rutas administrativas deben estar protegidas");

            TestContext.WriteLine("Test03_AccesoNoAutorizadoAlAdmin - PASÓ");
        }

        [Test]
        [Order(4)]
        public void Test04_LogoutExitoso()
        {
            TestContext.WriteLine("Iniciando Test04_LogoutExitoso");

            LoginAsAdmin();

            Assert.That(driver.Url, Does.Contain("/Admin/Dashboard"),
                "Precondición: el usuario debe estar logueado");

            Logout();

            bool redirectedCorrectly = driver.Url.Contains("/Admin/Login") ||
                                       driver.Url == $"{baseUrl}/" ||
                                       driver.Url == baseUrl;

            Assert.That(redirectedCorrectly, Is.True,
                "Después del logout, debe redirigir a login o página principal");

            driver.Navigate().GoToUrl($"{baseUrl}/Admin/Dashboard");
            Wait(2000);

            bool cannotAccessDashboard = driver.Url.Contains("/Admin/Login") ||
                                        driver.Url.Contains("/Admin/AccessDenied") ||
                                        !driver.Url.Contains("/Admin/Dashboard");

            Assert.That(cannotAccessDashboard, Is.True,
                "Después del logout, no debe poder acceder al dashboard");

            TestContext.WriteLine("Test04_LogoutExitoso - PASÓ");
        }

        [Test]
        [Order(5)]
        public void Test05_LoginCamposVacios()
        {
            TestContext.WriteLine("Iniciando Test05_LoginCamposVacios");

            driver.Navigate().GoToUrl($"{baseUrl}/Admin/Login");
            Wait(1000);

            var submitButton = WaitForElement(By.CssSelector("button[type='submit']"));
            submitButton.Click();

            Wait(2000); 

            Assert.That(driver.Url, Does.Contain("/Admin/Login"),
                "No debe permitir login con campos vacíos");

            var usernameField = driver.FindElement(By.Name("Username"));
            var passwordField = driver.FindElement(By.Name("Password"));

            bool hasHtmlValidation = usernameField.GetAttribute("required") != null ||
                                    passwordField.GetAttribute("required") != null;

            bool hasServerValidation = driver.PageSource.Contains("field-validation-error") ||
                                      driver.PageSource.Contains("validation-summary-errors") ||
                                      driver.PageSource.Contains("is-invalid") ||
                                      driver.PageSource.Contains("requerido") ||
                                      driver.PageSource.Contains("required");

            bool hasValidation = hasHtmlValidation || hasServerValidation;

            Assert.That(hasValidation, Is.True,
                "Los campos deben tener validación (HTML o del servidor)");

            TestContext.WriteLine($"Validación HTML: {hasHtmlValidation}");
            TestContext.WriteLine($"Validación Servidor: {hasServerValidation}");
            TestContext.WriteLine("Test05_LoginCamposVacios - PASÓ");
        }

        [Test]
        [Order(6)]
        public void Test06_SesionPersistente()
        {
            TestContext.WriteLine("Iniciando Test06_SesionPersistente");

            LoginAsAdmin();

            driver.Navigate().GoToUrl($"{baseUrl}/Properties");
            Wait(1500);

            bool sessionMaintained = !driver.Url.Contains("/Admin/Login") &&
                                    !driver.Url.Contains("/Admin/AccessDenied");

            Assert.That(sessionMaintained, Is.True,
                "La sesión debe persistir al navegar entre páginas administrativas");

            TestContext.WriteLine("Test06_SesionPersistente - PASÓ");
        }
    }
}