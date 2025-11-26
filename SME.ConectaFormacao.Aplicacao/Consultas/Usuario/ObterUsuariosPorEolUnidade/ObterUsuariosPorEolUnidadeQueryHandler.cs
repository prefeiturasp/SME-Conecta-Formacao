using AutoMapper;
using MediatR;
using SME.ConectaFormacao.Aplicacao.Consultas.Usuario.ObterUsuariosPorUnidadeEol;
using SME.ConectaFormacao.Aplicacao.Dtos.Usuario;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao.Consultas.Usuario.ObterUsuariosPorEolUnidade
{
    public class ObterUsuariosPorEolUnidadeQueryHandler(IMapper mapper, IRepositorioUsuario repositorioUsuario) :
        IRequestHandler<ObterUsuariosPorEolUnidadeQuery, IEnumerable<DadosLoginUsuarioDto>>
    {
        public async Task<IEnumerable<DadosLoginUsuarioDto>> Handle(ObterUsuariosPorEolUnidadeQuery request, CancellationToken cancellationToken)
        {
            var dadosUsuario = await repositorioUsuario.ObterUsuariosPorEolUnidadeAsync(request.CodigoEolUnidade, request.Login, request.Nome);
            var dadosUsuarioDto = mapper.Map<IEnumerable<DadosLoginUsuarioDto>>(dadosUsuario);
            return dadosUsuarioDto;
        }
    }
}
