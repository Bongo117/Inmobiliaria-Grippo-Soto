using MySql.Data.MySqlClient;
using System.Collections.Generic;

namespace Inmobiliaria_.Net_Core.Models
{
    public class RepositorioInmueble : RepositorioBase
    {
        public RepositorioInmueble(IConfiguration configuration) : base(configuration) { }

        public List<Inmueble> ObtenerTodos()
        {
            var lista = new List<Inmueble>();

            using (var connection = new MySqlConnection(connectionString))
            {
                var sql = @"SELECT i.Id, i.Direccion, i.Tipo, i.TipoInmuebleId, i.Ambientes, i.Precio, i.PropietarioId, i.Estado,
                            p.Dni, p.Apellido, p.Nombre, p.Email, p.Telefono, p.Domicilio,
                            ti.Nombre AS TipoNombre, ti.Descripcion AS TipoDescripcion, ti.UsoPermitido, ti.EsComercial
                            FROM inmuebles i
                            INNER JOIN propietarios p ON i.PropietarioId = p.Id
                            LEFT JOIN tipos_inmuebles ti ON i.TipoInmuebleId = ti.Id
                            WHERE i.Estado = 1";

                using (var command = new MySqlCommand(sql, connection))
                {
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var inmueble = new Inmueble
                            {
                                Id = reader.GetInt32("Id"),
                                Direccion = reader["Direccion"].ToString() ?? "",
                                Tipo = reader["Tipo"].ToString() ?? "",
                                TipoInmuebleId = reader["TipoInmuebleId"] == DBNull.Value ? null : reader.GetInt32("TipoInmuebleId"),
                                Ambientes = reader.GetInt32("Ambientes"),
                                Precio = reader.GetDecimal("Precio"),
                                PropietarioId = reader.GetInt32("PropietarioId"),
                                Estado = Convert.ToBoolean(reader["Estado"]),
                                Propietario = new Propietario
                                {
                                    Id = reader.GetInt32("PropietarioId"),
                                    Dni = reader["Dni"].ToString() ?? "",
                                    Apellido = reader["Apellido"].ToString() ?? "",
                                    Nombre = reader["Nombre"].ToString() ?? "",
                                    Email = reader["Email"] == DBNull.Value ? null : reader["Email"].ToString(),
                                    Telefono = reader["Telefono"] == DBNull.Value ? null : reader["Telefono"].ToString(),
                                    Domicilio = reader["Domicilio"] == DBNull.Value ? null : reader["Domicilio"].ToString()
                                },
                                TipoInmueble = reader["TipoInmuebleId"] == DBNull.Value ? null : new TipoInmueble
                                {
                                    Id = reader.GetInt32("TipoInmuebleId"),
                                    Nombre = reader["TipoNombre"]?.ToString() ?? "",
                                    Descripcion = reader["TipoDescripcion"] == DBNull.Value ? null : reader["TipoDescripcion"].ToString(),
                                    UsoPermitido = reader["UsoPermitido"]?.ToString() ?? "",
                                    EsComercial = reader["EsComercial"] == DBNull.Value ? false : Convert.ToBoolean(reader["EsComercial"])
                                }
                            };
                            lista.Add(inmueble);
                        }
                    }
                }
            }

