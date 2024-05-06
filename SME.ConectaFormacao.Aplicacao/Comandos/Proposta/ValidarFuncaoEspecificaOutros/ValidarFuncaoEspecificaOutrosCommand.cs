using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos.Proposta;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ValidarFuncaoEspecificaOutrosCommand : IRequest<List<string>>
    {
        public ValidarFuncaoEspecificaOutrosCommand(IEnumerable<PropostaFuncaoEspecificaDTO> propostaFuncoesEspecificas, string? funcaoEspecificaOutros)
        {
            PropostaFuncoesEspecificas = propostaFuncoesEspecificas;
            FuncaoEspecificaOutros = funcaoEspecificaOutros;
        }

        public IEnumerable<PropostaFuncaoEspecificaDTO> PropostaFuncoesEspecificas { get; }
        public string? FuncaoEspecificaOutros { get; set; }
    }
}
