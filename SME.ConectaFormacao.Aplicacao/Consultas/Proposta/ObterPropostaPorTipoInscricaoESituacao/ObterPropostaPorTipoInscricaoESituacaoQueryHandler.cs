using MediatR;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.ObjetosDeValor;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ObterPropostaPorTipoInscricaoESituacaoQueryHandler : IRequestHandler<ObterPropostaPorTipoInscricaoESituacaoQuery, IEnumerable<FormacaoResumida>>
    {
        private readonly IRepositorioProposta _repositorioProposta;

        public ObterPropostaPorTipoInscricaoESituacaoQueryHandler(IRepositorioProposta repositorioProposta)
        {
            _repositorioProposta = repositorioProposta ?? throw new ArgumentNullException(nameof(repositorioProposta));
        }

        public async Task<IEnumerable<FormacaoResumida>> Handle(ObterPropostaPorTipoInscricaoESituacaoQuery request, CancellationToken cancellationToken)
        {
            var propostas = await _repositorioProposta.ObterPropostaPorTipoInscricaoESituacao(request.TiposInscricoes, request.Situacao);

            var formacoesResumidas = new List<FormacaoResumida>();

            foreach (var proposta in propostas)
            {
                var formacaoResumida = await _repositorioProposta.ObterFormacaoResumidaPorPropostaId(proposta.Id);
                formacaoResumida.FormacaoHomologada = proposta.FormacaoHomologada.GetValueOrDefault();
                formacaoResumida.PropostaId = proposta.Id;
                formacaoResumida.TipoInscricao = proposta.TipoInscricao.GetValueOrDefault();
                formacoesResumidas.Add(formacaoResumida);
            }

            return formacoesResumidas;
        }
    }
}