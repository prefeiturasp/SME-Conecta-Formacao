using MediatR;
using SME.ConectaFormacao.Aplicacao;
using SME.ConectaFormacao.Aplicacao.Dtos.Usuario;
using SME.ConectaFormacao.TesteIntegracao.Usuario.Mocks;

namespace SME.ConectaFormacao.TesteIntegracao.Usuario.ServicosFakes
{
    public class ObterMeusDadosServicoAcessosPorLoginQueryHandlerFake : IRequestHandler<ObterMeusDadosServicoAcessosPorLoginQuery, DadosUsuarioDTO>
    {
        public Task<DadosUsuarioDTO> Handle(ObterMeusDadosServicoAcessosPorLoginQuery request, CancellationToken cancellationToken)
        {
            if (request.Login == UsuarioMeusDadosMock.Login)
                return Task.FromResult(UsuarioMeusDadosMock.DadosUsuarioDTO);

            return Task.FromResult(new DadosUsuarioDTO());
        }
    }
}
