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
                            c.FechaTerminacionAnticipada, c.MotivoTerminacion, c.MultaAplicada, c.FechaAplicacionMulta,
                            c.FechaCreacion, c.UsuarioCreador, c.FechaTerminacionRegistro, c.UsuarioTerminacion,
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
                                FechaTerminacionAnticipada = reader["FechaTerminacionAnticipada"] == DBNull.Value ? null : reader.GetDateTime("FechaTerminacionAnticipada"),
                                MotivoTerminacion = reader["MotivoTerminacion"] == DBNull.Value ? null : reader["MotivoTerminacion"].ToString(),
                                MultaAplicada = reader["MultaAplicada"] == DBNull.Value ? null : reader.GetDecimal("MultaAplicada"),
                                FechaAplicacionMulta = reader["FechaAplicacionMulta"] == DBNull.Value ? null : reader.GetDateTime("FechaAplicacionMulta"),
                                FechaCreacion = reader["FechaCreacion"] == DBNull.Value ? DateTime.MinValue : reader.GetDateTime("FechaCreacion"),
                                UsuarioCreador = reader["UsuarioCreador"] == DBNull.Value ? null : reader["UsuarioCreador"].ToString(),
                                FechaTerminacionRegistro = reader["FechaTerminacionRegistro"] == DBNull.Value ? null : reader.GetDateTime("FechaTerminacionRegistro"),
                                UsuarioTerminacion = reader["UsuarioTerminacion"] == DBNull.Value ? null : reader["UsuarioTerminacion"].ToString(),
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
                            c.FechaTerminacionAnticipada, c.MotivoTerminacion, c.MultaAplicada, c.FechaAplicacionMulta,
                            c.FechaCreacion, c.UsuarioCreador, c.FechaTerminacionRegistro, c.UsuarioTerminacion,
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
                                FechaTerminacionAnticipada = reader["FechaTerminacionAnticipada"] == DBNull.Value ? null : reader.GetDateTime("FechaTerminacionAnticipada"),
                                MotivoTerminacion = reader["MotivoTerminacion"] == DBNull.Value ? null : reader["MotivoTerminacion"].ToString(),
                                MultaAplicada = reader["MultaAplicada"] == DBNull.Value ? null : reader.GetDecimal("MultaAplicada"),
                                FechaAplicacionMulta = reader["FechaAplicacionMulta"] == DBNull.Value ? null : reader.GetDateTime("FechaAplicacionMulta"),
                                FechaCreacion = reader["FechaCreacion"] == DBNull.Value ? DateTime.MinValue : reader.GetDateTime("FechaCreacion"),
                                UsuarioCreador = reader["UsuarioCreador"] == DBNull.Value ? null : reader["UsuarioCreador"].ToString(),
                                FechaTerminacionRegistro = reader["FechaTerminacionRegistro"] == DBNull.Value ? null : reader.GetDateTime("FechaTerminacionRegistro"),
                                UsuarioTerminacion = reader["UsuarioTerminacion"] == DBNull.Value ? null : reader["UsuarioTerminacion"].ToString(),
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

        public Contrato? ObtenerPorIdIncluyeInactivos(int id)
        {
            using (var connection = new MySqlConnection(connectionString))
            {
                var sql = @"SELECT c.Id, c.FechaInicio, c.FechaFin, c.MontoMensual, c.InquilinoId, c.InmuebleId, c.Estado,
                            c.FechaTerminacionAnticipada, c.MotivoTerminacion, c.MultaAplicada, c.FechaAplicacionMulta,
                            c.FechaCreacion, c.UsuarioCreador, c.FechaTerminacionRegistro, c.UsuarioTerminacion,
                            inq.Dni as InquilinoDni, inq.Apellido as InquilinoApellido, inq.Nombre as InquilinoNombre,
                            inq.Email as InquilinoEmail, inq.Telefono as InquilinoTelefono, inq.Domicilio as InquilinoDomicilio,
                            i.Direccion, i.Tipo, i.Ambientes, i.Precio as InmueblePrecio,
                            p.Id as PropietarioId, p.Dni as PropietarioDni, p.Apellido as PropietarioApellido, p.Nombre as PropietarioNombre,
                            p.Email as PropietarioEmail, p.Telefono as PropietarioTelefono, p.Domicilio as PropietarioDomicilio
                            FROM contratos c
                            INNER JOIN inquilinos inq ON c.InquilinoId = inq.Id
                            INNER JOIN inmuebles i ON c.InmuebleId = i.Id
                            INNER JOIN propietarios p ON i.PropietarioId = p.Id
                            WHERE c.Id = @id";

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
                                FechaTerminacionAnticipada = reader["FechaTerminacionAnticipada"] == DBNull.Value ? null : reader.GetDateTime("FechaTerminacionAnticipada"),
                                MotivoTerminacion = reader["MotivoTerminacion"] == DBNull.Value ? null : reader["MotivoTerminacion"].ToString(),
                                MultaAplicada = reader["MultaAplicada"] == DBNull.Value ? null : reader.GetDecimal("MultaAplicada"),
                                FechaAplicacionMulta = reader["FechaAplicacionMulta"] == DBNull.Value ? null : reader.GetDateTime("FechaAplicacionMulta"),
                                FechaCreacion = reader["FechaCreacion"] == DBNull.Value ? DateTime.MinValue : reader.GetDateTime("FechaCreacion"),
                                UsuarioCreador = reader["UsuarioCreador"] == DBNull.Value ? null : reader["UsuarioCreador"].ToString(),
                                FechaTerminacionRegistro = reader["FechaTerminacionRegistro"] == DBNull.Value ? null : reader.GetDateTime("FechaTerminacionRegistro"),
                                UsuarioTerminacion = reader["UsuarioTerminacion"] == DBNull.Value ? null : reader["UsuarioTerminacion"].ToString(),
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
                var sql = @"INSERT INTO contratos (FechaInicio, FechaFin, MontoMensual, InquilinoId, InmuebleId, Estado, FechaCreacion, UsuarioCreador) 
                            VALUES (@fechaInicio, @fechaFin, @montoMensual, @inquilinoId, @inmuebleId, @estado, @fechaCreacion, @usuarioCreador);
                            SELECT LAST_INSERT_ID();";

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@fechaInicio", contrato.FechaInicio.Date);
                    command.Parameters.AddWithValue("@fechaFin", contrato.FechaFin.Date);
                    command.Parameters.AddWithValue("@montoMensual", contrato.MontoMensual);
                    command.Parameters.AddWithValue("@inquilinoId", contrato.InquilinoId);
                    command.Parameters.AddWithValue("@inmuebleId", contrato.InmuebleId);
                    command.Parameters.AddWithValue("@estado", contrato.Estado);
                    command.Parameters.AddWithValue("@fechaCreacion", contrato.FechaCreacion);
                    command.Parameters.AddWithValue("@usuarioCreador", contrato.UsuarioCreador ?? (object)DBNull.Value);

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
                            InquilinoId = @inquilinoId, InmuebleId = @inmuebleId,
                            FechaTerminacionAnticipada = @fechaTerminacionAnticipada,
                            MotivoTerminacion = @motivoTerminacion,
                            MultaAplicada = @multaAplicada,
                            FechaAplicacionMulta = @fechaAplicacionMulta,
                            FechaTerminacionRegistro = @fechaTerminacionRegistro,
                            UsuarioTerminacion = @usuarioTerminacion
                            WHERE Id = @id";

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@id", contrato.Id);
                    command.Parameters.AddWithValue("@fechaInicio", contrato.FechaInicio.Date);
                    command.Parameters.AddWithValue("@fechaFin", contrato.FechaFin.Date);
                    command.Parameters.AddWithValue("@montoMensual", contrato.MontoMensual);
                    command.Parameters.AddWithValue("@inquilinoId", contrato.InquilinoId);
                    command.Parameters.AddWithValue("@inmuebleId", contrato.InmuebleId);
                    command.Parameters.AddWithValue("@fechaTerminacionAnticipada", (object?)contrato.FechaTerminacionAnticipada ?? DBNull.Value);
                    command.Parameters.AddWithValue("@motivoTerminacion", (object?)contrato.MotivoTerminacion ?? DBNull.Value);
                    command.Parameters.AddWithValue("@multaAplicada", (object?)contrato.MultaAplicada ?? DBNull.Value);
                    command.Parameters.AddWithValue("@fechaAplicacionMulta", (object?)contrato.FechaAplicacionMulta ?? DBNull.Value);
                    command.Parameters.AddWithValue("@fechaTerminacionRegistro", (object?)contrato.FechaTerminacionRegistro ?? DBNull.Value);
                    command.Parameters.AddWithValue("@usuarioTerminacion", (object?)contrato.UsuarioTerminacion ?? DBNull.Value);

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
                // Lógica mejorada para detectar superposiciones:
                // Dos contratos se superponen si:
                // 1. El inicio del nuevo contrato está dentro del período del existente
                // 2. El fin del nuevo contrato está dentro del período del existente  
                // 3. El nuevo contrato contiene completamente al existente
                // 4. El existente contiene completamente al nuevo contrato
                var sql = @"SELECT COUNT(*) FROM contratos 
                           WHERE InmuebleId = @inmuebleId 
                           AND Estado = 1 
                           AND NOT (FechaFin < @fechaInicio OR FechaInicio > @fechaFin)";

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

        public List<Contrato> ObtenerContratosSuperpuestos(int inmuebleId, DateTime fechaInicio, DateTime fechaFin, int? contratoIdExcluir = null)
        {
            var lista = new List<Contrato>();

            using (var connection = new MySqlConnection(connectionString))
            {
                var sql = @"SELECT c.Id, c.FechaInicio, c.FechaFin, c.MontoMensual, c.InquilinoId, c.InmuebleId, c.Estado,
                            inq.Dni as InquilinoDni, inq.Apellido as InquilinoApellido, inq.Nombre as InquilinoNombre,
                            inq.Email as InquilinoEmail, inq.Telefono as InquilinoTelefono
                            FROM contratos c
                            INNER JOIN inquilinos inq ON c.InquilinoId = inq.Id
                            WHERE c.InmuebleId = @inmuebleId 
                            AND c.Estado = 1 
                            AND NOT (c.FechaFin < @fechaInicio OR c.FechaInicio > @fechaFin)";

                if (contratoIdExcluir.HasValue)
                {
                    sql += " AND c.Id != @contratoIdExcluir";
                }

                sql += " ORDER BY c.FechaInicio ASC";

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
                                    Telefono = reader["InquilinoTelefono"] == DBNull.Value ? null : reader["InquilinoTelefono"].ToString()
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

        public bool ExisteContratoActivoParaInquilinoEInmueble(int inquilinoId, int inmuebleId, int? contratoIdExcluir = null)
        {
            using (var connection = new MySqlConnection(connectionString))
            {
                var sql = @"SELECT COUNT(*) FROM contratos 
                           WHERE InquilinoId = @inquilinoId 
                           AND InmuebleId = @inmuebleId 
                           AND Estado = 1";

                if (contratoIdExcluir.HasValue)
                {
                    sql += " AND Id != @contratoIdExcluir";
                }

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@inquilinoId", inquilinoId);
                    command.Parameters.AddWithValue("@inmuebleId", inmuebleId);
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

        public List<Contrato> ObtenerContratosActivosPorInquilinoEInmueble(int inquilinoId, int inmuebleId)
        {
            var lista = new List<Contrato>();

            using (var connection = new MySqlConnection(connectionString))
            {
                var sql = @"SELECT c.Id, c.FechaInicio, c.FechaFin, c.MontoMensual, c.InquilinoId, c.InmuebleId, c.Estado,
                            c.FechaTerminacionAnticipada, c.MotivoTerminacion, c.MultaAplicada, c.FechaAplicacionMulta,
                            c.FechaCreacion, c.UsuarioCreador, c.FechaTerminacionRegistro, c.UsuarioTerminacion,
                            inq.Dni as InquilinoDni, inq.Apellido as InquilinoApellido, inq.Nombre as InquilinoNombre,
                            inq.Email as InquilinoEmail, inq.Telefono as InquilinoTelefono, inq.Domicilio as InquilinoDomicilio,
                            i.Direccion, i.Tipo, i.Ambientes, i.Precio as InmueblePrecio,
                            p.Dni as PropietarioDni, p.Apellido as PropietarioApellido, p.Nombre as PropietarioNombre
                            FROM contratos c
                            INNER JOIN inquilinos inq ON c.InquilinoId = inq.Id
                            INNER JOIN inmuebles i ON c.InmuebleId = i.Id
                            INNER JOIN propietarios p ON i.PropietarioId = p.Id
                            WHERE c.InquilinoId = @inquilinoId 
                            AND c.InmuebleId = @inmuebleId 
                            AND c.Estado = 1
                            ORDER BY c.FechaInicio DESC";

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@inquilinoId", inquilinoId);
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
                                Estado = reader.GetBoolean("Estado"),
                                FechaTerminacionAnticipada = reader["FechaTerminacionAnticipada"] == DBNull.Value ? null : reader.GetDateTime("FechaTerminacionAnticipada"),
                                MotivoTerminacion = reader["MotivoTerminacion"] == DBNull.Value ? null : reader["MotivoTerminacion"].ToString(),
                                MultaAplicada = reader["MultaAplicada"] == DBNull.Value ? null : reader.GetDecimal("MultaAplicada"),
                                FechaAplicacionMulta = reader["FechaAplicacionMulta"] == DBNull.Value ? null : reader.GetDateTime("FechaAplicacionMulta"),
                                FechaCreacion = reader.GetDateTime("FechaCreacion"),
                                UsuarioCreador = reader["UsuarioCreador"] == DBNull.Value ? null : reader["UsuarioCreador"].ToString(),
                                FechaTerminacionRegistro = reader["FechaTerminacionRegistro"] == DBNull.Value ? null : reader.GetDateTime("FechaTerminacionRegistro"),
                                UsuarioTerminacion = reader["UsuarioTerminacion"] == DBNull.Value ? null : reader["UsuarioTerminacion"].ToString(),
                                Inquilino = new Inquilino
                                {
                                    Id = reader.GetInt32("InquilinoId"),
                                    Dni = reader["InquilinoDni"]?.ToString() ?? "",
                                    Apellido = reader["InquilinoApellido"]?.ToString() ?? "",
                                    Nombre = reader["InquilinoNombre"]?.ToString() ?? "",
                                    Email = reader["InquilinoEmail"]?.ToString(),
                                    Telefono = reader["InquilinoTelefono"]?.ToString(),
                                    Domicilio = reader["InquilinoDomicilio"]?.ToString()
                                },
                                Inmueble = new Inmueble
                                {
                                    Id = reader.GetInt32("InmuebleId"),
                                    Direccion = reader["Direccion"]?.ToString() ?? "",
                                    Tipo = reader["Tipo"]?.ToString() ?? "",
                                    Ambientes = reader.GetInt32("Ambientes"),
                                    Precio = reader.GetDecimal("InmueblePrecio"),
                                    Propietario = new Propietario
                                    {
                                        Dni = reader["PropietarioDni"]?.ToString() ?? "",
                                        Apellido = reader["PropietarioApellido"]?.ToString() ?? "",
                                        Nombre = reader["PropietarioNombre"]?.ToString() ?? ""
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

        public List<Contrato> ObtenerVigentes()
        {
            var lista = new List<Contrato>();

            using (var connection = new MySqlConnection(connectionString))
            {
                var sql = @"SELECT c.Id, c.FechaInicio, c.FechaFin, c.MontoMensual, c.InquilinoId, c.InmuebleId, c.Estado,
                            c.FechaTerminacionAnticipada, c.MotivoTerminacion, c.MultaAplicada, c.FechaAplicacionMulta,
                            c.FechaCreacion, c.UsuarioCreador, c.FechaTerminacionRegistro, c.UsuarioTerminacion,
                            inq.Dni as InquilinoDni, inq.Apellido as InquilinoApellido, inq.Nombre as InquilinoNombre,
                            inq.Email as InquilinoEmail, inq.Telefono as InquilinoTelefono, inq.Domicilio as InquilinoDomicilio,
                            i.Direccion, i.Tipo, i.Ambientes, i.Precio as InmueblePrecio,
                            p.Dni as PropietarioDni, p.Apellido as PropietarioApellido, p.Nombre as PropietarioNombre,
                            p.Email as PropietarioEmail, p.Telefono as PropietarioTelefono, p.Domicilio as PropietarioDomicilio
                            FROM contratos c
                            INNER JOIN inquilinos inq ON c.InquilinoId = inq.Id
                            INNER JOIN inmuebles i ON c.InmuebleId = i.Id
                            INNER JOIN propietarios p ON i.PropietarioId = p.Id
                            WHERE c.Estado = 1 
                            AND c.FechaInicio <= CURDATE() 
                            AND c.FechaFin >= CURDATE()
                            ORDER BY c.FechaFin ASC";

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
                                FechaTerminacionAnticipada = reader["FechaTerminacionAnticipada"] == DBNull.Value ? null : reader.GetDateTime("FechaTerminacionAnticipada"),
                                MotivoTerminacion = reader["MotivoTerminacion"] == DBNull.Value ? null : reader["MotivoTerminacion"].ToString(),
                                MultaAplicada = reader["MultaAplicada"] == DBNull.Value ? null : reader.GetDecimal("MultaAplicada"),
                                FechaAplicacionMulta = reader["FechaAplicacionMulta"] == DBNull.Value ? null : reader.GetDateTime("FechaAplicacionMulta"),
                                FechaCreacion = reader["FechaCreacion"] == DBNull.Value ? DateTime.MinValue : reader.GetDateTime("FechaCreacion"),
                                UsuarioCreador = reader["UsuarioCreador"] == DBNull.Value ? null : reader["UsuarioCreador"].ToString(),
                                FechaTerminacionRegistro = reader["FechaTerminacionRegistro"] == DBNull.Value ? null : reader.GetDateTime("FechaTerminacionRegistro"),
                                UsuarioTerminacion = reader["UsuarioTerminacion"] == DBNull.Value ? null : reader["UsuarioTerminacion"].ToString(),
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
                                        Nombre = reader["PropietarioNombre"].ToString() ?? "",
                                        Email = reader["PropietarioEmail"] == DBNull.Value ? null : reader["PropietarioEmail"].ToString(),
                                        Telefono = reader["PropietarioTelefono"] == DBNull.Value ? null : reader["PropietarioTelefono"].ToString(),
                                        Domicilio = reader["PropietarioDomicilio"] == DBNull.Value ? null : reader["PropietarioDomicilio"].ToString()
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

        public List<Contrato> ObtenerVigentesEntreFechas(DateTime fechaDesde, DateTime fechaHasta)
        {
            var lista = new List<Contrato>();

            using (var connection = new MySqlConnection(connectionString))
            {
                var sql = @"SELECT c.Id, c.FechaInicio, c.FechaFin, c.MontoMensual, c.InquilinoId, c.InmuebleId, c.Estado,
                            c.FechaTerminacionAnticipada, c.MotivoTerminacion, c.MultaAplicada, c.FechaAplicacionMulta,
                            c.FechaCreacion, c.UsuarioCreador, c.FechaTerminacionRegistro, c.UsuarioTerminacion,
                            inq.Dni as InquilinoDni, inq.Apellido as InquilinoApellido, inq.Nombre as InquilinoNombre,
                            inq.Email as InquilinoEmail, inq.Telefono as InquilinoTelefono, inq.Domicilio as InquilinoDomicilio,
                            i.Direccion, i.Tipo, i.Ambientes, i.Precio as InmueblePrecio,
                            p.Dni as PropietarioDni, p.Apellido as PropietarioApellido, p.Nombre as PropietarioNombre,
                            p.Email as PropietarioEmail, p.Telefono as PropietarioTelefono, p.Domicilio as PropietarioDomicilio
                            FROM contratos c
                            INNER JOIN inquilinos inq ON c.InquilinoId = inq.Id
                            INNER JOIN inmuebles i ON c.InmuebleId = i.Id
                            INNER JOIN propietarios p ON i.PropietarioId = p.Id
                            WHERE c.Estado = 1 
                            AND NOT (c.FechaFin < @fechaDesde OR c.FechaInicio > @fechaHasta)
                            ORDER BY c.FechaFin ASC";

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@fechaDesde", fechaDesde.Date);
                    command.Parameters.AddWithValue("@fechaHasta", fechaHasta.Date);
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
                                FechaTerminacionAnticipada = reader["FechaTerminacionAnticipada"] == DBNull.Value ? null : reader.GetDateTime("FechaTerminacionAnticipada"),
                                MotivoTerminacion = reader["MotivoTerminacion"] == DBNull.Value ? null : reader["MotivoTerminacion"].ToString(),
                                MultaAplicada = reader["MultaAplicada"] == DBNull.Value ? null : reader.GetDecimal("MultaAplicada"),
                                FechaAplicacionMulta = reader["FechaAplicacionMulta"] == DBNull.Value ? null : reader.GetDateTime("FechaAplicacionMulta"),
                                FechaCreacion = reader["FechaCreacion"] == DBNull.Value ? DateTime.MinValue : reader.GetDateTime("FechaCreacion"),
                                UsuarioCreador = reader["UsuarioCreador"] == DBNull.Value ? null : reader["UsuarioCreador"].ToString(),
                                FechaTerminacionRegistro = reader["FechaTerminacionRegistro"] == DBNull.Value ? null : reader.GetDateTime("FechaTerminacionRegistro"),
                                UsuarioTerminacion = reader["UsuarioTerminacion"] == DBNull.Value ? null : reader["UsuarioTerminacion"].ToString(),
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
                                        Nombre = reader["PropietarioNombre"].ToString() ?? "",
                                        Email = reader["PropietarioEmail"] == DBNull.Value ? null : reader["PropietarioEmail"].ToString(),
                                        Telefono = reader["PropietarioTelefono"] == DBNull.Value ? null : reader["PropietarioTelefono"].ToString(),
                                        Domicilio = reader["PropietarioDomicilio"] == DBNull.Value ? null : reader["PropietarioDomicilio"].ToString()
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
    }
}
