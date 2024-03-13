using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos.Usuario;
using SME.ConectaFormacao.Aplicacao.Interfaces.Usuario;
using SME.ConectaFormacao.Aplicacao.ObterDominioDeEmailPermitido;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Dominio.Excecoes;
using SME.ConectaFormacao.Dominio.Extensoes;
using SME.ConectaFormacao.Infra.Servicos.Utilitarios;
using System.Text.RegularExpressions;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.Usuario
{
    public class CasoDeUsoInserirUsuarioExterno : CasoDeUsoAbstrato, ICasoDeUsoInserirUsuarioExterno
    {
        public CasoDeUsoInserirUsuarioExterno(IMediator mediator) : base(mediator)
        {
        }

        public async Task<InserirUsuarioRetornoDTO> InserirUsuarioExterno(UsuarioExternoDTO usuarioExternoDto)
        {
            var cpfSemPontos = usuarioExternoDto.Cpf.SomenteNumeros();
            usuarioExternoDto.Login = cpfSemPontos;
            usuarioExternoDto.Cpf = cpfSemPontos;

            ValidacoesPreenchimento(usuarioExternoDto.Senha, usuarioExternoDto.ConfirmarSenha, usuarioExternoDto.Cpf, usuarioExternoDto.Email);
            await UsuarioNaoExisteNoConecta(usuarioExternoDto.Login);

            var existeNoCoreSSO = await mediator.Send(new UsuarioExisteNoCoreSsoQuery(usuarioExternoDto.Login));

            bool usuarioCriadoCoresso;
            if (existeNoCoreSSO)
                usuarioCriadoCoresso = await mediator.Send(new AtualizarUsuarioServicoAcessoCommand(usuarioExternoDto.Login, usuarioExternoDto.Nome, usuarioExternoDto.Email, usuarioExternoDto.Senha));
            else
                usuarioCriadoCoresso = await mediator.Send(new CadastrarUsuarioServicoAcessoCommand(usuarioExternoDto.Login, usuarioExternoDto.Nome, usuarioExternoDto.Email, usuarioExternoDto.Senha));

            if (!usuarioCriadoCoresso)
                throw new NegocioException(MensagemNegocio.NAO_FOI_POSSIVEL_CADASTRAR_USUARIO_EXTERNO_NO_CORESSO);

            bool confirmarEmail = await ObterParametroConfirmarEmailUsuarioExterno();

            var tipo = usuarioExternoDto.Tipo ?? TipoUsuario.Externo;
            var situacaoCadastroUsuario = confirmarEmail ? SituacaoCadastroUsuario.AguardandoValidacaoEmail : SituacaoCadastroUsuario.Ativo;

            await mediator.Send(new SalvarUsuarioCommand(new Dominio.Entidades.Usuario(
                usuarioExternoDto.Login,
                usuarioExternoDto.Nome,
                usuarioExternoDto.Email,
                usuarioExternoDto.Cpf,
                tipo,
                situacaoCadastroUsuario,
                usuarioExternoDto.CodigoUnidade
            )));

            if (confirmarEmail)
                await mediator.Send(new EnviarEmailValidacaoUsuarioExternoServicoAcessoCommand(usuarioExternoDto.Login));

            var mensagem = confirmarEmail ? MensagemNegocio.VALIDAR_EMAIL_USUARIO_EXTERNO : MensagemNegocio.USUARIO_EXTRNO_CADASTRADO_COM_SUCESSO;

            return new InserirUsuarioRetornoDTO
            {
                ValidarEmail = confirmarEmail,
                Mensagem = mensagem
            };
        }

        private async Task<bool> ObterParametroConfirmarEmailUsuarioExterno()
        {
            var confirmarEmailUsuarioExterno = await mediator.Send(new ObterParametroSistemaPorTipoEAnoQuery(TipoParametroSistema.ConfirmarEmailUsuarioExterno, DateTimeExtension.HorarioBrasilia().Year));
            if (bool.TryParse(confirmarEmailUsuarioExterno?.Valor, out bool confirmarEmail))
                return confirmarEmail;

            return true;
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

        private static void ValidacoesPreenchimento(string senhaNova, string confirmarSenha, string cpf, string email)
        {
            var erros = new List<string>();

            var cpfValido = UtilValidacoes.CpfEhValido(cpf);
            if (!cpfValido)
                erros.Add(MensagemNegocio.CPF_COM_DIGITO_VERIFICADOR_INVALIDO.Parametros(cpf));

            var emailValido = UtilValidacoes.EmailEhValido(email);
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

            var regexSenha = new Regex(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d|\W)[^áàâãéèêíïóôõöúçñÁÀÂÃÉÈÊÍÏÓÔÕÖÚÇÑ]{8,12}$");

            if (!regexSenha.IsMatch(senhaNova))
                erros.Add(MensagemNegocio.A_SENHA_DEVE_CONTER_SOMENTE);

            if (erros.Any())
                throw new NegocioException(erros);
        }
    }
}