using System.Text.RegularExpressions;
using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos.Usuario;
using SME.ConectaFormacao.Aplicacao.Interfaces.Usuario;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Dominio.Excecoes;
using SME.ConectaFormacao.Dominio.Extensoes;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.Usuario
{
    public class CasoDeUsoInserirUsuarioExterno : CasoDeUsoAbstrato, ICasoDeUsoInserirUsuarioExterno
    {

        public CasoDeUsoInserirUsuarioExterno(IMediator mediator) : base(mediator) { }

        public async Task<bool> InserirUsuarioExterno(UsuarioExternoDTO usuarioExternoDto)
        {
            usuarioExternoDto.Login = usuarioExternoDto.Login.Replace(".", "").Replace("-", "");
            ValidarSenha(usuarioExternoDto.Senha, usuarioExternoDto.ConfirmarSenha);
            await ValidarCpfEmUsuarioAcervoECoreSSO(usuarioExternoDto.Login);

            var retornoCoreSSO = await mediator.Send(new CadastrarUsuarioServicoAcessoCommand(usuarioExternoDto.Login, usuarioExternoDto.Nome, usuarioExternoDto.Email, usuarioExternoDto.Senha));
            if (!retornoCoreSSO)
                throw new NegocioException(MensagemNegocio.NAO_FOI_POSSIVEL_CADASTRAR_USUARIO_EXTERNO_NO_CORESSO);

            await mediator.Send(new SalvarUsuarioCommand(new Dominio.Entidades.Usuario(
                usuarioExternoDto.Login,
                usuarioExternoDto.Nome,
                usuarioExternoDto.Email,
                usuarioExternoDto.Cpf,
                usuarioExternoDto.Tipo,
                SituacaoCadastroUsuario.AguardandoValidacaoEmail,
                usuarioExternoDto.CodigoUe
            )));
            return true;
        }

        private async Task ValidarCpfEmUsuarioAcervoECoreSSO(string cpf)
        {
            var usuario = await ObterPorLogin(cpf);
            if (usuario.NaoEhNulo())
                throw new NegocioException(MensagemNegocio.VOCE_JA_POSSUI_LOGIN_CONECTA);

            var usuarioCoreSSO = await mediator.Send(new UsuarioExisteNoCoreSsoQuery(cpf));
            if (usuarioCoreSSO)
                throw new NegocioException(MensagemNegocio.VOCE_JA_POSSUI_LOGIN_CORESSO);
        }

        private async Task<Dominio.Entidades.Usuario> ObterPorLogin(string login)
        {
            return await mediator.Send(new ObterUsuarioPorLoginQuery(login));
        }

        private void ValidarSenha(string senhaNova, string confirmarSenha)
        {
            var erros = new List<string>();

            if (senhaNova.Contains(" "))
                erros.Add(MensagemNegocio.A_SENHA_NAO_PODE_CONTER_ESPACOS_EM_BRANCO);

            if (!senhaNova.Equals(confirmarSenha))
                erros.Add(MensagemNegocio.CONFIRMACAO_SENHA_DEVE_SER_IGUAL_A_SENHA);

            if (senhaNova.Length < 8)
                erros.Add(MensagemNegocio.A_SENHA_DEVE_TER_NO_MÍNIMO_8_CARACTERES);

            if (senhaNova.Length > 12)
                erros.Add(MensagemNegocio.A_SENHA_DEVE_TER_NO_MÁXIMO_12_CARACTERES);

            var regexSenha = new Regex(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d|\W)[^áàâãéèêíïóôõöúçñÁÀÂÃÉÈÊÍÏÓÔÕÖÚÇÑ]{8,12}$");

            if (!regexSenha.IsMatch(senhaNova))
                erros.Add(MensagemNegocio.A_SENHA_DEVE_CONTER_SOMENTE);

            if (erros.Any())
                throw new NegocioException(erros);
        }
    }
}