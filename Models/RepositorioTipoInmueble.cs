using MySql.Data.MySqlClient;
using System.Collections.Generic;

namespace Inmobiliaria_.Net_Core.Models
{
    public class RepositorioTipoInmueble : RepositorioBase
    {
        public RepositorioTipoInmueble(IConfiguration configuration) : base(configuration) { }

        public List<TipoInmueble> ObtenerTodos(bool incluirInactivos = false)
        {
            var lista = new List<TipoInmueble>();

            using (var connection = new MySqlConnection(connectionString))
            {
                var sql = @"SELECT Id, Nombre, Descripcion, UsoPermitido, EsComercial, Estado
                            FROM tipos_inmuebles ";
                if (!incluirInactivos)
                {
                    sql += "WHERE Estado = 1 ";
                }
                sql += "ORDER BY Nombre";

                using (var command = new MySqlCommand(sql, connection))
                {
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var tipoInmueble = new TipoInmueble
                            {
                                Id = reader.GetInt32("Id"),
                                Nombre = reader["Nombre"].ToString() ?? "",
                                Descripcion = reader["Descripcion"] == DBNull.Value ? null : reader["Descripcion"].ToString(),
                                UsoPermitido = reader["UsoPermitido"].ToString() ?? "",
                                EsComercial = Convert.ToBoolean(reader["EsComercial"]),
                                Estado = Convert.ToBoolean(reader["Estado"])
                            };
                            lista.Add(tipoInmueble);
                        }
                    }
                }
            }

            return lista;
        }

        public TipoInmueble? ObtenerPorId(int id)
        {
            using (var connection = new MySqlConnection(connectionString))
            {
                var sql = @"SELECT Id, Nombre, Descripcion, UsoPermitido, EsComercial, Estado 
                            FROM tipos_inmuebles 
                            WHERE Id = @id AND Estado = 1";

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@id", id);
                    connection.Open();
                    
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new TipoInmueble
                            {
                                Id = reader.GetInt32("Id"),
                                Nombre = reader["Nombre"].ToString() ?? "",
                                Descripcion = reader["Descripcion"] == DBNull.Value ? null : reader["Descripcion"].ToString(),
                                UsoPermitido = reader["UsoPermitido"].ToString() ?? "",
                                EsComercial = Convert.ToBoolean(reader["EsComercial"]),
                                Estado = Convert.ToBoolean(reader["Estado"])
                            };
                        }
                    }
                }
            }
            return null;
        }

        public int Alta(TipoInmueble tipoInmueble)
        {
            using (var connection = new MySqlConnection(connectionString))
            {
                var sql = @"INSERT INTO tipos_inmuebles (Nombre, Descripcion, UsoPermitido, EsComercial, Estado) 
                            VALUES (@nombre, @descripcion, @usoPermitido, @esComercial, @estado);
                            SELECT LAST_INSERT_ID();";

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@nombre", tipoInmueble.Nombre ?? "");
                    command.Parameters.AddWithValue("@descripcion", tipoInmueble.Descripcion ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@usoPermitido", tipoInmueble.UsoPermitido ?? "");
                    command.Parameters.AddWithValue("@esComercial", tipoInmueble.EsComercial);
                    command.Parameters.AddWithValue("@estado", tipoInmueble.Estado);

                    connection.Open();
                    var result = command.ExecuteScalar();
                    return Convert.ToInt32(result);
                }
            }
        }

        public int Modificacion(TipoInmueble tipoInmueble)
        {
            using (var connection = new MySqlConnection(connectionString))
            {
                var sql = @"UPDATE tipos_inmuebles SET 
                            Nombre = @nombre, Descripcion = @descripcion, 
                            UsoPermitido = @usoPermitido, EsComercial = @esComercial, 
                            Estado = @estado 
                            WHERE Id = @id";

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@id", tipoInmueble.Id);
                    command.Parameters.AddWithValue("@nombre", tipoInmueble.Nombre ?? "");
                    command.Parameters.AddWithValue("@descripcion", tipoInmueble.Descripcion ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@usoPermitido", tipoInmueble.UsoPermitido ?? "");
                    command.Parameters.AddWithValue("@esComercial", tipoInmueble.EsComercial);
                    command.Parameters.AddWithValue("@estado", tipoInmueble.Estado);

                    connection.Open();
                    return command.ExecuteNonQuery();
                }
            }
        }

        public int Baja(int id)
        {
            using (var connection = new MySqlConnection(connectionString))
            {
                var sql = "UPDATE tipos_inmuebles SET Estado = 0 WHERE Id = @id";

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@id", id);
                    connection.Open();
                    return command.ExecuteNonQuery();
                }
            }
        }

        public bool TieneInmueblesAsignados(int tipoInmuebleId)
        {
            using (var connection = new MySqlConnection(connectionString))
            {
                var sql = @"SELECT COUNT(*) FROM inmuebles 
                           WHERE TipoInmuebleId = @tipoInmuebleId 
                           AND Estado = 1";

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@tipoInmuebleId", tipoInmuebleId);
                    connection.Open();
                    var result = command.ExecuteScalar();
                    return Convert.ToInt32(result) > 0;
                }
            }
        }

        public List<TipoInmueble> ObtenerPorUso(string uso)
        {
            var lista = new List<TipoInmueble>();

            using (var connection = new MySqlConnection(connectionString))
            {
                var sql = @"SELECT Id, Nombre, Descripcion, UsoPermitido, EsComercial, Estado 
                            FROM tipos_inmuebles 
                            WHERE Estado = 1 AND UsoPermitido = @uso
                            ORDER BY Nombre";

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@uso", uso);
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var tipoInmueble = new TipoInmueble
                            {
                                Id = reader.GetInt32("Id"),
                                Nombre = reader["Nombre"].ToString() ?? "",
                                Descripcion = reader["Descripcion"] == DBNull.Value ? null : reader["Descripcion"].ToString(),
                                UsoPermitido = reader["UsoPermitido"].ToString() ?? "",
                                EsComercial = Convert.ToBoolean(reader["EsComercial"]),
                                Estado = Convert.ToBoolean(reader["Estado"])
                            };
                            lista.Add(tipoInmueble);
                        }
                    }
                }
            }

            return lista;
        }
    }
}
