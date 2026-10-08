using MediatR;
using SME.ConectaFormacao.Dominio.Constantes;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ValidarSeDataInscricaoEhMaiorQueDataRealizacaoCommandHandler : IRequestHandler<ValidarSeDataInscricaoEhMaiorQueDataRealizacaoCommand, string>
    {
        public async Task<string> Handle(ValidarSeDataInscricaoEhMaiorQueDataRealizacaoCommand request, CancellationToken cancellationToken)
        {
            if (request.DataInscricaoFim.HasValue && request.DataRealizacaoFim.HasValue)
            {
                var dataRealizacaoFimLimite = request.DataRealizacaoFim.Value.TimeOfDay == TimeSpan.Zero
                    ? request.DataRealizacaoFim.Value.Date.AddDays(1).AddMilliseconds(-1)
                    : request.DataRealizacaoFim.Value;

                if (request.DataInscricaoFim.Value > dataRealizacaoFimLimite)
                    return MensagemNegocio.DATAFIM_INSCRICAO_NAO_PODE_SER_MAIOR_QUE_DATAFIM_REALIZACAO;
            }

            return string.Empty;
        }
    }
}