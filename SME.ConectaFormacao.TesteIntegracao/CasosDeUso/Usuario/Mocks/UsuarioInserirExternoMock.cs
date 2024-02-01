using Bogus;
using SME.ConectaFormacao.Aplicacao.Dtos.Usuario;
using SME.ConectaFormacao.Dominio.Enumerados;

namespace SME.ConectaFormacao.TesteIntegracao;

public class UsuarioInserirExternoMock
{
    public static UsuarioExternoDTO GerarUsuarioExternoDTO()
    {
        var pessoa = new Person("pt_BR");
        var cpf = new Faker().Random.AlphaNumeric(11);
        var senha = "Minha@Senha1";
        return new UsuarioExternoDTO
        {
            Nome = pessoa.FirstName,
            Login = cpf,
            Email = pessoa.Email,
            Cpf = cpf,
            Senha = senha,
            ConfirmarSenha = senha,
            CodigoUe = new Faker().Random.AlphaNumeric(4).ToString(),
            Tipo = TipoUsuario.Externo
        };
    }
}
