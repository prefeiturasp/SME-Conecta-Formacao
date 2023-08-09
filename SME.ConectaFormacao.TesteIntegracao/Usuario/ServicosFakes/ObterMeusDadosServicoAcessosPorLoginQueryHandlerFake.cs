using MediatR;
using SME.ConectaFormacao.Aplicacao;
using SME.ConectaFormacao.Aplicacao.DTOS;
using SME.ConectaFormacao.TesteIntegracao.Usuario.Mocks;

namespace SME.ConectaFormacao.TesteIntegracao.Usuario.ServicosFakes
{
    public class ObterMeusDadosServicoAcessosPorLoginQueryHandlerFake : IRequestHandler<ObterMeusDadosServicoAcessosPorLoginQuery, DadosUsuarioDTO>
    {
        public Task<DadosUsuarioDTO> Handle(ObterMeusDadosServicoAcessosPorLoginQuery request, CancellationToken cancellationToken)
        {
            if (request.Login == UsuarioMock.Login)
                return Task.FromResult(UsuarioMock.DadosUsuarioDTO);

            return Task.FromResult(new DadosUsuarioDTO());
        }
    }
}
