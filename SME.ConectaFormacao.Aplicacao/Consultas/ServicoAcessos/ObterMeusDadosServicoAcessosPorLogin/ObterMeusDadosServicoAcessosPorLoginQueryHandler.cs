using AutoMapper;
using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos.Usuario;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Dominio.Extensoes;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;
using SME.ConectaFormacao.Infra.Servicos.Acessos.Interfaces;
using System.Text.RegularExpressions;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ObterMeusDadosServicoAcessosPorLoginQueryHandler : IRequestHandler<ObterMeusDadosServicoAcessosPorLoginQuery, DadosUsuarioDTO>
    {
        private readonly IMapper _mapper;
        private readonly IServicoAcessos _servicoAcessos;
        private readonly IRepositorioUsuario _repositorioUsuario;
        private readonly IMediator _mediator;

        public ObterMeusDadosServicoAcessosPorLoginQueryHandler(IMapper mapper, IServicoAcessos servicoAcessos, IRepositorioUsuario repositorioUsuario,
        IMediator mediator)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _servicoAcessos = servicoAcessos ?? throw new ArgumentNullException(nameof(servicoAcessos));
            _repositorioUsuario = repositorioUsuario ?? throw new ArgumentNullException(nameof(repositorioUsuario));
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task<DadosUsuarioDTO> Handle(ObterMeusDadosServicoAcessosPorLoginQuery request, CancellationToken cancellationToken)
        {
            var acessoDadosUsuario = await _servicoAcessos.ObterMeusDados(request.Login);

            var usuario = await _repositorioUsuario.ObterPorLogin(request.Login);
            if (usuario.NaoEhNulo() && usuario.Tipo.EhExterno())
            {
                var unidade = !string.IsNullOrEmpty(usuario.CodigoEolUnidade) ? await _mediator.Send(new ObterUnidadePorCodigoEOLQuery(usuario.CodigoEolUnidade), cancellationToken) : null;
                acessoDadosUsuario.Tipo = (int)TipoUsuario.Externo;
                acessoDadosUsuario.NomeUnidade = unidade?.NomeUnidade!;
            }
            var (tipoEmail, emailEducacional) = await _repositorioUsuario.ObterEmailEducacionalPorLogin(request.Login);
            acessoDadosUsuario.TipoEmail = tipoEmail;
            acessoDadosUsuario.EmailEducacional = emailEducacional;
            acessoDadosUsuario.Nome = acessoDadosUsuario.Nome ?? await ObterNomeUsuarioPeloLogin(request.Login);
            acessoDadosUsuario.Login = acessoDadosUsuario.Login ?? request.Login;

            var pattern = @"@edu\.sme\.prefeitura\.sp\.gov\.br$";
            if (!string.IsNullOrEmpty(acessoDadosUsuario.Email) && Regex.IsMatch(acessoDadosUsuario.Email, pattern, RegexOptions.IgnoreCase) &&
                 acessoDadosUsuario.EmailEducacional.NaoEstaPreenchido())

                acessoDadosUsuario.EmailEducacional = acessoDadosUsuario.Email;

            if (usuario.NaoEhNulo() && acessoDadosUsuario.EmailEducacional.NaoEstaPreenchido())
                acessoDadosUsuario.EmailEducacional = await _mediator.Send(new GerarEmailEducacionalCommand(usuario), cancellationToken);

            return _mapper.Map<DadosUsuarioDTO>(acessoDadosUsuario);
        }

        private async Task<string> ObterNomeUsuarioPeloLogin(string login)
        {
            var cursista = await _mediator.Send(new ObterNomeCpfProfissionalPorRegistroFuncionalQuery(login));
            return cursista.Nome;
        }
    }
}
