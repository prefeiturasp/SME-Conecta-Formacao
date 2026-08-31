using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos.Proposta;
using System.Diagnostics.CodeAnalysis;

namespace SME.ConectaFormacao.Aplicacao
{
    [ExcludeFromCodeCoverage]
    public class ObterRoteiroPropostaFormativaQuery : IRequest<RoteiroPropostaFormativaDTO>
    {
        private static ObterRoteiroPropostaFormativaQuery _instancia;
        public static ObterRoteiroPropostaFormativaQuery Instancia => _instancia ??= new();
    }
}
