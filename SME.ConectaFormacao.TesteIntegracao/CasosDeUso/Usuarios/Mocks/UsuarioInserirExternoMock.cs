using Bogus;
using Bogus.Extensions.Brazil;
using SME.ConectaFormacao.Aplicacao.Dtos.Usuario;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Dominio.Extensoes;

namespace SME.ConectaFormacao.TesteIntegracao;

public class UsuarioInserirExternoMock
{
    public static UsuarioExternoDTO GerarUsuarioExternoDTO()
    {
        var pessoa = new Person("pt_BR");
        var cpf = pessoa.Cpf(false);
        var senha = "Minha@Senha1";
        return new UsuarioExternoDTO
        {
            Nome = pessoa.FirstName,
            Login = cpf,
            Email = pessoa.Email,
            EmailEducacional = "teste@edu.sme.prefeitura.sp.gov.br",
            Cpf = cpf,
            Senha = senha,
            ConfirmarSenha = senha,
            CodigoUnidade = new Faker().Random.AlphaNumeric(4).ToString(),
            Tipo = TipoUsuario.Externo,
            TipoEmail = TipoEmail.FuncionarioUnidadeParceira,
            CriadoEm = DateTimeExtension.HorarioBrasilia(),
            CriadoLogin = "Sistema",
            CriadoPor = "Sistema"
        };
    }
}
