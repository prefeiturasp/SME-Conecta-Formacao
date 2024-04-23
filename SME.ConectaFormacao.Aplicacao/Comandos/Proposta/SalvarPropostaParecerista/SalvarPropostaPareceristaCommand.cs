using MediatR;

namespace SME.ConectaFormacao.Aplicacao
{
    public class SalvarPropostaPareceristaCommand : IRequest<long>
    {
        public long PropostaId { get; }
        public string RegistroFuncional { get; }
        public string NomeParecerista { get; }
    }
}