            return lista;
        }

        public Inmueble? ObtenerPorId(int id)
        {
            using (var connection = new MySqlConnection(connectionString))
            {
                var sql = @"SELECT i.Id, i.Direccion, i.Tipo, i.TipoInmuebleId, i.Ambientes, i.Precio, i.PropietarioId, i.Estado,
                            p.Dni, p.Apellido, p.Nombre, p.Email, p.Telefono, p.Domicilio,
                            ti.Nombre AS TipoNombre, ti.Descripcion AS TipoDescripcion, ti.UsoPermitido, ti.EsComercial
                            FROM inmuebles i
                            INNER JOIN propietarios p ON i.PropietarioId = p.Id
                            LEFT JOIN tipos_inmuebles ti ON i.TipoInmuebleId = ti.Id
                            WHERE i.Id = @id AND i.Estado = 1";

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@id", id);
                    connection.Open();
                    
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new Inmueble
                            {
                                Id = reader.GetInt32("Id"),
                                Direccion = reader["Direccion"].ToString() ?? "",
                                Tipo = reader["Tipo"].ToString() ?? "",
                                TipoInmuebleId = reader["TipoInmuebleId"] == DBNull.Value ? null : reader.GetInt32("TipoInmuebleId"),
                                Ambientes = reader.GetInt32("Ambientes"),
                                Precio = reader.GetDecimal("Precio"),
                                PropietarioId = reader.GetInt32("PropietarioId"),
                                Estado = Convert.ToBoolean(reader["Estado"]),
                                Propietario = new Propietario
                                {
                                    Id = reader.GetInt32("PropietarioId"),
                                    Dni = reader["Dni"].ToString() ?? "",
                                    Apellido = reader["Apellido"].ToString() ?? "",
                                    Nombre = reader["Nombre"].ToString() ?? "",
                                    Email = reader["Email"] == DBNull.Value ? null : reader["Email"].ToString(),
                                    Telefono = reader["Telefono"] == DBNull.Value ? null : reader["Telefono"].ToString(),
                                    Domicilio = reader["Domicilio"] == DBNull.Value ? null : reader["Domicilio"].ToString()
                                },
                                TipoInmueble = reader["TipoInmuebleId"] == DBNull.Value ? null : new TipoInmueble
                                {
                                    Id = reader.GetInt32("TipoInmuebleId"),
                                    Nombre = reader["TipoNombre"]?.ToString() ?? "",
                                    Descripcion = reader["TipoDescripcion"] == DBNull.Value ? null : reader["TipoDescripcion"].ToString(),
                                    UsoPermitido = reader["UsoPermitido"]?.ToString() ?? "",
                                    EsComercial = reader["EsComercial"] == DBNull.Value ? false : Convert.ToBoolean(reader["EsComercial"])
                                }
                            };
                        }
                    }
                }
            }
            return null;
        }

        public int Alta(Inmueble inmueble)
        {
            using (var connection = new MySqlConnection(connectionString))
            {
                var sql = @"INSERT INTO inmuebles (Direccion, Tipo, TipoInmuebleId, Ambientes, Precio, PropietarioId, Estado) 
                            VALUES (@direccion, @tipo, @tipoInmuebleId, @ambientes, @precio, @propietarioId, @estado);
                            SELECT LAST_INSERT_ID();";

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@direccion", inmueble.Direccion ?? "");
                    command.Parameters.AddWithValue("@tipo", inmueble.Tipo ?? "");
                    command.Parameters.AddWithValue("@tipoInmuebleId", inmueble.TipoInmuebleId ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@ambientes", inmueble.Ambientes);
                    command.Parameters.AddWithValue("@precio", inmueble.Precio);
                    command.Parameters.AddWithValue("@propietarioId", inmueble.PropietarioId);
                    command.Parameters.AddWithValue("@estado", inmueble.Estado);

                    connection.Open();
                    var result = command.ExecuteScalar();
                    return Convert.ToInt32(result);
                }
            }
        }

        public int Modificacion(Inmueble inmueble)
        {
            using (var connection = new MySqlConnection(connectionString))
            {
                var sql = @"UPDATE inmuebles SET 
                            Direccion = @direccion, Tipo = @tipo, TipoInmuebleId = @tipoInmuebleId, 
                            Ambientes = @ambientes, Precio = @precio, PropietarioId = @propietarioId 
                            WHERE Id = @id";

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@id", inmueble.Id);
                    command.Parameters.AddWithValue("@direccion", inmueble.Direccion ?? "");
                    command.Parameters.AddWithValue("@tipo", inmueble.Tipo ?? "");
                    command.Parameters.AddWithValue("@tipoInmuebleId", inmueble.TipoInmuebleId ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@ambientes", inmueble.Ambientes);
                    command.Parameters.AddWithValue("@precio", inmueble.Precio);
                    command.Parameters.AddWithValue("@propietarioId", inmueble.PropietarioId);

                    connection.Open();
                    return command.ExecuteNonQuery();
                }
            }
        }

        public int Baja(int id)
        {
            using (var connection = new MySqlConnection(connectionString))
            {
                var sql = "UPDATE inmuebles SET Estado = 0 WHERE Id = @id";

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@id", id);
                    connection.Open();
                    return command.ExecuteNonQuery();
                }
            }
        }

        public List<Inmueble> ObtenerPorPropietario(int propietarioId)
        {
            var lista = new List<Inmueble>();

            using (var connection = new MySqlConnection(connectionString))
            {
                var sql = @"SELECT i.Id, i.Direccion, i.Tipo, i.Ambientes, i.Precio, i.PropietarioId, i.Estado,
                            p.Dni, p.Apellido, p.Nombre, p.Email, p.Telefono, p.Domicilio
                            FROM inmuebles i
                            INNER JOIN propietarios p ON i.PropietarioId = p.Id
                            WHERE i.PropietarioId = @propietarioId AND i.Estado = 1";

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@propietarioId", propietarioId);
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var inmueble = new Inmueble
                            {
                                Id = reader.GetInt32("Id"),
                                Direccion = reader["Direccion"].ToString() ?? "",
                                Tipo = reader["Tipo"].ToString() ?? "",
                                Ambientes = reader.GetInt32("Ambientes"),
                                Precio = reader.GetDecimal("Precio"),
                                PropietarioId = reader.GetInt32("PropietarioId"),
                                Estado = Convert.ToBoolean(reader["Estado"]),
                                Propietario = new Propietario
                                {
                                    Id = reader.GetInt32("PropietarioId"),
                                    Dni = reader["Dni"].ToString() ?? "",
                                    Apellido = reader["Apellido"].ToString() ?? "",
                                    Nombre = reader["Nombre"].ToString() ?? ""
                                }
                            };
                            lista.Add(inmueble);
                        }
                    }
                }
            }

            return lista;
        }

        public bool TieneContratosVigentes(int inmuebleId)
        {
            using (var connection = new MySqlConnection(connectionString))
            {
                var sql = @"SELECT COUNT(*) FROM contratos 
                           WHERE InmuebleId = @inmuebleId 
                           AND Estado = 1 
                           AND CURDATE() BETWEEN FechaInicio AND FechaFin";

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@inmuebleId", inmuebleId);
                    connection.Open();
                    var result = command.ExecuteScalar();
                    return Convert.ToInt32(result) > 0;
                }
            }
        }

        public List<Inmueble> ObtenerDisponiblesEntreFechas(DateTime fechaDesde, DateTime fechaHasta)
        {
            var lista = new List<Inmueble>();

            using (var connection = new MySqlConnection(connectionString))
            {
                var sql = @"SELECT i.Id, i.Direccion, i.Tipo, i.TipoInmuebleId, i.Ambientes, i.Precio, i.PropietarioId, i.Estado,
                            p.Dni, p.Apellido, p.Nombre, p.Email, p.Telefono, p.Domicilio,
                            ti.Nombre AS TipoNombre, ti.Descripcion AS TipoDescripcion, ti.UsoPermitido, ti.EsComercial
                            FROM inmuebles i
                            INNER JOIN propietarios p ON i.PropietarioId = p.Id
                            LEFT JOIN tipos_inmuebles ti ON i.TipoInmuebleId = ti.Id
                            WHERE i.Estado = 1 AND NOT EXISTS (
                                SELECT 1 FROM contratos c
                                WHERE c.InmuebleId = i.Id
                                  AND c.Estado = 1
                                  AND NOT (c.FechaFin < @desde OR c.FechaInicio > @hasta)
                            )";

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@desde", fechaDesde.Date);
                    command.Parameters.AddWithValue("@hasta", fechaHasta.Date);
                    connection.Open();

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var inmueble = new Inmueble
                            {
                                Id = reader.GetInt32("Id"),
                                Direccion = reader["Direccion"].ToString() ?? "",
                                Tipo = reader["Tipo"].ToString() ?? "",
                                TipoInmuebleId = reader["TipoInmuebleId"] == DBNull.Value ? null : reader.GetInt32("TipoInmuebleId"),
                                Ambientes = reader.GetInt32("Ambientes"),
                                Precio = reader.GetDecimal("Precio"),
                                PropietarioId = reader.GetInt32("PropietarioId"),
                                Estado = Convert.ToBoolean(reader["Estado"]),
                                Propietario = new Propietario
                                {
                                    Id = reader.GetInt32("PropietarioId"),
                                    Dni = reader["Dni"].ToString() ?? "",
                                    Apellido = reader["Apellido"].ToString() ?? "",
                                    Nombre = reader["Nombre"].ToString() ?? "",
                                    Email = reader["Email"] == DBNull.Value ? null : reader["Email"].ToString(),
                                    Telefono = reader["Telefono"] == DBNull.Value ? null : reader["Telefono"].ToString(),
                                    Domicilio = reader["Domicilio"] == DBNull.Value ? null : reader["Domicilio"].ToString()
                                },
                                TipoInmueble = reader["TipoInmuebleId"] == DBNull.Value ? null : new TipoInmueble
                                {
                                    Id = reader.GetInt32("TipoInmuebleId"),
                                    Nombre = reader["TipoNombre"]?.ToString() ?? "",
                                    Descripcion = reader["TipoDescripcion"] == DBNull.Value ? null : reader["TipoDescripcion"].ToString(),
                                    UsoPermitido = reader["UsoPermitido"]?.ToString() ?? "",
                                    EsComercial = reader["EsComercial"] == DBNull.Value ? false : Convert.ToBoolean(reader["EsComercial"])
                                }
                            };
                            lista.Add(inmueble);
                        }
                    }
                }
            }

            return lista;
        }
    }
}