using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Excecoes;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ObterPropostaTurmasPorIdQueryHandler : IRequestHandler<ObterPropostaTurmasPorIdQuery, IEnumerable<RetornoListagemDTO>>
    {
        public readonly IRepositorioProposta _repositorioProposta;

        public ObterPropostaTurmasPorIdQueryHandler(IRepositorioProposta repositorioProposta)
        {
            _repositorioProposta = repositorioProposta ?? throw new ArgumentNullException(nameof(repositorioProposta));
        }

        public async Task<IEnumerable<RetornoListagemDTO>> Handle(ObterPropostaTurmasPorIdQuery request, CancellationToken cancellationToken)
        {
            var proposta = await _repositorioProposta.ObterPorId(request.Id) ??
                throw new NegocioException(MensagemNegocio.PROPOSTA_NAO_ENCONTRADA, System.Net.HttpStatusCode.NotFound);

            var retorno = new List<RetornoListagemDTO>();
            for (int i = 1; i <= proposta.QuantidadeTurmas.GetValueOrDefault(); i++)
                retorno.Add(new RetornoListagemDTO { Id = i, Descricao = $"Turma {i}" });

            return retorno;
        }
    }
}
