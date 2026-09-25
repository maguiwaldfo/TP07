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
function Comentar(idPublicacion)
{
    let input = document.getElementById('texto-' + idPublicacion);
    let texto = input.value;

    if (texto == '')
    {
        return;
    }

    fetch('/Publicacion/Comentar?idPublicacion=' + idPublicacion + '&texto=' + encodeURIComponent(texto),
    {
        method: 'POST'
    })
    .then(response =>
    {
        if (response.ok)
        {
            location.reload();
        }
    });
}