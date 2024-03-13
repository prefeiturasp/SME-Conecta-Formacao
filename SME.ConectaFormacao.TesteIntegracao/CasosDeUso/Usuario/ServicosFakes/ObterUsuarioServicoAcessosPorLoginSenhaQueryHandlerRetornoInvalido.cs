using MediatR;
using SME.ConectaFormacao.Aplicacao;
using SME.ConectaFormacao.Aplicacao.Dtos.Autenticacao;

namespace SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Usuario.ServicosFakes
{
    public class ObterUsuarioServicoAcessosPorLoginSenhaQueryHandlerRetornoInvalido : IRequestHandler<ObterUsuarioServicoAcessosPorLoginSenhaQuery,UsuarioAutenticacaoRetornoDTO>
    {
        public async Task<UsuarioAutenticacaoRetornoDTO> Handle(ObterUsuarioServicoAcessosPorLoginSenhaQuery request, CancellationToken cancellationToken)
        {
            return new UsuarioAutenticacaoRetornoDTO {Login = string.Empty, Nome = string.Empty, Email = string.Empty};
        }
    }
}