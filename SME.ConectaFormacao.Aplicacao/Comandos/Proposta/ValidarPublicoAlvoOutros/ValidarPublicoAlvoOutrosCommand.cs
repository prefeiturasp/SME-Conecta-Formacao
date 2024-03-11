using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos.Proposta;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ValidarPublicoAlvoOutrosCommand : IRequest
    {
        public ValidarPublicoAlvoOutrosCommand(bool ehPropostaAutomatica, IEnumerable<PropostaPublicoAlvoDTO> publicosAlvo, string? publicoAlvoOutros)
        {
            EhPropostaAutomatica = ehPropostaAutomatica;
            PublicosAlvo = publicosAlvo;
            PublicoAlvoOutros = publicoAlvoOutros;
        }

        public bool EhPropostaAutomatica { get; set; }
        public IEnumerable<PropostaPublicoAlvoDTO> PublicosAlvo { get; }
        public string? PublicoAlvoOutros { get; }
    }
}
