using MediatR;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ObterPareceristasAdicionadosNaPropostaQueryHandler : IRequestHandler<ObterPareceristasAdicionadosNaPropostaQuery, IEnumerable<PropostaParecerista>> 
    {
        private readonly IRepositorioProposta _repositorioProposta;

        public ObterPareceristasAdicionadosNaPropostaQueryHandler(IRepositorioProposta repositorioProposta)
        {
            _repositorioProposta = repositorioProposta ?? throw new ArgumentNullException(nameof(repositorioProposta));
        }

        public async Task<IEnumerable<PropostaParecerista>> Handle(ObterPareceristasAdicionadosNaPropostaQuery request, CancellationToken cancellationToken)
        {
            return await _repositorioProposta.ObterPareceristasPorPropostaId(request.PropostaId);
        }
    }
}
