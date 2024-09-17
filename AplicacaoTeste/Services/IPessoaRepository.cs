using AplicacaoBD.Entities;
using Npgsql;

namespace AplicacaoBD.Services;

public interface IPessoaRepository
{
    IEnumerable<Pessoa> GetPessoas();
    (NpgsqlTransaction transaction, NpgsqlConnection connection) GetTransaction();
    Pessoa? GetPessoaByCPF(string CPF, NpgsqlTransaction? transaction = null, NpgsqlConnection? connection = null);
    void AddPessoa(Pessoa pessoa);
    void DeletePessoa(string cpf, NpgsqlTransaction? transaction = null, NpgsqlConnection? connection = null);
    void UpdatePessoa(Pessoa pessoa);
}
