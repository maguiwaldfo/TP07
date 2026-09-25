using System.Collections.Generic;
using System.IO;
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

        List<Publicacion> publicaciones = connection.Query<Publicacion>(query).ToList();

        foreach (Publicacion publicacion in publicaciones)
        {
            publicacion.Imagen = ResolverRutaImagen(publicacion.Imagen);
        }

        return publicaciones;
    }
}

private static string ResolverRutaImagen(string imagen)
{
    if (string.IsNullOrWhiteSpace(imagen))
    {
        return imagen;
    }

    string[] candidatos = new[]
    {
        imagen,
        imagen.Replace(".jpg", ".jpeg", StringComparison.OrdinalIgnoreCase),
        imagen.Replace(".jpeg", ".jpg", StringComparison.OrdinalIgnoreCase),
        imagen.Replace(".jpg", ".png", StringComparison.OrdinalIgnoreCase),
        imagen.Replace(".jpeg", ".png", StringComparison.OrdinalIgnoreCase)
    };

    string directorio = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "wwwroot", "imagenes"));

    foreach (string candidato in candidatos.Distinct())
    {
        if (string.IsNullOrWhiteSpace(candidato))
        {
            continue;
        }

        string rutaCompleta = Path.Combine(directorio, candidato);
        if (File.Exists(rutaCompleta))
        {
            return candidato;
        }
    }

    return imagen;
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
public List<Comentario> ObtenerComentarios(int idPublicacion)
{
    using (SqlConnection connection = new SqlConnection(_connectionString))
    {
        string query = @"SELECT 
                        Comentarios.Id,
                        Comentarios.IdPublicacion,
                        Comentarios.IdUsuarioComenta,
                        Comentarios.Texto,
                        Comentarios.FechaComentario,
                        Usuarios.NombreUsuario
                        FROM Comentarios
                        INNER JOIN Usuarios
                        ON Comentarios.IdUsuarioComenta = Usuarios.Id
                        WHERE Comentarios.IdPublicacion = @IdPublicacion
                        ORDER BY Comentarios.FechaComentario ASC";

        return connection.Query<Comentario>(
            query,
            new { IdPublicacion = idPublicacion }
        ).ToList();
    }
}


public void AgregarComentario(Comentario comentario)
{
    using (SqlConnection connection = new SqlConnection(_connectionString))
    {
        string query = @"INSERT INTO Comentarios
                        (IdPublicacion, IdUsuarioComenta, Texto, FechaComentario)
                        VALUES
                        (@IdPublicacion, @IdUsuarioComenta, @Texto, @FechaComentario)";

        connection.Execute(query, comentario);
    }
}

}
}