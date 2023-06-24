using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace T9_AF_2019630211
{
    // ...
    public static class EliminarArticuloCarrito
    {
        // ...

        [FunctionName("EliminarArticuloCarrito")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            try
            {
                log.LogInformation("C# HTTP trigger function processed a request to remove an item from the shopping cart.");

                string Server = Environment.GetEnvironmentVariable("Server");
                string UserID = Environment.GetEnvironmentVariable("UserID");
                string Password = Environment.GetEnvironmentVariable("Password");
                string Database = Environment.GetEnvironmentVariable("Database");

                string sc = "Server=" + Server + ";UserID=" + UserID + ";Password=" + Password + ";" + "Database=" + Database + ";SslMode=Preferred;";
                using (var conexion = new MySqlConnection(sc))
                {
                    await conexion.OpenAsync();

                    // Leer los datos del artículo a eliminar del cuerpo de la solicitud
                    string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                    dynamic data = JsonConvert.DeserializeObject(requestBody);
                    int articuloId = data != null && data.Id != null ? Convert.ToInt32(data.Id) : 0;

                    if (articuloId <= 0)
                    {
                        return new BadRequestObjectResult("Se debe proporcionar un ID de artículo válido en la solicitud.");
                    }

                    MySqlTransaction transaccion = conexion.BeginTransaction();

                    try
                    {
                        // Verificar si el artículo existe en el carrito_compra
                        var verificarCmd = new MySqlCommand();
                        verificarCmd.Connection = conexion;
                        verificarCmd.Transaction = transaccion;
                        verificarCmd.CommandText = "SELECT COUNT(*) FROM carrito_compra WHERE idarticulos = @ArticuloId";
                        verificarCmd.Parameters.AddWithValue("@ArticuloId", articuloId);
                        int articuloCount = Convert.ToInt32(verificarCmd.ExecuteScalar());

                        if (articuloCount == 0)
                        {
                            return new BadRequestObjectResult("El artículo no existe en el carrito de compras.");
                        }

                        // Obtener la cantidad del artículo en el carrito_compra
                        var obtenerCantidadCmd = new MySqlCommand();
                        obtenerCantidadCmd.Connection = conexion;
                        obtenerCantidadCmd.Transaction = transaccion;
                        obtenerCantidadCmd.CommandText = "SELECT Cantidad FROM carrito_compra WHERE idarticulos = @ArticuloId";
                        obtenerCantidadCmd.Parameters.AddWithValue("@ArticuloId", articuloId);
                        int cantidadArticulo = Convert.ToInt32(obtenerCantidadCmd.ExecuteScalar());

                        // Verificar si la cantidad a eliminar es mayor que la cantidad actual
                        int cantidadEliminar = data != null && data.cantidad != null ? Convert.ToInt32(data.cantidad) : 0;
                        if (cantidadEliminar > cantidadArticulo)
                        {
                            return new BadRequestObjectResult("La cantidad a eliminar es mayor que la cantidad actual en el carrito de compras.");
                        }

                        // Actualizar la tabla de artículos
                        if (cantidadArticulo == cantidadEliminar)
                        {
                            // Eliminar el artículo del carrito_compra si la cantidad es cero
                            var eliminarCmd = new MySqlCommand();
                            eliminarCmd.Connection = conexion;
                            eliminarCmd.Transaction = transaccion;
                            eliminarCmd.CommandText = "DELETE FROM carrito_compra WHERE idarticulos = @ArticuloId";
                            eliminarCmd.Parameters.AddWithValue("@ArticuloId", articuloId);
                            eliminarCmd.ExecuteNonQuery();
                        }
                        else
                        {
                            // Actualizar la cantidad del artículo en el carrito_compra
                            var actualizarCmd = new MySqlCommand();
                            actualizarCmd.Connection = conexion;
                            actualizarCmd.Transaction = transaccion;
                            actualizarCmd.CommandText = "UPDATE carrito_compra SET Cantidad = Cantidad - @Cantidad WHERE idarticulos = @ArticuloId";
                            actualizarCmd.Parameters.AddWithValue("@Cantidad", cantidadEliminar);
                            actualizarCmd.Parameters.AddWithValue("@ArticuloId", articuloId);
                            actualizarCmd.ExecuteNonQuery();
                        }


                        var actualizarArt = new MySqlCommand();
                        actualizarArt.Connection = conexion;
                        actualizarArt.Transaction = transaccion;
                        actualizarArt.CommandText = "UPDATE articulos SET cantidad = cantidad + @Cantidad WHERE idarticulos = @ArticuloId";
                        actualizarArt.Parameters.AddWithValue("@Cantidad", cantidadEliminar);
                        actualizarArt.Parameters.AddWithValue("@ArticuloId", articuloId);
                        actualizarArt.ExecuteNonQuery();
                        // Commit de la transacción
                        transaccion.Commit();

                        return new OkObjectResult("El artículo se ha eliminado del carrito correctamente.");
                    }
                    catch (Exception e)
                    {
                        transaccion.Rollback();
                        return new BadRequestObjectResult(e.Message);
                    }
                }
            }
            catch (Exception e)
            {
                return new BadRequestObjectResult(e.Message);
            }
        }

        // ...

    }
}
