using MediatR;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao.Consultas.Proposta.ObterSugestoesPareceristas
{
    public class ObterSugestoesPareceristasQueryHandler : IRequestHandler<ObterSugestoesPareceristasQuery, string>
    {
        private readonly IRepositorioProposta _repositorioProposta;

        public ObterSugestoesPareceristasQueryHandler(IRepositorioProposta repositorioProposta)
        {
            _repositorioProposta = repositorioProposta ?? throw new ArgumentNullException(nameof(repositorioProposta));
        }

        public async Task<string> Handle(ObterSugestoesPareceristasQuery request, CancellationToken cancellationToken)
        {
            var sugestoes = await _repositorioProposta.ObterSugestaoParecerPareceristas(request.PropostaId, request.Situacao);

            if (sugestoes.Any())
            {
                return string.Join('\n', sugestoes.OrderBy(o => o.Id).Select(s => s.Justificativa));
            }

            return string.Empty;
        }
    }
}
