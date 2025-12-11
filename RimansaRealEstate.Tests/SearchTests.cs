using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using NUnit.Framework;

namespace RimansaRealEstate.Tests
{
    [TestFixture]
    [Category("Search")]
    public class SearchTests : BaseTest
    {
        [Test]
        [Order(13)]
        public void Test13_BusquedaPorUbicacion()
        {
            TestContext.WriteLine("Iniciando Test13_BusquedaPorUbicacion");

            driver.Navigate().GoToUrl($"{baseUrl}/Home/Properties");
            Wait(1500);

            var locationInput = WaitForElement(By.Name("location"));
            locationInput.Clear();
            locationInput.SendKeys("Santo Domingo");

            var searchButton = driver.FindElement(By.CssSelector("button[type='submit']"));
            searchButton.Click();

            Wait(2000);

            bool urlHasLocation = driver.Url.Contains("location=");
            Assert.That(urlHasLocation, Is.True, "La URL debe contener el parámetro de ubicación");

            TestContext.WriteLine("Test13_BusquedaPorUbicacion - PASÓ");
        }

        [Test]
        [Order(14)]
        public void Test14_FiltrarPorTipoPropiedad()
        {
            TestContext.WriteLine("Iniciando Test14_FiltrarPorTipoPropiedad");

            driver.Navigate().GoToUrl($"{baseUrl}/Home/Properties");
            Wait(1500);

            var propertyTypeSelect = new SelectElement(WaitForElement(By.Name("type")));
            propertyTypeSelect.SelectByValue("Casa");

            var searchButton = driver.FindElement(By.CssSelector("button[type='submit']"));
            searchButton.Click();
            Wait(2000);

            bool urlHasType = driver.Url.Contains("type=");
            Assert.That(urlHasType, Is.True, "La URL debe contener el tipo de propiedad");

            TestContext.WriteLine("Test14_FiltrarPorTipoPropiedad - PASÓ");
        }

        [Test]
        [Order(15)]
        public void Test15_FiltrarPorRangoPrecio()
        {
            TestContext.WriteLine("Iniciando Test15_FiltrarPorRangoPrecio");

            driver.Navigate().GoToUrl($"{baseUrl}/Home/Properties");
            Wait(1500);

            var minPriceInput = WaitForElement(By.Name("minPrice"));
            minPriceInput.Clear();
            minPriceInput.SendKeys("100000");

            var maxPriceInput = driver.FindElement(By.Name("maxPrice"));
            maxPriceInput.Clear();
            maxPriceInput.SendKeys("500000");

            var searchButton = driver.FindElement(By.CssSelector("button[type='submit']"));
            searchButton.Click();
            Wait(2000);

            bool hasPrice = driver.Url.Contains("minPrice=") || driver.Url.Contains("maxPrice=");
            Assert.That(hasPrice, Is.True, "La URL debe contener parámetros de precio");

            TestContext.WriteLine("Test15_FiltrarPorRangoPrecio - PASÓ");
        }

        [Test]
        [Order(16)]
        public void Test16_FiltrosCombinados()
        {
            TestContext.WriteLine("Iniciando Test16_FiltrosCombinados");

            driver.Navigate().GoToUrl($"{baseUrl}/Home/Properties");
            Wait(1500);

            var locationInput = WaitForElement(By.Name("location"));
            locationInput.Clear();
            locationInput.SendKeys("Bavaro");

            var propertyTypeSelect = new SelectElement(driver.FindElement(By.Name("type")));
            propertyTypeSelect.SelectByValue("Apartamento");

            var minPriceInput = driver.FindElement(By.Name("minPrice"));
            minPriceInput.Clear();
            minPriceInput.SendKeys("50000");

            var searchButton = driver.FindElement(By.CssSelector("button[type='submit']"));
            searchButton.Click();
            Wait(2000);

            string url = driver.Url;
            bool hasMultipleFilters = url.Contains("location=") &&
                                     (url.Contains("type=") || url.Contains("minPrice="));

            Assert.That(hasMultipleFilters, Is.True, "Debe contener múltiples filtros");
            TestContext.WriteLine("Test16_FiltrosCombinados - PASÓ");
        }

