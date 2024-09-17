using AplicacaoTeste.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AplicacaoTeste.Context;

public class AppDbContext : IdentityDbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Pessoa> Pessoas { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        var builderPessoa = builder.Entity<Pessoa>();
        builderPessoa.HasKey(p => p.Cpf);
        builderPessoa.Property(p => p.Nome).HasMaxLength(100).IsRequired();
        builderPessoa.Property(p => p.DataNascimento).IsRequired();
        builderPessoa.Property(p => p.EnderecoCep).IsRequired(false).HasColumnName("endereco_cep");
        builderPessoa.Property(p => p.EnderecoLogradouro).IsRequired(false).HasColumnName("endereco_logradouro");
        builderPessoa.Property(p => p.EnderecoNumero).IsRequired(false).HasColumnName("endereco_numero");

        base.OnModelCreating(builder);
    }
}
