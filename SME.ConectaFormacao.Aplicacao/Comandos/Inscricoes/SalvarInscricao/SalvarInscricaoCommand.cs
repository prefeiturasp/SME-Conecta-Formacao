using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos.Inscricoes;
using SME.ConectaFormacao.Aplicacao.Dtos.Proposta;

namespace SME.ConectaFormacao.Aplicacao.Comandos.Inscricoes.SalvarInscricao
{
    public class SalvarInscricaoCommand(InscricaoDto inscricaoDto) : IRequest<RetornoDTO>
    {
        public InscricaoDto InscricaoDto { get; } = inscricaoDto;
    }
}
