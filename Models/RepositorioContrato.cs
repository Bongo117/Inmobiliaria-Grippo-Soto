using MySql.Data.MySqlClient;
using System.Collections.Generic;

namespace Inmobiliaria_.Net_Core.Models
{
    public class RepositorioContrato : RepositorioBase
    {
        public RepositorioContrato(IConfiguration configuration) : base(configuration) { }

        public List<Contrato> ObtenerTodos()
        {
            var lista = new List<Contrato>();

            using (var connection = new MySqlConnection(connectionString))
            {
                var sql = @"SELECT c.Id, c.FechaInicio, c.FechaFin, c.MontoMensual, c.InquilinoId, c.InmuebleId, c.Estado,
                            inq.Dni as InquilinoDni, inq.Apellido as InquilinoApellido, inq.Nombre as InquilinoNombre,
                            inq.Email as InquilinoEmail, inq.Telefono as InquilinoTelefono, inq.Domicilio as InquilinoDomicilio,
                            i.Direccion, i.Tipo, i.Ambientes, i.Precio as InmueblePrecio,
                            p.Dni as PropietarioDni, p.Apellido as PropietarioApellido, p.Nombre as PropietarioNombre
                            FROM contratos c
                            INNER JOIN inquilinos inq ON c.InquilinoId = inq.Id
                            INNER JOIN inmuebles i ON c.InmuebleId = i.Id
                            INNER JOIN propietarios p ON i.PropietarioId = p.Id
                            WHERE c.Estado = 1";

                using (var command = new MySqlCommand(sql, connection))
                {
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var contrato = new Contrato
                            {
                                Id = reader.GetInt32("Id"),
                                FechaInicio = reader.GetDateTime("FechaInicio"),
                                FechaFin = reader.GetDateTime("FechaFin"),
                                MontoMensual = reader.GetDecimal("MontoMensual"),
                                InquilinoId = reader.GetInt32("InquilinoId"),
                                InmuebleId = reader.GetInt32("InmuebleId"),
                                Estado = Convert.ToBoolean(reader["Estado"]),
                                Inquilino = new Inquilino
                                {
                                    Id = reader.GetInt32("InquilinoId"),
                                    Dni = reader["InquilinoDni"].ToString() ?? "",
                                    Apellido = reader["InquilinoApellido"].ToString() ?? "",
                                    Nombre = reader["InquilinoNombre"].ToString() ?? "",
                                    Email = reader["InquilinoEmail"] == DBNull.Value ? null : reader["InquilinoEmail"].ToString(),
                                    Telefono = reader["InquilinoTelefono"] == DBNull.Value ? null : reader["InquilinoTelefono"].ToString(),
                                    Domicilio = reader["InquilinoDomicilio"] == DBNull.Value ? null : reader["InquilinoDomicilio"].ToString()
                                },
                                Inmueble = new Inmueble
                                {
                                    Id = reader.GetInt32("InmuebleId"),
                                    Direccion = reader["Direccion"].ToString() ?? "",
                                    Tipo = reader["Tipo"].ToString() ?? "",
                                    Ambientes = reader.GetInt32("Ambientes"),
                                    Precio = reader.GetDecimal("InmueblePrecio"),
                                    Propietario = new Propietario
                                    {
                                        Dni = reader["PropietarioDni"].ToString() ?? "",
                                        Apellido = reader["PropietarioApellido"].ToString() ?? "",
                                        Nombre = reader["PropietarioNombre"].ToString() ?? ""
                                    }
                                }
                            };
                            lista.Add(contrato);
                        }
                    }
                }
            }

