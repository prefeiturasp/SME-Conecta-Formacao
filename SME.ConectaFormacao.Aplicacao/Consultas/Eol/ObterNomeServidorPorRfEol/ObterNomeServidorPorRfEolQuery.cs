using MediatR;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ObterNomeServidorPorRfEolQuery : IRequest<string>
    {
        public ObterNomeServidorPorRfEolQuery(string rfServidor)
        {
            RfServidor = rfServidor;
        }
        
        public string RfServidor { get; }
    }
}
