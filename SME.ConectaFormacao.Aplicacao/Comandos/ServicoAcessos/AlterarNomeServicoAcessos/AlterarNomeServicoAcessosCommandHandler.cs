using MediatR;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Extensoes;
using SME.ConectaFormacao.Infra.Servicos.Acessos.Interfaces;

namespace SME.ConectaFormacao.Aplicacao
{
    public class AlterarNomeServicoAcessosCommandHandler : IRequestHandler<AlterarNomeServicoAcessosCommand, bool>
    {
        private readonly IServicoAcessos _servicoAcessos;
        private readonly IMediator _mediator;

        public AlterarNomeServicoAcessosCommandHandler(IServicoAcessos servicoAcessos, IMediator mediator)
        {
            _servicoAcessos = servicoAcessos ?? throw new ArgumentNullException(nameof(servicoAcessos));
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task<bool> Handle(AlterarNomeServicoAcessosCommand request, CancellationToken cancellationToken)
        {
            await _mediator.Send(new RemoverCacheCommand(CacheDistribuidoNomes.CargosFuncoesDresEolFuncionario.Parametros(request.Login)));

            return await _servicoAcessos.AlterarNome(request.Login, request.Nome);
        }
    }
}
