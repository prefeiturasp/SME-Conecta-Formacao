using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos.Proposta;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ValidarCriterioValidacaoInscricaoOutrosCommand : IRequest
    {
        public ValidarCriterioValidacaoInscricaoOutrosCommand(IEnumerable<PropostaCriterioValidacaoInscricaoDTO> propostaCriterioValidacaoInscricaos, string criterioValidacaoInscricaoOutros)
        {
            PropostaCriterioValidacaoInscricaos = propostaCriterioValidacaoInscricaos;
            CriterioValidacaoInscricaoOutros = criterioValidacaoInscricaoOutros;
        }

        public IEnumerable<PropostaCriterioValidacaoInscricaoDTO> PropostaCriterioValidacaoInscricaos { get; }
        public string CriterioValidacaoInscricaoOutros { get; set; }
    }
}
