using AplicacaoTeste.Context;
using AplicacaoTeste.Entities;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using System.Transactions;

namespace AplicacaoTeste.Services;

public class PessoaRepository : IPessoaRepository
{
    private readonly string _connectionString;

    public PessoaRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection") ?? "";
    }

    public void AddPessoa(Pessoa pessoa)
    {
        using var connection = new NpgsqlConnection(_connectionString);
        connection.Open();
        var sql = "insert into projetodb.pessoa values (@cpf, @nome, @dt_nasc, @cep, @numero, @logradouro)";

        using var command = new NpgsqlCommand(sql, connection);
        command.Parameters.AddWithValue("@cpf", pessoa.Cpf);
        command.Parameters.AddWithValue("@nome", pessoa.Nome);
        command.Parameters.AddWithValue("@dt_nasc", pessoa.DataNascimento);
        command.Parameters.AddWithValue("@cep", pessoa.EnderecoCep ?? (object)DBNull.Value);
        command.Parameters.AddWithValue("@numero", pessoa.EnderecoNumero ?? (object)DBNull.Value);
        command.Parameters.AddWithValue("@logradouro", pessoa.EnderecoLogradouro ?? (object)DBNull.Value);

        command.ExecuteNonQuery();
    }

    public void DeletePessoa(string cpf, NpgsqlTransaction? transaction = null, NpgsqlConnection? connection = null)
    {
        bool createdConnection = false;
        if (connection is null)
        {
            connection = new NpgsqlConnection(_connectionString);
            connection.Open();
            createdConnection = true;
        }
        if (transaction is not null && connection is not null)
        {
            var sql = "delete from projetodb.pessoa where cpf = @cpf";
            using var command = new NpgsqlCommand(sql, connection, transaction);
            command.Parameters.AddWithValue("@cpf", cpf);
            command.ExecuteNonQuery();
        }
        else if (connection is not null)
        {
            var sql = "delete from projetodb.pessoa where cpf = @cpf";
            using var command = new NpgsqlCommand(sql, connection);
            command.Parameters.AddWithValue("@cpf", cpf);
            command.ExecuteNonQuery();
        }
        if (createdConnection && connection is not null)
        {
            connection.Dispose();
        }
    }

    public (NpgsqlTransaction transaction, NpgsqlConnection connection) GetTransaction()
    {
        var connection = new NpgsqlConnection(_connectionString);
        connection.Open();
        var transaction = connection.BeginTransaction();
        return (transaction, connection);
    }


    public Pessoa? GetPessoaByCPF(string CPF, NpgsqlTransaction? transaction = null, NpgsqlConnection? connection = null)
    {
        Pessoa? pessoa = null;
        bool createdConnection = false;
        if (connection is null)
        {
            connection = new NpgsqlConnection(_connectionString);
            connection.Open();
            createdConnection = true;
        }
        if (connection is not null && transaction is not null)
        {
            string sql = "SELECT cpf, nome, dt_nasc, endereco_cep, endereco_logradouro, endereco_numero from projetodb.pessoa where cpf = @cpf";

            using var command = new NpgsqlCommand(sql, connection, transaction);
            command.Parameters.AddWithValue("@cpf", CPF);

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                pessoa = new Pessoa(
                        Cpf: reader.GetString(0),
                        Nome: reader.GetString(1),
                        DataNascimento: reader.GetDateTime(2),
                        EnderecoCep: reader.IsDBNull(3) ? null : reader.GetString(3),
                        EnderecoLogradouro: reader.IsDBNull(4) ? null : reader.GetString(4),
                        EnderecoNumero: reader.IsDBNull(5) ? null : reader.GetString(5)
                );

            }

            return pessoa;
        }
        else if (connection is not null)
        {
            string sql = "SELECT cpf, nome, dt_nasc, endereco_cep, endereco_logradouro, endereco_numero from projetodb.pessoa where cpf = @cpf";

            using var command = new NpgsqlCommand(sql, connection);
            command.Parameters.AddWithValue("@cpf", CPF);

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                pessoa = new Pessoa(
                        Cpf: reader.GetString(0),
                        Nome: reader.GetString(1),
                        DataNascimento: reader.GetDateTime(2),
                        EnderecoCep: reader.IsDBNull(3) ? null : reader.GetString(3),
                        EnderecoLogradouro: reader.IsDBNull(4) ? null : reader.GetString(4),
                        EnderecoNumero: reader.IsDBNull(5) ? null : reader.GetString(5)
                );

            }

            return pessoa;
        }

        if (connection is not null && createdConnection) 
        { 
            connection.Dispose(); 
        }

        return null;
    }

    public IEnumerable<Pessoa> GetPessoas()
    {
        var pessoas = new List<Pessoa>();

        using (var connection = new NpgsqlConnection(_connectionString))
        {
            connection.Open();
            string sql = "SELECT cpf, nome, dt_nasc, endereco_cep, endereco_logradouro, endereco_numero from projetodb.pessoa";

            using var command = new NpgsqlCommand(sql, connection);
            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                pessoas.Add(
                    new Pessoa(
                        Cpf: reader.GetString(0),
                        Nome: reader.GetString(1),
                        DataNascimento: reader.GetDateTime(2),
                        EnderecoCep: reader.IsDBNull(3) ? null : reader.GetString(3),
                        EnderecoLogradouro: reader.IsDBNull(4) ? null : reader.GetString(4),
                        EnderecoNumero: reader.IsDBNull(5) ? null : reader.GetString(5)
                    )
                );

            }
        }

        return pessoas;
    }

    public void UpdatePessoa(Pessoa pessoa)
    {
        using var connection = new NpgsqlConnection(_connectionString);
        connection.Open();
        var sql = "update projetodb.pessoa set nome = @nome, data_nasc = @dt_nasc enderecoCEP = @cep, enderecoNumero = @numero, enderecoLogradouro = @logradouro where cpf = @cpf";

        using var command = new NpgsqlCommand(sql, connection);
        command.Parameters.AddWithValue("@cpf", pessoa.Cpf);
        command.Parameters.AddWithValue("@nome", pessoa.Nome);
        command.Parameters.AddWithValue("@dt_nasc", pessoa.DataNascimento);
        command.Parameters.AddWithValue("@cep", pessoa.EnderecoCep ?? (object)DBNull.Value);
        command.Parameters.AddWithValue("@numero", pessoa.EnderecoNumero ?? (object)DBNull.Value);
        command.Parameters.AddWithValue("@logradouro", pessoa.EnderecoLogradouro ?? (object)DBNull.Value);

        command.ExecuteNonQuery();
    }
}