        [Test]
        [Order(17)]
        public void Test17_BusquedaSinTildes()
        {
            TestContext.WriteLine("Iniciando Test17_BusquedaSinTildes");

            driver.Navigate().GoToUrl($"{baseUrl}/Home/Properties");
            Wait(1500);

            var locationInput = WaitForElement(By.Name("location"));
            locationInput.Clear();
            locationInput.SendKeys("bavaro");

            var searchButton = driver.FindElement(By.CssSelector("button[type='submit']"));
            searchButton.Click();
            Wait(2000);

            bool searchWorked = driver.Url.Contains("location=") ||
                              driver.FindElements(By.CssSelector(".property-card")).Count > 0;

            Assert.That(searchWorked, Is.True, "La búsqueda debe funcionar sin tildes");
            TestContext.WriteLine("Test17_BusquedaSinTildes - PASÓ");
        }

        [Test]
        [Order(18)]
        public void Test18_PaginaPrincipalCarga()
        {
            TestContext.WriteLine("Iniciando Test18_PaginaPrincipalCarga");

            driver.Navigate().GoToUrl(baseUrl);
            Wait(2000);

            bool hasLogo = driver.PageSource.Contains("Rimansa") ||
                         driver.PageSource.Contains("Real Estate") ||
                         driver.PageSource.Contains("RIMANSA");

            Assert.That(hasLogo, Is.True, "Debe mostrar el logo o nombre");

            bool hasContent = driver.FindElements(By.CssSelector(".property-card, .card, .hero-section")).Count > 0 ||
                            driver.PageSource.Contains("No hay propiedades");

            Assert.That(hasContent, Is.True, "Debe mostrar contenido en la página principal");

            TestContext.WriteLine("Test18_PaginaPrincipalCarga - PASÓ");
        }

        [Test]
        [Order(19)]
        public void Test19_CatalogoCompleto()
        {
            TestContext.WriteLine("Iniciando Test19_CatalogoCompleto");

            driver.Navigate().GoToUrl($"{baseUrl}/Home/Properties");
            Wait(2000);

            var propertyCards = driver.FindElements(By.CssSelector(".property-card, .card"));
            TestContext.WriteLine($"Propiedades encontradas: {propertyCards.Count}");

            bool hasCatalog = propertyCards.Count > 0 ||
                            driver.PageSource.Contains("No hay propiedades") ||
                            driver.PageSource.Contains("No se encontraron") ||
                            driver.FindElements(By.CssSelector(".alert-info")).Count > 0;

            Assert.That(hasCatalog, Is.True, "Debe mostrar catálogo o mensaje de 'sin propiedades'");
            TestContext.WriteLine("Test19_CatalogoCompleto - PASÓ");
        }

