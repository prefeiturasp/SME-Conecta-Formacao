using MediatR;
using SME.ConectaFormacao.Aplicacao.Consultas.ServicoAcessos.ObterUsuariosPareceristas;
using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Infra.Servicos.Acessos.Interfaces;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ObterUsuariosPareceristasQueryHandler : IRequestHandler<ObterUsuariosPareceristasQuery, IEnumerable<RetornoUsuarioLoginNomeDTO>>
    {
        private readonly IServicoAcessos _servicoAcessos;

        public ObterUsuariosPareceristasQueryHandler(IServicoAcessos servicoAcessos)
        {
            _servicoAcessos = servicoAcessos ?? throw new ArgumentNullException(nameof(servicoAcessos));
        }

        public async Task<IEnumerable<RetornoUsuarioLoginNomeDTO>> Handle(ObterUsuariosPareceristasQuery request, CancellationToken cancellationToken)
        {
            var consulta = await _servicoAcessos.ObterUsuariosPerfilPareceristas();
            if (!consulta.Any())
                return Enumerable.Empty<RetornoUsuarioLoginNomeDTO>();

            return consulta.Select(x => new RetornoUsuarioLoginNomeDTO() { Login = x.Login, Nome = x.Nome });
        }
    }
}