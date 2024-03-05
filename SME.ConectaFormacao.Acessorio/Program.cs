using Microsoft.Extensions.Configuration;
using SME.ConectaFormacao.Acessorio.Infra.Dados.Coresso;
using SME.ConectaFormacao.Acessorio.Infra.Dados.Eol;
using System.Data.SqlClient;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;


/* PROJETO CRIADO PARA EXECUTAR PROCESSOS PONTUAIS */

Console.WriteLine("Iniciando aplicação");

try
{
    var builder = new ConfigurationBuilder();
    builder.SetBasePath(Directory.GetCurrentDirectory())
       .AddUserSecrets(Assembly.GetExecutingAssembly());

    IConfiguration configuration = builder.Build();

    Console.WriteLine("Obtendo configurações");

    var connectionStringEol = configuration.GetConnectionString("Eol");
    if (string.IsNullOrEmpty(connectionStringEol))
        throw new Exception("ConnectionString EOL não informada");

    var connectionStringCoresso = configuration.GetConnectionString("Coresso");
    if (string.IsNullOrEmpty(connectionStringCoresso))
        throw new Exception("ConnectionString CORESSO não informada");

    Console.WriteLine("Iniciando conexão e repositórios do banco de dados EOL");

    using var conexaoEol = new SqlConnection(connectionStringEol);
    conexaoEol.Open();

    var funcionarioEolRepositorio = new FuncionarioEolRepositorio(conexaoEol);

    Console.WriteLine("Iniciando conexão e repositórios do banco de dados CORESSO");

    using var conexaoCoresso = new SqlConnection(connectionStringCoresso);
    conexaoCoresso.Open();

    var usuarioCoressoRepositorio = new UsuarioCoressoRepositorio(conexaoCoresso);

    Console.WriteLine("Obentendo funcionários CEI");
    var funcionarios = await funcionarioEolRepositorio.ObterFuncionariosPorTipoEscola(10, 18);

    Console.WriteLine($"{funcionarios.Count()} funcionarios encontrados");

    foreach (var funcionario in funcionarios)
    {
        Console.WriteLine($"* Processando funcionario {funcionario}");

        Console.WriteLine($"    - Gerando nova senha");
        var novaSenha = $"Sgp{funcionario[^4..]}";
        Console.WriteLine($"    - nova senha {novaSenha}");

        Console.WriteLine($"    - Criptografando senha");
        byte[] tdesKey = new byte[] { 107, 8, 82, 60, 113, 135, 190, 128, 188, 51, 238, 120, 59, 135, 57, 140, 107, 8, 82, 60, 113, 135, 190, 128 };
        byte[] tdesIV = new byte[] { 113, 135, 190, 128, 186, 217, 34, 47 };
        byte[] plainByte = ASCIIEncoding.ASCII.GetBytes(novaSenha);
        using MemoryStream ms = new MemoryStream();
        SymmetricAlgorithm sym = TripleDES.Create();
        CryptoStream encStream = new CryptoStream(ms, sym.CreateEncryptor(tdesKey, tdesIV), CryptoStreamMode.Write);
        encStream.Write(plainByte, 0, plainByte.Length);
        encStream.FlushFinalBlock();
        byte[] cryptoByte = ms.ToArray();
        var senhaCriptografada = Convert.ToBase64String(cryptoByte);
        Console.WriteLine($"    - Senha criptografada {senhaCriptografada}");

        // Console.WriteLine("    - Atualizando senha no Coresso");
        // await usuarioCoressoRepositorio.AtualizarSenhaUsuario(funcionario, senhaCriptografada);
        // Console.WriteLine("    - Senha atualizada com sucesso");
    }
}
catch (Exception ex)
{
    Console.WriteLine(ex);
}

Console.WriteLine("Fim da aplicação");
Console.ReadKey();
