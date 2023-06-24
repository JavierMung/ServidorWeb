using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using MySql.Data.MySqlClient;

namespace T9_AF_2019630211
{
    public static class CapturaArticulo
    {
        [FunctionName("CapturaArticulo")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequest req, ILogger log)
        {
            try
            {
                log.LogInformation("C# HTTP trigger function processed a request for CapturaArticulo.");

                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                dynamic data = JsonConvert.DeserializeObject(requestBody);

                string nombreArticulo = data?.nombre;
                string descripcionArticulo = data?.descripcion;
                decimal precioArticulo = data.precio;
                int cantidad = data.cantidad;
                string foto = data?.foto;
                byte[] fotoArticulo = Convert.FromBase64String(foto);

                if (string.IsNullOrEmpty(nombreArticulo) || string.IsNullOrEmpty(descripcionArticulo)) {
                    return new BadRequestObjectResult("Invalid request body. Please provide all required fields.");
                }

                string server = Environment.GetEnvironmentVariable("Server");
                string userId = Environment.GetEnvironmentVariable("UserId");
                string password = Environment.GetEnvironmentVariable("Password");
                string database = Environment.GetEnvironmentVariable("Database");

                string connectionString = $"Server={server};UserId={userId};Password={password};Database={database};SslMode=Preferred;";
                using (var connection = new MySqlConnection(connectionString))
                {
                    await connection.OpenAsync();

                    var command = new MySqlCommand();
                    command.Connection = connection;
                    command.CommandText = "INSERT INTO articulos (nombre, descripcion, precio, cantidad, fotografia) " +
                                          "VALUES (@nombre, @descripcion, @precio, @cantidad, @foto) ";
                    command.Parameters.AddWithValue("@nombre", nombreArticulo);
                    command.Parameters.AddWithValue("@descripcion", descripcionArticulo);
                    command.Parameters.AddWithValue("@precio", precioArticulo);
                    command.Parameters.AddWithValue("@cantidad", cantidad);
                    command.Parameters.AddWithValue("@foto", fotoArticulo);

                    await command.ExecuteNonQueryAsync();
                }

                return new OkObjectResult("Artículo capturado exitosamente.");
            }
            catch (Exception e)
            {
                return new BadRequestObjectResult(e.Message);
            }
        }
    }
}
