using AplicacaoBD;
using AplicacaoBD.Context;
using AplicacaoBD.Endpoints;
using AplicacaoBD.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IPessoaRepository, PessoaRepository>();

string myPgSqlConnection = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new Exception("A string de conexão não foi configurada.");

builder.Services.AddDbContext<AppDbContext>(options => options.UseNpgsql(myPgSqlConnection));

var app = builder.Build();

app.UseStatusCodePages(async statusCodeContext
    => await Results.Problem(statusCode: statusCodeContext.HttpContext.Response.StatusCode)
        .ExecuteAsync(statusCodeContext.HttpContext));

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.RegisterPessoaEndpoints();

app.Run();

