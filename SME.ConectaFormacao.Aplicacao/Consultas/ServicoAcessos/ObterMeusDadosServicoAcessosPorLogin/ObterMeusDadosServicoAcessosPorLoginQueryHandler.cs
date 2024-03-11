using AutoMapper;
using MediatR;
using SME.ConectaFormacao.Aplicacao.Consultas.Eol.ObterDadosFuncionarioExterno;
using SME.ConectaFormacao.Aplicacao.Dtos.Usuario;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Dominio.Extensoes;
using SME.ConectaFormacao.Infra.Servicos.Acessos.Interfaces;
using SME.ConectaFormacao.Infra.Servicos.Cache;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ObterMeusDadosServicoAcessosPorLoginQueryHandler : IRequestHandler<ObterMeusDadosServicoAcessosPorLoginQuery, DadosUsuarioDTO>
    {
        private readonly IMapper _mapper;
        private readonly IServicoAcessos _servicoAcessos;
        private readonly IMediator _mediator;
        private readonly ICacheDistribuido _cacheDistribuido;

        public ObterMeusDadosServicoAcessosPorLoginQueryHandler(IMapper mapper, IServicoAcessos servicoAcessos,IMediator mediator,ICacheDistribuido cacheDistribuido)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _servicoAcessos = servicoAcessos ?? throw new ArgumentNullException(nameof(servicoAcessos));
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _cacheDistribuido = cacheDistribuido ?? throw new ArgumentNullException(nameof(cacheDistribuido));
        }

        public async Task<DadosUsuarioDTO> Handle(ObterMeusDadosServicoAcessosPorLoginQuery request, CancellationToken cancellationToken)
        {
            var usuarioLogado = await _mediator.Send(new ObterUsuarioLogadoQuery());
            var acessoDadosUsuario = await _servicoAcessos.ObterMeusDados(request.Login);
            if (usuarioLogado.Tipo == TipoUsuario.Externo)
            {
                var unidade =  await _mediator.Send(new ObterUnidadePorCodigoEOLQuery(usuarioLogado.CodigoEolUnidade));
                await _cacheDistribuido.ObterAsync(CacheDistribuidoNomes.NomeUnidade.Parametros(usuarioLogado.CodigoEolUnidade), () => _mediator.Send(new ObterUnidadePorCodigoEOLQuery(usuarioLogado.CodigoEolUnidade)));
                
                acessoDadosUsuario.Tipo = (int)TipoUsuario.Externo;
                acessoDadosUsuario.NomeUnidade = unidade.NomeUnidade;
            }
            return _mapper.Map<DadosUsuarioDTO>(acessoDadosUsuario);
        }
    }
}
