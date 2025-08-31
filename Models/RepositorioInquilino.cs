using MySql.Data.MySqlClient;
using System.Collections.Generic;

namespace Inmobiliaria_.Net_Core.Models
{
    public class RepositorioInquilino : RepositorioBase
    {
        public RepositorioInquilino(IConfiguration configuration) : base(configuration) { }

        public List<Inquilino> ObtenerTodos()
        {
            var lista = new List<Inquilino>();

            using (var connection = new MySqlConnection(connectionString))
            {
                var sql = @"SELECT Id, Dni, Apellido, Nombre, Email, Telefono, Domicilio, Estado 
                            FROM inquilinos WHERE Estado = 1";

                using (var command = new MySqlCommand(sql, connection))
                {
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var i = new Inquilino
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
                            lista.Add(i);
                        }
                    }
                }
            }

            return lista;
        }

        public Inquilino? ObtenerPorId(int id)
        {
            using (var connection = new MySqlConnection(connectionString))
            {
                var sql = @"SELECT Id, Dni, Apellido, Nombre, Email, Telefono, Domicilio, Estado 
                            FROM inquilinos WHERE Id = @id AND Estado = 1";

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@id", id);
                    connection.Open();
                    
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new Inquilino
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

        public int Alta(Inquilino inquilino)
        {
            using (var connection = new MySqlConnection(connectionString))
            {
                var sql = @"INSERT INTO inquilinos (Dni, Apellido, Nombre, Email, Telefono, Domicilio, Estado) 
                            VALUES (@dni, @apellido, @nombre, @email, @telefono, @domicilio, @estado);
                            SELECT LAST_INSERT_ID();";

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@dni", inquilino.Dni ?? "");
                    command.Parameters.AddWithValue("@apellido", inquilino.Apellido ?? "");
                    command.Parameters.AddWithValue("@nombre", inquilino.Nombre ?? "");
                    command.Parameters.AddWithValue("@email", string.IsNullOrEmpty(inquilino.Email) ? DBNull.Value : inquilino.Email);
                    command.Parameters.AddWithValue("@telefono", string.IsNullOrEmpty(inquilino.Telefono) ? DBNull.Value : inquilino.Telefono);
                    command.Parameters.AddWithValue("@domicilio", string.IsNullOrEmpty(inquilino.Domicilio) ? DBNull.Value : inquilino.Domicilio);
                    command.Parameters.AddWithValue("@estado", inquilino.Estado);

                    connection.Open();
                    var result = command.ExecuteScalar();
                    return Convert.ToInt32(result);
                }
            }
        }

        public int Modificacion(Inquilino inquilino)
        {
            using (var connection = new MySqlConnection(connectionString))
            {
                var sql = @"UPDATE inquilinos SET 
                            Dni = @dni, Apellido = @apellido, Nombre = @nombre, 
                            Email = @email, Telefono = @telefono, Domicilio = @domicilio 
                            WHERE Id = @id";

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@id", inquilino.Id);
                    command.Parameters.AddWithValue("@dni", inquilino.Dni ?? "");
                    command.Parameters.AddWithValue("@apellido", inquilino.Apellido ?? "");
                    command.Parameters.AddWithValue("@nombre", inquilino.Nombre ?? "");
                    command.Parameters.AddWithValue("@email", string.IsNullOrEmpty(inquilino.Email) ? DBNull.Value : inquilino.Email);
                    command.Parameters.AddWithValue("@telefono", string.IsNullOrEmpty(inquilino.Telefono) ? DBNull.Value : inquilino.Telefono);
                    command.Parameters.AddWithValue("@domicilio", string.IsNullOrEmpty(inquilino.Domicilio) ? DBNull.Value : inquilino.Domicilio);

                    connection.Open();
                    return command.ExecuteNonQuery();
                }
            }
        }

        public int Baja(int id)
        {
            using (var connection = new MySqlConnection(connectionString))
            {
                var sql = "UPDATE inquilinos SET Estado = 0 WHERE Id = @id";

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
                var sql = "SELECT COUNT(*) FROM inquilinos WHERE Dni = @dni AND Estado = 1";
                
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