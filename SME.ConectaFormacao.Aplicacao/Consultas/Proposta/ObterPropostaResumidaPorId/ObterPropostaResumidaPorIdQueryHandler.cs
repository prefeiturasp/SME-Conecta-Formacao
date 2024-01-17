using MediatR;
using SME.ConectaFormacao.Dominio.ObjetosDeValor;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ObterPropostaResumidaPorIdQueryHandler : IRequestHandler<ObterPropostaResumidaPorIdQuery, IEnumerable<FormacaoResumida>>
    {
        private readonly IRepositorioProposta _repositorioProposta;

        public ObterPropostaResumidaPorIdQueryHandler(IRepositorioProposta repositorioProposta)
        {
            _repositorioProposta = repositorioProposta ?? throw new ArgumentNullException(nameof(repositorioProposta));
        }

        public async Task<IEnumerable<FormacaoResumida>> Handle(ObterPropostaResumidaPorIdQuery request, CancellationToken cancellationToken)
        {
            var propostas = await _repositorioProposta.ObterPropostaResumidaPorId(request.PropostaId);

            var formacoesResumidas = new List<FormacaoResumida>();

            foreach (var proposta in propostas)
            {
                var formacaoResumida = await _repositorioProposta.ObterFormacaoResumidaPorPropostaId(proposta.Id);
                formacaoResumida.PropostaId = proposta.Id;
                formacaoResumida.TipoInscricao = proposta.TipoInscricao.GetValueOrDefault();
                formacoesResumidas.Add(formacaoResumida);
            }

            return formacoesResumidas;
        }
    }
}