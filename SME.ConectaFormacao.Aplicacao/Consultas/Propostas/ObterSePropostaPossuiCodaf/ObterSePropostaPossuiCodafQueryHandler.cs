using MediatR;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao.Consultas.Propostas.ObterSePropostaPossuiCodaf
{
    public class ObterSePropostaPossuiCodafQueryHandler(
        IRepositorioCodafListaPresenca repositorioCodafListaPresenca,
        IRepositorioCodafCursoNaoHomologado repositorioCodafCursoNaoHomologado)
        : IRequestHandler<ObterSePropostaPossuiCodafQuery, bool>
    {
        public async Task<bool> Handle(ObterSePropostaPossuiCodafQuery request, CancellationToken cancellationToken)
        {
            if (await repositorioCodafListaPresenca.PossuiPorPropostaIdAsync(request.PropostaId))
                return true;

            return await repositorioCodafCursoNaoHomologado.PossuiPorPropostaIdAsync(request.PropostaId);
        }
    }
}
