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

var listUsuarios = new List<Usuario>(
    [
    new Usuario("92508451037", "Oscar", new DateTime(2002,10,25)),
    new Usuario("35746655040", "João", new DateTime(2000,2,12)),
    new Usuario("64969891095", "Maria", new DateTime(2001,12,30))
    ]);

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
    if (!ValidaCPF.IsCpf(usuario.cpf)) return Results.BadRequest("CPF inválido");
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

