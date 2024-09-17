namespace AplicacaoBD.Entities;

public record Pessoa(string Cpf, string Nome, DateTime DataNascimento, string? EnderecoCep, string? EnderecoLogradouro, string? EnderecoNumero);
