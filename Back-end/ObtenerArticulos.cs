using System;
using System.Collections.Generic;
using System.Data;
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
    public static class ObtenerArticulos
    {
        [FunctionName("ObtenerArticulos")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get")] HttpRequest req, ILogger log)
        {
            try
            {
                log.LogInformation("C# HTTP trigger function processed a request for ObtenerArticulos.");

                string server = Environment.GetEnvironmentVariable("Server");
                string userId = Environment.GetEnvironmentVariable("UserId");
                string password = Environment.GetEnvironmentVariable("Password");
                string database = Environment.GetEnvironmentVariable("Database");

                string connectionString = $"Server={server};UserId={userId};Password={password};Database={database};SslMode=Preferred;";
                using (var connection = new MySqlConnection(connectionString))
                {
                    await connection.OpenAsync();

                    var command = new MySqlCommand("SELECT * FROM articulos", connection);
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        List<Articulo> articulos = new List<Articulo>();
                        while (reader.Read())
                        {
                            Articulo articulo = new Articulo();
                            articulo.Id = reader.GetInt32("idarticulos");
                            articulo.Nombre = reader.GetString("nombre");
                            articulo.Descripcion = reader.GetString("descripcion");
                            articulo.Precio = reader.GetDecimal("precio");
                            articulo.cantidad = reader.GetInt32("cantidad");
                            if (!reader.IsDBNull(reader.GetOrdinal("fotografia")))
                            {
                                byte[] fotoBytes = (byte[])reader["fotografia"];
                                articulo.Foto = Convert.ToBase64String(fotoBytes);
                            }
                            else
                            {
                                articulo.Foto = null;
                            }

                            articulos.Add(articulo);
                        }

                        return new OkObjectResult(JsonConvert.SerializeObject(articulos));
                    }
                }
            }
            catch (Exception e)
            {
                return new BadRequestObjectResult(e.Message);
            }
        }
    }

    public class Articulo
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public decimal Precio { get; set; }
        public int cantidad { get; set; }
        public string Foto { get; set; }
    }
}
