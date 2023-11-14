using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos.Proposta;
using SME.ConectaFormacao.Dominio.Entidades;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ValidarInformacoesGeraisCommand : IRequest
    {
        public ValidarInformacoesGeraisCommand(PropostaDTO propostaDto)
        {
            PropostaDTO = propostaDto;
        }

        public PropostaDTO PropostaDTO { get; }
    }
}