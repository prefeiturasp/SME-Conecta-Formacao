using MediatR;
using SME.ConectaFormacao.Infra.Servicos.Relatorio.Interfaces;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ObterRelatorioProspostaLaudaCompletaQueryHandler : IRequestHandler<ObterRelatorioProspostaLaudaCompletaQuery, string>
    {
        private readonly IServicoRelatorio _servicoRelatorio;

        public ObterRelatorioProspostaLaudaCompletaQueryHandler(IServicoRelatorio servicoRelatorio)
        {
            _servicoRelatorio = servicoRelatorio ?? throw new ArgumentNullException(nameof(servicoRelatorio));
        }

        public Task<string> Handle(ObterRelatorioProspostaLaudaCompletaQuery request, CancellationToken cancellationToken)
        {
            return _servicoRelatorio.ObterRelatorioPropostaLaudaCompleta(request.PropostaId);
        }
    }
}
