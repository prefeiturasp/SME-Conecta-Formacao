using MediatR;
using SME.ConectaFormacao.Aplicacao;
using SME.ConectaFormacao.Aplicacao.Dtos.Autenticacao;

namespace SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Usuario.ServicosFakes
{
    public class ObterUsuarioServicoAcessosPorLoginSenhaQueryHandlerRetornoValido : IRequestHandler<ObterUsuarioServicoAcessosPorLoginSenhaQuery,UsuarioAutenticacaoRetornoDTO>
    {
        public async Task<UsuarioAutenticacaoRetornoDTO> Handle(ObterUsuarioServicoAcessosPorLoginSenhaQuery request, CancellationToken cancellationToken)
        {
            return new UsuarioAutenticacaoRetornoDTO {Login = request.Login, Nome = "Teste", Email = "teste@teste.com"};
        }
    }
}