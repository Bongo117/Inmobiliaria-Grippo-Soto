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
                var sql = @"SELECT Id, Dni, Apellido, Nombre, Email, Telefono, Domicilio, Estado 
                            FROM propietarios WHERE Estado = 1";

                using (var command = new MySqlCommand(sql, connection))
                {
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var p = new Propietario
                            {
                                Id = reader.GetInt32("Id"),
                                Dni = reader["Dni"].ToString() ?? "",
                                Apellido = reader["Apellido"].ToString() ?? "",
                                Nombre = reader["Nombre"].ToString() ?? "",
                                Email = reader["Email"] == DBNull.Value ? null : reader["Email"].ToString(),
                                Telefono = reader["Telefono"] == DBNull.Value ? null : reader["Telefono"].ToString(),
                                Domicilio = reader["Domicilio"] == DBNull.Value ? null : reader["Domicilio"].ToString(),
                                Estado = Convert.ToBoolean(reader["Estado"])
                            };
                            lista.Add(p);
                        }
                    }
                }
            }

            return lista;
        }

        public Propietario? ObtenerPorId(int id)
        {
            using (var connection = new MySqlConnection(connectionString))
            {
                var sql = @"SELECT Id, Dni, Apellido, Nombre, Email, Telefono, Domicilio, Estado 
                            FROM propietarios WHERE Id = @id AND Estado = 1";

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@id", id);
                    connection.Open();
                    
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new Propietario
                            {
                                Id = reader.GetInt32("Id"),
                                Dni = reader["Dni"].ToString() ?? "",
                                Apellido = reader["Apellido"].ToString() ?? "",
                                Nombre = reader["Nombre"].ToString() ?? "",
                                Email = reader["Email"] == DBNull.Value ? null : reader["Email"].ToString(),
                                Telefono = reader["Telefono"] == DBNull.Value ? null : reader["Telefono"].ToString(),
                                Domicilio = reader["Domicilio"] == DBNull.Value ? null : reader["Domicilio"].ToString(),
                                Estado = Convert.ToBoolean(reader["Estado"])
                            };
                        }
                    }
                }
            }
            return null;
        }

        public int Alta(Propietario propietario)
        {
            using (var connection = new MySqlConnection(connectionString))
            {
                var sql = @"INSERT INTO propietarios (Dni, Apellido, Nombre, Email, Telefono, Domicilio, Estado) 
                            VALUES (@dni, @apellido, @nombre, @email, @telefono, @domicilio, @estado);
                            SELECT LAST_INSERT_ID();";

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@dni", propietario.Dni ?? "");
                    command.Parameters.AddWithValue("@apellido", propietario.Apellido ?? "");
                    command.Parameters.AddWithValue("@nombre", propietario.Nombre ?? "");
                    command.Parameters.AddWithValue("@email", string.IsNullOrEmpty(propietario.Email) ? DBNull.Value : propietario.Email);
                    command.Parameters.AddWithValue("@telefono", string.IsNullOrEmpty(propietario.Telefono) ? DBNull.Value : propietario.Telefono);
                    command.Parameters.AddWithValue("@domicilio", string.IsNullOrEmpty(propietario.Domicilio) ? DBNull.Value : propietario.Domicilio);
                    command.Parameters.AddWithValue("@estado", propietario.Estado);

                    connection.Open();
                    var result = command.ExecuteScalar();
                    return Convert.ToInt32(result);
                }
            }
        }

        public int Modificacion(Propietario propietario)
        {
            using (var connection = new MySqlConnection(connectionString))
            {
                var sql = @"UPDATE propietarios SET 
                            Dni = @dni, Apellido = @apellido, Nombre = @nombre, 
                            Email = @email, Telefono = @telefono, Domicilio = @domicilio 
                            WHERE Id = @id";

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@id", propietario.Id);
                    command.Parameters.AddWithValue("@dni", propietario.Dni ?? "");
                    command.Parameters.AddWithValue("@apellido", propietario.Apellido ?? "");
                    command.Parameters.AddWithValue("@nombre", propietario.Nombre ?? "");
                    command.Parameters.AddWithValue("@email", string.IsNullOrEmpty(propietario.Email) ? DBNull.Value : propietario.Email);
                    command.Parameters.AddWithValue("@telefono", string.IsNullOrEmpty(propietario.Telefono) ? DBNull.Value : propietario.Telefono);
                    command.Parameters.AddWithValue("@domicilio", string.IsNullOrEmpty(propietario.Domicilio) ? DBNull.Value : propietario.Domicilio);

                    connection.Open();
                    return command.ExecuteNonQuery();
                }
            }
        }

        public int Baja(int id)
        {
            using (var connection = new MySqlConnection(connectionString))
            {
                var sql = "UPDATE propietarios SET Estado = 0 WHERE Id = @id";

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@id", id);
                    connection.Open();
                    return command.ExecuteNonQuery();
                }
            }
        }

        public bool ExisteDni(string dni, int? idExcluir = null)
        {
            using (var connection = new MySqlConnection(connectionString))
            {
                var sql = "SELECT COUNT(*) FROM propietarios WHERE Dni = @dni AND Estado = 1";
                
                if (idExcluir.HasValue)
                {
                    sql += " AND Id != @idExcluir";
                }

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@dni", dni);
                    if (idExcluir.HasValue)
                    {
                        command.Parameters.AddWithValue("@idExcluir", idExcluir.Value);
                    }

                    connection.Open();
                    var result = command.ExecuteScalar();
                    return Convert.ToInt32(result) > 0;
                }
            }
        }
    }
}