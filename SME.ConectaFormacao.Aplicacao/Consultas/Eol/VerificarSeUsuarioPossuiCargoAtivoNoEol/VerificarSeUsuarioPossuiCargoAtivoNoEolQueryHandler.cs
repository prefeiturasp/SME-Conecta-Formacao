using MediatR;
using SME.ConectaFormacao.Infra.Servicos.Eol.Interfaces;

namespace SME.ConectaFormacao.Aplicacao
{
    public class VerificarSeUsuarioPossuiCargoAtivoNoEolQueryHandler : IRequestHandler<VerificarSeUsuarioPossuiCargoAtivoNoEolQuery, IEnumerable<string>>
    {
        private readonly IServicoEol _servicoEol;

        public VerificarSeUsuarioPossuiCargoAtivoNoEolQueryHandler(IServicoEol servicoEol)
        {
            _servicoEol = servicoEol ?? throw new ArgumentNullException(nameof(servicoEol));
        }

        public async Task<IEnumerable<string>> Handle(VerificarSeUsuarioPossuiCargoAtivoNoEolQuery request, CancellationToken cancellationToken)
        {
            return await _servicoEol.VerificarSeUsuarioEstaAtivo(request.Login);
        }
    }
}