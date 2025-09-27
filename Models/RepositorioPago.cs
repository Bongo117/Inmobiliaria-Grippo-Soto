using MySql.Data.MySqlClient;
using System.Collections.Generic;

namespace Inmobiliaria_.Net_Core.Models
{
    public class RepositorioPago : RepositorioBase
    {
        public RepositorioPago(IConfiguration configuration) : base(configuration) { }

        public List<Pago> ObtenerTodos()
        {
            var lista = new List<Pago>();

            using (var connection = new MySqlConnection(connectionString))
            {
                var sql = @"SELECT p.Id, p.NumeroPago, p.FechaPago, p.Detalle, p.Importe, p.ContratoId, 
                           p.Estado, p.FechaCreacion, p.UsuarioCreador, p.FechaAnulacion, p.UsuarioAnulacion,
                           c.FechaInicio, c.FechaFin, c.MontoMensual,
                           inq.Dni as InquilinoDni, inq.Apellido as InquilinoApellido, inq.Nombre as InquilinoNombre,
                           i.Direccion, i.Tipo
                           FROM pagos p
                           INNER JOIN contratos c ON p.ContratoId = c.Id
                           INNER JOIN inquilinos inq ON c.InquilinoId = inq.Id
                           INNER JOIN inmuebles i ON c.InmuebleId = i.Id
                           ORDER BY p.ContratoId, p.NumeroPago";

                using (var command = new MySqlCommand(sql, connection))
                {
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var pago = new Pago
                            {
                                Id = reader.GetInt32("Id"),
                                NumeroPago = reader.GetInt32("NumeroPago"),
                                FechaPago = reader.GetDateTime("FechaPago"),
                                Detalle = reader["Detalle"].ToString() ?? "",
                                Importe = reader.GetDecimal("Importe"),
                                ContratoId = reader.GetInt32("ContratoId"),
                                Estado = Convert.ToBoolean(reader["Estado"]),
                                FechaCreacion = reader.GetDateTime("FechaCreacion"),
                                UsuarioCreador = reader["UsuarioCreador"] == DBNull.Value ? null : reader["UsuarioCreador"].ToString(),
                                FechaAnulacion = reader["FechaAnulacion"] == DBNull.Value ? null : reader.GetDateTime("FechaAnulacion"),
                                UsuarioAnulacion = reader["UsuarioAnulacion"] == DBNull.Value ? null : reader["UsuarioAnulacion"].ToString(),
                                Contrato = new Contrato
                                {
                                    Id = reader.GetInt32("ContratoId"),
                                    FechaInicio = reader.GetDateTime("FechaInicio"),
                                    FechaFin = reader.GetDateTime("FechaFin"),
                                    MontoMensual = reader.GetDecimal("MontoMensual"),
                                    Inquilino = new Inquilino
                                    {
                                        Dni = reader["InquilinoDni"].ToString() ?? "",
                                        Apellido = reader["InquilinoApellido"].ToString() ?? "",
                                        Nombre = reader["InquilinoNombre"].ToString() ?? ""
                                    },
                                    Inmueble = new Inmueble
                                    {
                                        Direccion = reader["Direccion"].ToString() ?? "",
                                        Tipo = reader["Tipo"].ToString() ?? ""
                                    }
                                }
                            };
                            lista.Add(pago);
                        }
                    }
                }
            }