            return lista;
        }

        public Contrato? ObtenerPorId(int id)
        {
            using (var connection = new MySqlConnection(connectionString))
            {
                var sql = @"SELECT c.Id, c.FechaInicio, c.FechaFin, c.MontoMensual, c.InquilinoId, c.InmuebleId, c.Estado,
                            inq.Dni as InquilinoDni, inq.Apellido as InquilinoApellido, inq.Nombre as InquilinoNombre,
                            inq.Email as InquilinoEmail, inq.Telefono as InquilinoTelefono, inq.Domicilio as InquilinoDomicilio,
                            i.Direccion, i.Tipo, i.Ambientes, i.Precio as InmueblePrecio,
                            p.Id as PropietarioId, p.Dni as PropietarioDni, p.Apellido as PropietarioApellido, p.Nombre as PropietarioNombre,
                            p.Email as PropietarioEmail, p.Telefono as PropietarioTelefono, p.Domicilio as PropietarioDomicilio
                            FROM contratos c
                            INNER JOIN inquilinos inq ON c.InquilinoId = inq.Id
                            INNER JOIN inmuebles i ON c.InmuebleId = i.Id
                            INNER JOIN propietarios p ON i.PropietarioId = p.Id
                            WHERE c.Id = @id AND c.Estado = 1";

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@id", id);
                    connection.Open();

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new Contrato
                            {
                                Id = reader.GetInt32("Id"),
                                FechaInicio = reader.GetDateTime("FechaInicio"),
                                FechaFin = reader.GetDateTime("FechaFin"),
                                MontoMensual = reader.GetDecimal("MontoMensual"),
                                InquilinoId = reader.GetInt32("InquilinoId"),
                                InmuebleId = reader.GetInt32("InmuebleId"),
                                Estado = Convert.ToBoolean(reader["Estado"]),
                                Inquilino = new Inquilino
                                {
                                    Id = reader.GetInt32("InquilinoId"),
                                    Dni = reader["InquilinoDni"].ToString() ?? "",
                                    Apellido = reader["InquilinoApellido"].ToString() ?? "",
                                    Nombre = reader["InquilinoNombre"].ToString() ?? "",
                                    Email = reader["InquilinoEmail"] == DBNull.Value ? null : reader["InquilinoEmail"].ToString(),
                                    Telefono = reader["InquilinoTelefono"] == DBNull.Value ? null : reader["InquilinoTelefono"].ToString(),
                                    Domicilio = reader["InquilinoDomicilio"] == DBNull.Value ? null : reader["InquilinoDomicilio"].ToString()
                                },
                                Inmueble = new Inmueble
                                {
                                    Id = reader.GetInt32("InmuebleId"),
                                    Direccion = reader["Direccion"].ToString() ?? "",
                                    Tipo = reader["Tipo"].ToString() ?? "",
                                    Ambientes = reader.GetInt32("Ambientes"),
                                    Precio = reader.GetDecimal("InmueblePrecio"),
                                    PropietarioId = reader.GetInt32("PropietarioId"),
                                    Propietario = new Propietario
                                    {
                                        Id = reader.GetInt32("PropietarioId"),
                                        Dni = reader["PropietarioDni"].ToString() ?? "",
                                        Apellido = reader["PropietarioApellido"].ToString() ?? "",
                                        Nombre = reader["PropietarioNombre"].ToString() ?? "",
                                        Email = reader["PropietarioEmail"] == DBNull.Value ? null : reader["PropietarioEmail"].ToString(),
                                        Telefono = reader["PropietarioTelefono"] == DBNull.Value ? null : reader["PropietarioTelefono"].ToString(),
                                        Domicilio = reader["PropietarioDomicilio"] == DBNull.Value ? null : reader["PropietarioDomicilio"].ToString()
                                    }
                                }
                            };
                        }
                    }
                }
            }
            return null;
        }

        public int Alta(Contrato contrato)
        {
            using (var connection = new MySqlConnection(connectionString))
            {
                var sql = @"INSERT INTO contratos (FechaInicio, FechaFin, MontoMensual, InquilinoId, InmuebleId, Estado) 
                            VALUES (@fechaInicio, @fechaFin, @montoMensual, @inquilinoId, @inmuebleId, @estado);
                            SELECT LAST_INSERT_ID();";

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@fechaInicio", contrato.FechaInicio.Date);
                    command.Parameters.AddWithValue("@fechaFin", contrato.FechaFin.Date);
                    command.Parameters.AddWithValue("@montoMensual", contrato.MontoMensual);
                    command.Parameters.AddWithValue("@inquilinoId", contrato.InquilinoId);
                    command.Parameters.AddWithValue("@inmuebleId", contrato.InmuebleId);
                    command.Parameters.AddWithValue("@estado", contrato.Estado);

                    connection.Open();
                    var result = command.ExecuteScalar();
                    return Convert.ToInt32(result);
                }
            }
        }

        public int Modificacion(Contrato contrato)
        {
            using (var connection = new MySqlConnection(connectionString))
            {
                var sql = @"UPDATE contratos SET 
                            FechaInicio = @fechaInicio, FechaFin = @fechaFin, MontoMensual = @montoMensual,
                            InquilinoId = @inquilinoId, InmuebleId = @inmuebleId
                            WHERE Id = @id";

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@id", contrato.Id);
                    command.Parameters.AddWithValue("@fechaInicio", contrato.FechaInicio.Date);
                    command.Parameters.AddWithValue("@fechaFin", contrato.FechaFin.Date);
                    command.Parameters.AddWithValue("@montoMensual", contrato.MontoMensual);
                    command.Parameters.AddWithValue("@inquilinoId", contrato.InquilinoId);
                    command.Parameters.AddWithValue("@inmuebleId", contrato.InmuebleId);

                    connection.Open();
                    return command.ExecuteNonQuery();
                }
            }
        }

        public int Baja(int id)
        {
            using (var connection = new MySqlConnection(connectionString))
            {
                var sql = "UPDATE contratos SET Estado = 0 WHERE Id = @id";

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@id", id);
                    connection.Open();
                    return command.ExecuteNonQuery();
                }
            }
        }

        public bool ExisteContratoEnFechas(int inmuebleId, DateTime fechaInicio, DateTime fechaFin, int? contratoIdExcluir = null)
        {
            using (var connection = new MySqlConnection(connectionString))
            {
                var sql = @"SELECT COUNT(*) FROM contratos 
                           WHERE InmuebleId = @inmuebleId 
                           AND Estado = 1 
                           AND ((@fechaInicio BETWEEN FechaInicio AND FechaFin) 
                                OR (@fechaFin BETWEEN FechaInicio AND FechaFin)
                                OR (FechaInicio BETWEEN @fechaInicio AND @fechaFin))";

                if (contratoIdExcluir.HasValue)
                {
                    sql += " AND Id != @contratoIdExcluir";
                }

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@inmuebleId", inmuebleId);
                    command.Parameters.AddWithValue("@fechaInicio", fechaInicio.Date);
                    command.Parameters.AddWithValue("@fechaFin", fechaFin.Date);
                    if (contratoIdExcluir.HasValue)
                    {
                        command.Parameters.AddWithValue("@contratoIdExcluir", contratoIdExcluir.Value);
                    }

                    connection.Open();
                    var result = command.ExecuteScalar();
                    return Convert.ToInt32(result) > 0;
                }
            }
        }

        public List<Contrato> ObtenerVigentes()
        {
            var lista = new List<Contrato>();

            using (var connection = new MySqlConnection(connectionString))
            {
                var sql = @"SELECT c.Id, c.FechaInicio, c.FechaFin, c.MontoMensual, c.InquilinoId, c.InmuebleId, c.Estado,
                            inq.Dni as InquilinoDni, inq.Apellido as InquilinoApellido, inq.Nombre as InquilinoNombre,
                            i.Direccion, i.Tipo, i.Ambientes
                            FROM contratos c
                            INNER JOIN inquilinos inq ON c.InquilinoId = inq.Id
                            INNER JOIN inmuebles i ON c.InmuebleId = i.Id
                            WHERE c.Estado = 1 AND CURDATE() BETWEEN c.FechaInicio AND c.FechaFin";

                using (var command = new MySqlCommand(sql, connection))
                {
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var contrato = new Contrato
                            {
                                Id = reader.GetInt32("Id"),
                                FechaInicio = reader.GetDateTime("FechaInicio"),
                                FechaFin = reader.GetDateTime("FechaFin"),
                                MontoMensual = reader.GetDecimal("MontoMensual"),
                                InquilinoId = reader.GetInt32("InquilinoId"),
                                InmuebleId = reader.GetInt32("InmuebleId"),
                                Estado = Convert.ToBoolean(reader["Estado"]),
                                Inquilino = new Inquilino
                                {
                                    Id = reader.GetInt32("InquilinoId"),
                                    Dni = reader["InquilinoDni"].ToString() ?? "",
                                    Apellido = reader["InquilinoApellido"].ToString() ?? "",
                                    Nombre = reader["InquilinoNombre"].ToString() ?? ""
                                },
                                Inmueble = new Inmueble
                                {
                                    Id = reader.GetInt32("InmuebleId"),
                                    Direccion = reader["Direccion"].ToString() ?? "",
                                    Tipo = reader["Tipo"].ToString() ?? "",
                                    Ambientes = reader.GetInt32("Ambientes")
                                }
                            };
                            lista.Add(contrato);
                        }
                    }
                }
            }

            return lista;
        }

        public List<Contrato> ObtenerPorInmueble(int inmuebleId)
        {
            var lista = new List<Contrato>();

            using (var connection = new MySqlConnection(connectionString))
            {
                var sql = @"SELECT c.Id, c.FechaInicio, c.FechaFin, c.MontoMensual, c.InquilinoId, c.InmuebleId, c.Estado,
                            inq.Dni as InquilinoDni, inq.Apellido as InquilinoApellido, inq.Nombre as InquilinoNombre
                            FROM contratos c
                            INNER JOIN inquilinos inq ON c.InquilinoId = inq.Id
                            WHERE c.InmuebleId = @inmuebleId AND c.Estado = 1
                            ORDER BY c.FechaInicio DESC";

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@inmuebleId", inmuebleId);
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var contrato = new Contrato
                            {
                                Id = reader.GetInt32("Id"),
                                FechaInicio = reader.GetDateTime("FechaInicio"),
                                FechaFin = reader.GetDateTime("FechaFin"),
                                MontoMensual = reader.GetDecimal("MontoMensual"),
                                InquilinoId = reader.GetInt32("InquilinoId"),
                                InmuebleId = reader.GetInt32("InmuebleId"),
                                Estado = Convert.ToBoolean(reader["Estado"]),
                                Inquilino = new Inquilino
                                {
                                    Id = reader.GetInt32("InquilinoId"),
                                    Dni = reader["InquilinoDni"].ToString() ?? "",
                                    Apellido = reader["InquilinoApellido"].ToString() ?? "",
                                    Nombre = reader["InquilinoNombre"].ToString() ?? ""
                                }
                            };
                            lista.Add(contrato);
                        }
                    }
                }
            }

            return lista;
        }
    }
}