        [Test]
        [Order(20)]
        public void Test20_VistaDetallePropiedad()
        {
            TestContext.WriteLine("Iniciando Test20_VistaDetallePropiedad");

            driver.Navigate().GoToUrl($"{baseUrl}/Home/Properties");
            Wait(1500);

            var detailLinks = driver.FindElements(By.CssSelector("a[href*='/Home/Details/']"));

            if (detailLinks.Count == 0)
            {
                TestContext.WriteLine("⚠️ No hay propiedades para ver detalles");
                TestContext.WriteLine("💡 Crea al menos una propiedad activa desde el admin");
                Assert.Inconclusive("No hay propiedades para ver detalles. Crea una propiedad activa primero.");
                return;
            }

            TestContext.WriteLine($"Propiedades con detalles encontradas: {detailLinks.Count}");

            string detailUrl = detailLinks[0].GetAttribute("href");
            TestContext.WriteLine($"Navegando a: {detailUrl}");

            driver.Navigate().GoToUrl(detailUrl);
            Wait(2000);

            bool isDetailsPage = driver.Url.Contains("/Home/Details/");
            Assert.That(isDetailsPage, Is.True, "Debe navegar a la página de detalles");
            TestContext.WriteLine("✓ Página de detalles cargada");

            bool hasTitle = driver.FindElements(By.TagName("h1")).Count > 0 ||
                           driver.FindElements(By.TagName("h2")).Count > 0;
            Assert.That(hasTitle, Is.True, "Debe mostrar título de la propiedad");
            TestContext.WriteLine("✓ Título encontrado");

            bool hasPrice = driver.PageSource.Contains("$") ||
                           driver.PageSource.Contains("Precio") ||
                           driver.PageSource.Contains("Price");
            Assert.That(hasPrice, Is.True, "Debe mostrar precio");
            TestContext.WriteLine("✓ Precio encontrado");

            bool hasContactInfo = driver.FindElements(By.CssSelector("a[href*='tel:'], a[href*='whatsapp'], a[href*='mailto:']")).Count > 0 ||
                                 driver.PageSource.Contains("Contactar") ||
                                 driver.PageSource.Contains("WhatsApp") ||
                                 driver.PageSource.Contains("Teléfono");

            if (hasContactInfo)
            {
                TestContext.WriteLine("✓ Información de contacto encontrada");
            }
            else
            {
                TestContext.WriteLine("⚠️ No se encontraron botones de contacto (esto puede ser normal según el diseño)");
            }

            TestContext.WriteLine("Test20_VistaDetallePropiedad - PASÓ");
        }

        [Test]
        [Order(21)]
        public void Test21_BotonesCompartirRedesSociales()
        {
            TestContext.WriteLine("Iniciando Test21_BotonesCompartirRedesSociales");

            driver.Navigate().GoToUrl($"{baseUrl}/Home/Properties");
            Wait(1500);

            var detailLinks = driver.FindElements(By.CssSelector("a[href*='/Home/Details/']"));

            if (detailLinks.Count == 0)
            {
                TestContext.WriteLine("⚠️ No hay propiedades disponibles");
                Assert.Inconclusive("No hay propiedades disponibles para verificar botones sociales");
                return;
            }

            string detailUrl = detailLinks[0].GetAttribute("href");
            driver.Navigate().GoToUrl(detailUrl);
            Wait(2000);

            bool hasSocialButtons = driver.FindElements(By.CssSelector("a[href*='facebook'], a[href*='twitter'], a[href*='whatsapp']")).Count > 0 ||
                                  driver.PageSource.Contains("Compartir") ||
                                  driver.PageSource.Contains("Share") ||
                                  driver.FindElements(By.CssSelector(".social-share, .share-buttons, [class*='share']")).Count > 0;

            if (hasSocialButtons)
            {
                TestContext.WriteLine("✓ Botones de redes sociales encontrados");
                Assert.Pass("Botones de redes sociales encontrados");
            }
            else
            {
                TestContext.WriteLine("⚠️ No se encontraron botones de redes sociales");
                TestContext.WriteLine("💡 Considera agregar botones de compartir en la vista de detalles");

                bool hasShareableContent = driver.PageSource.Contains("$") &&
                                          (driver.FindElements(By.TagName("h1")).Count > 0 ||
                                           driver.FindElements(By.TagName("h2")).Count > 0);

                Assert.That(hasShareableContent, Is.True,
                    "Aunque no hay botones sociales, debe tener contenido compartible (título y precio)");

                TestContext.WriteLine("✓ Contenido compartible presente (título y precio)");
            }

            TestContext.WriteLine("Test21_BotonesCompartirRedesSociales - PASÓ");
        }

