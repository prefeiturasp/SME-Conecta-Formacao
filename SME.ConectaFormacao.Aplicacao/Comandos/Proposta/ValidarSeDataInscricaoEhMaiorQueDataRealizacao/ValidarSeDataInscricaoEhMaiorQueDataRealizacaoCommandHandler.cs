using MediatR;
using SME.ConectaFormacao.Dominio.Constantes;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ValidarSeDataInscricaoEhMaiorQueDataRealizacaoCommandHandler : IRequestHandler<ValidarSeDataInscricaoEhMaiorQueDataRealizacaoCommand, string>
    {
        public async Task<string> Handle(ValidarSeDataInscricaoEhMaiorQueDataRealizacaoCommand request, CancellationToken cancellationToken)
        {
            if (request.DataInscricaoFim > request.DataRealizacaoFim)
                return MensagemNegocio.DATAFIM_INSCRICAO_NAO_PODE_SER_MAIOR_QUE_DATAFIM_REALIZACAO;

            return string.Empty;
        }
    }
}