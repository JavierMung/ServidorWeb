using System;
using System.Collections.Generic;
using System.Data;
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
    public static class BuscarArticulos
    {
        [FunctionName("BuscarArticulos")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req,
            ILogger log)
        {
            try
            {
                log.LogInformation("C# HTTP trigger function processed a request to search for articles.");

                string Server = Environment.GetEnvironmentVariable("Server");
                string UserID = Environment.GetEnvironmentVariable("UserID");
                string Password = Environment.GetEnvironmentVariable("Password");
                string Database = Environment.GetEnvironmentVariable("Database");

                string sc = "Server=" + Server + ";UserID=" + UserID + ";Password=" + Password + ";" + "Database=" + Database + ";SslMode=Preferred;";
                using (var conexion = new MySqlConnection(sc))
                {
                    await conexion.OpenAsync();

                    string searchTerm = req.Query["busqueda"];

                    string query = "SELECT idarticulos, nombre, descripcion, precio, cantidad, fotografia FROM articulos";
                    if (!string.IsNullOrEmpty(searchTerm))
                    {
                        query += " WHERE nombre LIKE @searchTerm OR descripcion LIKE @searchTerm";
                    }

                    using (var cmd = new MySqlCommand(query, conexion))
                    {
                        if (!string.IsNullOrEmpty(searchTerm))
                        {
                            cmd.Parameters.AddWithValue("@searchTerm", $"%{searchTerm}%");
                        }

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                           
                                var listaArticulosCarrito = new List<Articulo>();
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
                                    listaArticulosCarrito.Add(articulo);
                                }

                                var result = JsonConvert.SerializeObject(listaArticulosCarrito);
                                return new OkObjectResult(result);
                            
                        }
                    }
                }
            }
            catch (Exception e)
            {
                return new BadRequestObjectResult(e.Message);
            }
        }
    }

}