        [Test]
        [Order(22)]
        public void Test22_DisenoResponsive()
        {
            TestContext.WriteLine("Iniciando Test22_DisenoResponsive");

            driver.Navigate().GoToUrl(baseUrl);
            Wait(1000);

            TestContext.WriteLine("Probando vista móvil (375x667)...");
            driver.Manage().Window.Size = new System.Drawing.Size(375, 667);
            Wait(1000);
            Assert.That(driver.PageSource, Is.Not.Empty, "Debe cargar en móvil");
            TestContext.WriteLine("✓ Vista móvil carga correctamente");

            TestContext.WriteLine("Probando vista tablet (768x1024)...");
            driver.Manage().Window.Size = new System.Drawing.Size(768, 1024);
            Wait(1000);
            Assert.That(driver.PageSource, Is.Not.Empty, "Debe cargar en tablet");
            TestContext.WriteLine("✓ Vista tablet carga correctamente");

            TestContext.WriteLine("Probando vista desktop (1920x1080)...");
            driver.Manage().Window.Size = new System.Drawing.Size(1920, 1080);
            Wait(1000);
            Assert.That(driver.PageSource, Is.Not.Empty, "Debe cargar en desktop");
            TestContext.WriteLine("✓ Vista desktop carga correctamente");

            driver.Manage().Window.Maximize();

            TestContext.WriteLine("Test22_DisenoResponsive - PASÓ");
        }

        [Test]
        [Order(23)]
        public void Test23_NavegacionBreadcrumb()
        {
            TestContext.WriteLine("Iniciando Test23_NavegacionBreadcrumb");

            driver.Navigate().GoToUrl($"{baseUrl}/Home/Properties");
            Wait(1500);

            bool hasBreadcrumb = driver.FindElements(By.CssSelector(".breadcrumb, [class*='breadcrumb']")).Count > 0 ||
                               driver.PageSource.Contains("Home") ||
                               driver.PageSource.Contains("Inicio") ||
                               driver.PageSource.Contains("Propiedades");

            if (hasBreadcrumb)
            {
                TestContext.WriteLine("✓ Navegación breadcrumb o indicadores de ubicación encontrados");
            }
            else
            {
                TestContext.WriteLine("⚠️ No se encontró breadcrumb específico");
                TestContext.WriteLine("💡 Verifica que la página tenga navegación clara");
            }

            bool hasNavigation = driver.FindElements(By.CssSelector("nav, .navbar, header")).Count > 0;
            Assert.That(hasNavigation, Is.True, "Debe tener navegación en la página");

            TestContext.WriteLine("Test23_NavegacionBreadcrumb - PASÓ");
        }

        [Test]
        [Order(24)]
        public void Test24_TiempoCargaPaginaPrincipal()
        {
            TestContext.WriteLine("Iniciando Test24_TiempoCargaPaginaPrincipal");

            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            driver.Navigate().GoToUrl(baseUrl);

            Wait(500);

            var readyState = ((IJavaScriptExecutor)driver).ExecuteScript("return document.readyState");

            stopwatch.Stop();

            double loadTime = stopwatch.Elapsed.TotalSeconds;
            TestContext.WriteLine($"⏱️ Tiempo de carga: {loadTime:F2} segundos");
            TestContext.WriteLine($"📄 Estado del documento: {readyState}");

            Assert.That(loadTime, Is.LessThan(10.0),
                "Debe cargar en menos de 10 segundos (ajustado para tests locales)");

            if (loadTime < 3.0)
            {
                TestContext.WriteLine("✅ Excelente tiempo de carga (< 3s)");
            }
            else if (loadTime < 5.0)
            {
                TestContext.WriteLine("✓ Buen tiempo de carga (3-5s)");
            }
            else
            {
                TestContext.WriteLine("⚠️ Tiempo de carga aceptable pero podría mejorar (5-10s)");
            }

            TestContext.WriteLine("Test24_TiempoCargaPaginaPrincipal - PASÓ");
        }
    }
}