using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using NUnit.Framework;

namespace RimansaRealEstate.Tests
{
    /// Maneja la configuración del WebDriver y métodos comunes
    public class BaseTest
    {
        protected IWebDriver driver;
        protected WebDriverWait wait;

        protected string baseUrl = "https://localhost:44313";

        [SetUp]
        public void Setup()
        {
            try
            {

                var options = new ChromeOptions();
                options.AddArgument("--start-maximized");
                options.AddArgument("--disable-notifications");
                options.AddArgument("--disable-popup-blocking");
                options.AddArgument("--ignore-certificate-errors");
                options.AddArgument("--disable-gpu");

                driver = new ChromeDriver(options);

                driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);
                driver.Manage().Timeouts().PageLoad = TimeSpan.FromSeconds(30);

                wait = new WebDriverWait(driver, TimeSpan.FromSeconds(15));
            }
            catch (Exception ex)
            {
                TestContext.WriteLine($"Error en Setup: {ex.Message}");
                throw;
            }
        }

        [TearDown]
        public void TearDown()
        {
            try
            {
                if (TestContext.CurrentContext.Result.Outcome.Status == NUnit.Framework.Interfaces.TestStatus.Failed)
                {
                    TakeScreenshot(TestContext.CurrentContext.Test.Name);
                }
            }
            catch (Exception ex)
            {
                TestContext.WriteLine($"Error al tomar screenshot: {ex.Message}");
            }
            finally
            {
                try
                {
                    driver?.Quit();
                    driver?.Dispose();
                }
                catch (Exception ex)
                {
                    TestContext.WriteLine($"Error al cerrar driver: {ex.Message}");
                }
            }
        }

        /// Toma un screenshot cuando una prueba falla
        protected void TakeScreenshot(string testName)
        {
            try
            {
                var screenshot = ((ITakesScreenshot)driver).GetScreenshot();
                string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                string filename = $"Screenshot_{testName}_{timestamp}.png";

                string projectPath = TestContext.CurrentContext.TestDirectory;
                string screenshotPath = Path.Combine(projectPath, "Screenshots");

                if (!Directory.Exists(screenshotPath))
                {
                    Directory.CreateDirectory(screenshotPath);
                }

                string filepath = Path.Combine(screenshotPath, filename);
                screenshot.SaveAsFile(filepath);

                TestContext.WriteLine($"✅ Screenshot guardado: {filepath}");
                TestContext.AddTestAttachment(filepath, "Screenshot del error");
            }
            catch (Exception ex)
            {
                TestContext.WriteLine($"❌ Error al tomar screenshot: {ex.Message}");
            }
        }

        /// Espera a que un elemento sea visible y clickeable
        protected IWebElement WaitForElement(By locator, int seconds = 15)
        {
            try
            {
                var customWait = new WebDriverWait(driver, TimeSpan.FromSeconds(seconds));
                return customWait.Until(drv =>
                {
                    try
                    {
                        var element = drv.FindElement(locator);
                        return (element.Displayed && element.Enabled) ? element : null;
                    }
                    catch (StaleElementReferenceException)
                    {
                        return null;
                    }
                    catch (NoSuchElementException)
                    {
                        return null;
                    }
                });
            }
            catch (WebDriverTimeoutException)
            {
                TestContext.WriteLine($"⚠️ Timeout esperando elemento: {locator}");
                throw;
            }
        }

        /// Espera a que la URL contenga cierto texto
        protected bool WaitForUrl(string urlPart, int seconds = 15)
        {
            try
            {
                var customWait = new WebDriverWait(driver, TimeSpan.FromSeconds(seconds));
                return customWait.Until(drv => drv.Url.Contains(urlPart));
            }
            catch (WebDriverTimeoutException)
            {
                TestContext.WriteLine($"⚠️ Timeout esperando URL con: '{urlPart}'");
                TestContext.WriteLine($"URL actual: {driver.Url}");
                return false;
            }
        }

        /// Hacer login como administrador
        protected void LoginAsAdmin(string username = "admin", string password = "Admin123!")
        {
            try
            {
                TestContext.WriteLine("🔐 Iniciando sesión como administrador...");

                driver.Navigate().GoToUrl($"{baseUrl}/Admin/Login");

                var usernameField = WaitForElement(By.Name("Username"));
                usernameField.Clear();
                usernameField.SendKeys(username);

                var passwordField = driver.FindElement(By.Name("Password"));
                passwordField.Clear();
                passwordField.SendKeys(password);

                var submitButton = driver.FindElement(By.CssSelector("button[type='submit']"));
                submitButton.Click();

                WaitForUrl("/Admin/Dashboard", 10);

                TestContext.WriteLine("✅ Login exitoso");
            }
            catch (Exception ex)
            {
                TestContext.WriteLine($"❌ Error en login: {ex.Message}");
                throw;
            }
        }

        /// Hacer logout
        protected void Logout()
        {
            try
            {
                TestContext.WriteLine("🚪 Cerrando sesión...");
                driver.Navigate().GoToUrl($"{baseUrl}/Admin/Logout");
                Wait(2000); 

                bool loggedOut = driver.Url.Contains("/Admin/Login") ||
                                driver.Url == $"{baseUrl}/" ||
                                driver.Url == baseUrl;

                if (loggedOut)
                {
                    TestContext.WriteLine("✅ Logout exitoso");
                }
                else
                {
                    TestContext.WriteLine($"⚠️ URL después del logout: {driver.Url}");
                }
            }
            catch (Exception ex)
            {
                TestContext.WriteLine($"❌ Error en logout: {ex.Message}");
                throw;
            }
        }

        /// Scroll a un elemento para que sea visible
        protected void ScrollToElement(IWebElement element)
        {
            try
            {
                ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView({behavior: 'smooth', block: 'center'});", element);
                Thread.Sleep(500);
            }
            catch (Exception ex)
            {
                TestContext.WriteLine($"⚠️ Error en scroll: {ex.Message}");
            }
        }

        /// Scroll hasta el final de la página
        protected void ScrollToBottom()
        {
            ((IJavaScriptExecutor)driver).ExecuteScript("window.scrollTo(0, document.body.scrollHeight);");
            Thread.Sleep(500);
        }

        /// Esperar un tiempo específico
        protected void Wait(int milliseconds)
        {
            Thread.Sleep(milliseconds);
        }

        /// Verificar si un elemento existe
        protected bool ElementExists(By locator)
        {
            try
            {
                driver.FindElement(locator);
                return true;
            }
            catch (NoSuchElementException)
            {
                return false;
            }
        }

        /// Obtener texto de un elemento de forma segura
        protected string GetElementText(By locator)
        {
            try
            {
                return driver.FindElement(locator).Text;
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        /// Click usando JavaScript para evitar problemas de interacción
        protected void ClickElement(IWebElement element)
        {
            try
            {
                element.Click();
            }
            catch (Exception ex) when (ex is ElementNotInteractableException ||
                                        ex is ElementClickInterceptedException ||
                                        ex.GetType().Name.Contains("Click") ||
                                        ex.GetType().Name.Contains("Interactable"))
            {
                TestContext.WriteLine($"⚠️ Click normal falló ({ex.GetType().Name}), usando JavaScript");
                ScrollToElement(element);
                Wait(500);
                ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", element);
            }
        }
    }

}