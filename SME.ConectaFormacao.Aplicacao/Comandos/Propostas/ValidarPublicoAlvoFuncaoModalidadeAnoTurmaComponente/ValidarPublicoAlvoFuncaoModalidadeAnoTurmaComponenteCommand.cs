using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos.Proposta;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ValidarPublicoAlvoFuncaoModalidadeAnoTurmaComponenteCommand : IRequest<List<string>>
    {
        public ValidarPublicoAlvoFuncaoModalidadeAnoTurmaComponenteCommand(IEnumerable<PropostaPublicoAlvoDTO> publicosAlvoDaProposta, IEnumerable<PropostaFuncaoEspecificaDTO> funcoesEspecificasDaProposta,
            IEnumerable<PropostaModalidadeDTO> modalidadesDaProposta, IEnumerable<PropostaAnoTurmaDTO> anosTurmaDaProposta, IEnumerable<PropostaComponenteCurricularDTO> componentesCurricularesDaProposta)
        {
            PublicosAlvoDaProposta = publicosAlvoDaProposta;
            FuncoesEspecificasDaProposta = funcoesEspecificasDaProposta;
            ModalidadesDaProposta = modalidadesDaProposta;
            AnosTurmaDaProposta = anosTurmaDaProposta;
            ComponentesCurricularesDaProposta = componentesCurricularesDaProposta;
        }

        public IEnumerable<PropostaPublicoAlvoDTO> PublicosAlvoDaProposta { get; }
        public IEnumerable<PropostaFuncaoEspecificaDTO> FuncoesEspecificasDaProposta { get; }
        public IEnumerable<PropostaModalidadeDTO> ModalidadesDaProposta { get; }
        public IEnumerable<PropostaAnoTurmaDTO> AnosTurmaDaProposta { get; }
        public IEnumerable<PropostaComponenteCurricularDTO> ComponentesCurricularesDaProposta { get; }
    }
}
