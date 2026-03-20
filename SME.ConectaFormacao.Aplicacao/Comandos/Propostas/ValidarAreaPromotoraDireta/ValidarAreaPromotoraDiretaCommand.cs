using MediatR;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ValidarAreaPromotoraCommand : IRequest
    {
        public ValidarAreaPromotoraCommand(long areaPromotoraId, bool? integrarNoGsa)
        {
            IntegrarNoGSA = integrarNoGsa;
            AreaPromotoraId = areaPromotoraId;
        }
        public long AreaPromotoraId { get; set; }
        public bool? IntegrarNoGSA;
    }
}
