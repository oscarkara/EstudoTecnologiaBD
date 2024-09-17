using AplicacaoBD.Entities;
using AplicacaoBD.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;

namespace AplicacaoBD.Endpoints;

public static class PessoaEndpoints
{

    public static void RegisterPessoaEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPost("/pessoa", (Pessoa pessoa, IPessoaRepository _pessoaService) =>
        {
            var pessoaOld = _pessoaService.GetPessoaByCPF(pessoa.Cpf);

            if (pessoaOld is not null) return Results.Forbid();

            _pessoaService.AddPessoa(pessoa);
            return Results.Created((string?) null, new { pessoa.Cpf });
        })
        .WithName("AddPessoa")
        .WithOpenApi(x => new OpenApiOperation(x)
        {
            Summary = "Adicionar Pessoa",
            Description = "Adiciona uma pessoa ao projeto",
            Tags = [new OpenApiTag { Name = "Meu Projeto"}]
        });

        endpoints.MapGet("/pessoas", (IPessoaRepository _pessoaService) => 
            TypedResults.Ok(_pessoaService.GetPessoas()))
        .WithName("GetPessoas")
        .WithOpenApi(x => new OpenApiOperation(x)
        {
            Summary = "Listar Pessoas",
            Description = "Lista todas as pessoas do projeto",
            Tags = [new OpenApiTag { Name = "Meu Projeto" }]
        });

        endpoints.MapGet("/pessoa/{Cpf}", (IPessoaRepository _pessoaService, [FromRoute]string Cpf) =>
        {
            var pessoa = _pessoaService.GetPessoaByCPF(Cpf);
            return pessoa is null ? Results.NotFound() : Results.Ok(pessoa);
        })
        .WithName("GetPessoaPorCPF")
        .WithOpenApi(x => new OpenApiOperation(x)
        {
            Summary = "Pegar pessoa por CPF",
            Description = "Retorna uma pessoa a partir do seu CPF",
            Tags = [new OpenApiTag { Name = "Meu Projeto" }]
        });

        endpoints.MapDelete("/pessoa/{Cpf}", (IPessoaRepository _pessoaService, string Cpf) =>
        {
            IResult endpointResult = Results.Ok($"Pessoa de Cpf = {Cpf} foi deletado");
            var (transaction, connection) = _pessoaService.GetTransaction();
            var pessoa = _pessoaService.GetPessoaByCPF(Cpf, transaction, connection);
            if (pessoa is null) 
            {
                endpointResult = Results.NotFound();
            }
            else
            {
                _pessoaService.DeletePessoa(Cpf, transaction, connection);
                transaction.Commit();
            }
            transaction.Dispose();
            connection.Dispose();
            return endpointResult;
        })
        .WithName("DeletePessoaPorCPF")
        .WithOpenApi(x => new OpenApiOperation(x)
        {
            Summary = "Deleta uma pessoa por CPF",
            Description = "Deleta uma pessoa a partir do seu CPF",
            Tags = [new OpenApiTag { Name = "Meu Projeto" }]
        });

        endpoints.MapPut("/pessoa/{Cpf}", (IPessoaRepository _pessoaService, string Cpf, Pessoa pessoa) =>
        {
            if(pessoa is null)
                return Results.BadRequest("Dados inválidos");
            
            if(Cpf != pessoa.Cpf)
                return Results.BadRequest();

            _pessoaService.UpdatePessoa(pessoa);

            return Results.Ok(pessoa);
        })
        .WithName("UpdatePessoa")
        .WithOpenApi(x => new OpenApiOperation(x)
        {
            Summary = "Atualiza uma pessoa",
            Description = "Atualiza uma pessoa a partir do seu CPF e novos dados inseridos em um json",
            Tags = [new OpenApiTag { Name = "Meu Projeto" }]
        });
    }
}
