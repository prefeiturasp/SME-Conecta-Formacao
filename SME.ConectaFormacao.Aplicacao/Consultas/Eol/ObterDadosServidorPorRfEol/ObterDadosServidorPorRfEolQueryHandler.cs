using MediatR;
using SME.ConectaFormacao.Infra.Servicos.Eol;
using SME.ConectaFormacao.Infra.Servicos.Eol.Interfaces;

namespace SME.ConectaFormacao.Aplicacao.Consultas.Eol.ObterDadosServidorPorRfEol
{
    public class ObterDadosServidorPorRfEolQueryHandler(IServicoEol servicoEol) : IRequestHandler<ObterDadosServidorPorRfEolQuery, UsuarioEolDto?>
    {
        public async Task<UsuarioEolDto?> Handle(ObterDadosServidorPorRfEolQuery request, CancellationToken cancellationToken)
        {
            return await servicoEol.ObterDadosServidorPorRfEol(request.RfServidor);
        }
    }
}
