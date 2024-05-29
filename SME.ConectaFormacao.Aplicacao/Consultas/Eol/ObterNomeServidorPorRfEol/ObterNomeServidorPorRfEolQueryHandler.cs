using MediatR;
using SME.ConectaFormacao.Infra.Servicos.Eol.Interfaces;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ObterNomeServidorPorRfEolQueryHandler : IRequestHandler<ObterNomeServidorPorRfEolQuery, string>
    {
        private readonly IServicoEol _servicoEol;

        public ObterNomeServidorPorRfEolQueryHandler(IServicoEol servicoEol)
        {
            _servicoEol = servicoEol ?? throw new ArgumentNullException(nameof(servicoEol));
        }

        public async Task<string> Handle(ObterNomeServidorPorRfEolQuery request, CancellationToken cancellationToken)
        {
            return await _servicoEol.ObterNomeServidorPorRfEol(request.RfServidor);
        }
    }
}
