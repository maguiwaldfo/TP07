using System.Collections.Generic;
using System.Linq;
using Microsoft.Data.SqlClient;
using Dapper;

namespace TP07.Models
{
    public class BD
    {
       private string _connectionString = @"Server=localhost;DataBase=DBRedocial;Integrated Security=True;TrustServerCertificate=True;";

        public List<Usuario> ObtenerUsuarios()
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = "SELECT * FROM Usuarios";

                return connection.Query<Usuario>(query).ToList();
            }
        }

        public bool UsernameExists(string nombreUsuario)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = "SELECT COUNT(1) FROM Usuarios WHERE NombreUsuario = @NombreUsuario";

                int cantidad = connection.ExecuteScalar<int>(
                    query,
                    new
                    {
                        NombreUsuario = nombreUsuario
                    }
                );

                return cantidad > 0;
            }
        }

        public bool RegistrarUsuario(Usuario usuario)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = @"INSERT INTO Usuarios 
                                (NombreUsuario, Contraseña, Nombre, Apellido) 
                                VALUES 
                                (@NombreUsuario, @Contraseña, @Nombre, @Apellido)";

                int cantidad = connection.Execute(query, usuario);

                return cantidad > 0;
            }
        }

        public Usuario ValidarCredenciales(string nombreUsuario, string contraseña)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = @"SELECT * FROM Usuarios 
                                WHERE NombreUsuario = @NombreUsuario 
                                AND Contraseña = @Contraseña";

                List<Usuario> usuarios = connection.Query<Usuario>(
                    query,
                    new
                    {
                        NombreUsuario = nombreUsuario,
                        Contraseña = contraseña
                    }
                ).ToList();

                if (usuarios.Count > 0)
                {
                    return usuarios[0];
                }

                return null;
            }
        }
        public bool CrearPublicacion(Publicacion publicacion)
{
    using (SqlConnection connection = new SqlConnection(_connectionString))
    {
        string query = @"INSERT INTO Publicaciones
                        (IdUsuario, Titulo, Descripcion, Imagen, FechaPublicacion)
                        VALUES
                        (@IdUsuario, @Titulo, @Descripcion, @Imagen, @FechaPublicacion)";

        int cantidad = connection.Execute(query, publicacion);

        return cantidad > 0;
    }
}

public List<Publicacion> ObtenerPublicaciones()
{
    using (SqlConnection connection = new SqlConnection(_connectionString))
    {
        string query = @"SELECT TOP 10
                        Publicaciones.Id,
                        Publicaciones.IdUsuario,
                        Publicaciones.Titulo,
                        Publicaciones.Descripcion,
                        Publicaciones.Imagen,
                        Publicaciones.FechaPublicacion,
                        Usuarios.NombreUsuario,
                        (SELECT COUNT(*)
                         FROM PublicacionesMeGusta
                         WHERE IdPublicación = Publicaciones.Id) AS CantidadMeGusta
                        FROM Publicaciones
                        INNER JOIN Usuarios
                        ON Publicaciones.IdUsuario = Usuarios.Id
                        ORDER BY Publicaciones.FechaPublicacion DESC";

        return connection.Query<Publicacion>(query).ToList();
    }
}
public bool TieneMeGusta(int idPublicacion, int idUsuario)
{
    using (SqlConnection connection = new SqlConnection(_connectionString))
    {
        string query = @"SELECT COUNT(1)
                         FROM PublicacionesMeGusta
                         WHERE IdPublicación = @IdPublicacion
                         AND IdUsuario = @IdUsuario";

        int cantidad = connection.ExecuteScalar<int>(
            query,
            new
            {
                IdPublicacion = idPublicacion,
                IdUsuario = idUsuario
            }
        );

        return cantidad > 0;
    }
}


public void AgregarMeGusta(int idPublicacion, int idUsuario)
{
    using (SqlConnection connection = new SqlConnection(_connectionString))
    {
        string query = @"INSERT INTO PublicacionesMeGusta
                         (IdPublicación, IdUsuario)
                         VALUES
                         (@IdPublicacion, @IdUsuario)";

        connection.Execute(
            query,
            new
            {
                IdPublicacion = idPublicacion,
                IdUsuario = idUsuario
            }
        );
    }
}


public void SacarMeGusta(int idPublicacion, int idUsuario)
{
    using (SqlConnection connection = new SqlConnection(_connectionString))
    {
        string query = @"DELETE FROM PublicacionesMeGusta
                         WHERE IdPublicación = @IdPublicacion
                         AND IdUsuario = @IdUsuario";

        connection.Execute(
            query,
            new
            {
                IdPublicacion = idPublicacion,
                IdUsuario = idUsuario
            }
        );
    }
}


public int CantidadMeGusta(int idPublicacion)
{
    using (SqlConnection connection = new SqlConnection(_connectionString))
    {
        string query = @"SELECT COUNT(*)
                         FROM PublicacionesMeGusta
                         WHERE IdPublicación = @IdPublicacion";

        return connection.ExecuteScalar<int>(
            query,
            new { IdPublicacion = idPublicacion }
        );
    }
}

}
}