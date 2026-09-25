using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using TP07.Models;

namespace TP07.Controllers;

public class PublicacionController : Controller
{
    public IActionResult Crear()
    {
        if (HttpContext.Session.GetInt32("Id") == null)
        {
            return RedirectToAction("Login", "Usuario");
        }

        return View();
    }


    [HttpPost]
    public IActionResult Crear(Publicacion publicacion)
    {
        if (HttpContext.Session.GetInt32("Id") == null)
        {
            return RedirectToAction("Login", "Usuario");
        }

        int idUsuario = (int)HttpContext.Session.GetInt32("Id");

        publicacion.IdUsuario = idUsuario;
        publicacion.FechaPublicacion = DateTime.Now;

        BD bd = new BD();

        bool creada = bd.CrearPublicacion(publicacion);

        if (creada)
        {
            return RedirectToAction("Index", "Home");
        }

        ViewBag.Error = "No se pudo crear la publicación";

        return View(publicacion);
    }

[HttpPost]
public IActionResult MeGusta(int idPublicacion)
{
    int idUsuario = HttpContext.Session.GetInt32("Id").Value;

    BD bd = new BD();

    bool tieneMeGusta = bd.TieneMeGusta(idPublicacion, idUsuario);

    if (tieneMeGusta == true)
    {
        bd.SacarMeGusta(idPublicacion, idUsuario);
    }
    else
    {
        bd.AgregarMeGusta(idPublicacion, idUsuario);
    }

    int cantidad = bd.CantidadMeGusta(idPublicacion);

    return Ok(cantidad);
}
[HttpPost]
public IActionResult Comentar(int idPublicacion, string texto)
{
    if (HttpContext.Session.GetInt32("Id") == null)
    {
        return BadRequest();
    }

    if (texto == null || texto == "")
    {
        return BadRequest();
    }

    int idUsuario = HttpContext.Session.GetInt32("Id").Value;

    Comentario comentario = new Comentario();

    comentario.IdPublicacion = idPublicacion;
    comentario.IdUsuarioComenta = idUsuario;
    comentario.Texto = texto;
    comentario.FechaComentario = DateTime.Now;

    BD bd = new BD();

    bd.AgregarComentario(comentario);

    return Ok();
}

}