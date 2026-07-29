using MediatR;
using SME.ConectaFormacao.Infra.Servicos.Eol;

namespace SME.ConectaFormacao.Aplicacao.Consultas.Eol.ObterDadosServidorPorRfEol
{
    public class ObterDadosServidorPorRfEolQuery(string rfServidor) : IRequest<UsuarioEolDto?>
    {
        public string RfServidor { get; } = rfServidor;
    }
}
