using CsvHelper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.VisualBasic.FileIO;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using System.Data;
using System.Data.OleDb;
using System.Globalization;
using System.IO;

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

            int numeroProducto = 0;

            foreach(var product in productElements)
            {
                ML.Producto producto = new ML.Producto();
                producto.IdProducto = numeroProducto++;
                var nombreElement = product.FindElements(By.ClassName("ui-search-item__title"));
                foreach(var nombre in nombreElement)
                {
                    string nombreConvertido = "";
                    foreach(var letra in nombre.Text)
                    {
                        var letraChar = letra;
                        if(letraChar.ToString().Equals(","))
                        {
                            letraChar = '|';
                        }
                        nombreConvertido += letraChar;
                    }
                    producto.Nombre = nombreConvertido;
                }  
                var precioElement = product.FindElement(By.ClassName("andes-money-amount__fraction"));
                producto.Precio = float.Parse(precioElement.Text, CultureInfo.InvariantCulture.NumberFormat);
                var rutaImagenElement = product.FindElement(By.ClassName("ui-search-result-image__element"));
                producto.RutaImagen = rutaImagenElement.GetAttribute("src");

                productoLista.Productos.Add(producto);
            }

            using (var writer = new StreamWriter("productos.csv"))

            using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
            {
                csv.WriteRecords(productoLista.Productos);
            }

            driver.Quit();

            return View(productoLista);
        }

        
        public IActionResult GuardarDatos()
        {
            try
            {
                string exportPath = @"C:\Users\digis\Documents\Ali Rodriguez Cortes\apuntes\WebScrapping\ARodriguezWebScrapping\WebScrapping\";
                string exportCsv = "productos.csv";

                StreamWriter csvFile = null;

                if (!Directory.Exists(exportPath))
                {

                    // Display a message stating file path does not exist.
                    Console.WriteLine("File path does not exist.");

                    // Stop program execution.
                    System.Environment.Exit(1);

                }

                DataTable dataTable = new DataTable();
                using (StreamReader reader = new StreamReader(exportPath + exportCsv))
                {
                    string[] headers = reader.ReadLine().Split(',');
                    foreach (string header in headers)
                    {
                        dataTable.Columns.Add(header);
                    }
                    while (!reader.EndOfStream)
                    {
                        string[] rows = reader.ReadLine().Split(',');
                        dataTable.Rows.Add(rows);
                    }
                }

                using (SqlConnection connection = new SqlConnection(DL.ARodriguezWebScrappingConnection.GetConnectionString()))
                using (SqlBulkCopy bc = new SqlBulkCopy(connection))
                {
                    bc.DestinationTableName = "dbo.Producto";
                    for (int i = 0; i < dataTable.Columns.Count; i++)
                    {
                        bc.ColumnMappings.Add(i, i);
                    }
                    connection.Open();
                    bc.WriteToServer(dataTable);
                    connection.Close();
                }

                ViewBag.Mensaje = "Carga de datos correcta";
                return PartialView("Modal");
            }
            catch (Exception ex)
            {
                ViewBag.Mensaje = "Ocurrio un error: " + ex.Message;
                return PartialView("Modal");
            }
        }
    }
}
