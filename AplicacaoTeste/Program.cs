using AplicacaoTeste;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

List<Usuario> listUsuarios = new List<Usuario>();

app.MapGet("/usuarios", () =>
{
    return Results.Ok(listUsuarios);
})
.WithName("GetUsuarios")
.WithOpenApi();

app.MapGet("/usuario/{cpf}", ([FromRoute] string cpf) =>
{
    var usuario = listUsuarios.Find(u => u.cpf == cpf);
    if(usuario is not null) return Results.Ok(usuario);
    else return Results.NotFound("Usuário não encontrado!");
})
.WithName("GetUsuario")
.WithOpenApi();

app.MapPost("/usuario", (Usuario usuario) =>
{
    var usuarioExistente = listUsuarios.Find(u => u.cpf == usuario.cpf);
    if (usuarioExistente is not null)
    {
        return Results.Conflict("Usuário já existe!");
    }else listUsuarios.Add(usuario);
    return Results.Ok("Usuário criado com sucesso");
})
.WithName("PostUsuario")
.WithOpenApi();

app.MapDelete("/usuario/{cpf}", ([FromRoute] string cpf) => {
    var usuario = listUsuarios.Find(u => u.cpf == cpf);
    if(usuario is not null)
    {
        listUsuarios.Remove(usuario);
        return Results.NoContent();
    }
    else return Results.NotFound("Usuário não encontrado!");
})
.WithName("DeleteUsuario")
.WithOpenApi();

app.Run();

