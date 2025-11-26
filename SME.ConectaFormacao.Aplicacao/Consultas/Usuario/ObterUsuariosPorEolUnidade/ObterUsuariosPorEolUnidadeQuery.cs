using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos.Usuario;
using SME.ConectaFormacao.Dominio.Extensoes;

namespace SME.ConectaFormacao.Aplicacao.Consultas.Usuario.ObterUsuariosPorUnidadeEol
{
    public class ObterUsuariosPorEolUnidadeQuery : IRequest<IEnumerable<DadosLoginUsuarioDto>>
    {
        public string CodigoEolUnidade { get; set; }
        public string? Login { get; set; }
        public string? Nome { get; set; }

        public ObterUsuariosPorEolUnidadeQuery(string codigoEolUnidade, string? login = null, string? nome = null)
        {
            CodigoEolUnidade = codigoEolUnidade;

            if (login is not null) 
                Login = login.SomenteNumeros();

            if (!string.IsNullOrWhiteSpace(nome))
                Nome = nome.ToLower().Trim();
        }
    }
}
