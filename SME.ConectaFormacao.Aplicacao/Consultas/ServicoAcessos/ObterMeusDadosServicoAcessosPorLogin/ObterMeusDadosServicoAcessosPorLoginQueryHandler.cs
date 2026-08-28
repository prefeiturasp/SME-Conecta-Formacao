using AutoMapper;
using MediatR;
using SME.ConectaFormacao.Aplicacao.Consultas.Eol.ObterNomesFuncionarioPorRf;
using SME.ConectaFormacao.Aplicacao.Dtos.Usuario;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;
using SME.ConectaFormacao.Infra.Servicos.Acessos.Interfaces;
using SME.ConectaFormacao.Infra.Servicos.Eol;
using System.Text.RegularExpressions;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ObterMeusDadosServicoAcessosPorLoginQueryHandler(
        IMapper mapper, IServicoAcessos servicoAcessos, IRepositorioUsuario repositorioUsuario,
        IMediator mediator, IRepositorioUsuarioAcessibilidade repositorioUsuarioAcessibilidade) :
        IRequestHandler<ObterMeusDadosServicoAcessosPorLoginQuery, DadosUsuarioDTO>
    {
        public async Task<DadosUsuarioDTO> Handle(ObterMeusDadosServicoAcessosPorLoginQuery request, CancellationToken cancellationToken)
        {
            var acessoDadosUsuario = await servicoAcessos.ObterMeusDados(request.Login);

            var usuario = await repositorioUsuario.ObterPorLogin(request.Login);
            if (usuario is not null && usuario.Tipo.EhExterno())
            {
                var unidade = !string.IsNullOrEmpty(usuario.CodigoEolUnidade) ? await mediator.Send(new ObterUnidadePorCodigoEOLQuery(usuario.CodigoEolUnidade), cancellationToken) : null;
                acessoDadosUsuario.Tipo = (int)TipoUsuario.Externo;
                acessoDadosUsuario.NomeUnidade = unidade?.NomeUnidade!;
            }
            var (tipoEmail, emailEducacional) = await repositorioUsuario.ObterEmailEducacionalPorLogin(request.Login);

            if (usuario is not null && !string.IsNullOrEmpty(usuario.Telefone))
            {
                acessoDadosUsuario.Telefone = usuario.Telefone;
            }

            acessoDadosUsuario.TipoEmail = tipoEmail;
            acessoDadosUsuario.EmailEducacional = emailEducacional;
            var nomesUsuario = await ObterNomeUsuarioPeloLogin(request.Login);
            acessoDadosUsuario.Nome = acessoDadosUsuario.Nome ?? nomesUsuario?.Nome ?? throw new ArgumentException("NomeSocial usuário não localizado");
            acessoDadosUsuario.NomeSocial = acessoDadosUsuario.NomeSocial ?? nomesUsuario?.NomeSocial;
            acessoDadosUsuario.Login = acessoDadosUsuario.Login ?? request.Login;

            var pattern = @"@edu\.sme\.prefeitura\.sp\.gov\.br$";
            if (!string.IsNullOrEmpty(acessoDadosUsuario.Email) && Regex.IsMatch(acessoDadosUsuario.Email, pattern, RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(500)) &&
                 string.IsNullOrEmpty(acessoDadosUsuario.EmailEducacional))

                acessoDadosUsuario.EmailEducacional = acessoDadosUsuario.Email;

            if (usuario is not null && string.IsNullOrWhiteSpace(acessoDadosUsuario.EmailEducacional))
                acessoDadosUsuario.EmailEducacional = await mediator.Send(new GerarEmailEducacionalCommand(usuario), cancellationToken);

            var dtoUsuario = mapper.Map<DadosUsuarioDTO>(acessoDadosUsuario);
            var acessibilidade = await repositorioUsuarioAcessibilidade.ObterAcessibilidadeAtualDoUsuarioAsync();
            dtoUsuario.UsuarioAcessibilidade = mapper.Map<UsuarioAcessibilidadeDto>(acessibilidade);
            return dtoUsuario;
        }

        private async Task<FuncionarioNomesDto?> ObterNomeUsuarioPeloLogin(string login)
        {
            return await mediator.Send(new ObterNomesFuncionarioPorRfQuery(login));
        }
    }
}
