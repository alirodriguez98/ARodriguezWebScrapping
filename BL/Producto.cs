using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL
{
    public class Producto
    {
        public static Dictionary<string, object> Add(ML.Producto producto)
        {
            Dictionary<string, object> diccionario = new Dictionary<string, object>() { { "Resultado", false }, { "Excepcion", "" } };

            try
            {
                using (DL.ArodriguezWebScrappingContext context = new DL.ArodriguezWebScrappingContext())
                {
                    var filasAfectadas = context.Database.ExecuteSqlRaw($"ProductoAdd '{producto.Nombre}', {producto.Precio} '{producto.RutaImagen}'");

                    if(filasAfectadas > 0)
                    {
                        diccionario["Resultado"] = true;
                    }
                    else
                    {
                        diccionario["Resultado"] = false;
                    }
                }
            }
            catch (Exception ex)
            {
                diccionario["Excepcion"] = ex.Message;
                diccionario["Resultado"] = false;
                throw;
            }

            return diccionario;
        }
    }
}
