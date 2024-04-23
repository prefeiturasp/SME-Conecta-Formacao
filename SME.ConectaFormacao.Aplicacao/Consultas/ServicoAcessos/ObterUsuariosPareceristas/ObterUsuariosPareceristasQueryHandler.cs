using System.Collections.Immutable;
using MediatR;
using SME.ConectaFormacao.Aplicacao.Consultas.ServicoAcessos.ObterUsuariosPareceristas;
using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Infra.Servicos.Acessos.Interfaces;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ObterUsuariosPareceristasQueryHandler : IRequestHandler<ObterUsuariosPareceristasQuery,IEnumerable<RetornoUsuriosPareceristasDTO>>
    {
        private readonly IServicoAcessos _servicoAcessos;

        public ObterUsuariosPareceristasQueryHandler(IServicoAcessos servicoAcessos)
        {
            _servicoAcessos = servicoAcessos ?? throw new ArgumentNullException(nameof(servicoAcessos));
        }

        public async Task<IEnumerable<RetornoUsuriosPareceristasDTO>> Handle(ObterUsuariosPareceristasQuery request, CancellationToken cancellationToken)
        {
            var consulta = await _servicoAcessos.ObterUsuariosPerfilPareceristas(request.Rf, request.Nome);
            if(!consulta.Any())
                return Enumerable.Empty<RetornoUsuriosPareceristasDTO>();

            return consulta.Select(x => new RetornoUsuriosPareceristasDTO() { Login = x.Login, Nome = x.Nome });
        }
    }
}