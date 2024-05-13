using MediatR;
using SME.ConectaFormacao.Infra.Servicos.Relatorio.Interfaces;

namespace SME.ConectaFormacao.Aplicacao.Consultas.Relatorio.ObterRelatorioSincrono
{
    public class ObterRelatorioProspostaLaudaPublicacaoHandler : IRequestHandler<ObterRelatorioProspostaLaudaPublicacaoQuery, string>
    {
        private readonly IServicoRelatorio _servicoRelatorio;

        public ObterRelatorioProspostaLaudaPublicacaoHandler(IServicoRelatorio servicoRelatorio)
        {
            _servicoRelatorio = servicoRelatorio ?? throw new ArgumentNullException(nameof(servicoRelatorio));
        }

        public Task<string> Handle(ObterRelatorioProspostaLaudaPublicacaoQuery request, CancellationToken cancellationToken)
        {
            return _servicoRelatorio.ObterRelatorioPropostaLaudaDePublicacao(request.PropostaId);
        }
    }
}
