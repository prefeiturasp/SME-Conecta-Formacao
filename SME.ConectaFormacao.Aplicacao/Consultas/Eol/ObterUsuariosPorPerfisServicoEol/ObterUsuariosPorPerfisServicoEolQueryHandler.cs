using MediatR;
using SME.ConectaFormacao.Infra.Servicos.Eol;
using SME.ConectaFormacao.Infra.Servicos.Eol.Interfaces;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ObterUsuariosPorPerfisServicoEolQueryHandler : IRequestHandler<ObterUsuariosPorPerfisServicoEolQuery, IEnumerable<UsuarioPerfilServicoEol>>
    {
        private readonly IServicoEol _servicoEol;

        public ObterUsuariosPorPerfisServicoEolQueryHandler(IServicoEol servicoEol)
        {
            _servicoEol = servicoEol ?? throw new ArgumentNullException(nameof(servicoEol));
        }

        public async Task<IEnumerable<UsuarioPerfilServicoEol>> Handle(
            ObterUsuariosPorPerfisServicoEolQuery request, CancellationToken cancellationToken)
        {
            return await _servicoEol.ObterUsuariosPorPerfis(request.Perfis);
        }
    }
}