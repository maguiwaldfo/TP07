function MeGusta(idPublicacion)
{
    fetch('/Publicacion/MeGusta?idPublicacion=' + idPublicacion,
    {
        method: 'POST'
    })
    .then(response => response.text())
    .then(cantidad =>
    {
        document.getElementById('cantidad-' + idPublicacion).innerHTML = cantidad;
    });
}