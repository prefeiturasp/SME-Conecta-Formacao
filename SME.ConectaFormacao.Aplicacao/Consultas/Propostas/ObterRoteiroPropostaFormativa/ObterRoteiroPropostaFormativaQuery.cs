using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos.Proposta;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ObterRoteiroPropostaFormativaQuery : IRequest<RoteiroPropostaFormativaDTO>
    {
        private static ObterRoteiroPropostaFormativaQuery _instancia;
        public static ObterRoteiroPropostaFormativaQuery Instancia => _instancia ??= new();
    }
}
