using MySql.Data.MySqlClient;
using System.Collections.Generic;

namespace Inmobiliaria_.Net_Core.Models
{
    public class RepositorioPropietario : RepositorioBase
    {
        public RepositorioPropietario(IConfiguration configuration) : base(configuration) { }

        public List<Propietario> ObtenerTodos()
        {
            var lista = new List<Propietario>();

            using (var connection = new MySqlConnection(connectionString))
            {
                var sql = @"SELECT IdPropietario, Nombre, Apellido, Dni, Telefono, Email 
                            FROM Propietarios";

                using (var command = new MySqlCommand(sql, connection))
                {
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var p = new Propietario
                            {
                                IdPropietario = reader.GetInt32("IdPropietario"),
                                Nombre = reader.GetString("Nombre"),
                                Apellido = reader.GetString("Apellido"),
                                Dni = reader.GetString("Dni"),
                                Telefono = reader.GetString("Telefono"),
                                Email = reader.GetString("Email")
                            };
                            lista.Add(p);
                        }
                    }
                }
            }

            return lista;
        }
    }
}