            return lista;
        }

        public Pago? ObtenerPorId(int id)
        {
            using (var connection = new MySqlConnection(connectionString))
            {
                var sql = @"SELECT p.Id, p.NumeroPago, p.FechaPago, p.Detalle, p.Importe, p.ContratoId, 
                           p.Estado, p.FechaCreacion, p.UsuarioCreador, p.FechaAnulacion, p.UsuarioAnulacion,
                           c.FechaInicio, c.FechaFin, c.MontoMensual,
                           inq.Dni as InquilinoDni, inq.Apellido as InquilinoApellido, inq.Nombre as InquilinoNombre,
                           i.Direccion, i.Tipo
                           FROM pagos p
                           INNER JOIN contratos c ON p.ContratoId = c.Id
                           INNER JOIN inquilinos inq ON c.InquilinoId = inq.Id
                           INNER JOIN inmuebles i ON c.InmuebleId = i.Id
                           WHERE p.Id = @id";

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@id", id);
                    connection.Open();

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new Pago
                            {
                                Id = reader.GetInt32("Id"),
                                NumeroPago = reader.GetInt32("NumeroPago"),
                                FechaPago = reader.GetDateTime("FechaPago"),
                                Detalle = reader["Detalle"].ToString() ?? "",
                                Importe = reader.GetDecimal("Importe"),
                                ContratoId = reader.GetInt32("ContratoId"),
                                Estado = Convert.ToBoolean(reader["Estado"]),
                                FechaCreacion = reader.GetDateTime("FechaCreacion"),
                                UsuarioCreador = reader["UsuarioCreador"] == DBNull.Value ? null : reader["UsuarioCreador"].ToString(),
                                FechaAnulacion = reader["FechaAnulacion"] == DBNull.Value ? null : reader.GetDateTime("FechaAnulacion"),
                                UsuarioAnulacion = reader["UsuarioAnulacion"] == DBNull.Value ? null : reader["UsuarioAnulacion"].ToString(),
                                Contrato = new Contrato
                                {
                                    Id = reader.GetInt32("ContratoId"),
                                    FechaInicio = reader.GetDateTime("FechaInicio"),
                                    FechaFin = reader.GetDateTime("FechaFin"),
                                    MontoMensual = reader.GetDecimal("MontoMensual"),
                                    Inquilino = new Inquilino
                                    {
                                        Dni = reader["InquilinoDni"].ToString() ?? "",
                                        Apellido = reader["InquilinoApellido"].ToString() ?? "",
                                        Nombre = reader["InquilinoNombre"].ToString() ?? ""
                                    },
                                    Inmueble = new Inmueble
                                    {
                                        Direccion = reader["Direccion"].ToString() ?? "",
                                        Tipo = reader["Tipo"].ToString() ?? ""
                                    }
                                }
                            };
                        }
                    }
                }
            }
            return null;
        }

        public List<Pago> ObtenerPorContrato(int contratoId)
        {
            var lista = new List<Pago>();

            using (var connection = new MySqlConnection(connectionString))
            {
                var sql = @"SELECT p.Id, p.NumeroPago, p.FechaPago, p.Detalle, p.Importe, p.ContratoId, 
                           p.Estado, p.FechaCreacion, p.UsuarioCreador, p.FechaAnulacion, p.UsuarioAnulacion
                           FROM pagos p
                           WHERE p.ContratoId = @contratoId
                           ORDER BY p.NumeroPago";

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@contratoId", contratoId);
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var pago = new Pago
                            {
                                Id = reader.GetInt32("Id"),
                                NumeroPago = reader.GetInt32("NumeroPago"),
                                FechaPago = reader.GetDateTime("FechaPago"),
                                Detalle = reader["Detalle"].ToString() ?? "",
                                Importe = reader.GetDecimal("Importe"),
                                ContratoId = reader.GetInt32("ContratoId"),
                                Estado = Convert.ToBoolean(reader["Estado"]),
                                FechaCreacion = reader.GetDateTime("FechaCreacion"),
                                UsuarioCreador = reader["UsuarioCreador"] == DBNull.Value ? null : reader["UsuarioCreador"].ToString(),
                                FechaAnulacion = reader["FechaAnulacion"] == DBNull.Value ? null : reader.GetDateTime("FechaAnulacion"),
                                UsuarioAnulacion = reader["UsuarioAnulacion"] == DBNull.Value ? null : reader["UsuarioAnulacion"].ToString()
                            };
                            lista.Add(pago);
                        }
                    }
                }
            }

            return lista;
        }

        public int Alta(Pago pago)
        {
            using (var connection = new MySqlConnection(connectionString))
            {
                var sql = @"INSERT INTO pagos (NumeroPago, FechaPago, Detalle, Importe, ContratoId, Estado, FechaCreacion, UsuarioCreador) 
                            VALUES (@numeroPago, @fechaPago, @detalle, @importe, @contratoId, @estado, @fechaCreacion, @usuarioCreador);
                            SELECT LAST_INSERT_ID();";

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@numeroPago", pago.NumeroPago);
                    command.Parameters.AddWithValue("@fechaPago", pago.FechaPago.Date);
                    command.Parameters.AddWithValue("@detalle", pago.Detalle);
                    command.Parameters.AddWithValue("@importe", pago.Importe);
                    command.Parameters.AddWithValue("@contratoId", pago.ContratoId);
                    command.Parameters.AddWithValue("@estado", pago.Estado);
                    command.Parameters.AddWithValue("@fechaCreacion", pago.FechaCreacion);
                    command.Parameters.AddWithValue("@usuarioCreador", pago.UsuarioCreador ?? (object)DBNull.Value);

                    connection.Open();
                    var result = command.ExecuteScalar();
                    return Convert.ToInt32(result);
                }
            }
        }

        public int Modificacion(Pago pago)
        {
            using (var connection = new MySqlConnection(connectionString))
            {
                // Según la narrativa, solo se puede editar el detalle/concepto
                var sql = @"UPDATE pagos SET Detalle = @detalle WHERE Id = @id";

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@id", pago.Id);
                    command.Parameters.AddWithValue("@detalle", pago.Detalle);

                    connection.Open();
                    return command.ExecuteNonQuery();
                }
            }
        }

        public int AnularPago(int id, string usuarioAnulacion)
        {
            using (var connection = new MySqlConnection(connectionString))
            {
                // Cambio de estado según la narrativa, no se elimina físicamente
                var sql = @"UPDATE pagos SET 
                           Estado = 0, 
                           FechaAnulacion = @fechaAnulacion, 
                           UsuarioAnulacion = @usuarioAnulacion 
                           WHERE Id = @id";

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@id", id);
                    command.Parameters.AddWithValue("@fechaAnulacion", DateTime.Now);
                    command.Parameters.AddWithValue("@usuarioAnulacion", usuarioAnulacion);

                    connection.Open();
                    return command.ExecuteNonQuery();
                }
            }
        }

        public int ObtenerSiguienteNumeroPago(int contratoId)
        {
            using (var connection = new MySqlConnection(connectionString))
            {
                var sql = @"SELECT COALESCE(MAX(NumeroPago), 0) + 1 FROM pagos WHERE ContratoId = @contratoId";

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@contratoId", contratoId);
                    connection.Open();
                    var result = command.ExecuteScalar();
                    return Convert.ToInt32(result);
                }
            }
        }

        public bool ExisteNumeroPago(int contratoId, int numeroPago, int? pagoIdExcluir = null)
        {
            using (var connection = new MySqlConnection(connectionString))
            {
                var sql = @"SELECT COUNT(*) FROM pagos 
                           WHERE ContratoId = @contratoId AND NumeroPago = @numeroPago";

                if (pagoIdExcluir.HasValue)
                {
                    sql += " AND Id != @pagoIdExcluir";
                }

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@contratoId", contratoId);
                    command.Parameters.AddWithValue("@numeroPago", numeroPago);
                    if (pagoIdExcluir.HasValue)
                    {
                        command.Parameters.AddWithValue("@pagoIdExcluir", pagoIdExcluir.Value);
                    }

                    connection.Open();
                    var result = command.ExecuteScalar();
                    return Convert.ToInt32(result) > 0;
                }
            }
        }

        public decimal ObtenerTotalPagos(int contratoId, bool soloActivos = true)
        {
            using (var connection = new MySqlConnection(connectionString))
            {
                var sql = @"SELECT COALESCE(SUM(Importe), 0) FROM pagos WHERE ContratoId = @contratoId";
                
                if (soloActivos)
                {
                    sql += " AND Estado = 1";
                }

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@contratoId", contratoId);
                    connection.Open();
                    var result = command.ExecuteScalar();
                    return Convert.ToDecimal(result);
                }
            }
        }
    }
}
