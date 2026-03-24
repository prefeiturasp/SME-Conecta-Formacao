using MediatR;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ObterPropostaTipoInscricaoPorIdQueryHandler : IRequestHandler<ObterPropostaTipoInscricaoPorIdQuery, IEnumerable<PropostaTipoInscricao>>
    {
        private readonly IRepositorioProposta _repositorioProposta;

        public ObterPropostaTipoInscricaoPorIdQueryHandler(IRepositorioProposta repositorioProposta)
        {
            _repositorioProposta = repositorioProposta ?? throw new ArgumentNullException(nameof(repositorioProposta));
        }

        public async Task<IEnumerable<PropostaTipoInscricao>> Handle(ObterPropostaTipoInscricaoPorIdQuery request, CancellationToken cancellationToken)
        {
            return await _repositorioProposta.ObterTiposInscricaoPorId(request.PropostaId);
        }
    }
}
