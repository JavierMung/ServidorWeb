using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;

namespace T9_AF_2019630211
{
    public static class AgregarAlCarrito
    {
     
        [FunctionName("AgregarAlCarrito")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            try
            {
                log.LogInformation("C# HTTP trigger function processed a request to add an item to the shopping cart.");

                string Server = Environment.GetEnvironmentVariable("Server");
                string UserID = Environment.GetEnvironmentVariable("UserID");
                string Password = Environment.GetEnvironmentVariable("Password");
                string Database = Environment.GetEnvironmentVariable("Database");

                string sc = "Server=" + Server + ";UserID=" + UserID + ";Password=" + Password + ";" + "Database=" + Database + ";SslMode=Preferred;";
                using (var conexion = new MySqlConnection(sc))
                {
                    await conexion.OpenAsync();

                    // Leer los datos del artículo a agregar del cuerpo de la solicitud
                    string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                    dynamic data = JsonConvert.DeserializeObject(requestBody);
                    int articuloId = data != null && data.Id != null ? Convert.ToInt32(data.Id) : 0;
                    int cantidad = data != null && data.cantidad != null ? Convert.ToInt32(data.cantidad) : 0;

                    if (articuloId <= 0 || cantidad <= 0)
                    {
                        return new BadRequestObjectResult("Se deben proporcionar un ID de artículo y una cantidad válida en la solicitud.");
                    }

                    MySqlTransaction transaccion = conexion.BeginTransaction();

                    try
                    {
                        // Verificar la cantidad disponible en la tabla "articulos"
                        var verificarCantidadCmd = new MySqlCommand();
                        verificarCantidadCmd.Connection = conexion;
                        verificarCantidadCmd.Transaction = transaccion;
                        verificarCantidadCmd.CommandText = "SELECT cantidad FROM articulos WHERE idarticulos = @ArticuloId";
                        verificarCantidadCmd.Parameters.AddWithValue("@ArticuloId", articuloId);
                        int cantidadDisponible = Convert.ToInt32(verificarCantidadCmd.ExecuteScalar());

                        if (cantidad > cantidadDisponible)
                        {
                            transaccion.Rollback();
                            var messageError = new { message = "No hay suficiente cantidad disponible del artículo. Cantidad disponible: " + cantidadDisponible };
                            return new BadRequestObjectResult(JsonConvert.SerializeObject(messageError));
                        }

                        // Verificar si el artículo ya está en el carrito_compra
                        var verificarCmd = new MySqlCommand();
                        verificarCmd.Connection = conexion;
                        verificarCmd.Transaction = transaccion;
                        verificarCmd.CommandText = "SELECT COUNT(*) FROM carrito_compra WHERE idarticulos = @ArticuloId";
                        verificarCmd.Parameters.AddWithValue("@ArticuloId", articuloId);
                        int articuloCount = Convert.ToInt32(verificarCmd.ExecuteScalar());

                        if (articuloCount > 0)
                        {
                            // El artículo ya existe en el carrito, actualizar la cantidad
                            var actualizarCmd = new MySqlCommand();
                            actualizarCmd.Connection = conexion;
                            actualizarCmd.Transaction = transaccion;
                            actualizarCmd.CommandText = "UPDATE carrito_compra SET cantidad = cantidad + @Cantidad WHERE idarticulos = @ArticuloId";
                            actualizarCmd.Parameters.AddWithValue("@ArticuloId", articuloId);
                            actualizarCmd.Parameters.AddWithValue("@Cantidad", cantidad);
                            actualizarCmd.ExecuteNonQuery();
                        }
                        else
                        {
                            // El artículo no existe en el carrito, insertarlo
                            var insertarCmd = new MySqlCommand();
                            insertarCmd.Connection = conexion;
                            insertarCmd.Transaction = transaccion;
                            insertarCmd.CommandText = "INSERT INTO carrito_compra (idarticulos, cantidad) VALUES (@ArticuloId, @Cantidad)";
                            insertarCmd.Parameters.AddWithValue("@ArticuloId", articuloId);
                            insertarCmd.Parameters.AddWithValue("@Cantidad", cantidad);
                            insertarCmd.ExecuteNonQuery();
                        }

                        // Actualizar la cantidad disponible del artículo en la tabla "articulos"
                        var actualizarCantidadCmd = new MySqlCommand();
                        actualizarCantidadCmd.Connection = conexion;
                        actualizarCantidadCmd.Transaction = transaccion;
                        actualizarCantidadCmd.CommandText = "UPDATE articulos SET cantidad = cantidad - @Cantidad WHERE idarticulos = @ArticuloId";
                        actualizarCantidadCmd.Parameters.AddWithValue("@ArticuloId", articuloId);
                        actualizarCantidadCmd.Parameters.AddWithValue("@Cantidad", cantidad);
                        actualizarCantidadCmd.ExecuteNonQuery();

                        transaccion.Commit();
                        var message = new { message = "El artículo se ha agregado al carrito correctamente."};
                        return new OkObjectResult(JsonConvert.SerializeObject(message));
                        
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


    }

    public class AddToCartRequest
    {
        public int idarticulos { get; set; }
        public int cantidad { get; set; }
    }
}
