using MediatR;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao
{
    public class PropostasConfirmadasQueNaoEncerramAindaQueryHandler : IRequestHandler<PropostasConfirmadasQueNaoEncerramAindaQuery, IEnumerable<long>>
    {
        private readonly IRepositorioProposta _repositorioProposta;

        public PropostasConfirmadasQueNaoEncerramAindaQueryHandler(IRepositorioProposta repositorioProposta)
        {
            _repositorioProposta = repositorioProposta ?? throw new ArgumentNullException(nameof(repositorioProposta));
        }

        public async Task<IEnumerable<long>> Handle(PropostasConfirmadasQueNaoEncerramAindaQuery request, CancellationToken cancellationToken)
        {
            return await _repositorioProposta.PropostasConfirmadasQueNaoEncerramAinda();
        }
    }
}