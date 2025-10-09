using MySql.Data.MySqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;

namespace Inmobiliaria_.Net_Core.Models
{
    public class RepositorioUsuario : RepositorioBase
    {
        public RepositorioUsuario(IConfiguration configuration) : base(configuration) { }

        public List<Usuario> ObtenerTodos()
        {
            var lista = new List<Usuario>();
            using (var connection = new MySqlConnection(connectionString))
            {
                var sql = @"SELECT Id, Email, ClaveHash, Rol, Apellido, Nombre, AvatarUrl, Estado, FechaAlta
                            FROM usuarios WHERE Estado = 1";
                using (var command = new MySqlCommand(sql, connection))
                {
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            lista.Add(Map(reader));
                        }
                    }
                }
            }
            return lista;
        }

        public Usuario? ObtenerPorId(int id)
        {
            using (var connection = new MySqlConnection(connectionString))
            {
                var sql = @"SELECT Id, Email, ClaveHash, Rol, Apellido, Nombre, AvatarUrl, Estado, FechaAlta
                            FROM usuarios WHERE Id = @id AND Estado = 1";
                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@id", id);
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read()) return Map(reader);
                    }
                }
            }
            return null;
        }

        // Ahora permite incluir usuarios inactivos con soloActivos = false
        public Usuario? ObtenerPorEmail(string email, bool soloActivos = true)
        {
            if (string.IsNullOrEmpty(email)) return null;
            var emailNorm = email.Trim().ToLowerInvariant();

            using (var connection = new MySqlConnection(connectionString))
            {
                string sql;
                if (soloActivos)
                {
                    sql = @"SELECT Id, Email, ClaveHash, Rol, Apellido, Nombre, AvatarUrl, Estado, FechaAlta
                            FROM usuarios WHERE LOWER(TRIM(Email)) = @email AND Estado = 1";
                }
                else
                {
                    sql = @"SELECT Id, Email, ClaveHash, Rol, Apellido, Nombre, AvatarUrl, Estado, FechaAlta
                            FROM usuarios WHERE LOWER(TRIM(Email)) = @email";
                }

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@email", emailNorm);
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read()) return Map(reader);
                    }
                }
            }
            return null;
        }

        public int Alta(Usuario usuario)
        {
            if (usuario == null) throw new ArgumentNullException(nameof(usuario));
            // Normalizar email antes de operar, asegurando que no sea nulo
            usuario.Email = (usuario.Email ?? string.Empty).Trim().ToLowerInvariant();

            using (var connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                // 1) Revisar si existe usuario (activo o inactivo) con ese email (aseguramos que no sea nulo)
                var existente = ObtenerPorEmail(usuario.Email, soloActivos: false);
                if (existente != null)
                {
                    if (!existente.Estado)
                    {
                        // Reactivar usuario inactivo: actualizamos campos relevantes y retornamos su id
                        var sqlUpdate = @"UPDATE usuarios SET 
                                            ClaveHash = @ClaveHash,
                                            Rol = @Rol,
                                            Apellido = @Apellido,
                                            Nombre = @Nombre,
                                            AvatarUrl = @AvatarUrl,
                                            Estado = 1
                                          WHERE Id = @Id";
                        using (var command = new MySqlCommand(sqlUpdate, connection))
                        {
                            command.Parameters.AddWithValue("@ClaveHash", usuario.ClaveHash);
                            command.Parameters.AddWithValue("@Rol", usuario.Rol);
                            command.Parameters.AddWithValue("@Apellido", (object?)usuario.Apellido ?? DBNull.Value);
                            command.Parameters.AddWithValue("@Nombre", (object?)usuario.Nombre ?? DBNull.Value);
                            command.Parameters.AddWithValue("@AvatarUrl", (object?)usuario.AvatarUrl ?? DBNull.Value);
                            command.Parameters.AddWithValue("@Id", existente.Id);
                            command.ExecuteNonQuery();
                        }
                        return existente.Id;
                    }
                    else
                    {
                        // Ya existe activo: respetamos la restricción a nivel aplicación
                        throw new InvalidOperationException("Ya existe un usuario activo con ese email.");
                    }
                }

                // 2) Si no existe, insertar nuevo
                var sql = @"INSERT INTO usuarios (Email, ClaveHash, Rol, Apellido, Nombre, AvatarUrl, Estado)
                            VALUES (@Email, @ClaveHash, @Rol, @Apellido, @Nombre, @AvatarUrl, @Estado);
                            SELECT LAST_INSERT_ID();";
                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@Email", usuario.Email);
                    command.Parameters.AddWithValue("@ClaveHash", usuario.ClaveHash);
                    command.Parameters.AddWithValue("@Rol", usuario.Rol);
                    command.Parameters.AddWithValue("@Apellido", (object?)usuario.Apellido ?? DBNull.Value);
                    command.Parameters.AddWithValue("@Nombre", (object?)usuario.Nombre ?? DBNull.Value);
                    command.Parameters.AddWithValue("@AvatarUrl", (object?)usuario.AvatarUrl ?? DBNull.Value);
                    command.Parameters.AddWithValue("@Estado", usuario.Estado);
                    var result = command.ExecuteScalar();
                    return Convert.ToInt32(result);
                }
            }
        }

        public int Modificacion(Usuario usuario)
        {
            using (var connection = new MySqlConnection(connectionString))
            {
                var sql = @"UPDATE usuarios SET 
                            Email=@Email, Rol=@Rol, Apellido=@Apellido, Nombre=@Nombre, AvatarUrl=@AvatarUrl
                            WHERE Id=@Id";
                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@Id", usuario.Id);
                    // Normalizar email al modificar
                    command.Parameters.AddWithValue("@Email", usuario.Email?.Trim().ToLowerInvariant());
                    command.Parameters.AddWithValue("@Rol", usuario.Rol);
                    command.Parameters.AddWithValue("@Apellido", (object?)usuario.Apellido ?? DBNull.Value);
                    command.Parameters.AddWithValue("@Nombre", (object?)usuario.Nombre ?? DBNull.Value);
                    command.Parameters.AddWithValue("@AvatarUrl", (object?)usuario.AvatarUrl ?? DBNull.Value);
                    connection.Open();
                    return command.ExecuteNonQuery();
                }
            }
        }

        public int CambiarClave(int id, string nuevaClave)
        {
            using (var connection = new MySqlConnection(connectionString))
            {
                var sql = @"UPDATE usuarios SET ClaveHash=@ClaveHash WHERE Id=@Id";
                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);
                    command.Parameters.AddWithValue("@ClaveHash", nuevaClave);
                    connection.Open();
                    return command.ExecuteNonQuery();
                }
            }
        }

        // Soft-delete (conserva registro)
        public int Baja(int id)
        {
            using (var connection = new MySqlConnection(connectionString))
            {
                var sql = "UPDATE usuarios SET Estado = 0 WHERE Id = @id";
                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@id", id);
                    connection.Open();
                    return command.ExecuteNonQuery();
                }
            }
        }

        // Hard-delete (por si querés borrar físicamente)
        public int EliminarDefinitivo(int id)
        {
            using (var connection = new MySqlConnection(connectionString))
            {
                var sql = "DELETE FROM usuarios WHERE Id = @id";
                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@id", id);
                    connection.Open();
                    return command.ExecuteNonQuery();
                }
            }
        }

        public bool ExisteEmail(string email, int? idExcluir = null)
        {
            if (string.IsNullOrEmpty(email)) return false;
            var emailNorm = email.Trim().ToLowerInvariant();

            using (var connection = new MySqlConnection(connectionString))
            {
                var sql = "SELECT COUNT(*) FROM usuarios WHERE LOWER(TRIM(Email)) = @Email AND Estado = 1";
                if (idExcluir.HasValue) sql += " AND Id != @IdExcluir";
                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@Email", emailNorm);
                    if (idExcluir.HasValue) command.Parameters.AddWithValue("@IdExcluir", idExcluir.Value);
                    connection.Open();
                    var result = command.ExecuteScalar();
                    return Convert.ToInt32(result) > 0;
                }
            }
        }

        private static Usuario Map(MySqlDataReader reader)
        {
            return new Usuario
            {
                Id = reader.GetInt32("Id"),
                Email = reader["Email"].ToString() ?? string.Empty,
                ClaveHash = reader["ClaveHash"].ToString() ?? string.Empty,
                Rol = reader["Rol"].ToString() ?? "Empleado",
                Apellido = reader["Apellido"] == DBNull.Value ? null : reader["Apellido"].ToString(),
                Nombre = reader["Nombre"] == DBNull.Value ? null : reader["Nombre"].ToString(),
                AvatarUrl = reader["AvatarUrl"] == DBNull.Value ? null : reader["AvatarUrl"].ToString(),
                Estado = Convert.ToBoolean(reader["Estado"]),
                FechaAlta = reader.GetDateTime("FechaAlta")
            };
        }
    }
}
