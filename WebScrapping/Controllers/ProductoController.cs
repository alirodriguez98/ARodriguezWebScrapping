using Microsoft.AspNetCore.Mvc;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using System.Globalization;

namespace WebScrapping.Controllers
{
    public class ProductoController : Controller
    {
        public IActionResult WebScrapping()
        {
            ML.Producto productoLista = new ML.Producto();
            productoLista.Productos = new List<object>();

            IWebDriver driver = new FirefoxDriver();

            driver.Navigate().GoToUrl("https://www.mercadolibre.com.mx/");

            var searchText = driver.FindElement(By.ClassName("nav-search-input"));
            searchText.SendKeys("petacas miguel");
            searchText.SendKeys(Keys.Enter);

            var productElements = driver.FindElements(By.ClassName("ui-search-layout__item"));

            foreach(var product in productElements)
            {
                ML.Producto producto = new ML.Producto();
                var nombreElement = product.FindElement(By.ClassName("ui-search-item__title"));
                producto.Nombre = nombreElement.Text;
                var precioElement = product.FindElement(By.ClassName("andes-money-amount__fraction"));
                producto.Precio = float.Parse(precioElement.Text, CultureInfo.InvariantCulture.NumberFormat);
                var rutaImagenElement = product.FindElement(By.ClassName("ui-search-result-image__element"));
                producto.RutaImagen = rutaImagenElement.GetAttribute("src");

                productoLista.Productos.Add(producto);
            }

            return View(productoLista);
        }
    }
}
