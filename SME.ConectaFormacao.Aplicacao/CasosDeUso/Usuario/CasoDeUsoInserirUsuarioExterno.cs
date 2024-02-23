using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos.Usuario;
using SME.ConectaFormacao.Aplicacao.Interfaces.Usuario;
using SME.ConectaFormacao.Aplicacao.ObterDominioDeEmailPermitido;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Dominio.Excecoes;
using SME.ConectaFormacao.Dominio.Extensoes;
using System.Text.RegularExpressions;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.Usuario
{
    public class CasoDeUsoInserirUsuarioExterno : CasoDeUsoAbstrato, ICasoDeUsoInserirUsuarioExterno
    {
        public CasoDeUsoInserirUsuarioExterno(IMediator mediator) : base(mediator)
        {
        }

        public async Task<bool> InserirUsuarioExterno(UsuarioExternoDTO usuarioExternoDto)
        {
            var cpfSemPontos = usuarioExternoDto.Cpf.Replace(".", "").Replace("-", "");
            usuarioExternoDto.Login = cpfSemPontos;
            usuarioExternoDto.Cpf = cpfSemPontos;
            await Validacoes(usuarioExternoDto.Senha, usuarioExternoDto.ConfirmarSenha, usuarioExternoDto.Cpf, usuarioExternoDto.Email);
            await ValidarCpfEmUsuarioExisteNoCoreSSO(usuarioExternoDto.Login);
            await UsuarioNaoExisteNoConecta(usuarioExternoDto.Login);

            var retornoCoreSSO = await mediator.Send(new CadastrarUsuarioServicoAcessoCommand(usuarioExternoDto.Login, usuarioExternoDto.Nome, usuarioExternoDto.Email, usuarioExternoDto.Senha));
            if (!retornoCoreSSO)
                throw new NegocioException(MensagemNegocio.NAO_FOI_POSSIVEL_CADASTRAR_USUARIO_EXTERNO_NO_CORESSO);


            var tipo = usuarioExternoDto.Tipo ?? TipoUsuario.Externo;
            await mediator.Send(new SalvarUsuarioCommand(new Dominio.Entidades.Usuario(
                usuarioExternoDto.Login,
                usuarioExternoDto.Nome,
                usuarioExternoDto.Email,
                usuarioExternoDto.Cpf,
                tipo,
                SituacaoCadastroUsuario.AguardandoValidacaoEmail,
                usuarioExternoDto.CodigoUnidade
            )));

            return await mediator.Send(new EnviarEmailValidacaoUsuarioExternoServicoAcessoCommand(usuarioExternoDto.Login));
        }

        private async Task ValidarCpfEmUsuarioExisteNoCoreSSO(string cpf)
        {
            var coresso = await mediator.Send(new UsuarioExisteNoCoreSsoQuery(cpf));
            if (coresso)
                throw new NegocioException(MensagemNegocio.VOCE_JA_POSSUI_LOGIN_CORESSO);
        }

        private async Task UsuarioNaoExisteNoConecta(string login)
        {
            var usuario = await mediator.Send(new ObterUsuarioPorLoginQuery(login));
            if (usuario.NaoEhNulo())
                throw new NegocioException(MensagemNegocio.VOCE_JA_POSSUI_LOGIN_CONECTA);
        }

        private async Task<IEnumerable<string>> ObterDominiosPermitidos()
        {
            return await mediator.Send(new ObterDominioDeEmailPermitidoQuery());
        }

        private async Task Validacoes(string senhaNova, string confirmarSenha, string cpf, string email)
        {
            var erros = new List<string>();

            var emailValido = EmailEhValido(email);
            if (!emailValido)
                erros.Add(MensagemNegocio.EMAIL_INVALIDO.Parametros(email));

            if (senhaNova.Contains(" "))
                erros.Add(MensagemNegocio.A_SENHA_NAO_PODE_CONTER_ESPACOS_EM_BRANCO);

            if (!senhaNova.Equals(confirmarSenha))
                erros.Add(MensagemNegocio.CONFIRMACAO_SENHA_DEVE_SER_IGUAL_A_SENHA);

            if (senhaNova.Length < 8)
                erros.Add(MensagemNegocio.A_SENHA_DEVE_TER_NO_MÍNIMO_8_CARACTERES);

            if (senhaNova.Length > 12)
                erros.Add(MensagemNegocio.A_SENHA_DEVE_TER_NO_MÁXIMO_12_CARACTERES);

            if (!cpf.CpfEhValido())
                erros.Add(MensagemNegocio.CPF_INVALIDO);

            var regexSenha = new Regex(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d|\W)[^áàâãéèêíïóôõöúçñÁÀÂÃÉÈÊÍÏÓÔÕÖÚÇÑ]{8,12}$");

            if (!regexSenha.IsMatch(senhaNova))
                erros.Add(MensagemNegocio.A_SENHA_DEVE_CONTER_SOMENTE);

            if (erros.Any())
                throw new NegocioException(erros);
        }
        public static bool EmailEhValido(string email)
        {
            string pattern = @"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$";
            Regex regex = new Regex(pattern);
            return regex.IsMatch(email);
        }
    }
}