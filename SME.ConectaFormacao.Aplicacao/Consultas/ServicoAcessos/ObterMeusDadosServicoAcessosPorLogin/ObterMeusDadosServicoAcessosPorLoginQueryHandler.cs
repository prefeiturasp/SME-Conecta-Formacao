using System.Text.RegularExpressions;
using AutoMapper;
using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos.Usuario;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Dominio.Excecoes;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;
using SME.ConectaFormacao.Infra.Servicos.Acessos.Interfaces;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ObterMeusDadosServicoAcessosPorLoginQueryHandler : IRequestHandler<ObterMeusDadosServicoAcessosPorLoginQuery, DadosUsuarioDTO>
    {
        private readonly IMapper _mapper;
        private readonly IServicoAcessos _servicoAcessos;
        private readonly IRepositorioUsuario _repositorioUsuario;
        private readonly IMediator _mediator;

        public ObterMeusDadosServicoAcessosPorLoginQueryHandler(IMapper mapper, IServicoAcessos servicoAcessos,IRepositorioUsuario repositorioUsuario,
        IMediator mediator)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _servicoAcessos = servicoAcessos ?? throw new ArgumentNullException(nameof(servicoAcessos));
            _repositorioUsuario = repositorioUsuario ?? throw new ArgumentNullException(nameof(repositorioUsuario));
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task<DadosUsuarioDTO> Handle(ObterMeusDadosServicoAcessosPorLoginQuery request, CancellationToken cancellationToken)
        {
            var usuarioLogado = await _mediator.Send(new ObterUsuarioLogadoQuery());
            var acessoDadosUsuario = await _servicoAcessos.ObterMeusDados(request.Login);
            if (usuarioLogado.Tipo == TipoUsuario.Externo)
            {
                var unidade = await  _mediator.Send(new ObterUnidadePorCodigoEOLQuery(usuarioLogado.CodigoEolUnidade));
                acessoDadosUsuario.Tipo = (int)TipoUsuario.Externo;
                acessoDadosUsuario.NomeUnidade = unidade.NomeUnidade;
            }
            acessoDadosUsuario.EmailEducacional = await _repositorioUsuario.ObterEmailEducacionalPorLogin(request.Login);
            
            var pattern = @"@edu\.sme\.prefeitura\.sp\.gov\.br$";
            if (Regex.IsMatch(acessoDadosUsuario.Email, pattern, RegexOptions.IgnoreCase) && string.IsNullOrEmpty(acessoDadosUsuario.EmailEducacional))
                acessoDadosUsuario.EmailEducacional = acessoDadosUsuario.Email;
            
            if(string.IsNullOrEmpty(acessoDadosUsuario.EmailEducacional))
                acessoDadosUsuario.EmailEducacional = await _mediator.Send(new GerarEmailEducacionalCommand(usuarioLogado), cancellationToken);
            
            return _mapper.Map<DadosUsuarioDTO>(acessoDadosUsuario);
        }
    }
}
