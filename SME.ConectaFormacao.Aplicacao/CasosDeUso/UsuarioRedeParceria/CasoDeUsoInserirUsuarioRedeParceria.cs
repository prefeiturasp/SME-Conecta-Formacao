using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos.Usuario;
using SME.ConectaFormacao.Aplicacao.Dtos.UsuarioRedeParceria;
using SME.ConectaFormacao.Aplicacao.Interfaces.UsuarioRedeParceria;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Dominio.Excecoes;
using SME.ConectaFormacao.Dominio.Extensoes;
using SME.ConectaFormacao.Infra.Servicos.Utilitarios;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.UsuarioRedeParceria
{
    public class CasoDeUsoInserirUsuarioRedeParceria : CasoDeUsoAbstrato, ICasoDeUsoInserirUsuarioRedeParceria
    {
        private const string PREFIXO_SENHA_PADRAO = "Sgp";
        public CasoDeUsoInserirUsuarioRedeParceria(IMediator mediator) : base(mediator)
        {
        }

        public async Task<bool> Executar(UsuarioRedeParceriaDTO usuarioRedeParceriaDTO)
        {
            ValidarPreenchimento(usuarioRedeParceriaDTO);

            usuarioRedeParceriaDTO.Cpf = usuarioRedeParceriaDTO.Cpf.SomenteNumeros();
            usuarioRedeParceriaDTO.Telefone = usuarioRedeParceriaDTO.Telefone.SomenteNumeros();

            var usuario = await mediator.Send(new ObterUsuarioPorLoginQuery(usuarioRedeParceriaDTO.Cpf));
            if (usuario.NaoEhNulo() && usuario.Tipo.EhRedeParceria())
                throw new NegocioException(MensagemNegocio.USUARIO_JA_POSSUI_CADASTRO_COMO_REDE_PARCERIA);

            var areaPromotora = await mediator.Send(new ObterAreaPromotoraPorIdQuery(usuarioRedeParceriaDTO.AreaPromotoraId)) ??
                throw new NegocioException(MensagemNegocio.AREA_PROMOTORA_NAO_ENCONTRADA);

            bool existeNoConecta = usuario.NaoEhNulo();
            if (usuario.EhNulo())
                usuario = new Dominio.Entidades.Usuario();

            usuario.Tipo = TipoUsuario.RedeParceria;
            usuario.Nome = usuarioRedeParceriaDTO.Nome;
            usuario.Cpf = usuarioRedeParceriaDTO.Cpf;
            usuario.AreaPromotoraId = usuarioRedeParceriaDTO.AreaPromotoraId;
            usuario.Telefone = usuarioRedeParceriaDTO.Telefone;
            usuario.Email = usuarioRedeParceriaDTO.Email;
            usuario.Situacao = usuarioRedeParceriaDTO.Situacao;

            var criadoCoresso = await CadastrarUsuarioNoCoreSSO(usuario, areaPromotora, existeNoConecta);
            if (!criadoCoresso)
                throw new NegocioException(MensagemNegocio.ERRO_AO_CRIAR_ATUALIZAR_USUARIO_NO_CORESSO);

            await mediator.Send(new SalvarUsuarioCommand(usuario));

            return true;
        }

        private async Task<bool> CadastrarUsuarioNoCoreSSO(Dominio.Entidades.Usuario usuario, Dominio.Entidades.AreaPromotora areaPromotora, bool existeNoConecta)
        {
            var existeNoCoreSSO = await mediator.Send(new UsuarioExisteNoCoreSsoQuery(usuario.Login));

            var senha = existeNoConecta && existeNoCoreSSO ? string.Empty : string.Concat(PREFIXO_SENHA_PADRAO, usuario.Cpf[^4..]);

            bool usuarioCriadoCoresso;
            if (existeNoCoreSSO)
                usuarioCriadoCoresso = await mediator.Send(new AtualizarUsuarioServicoAcessoCommand(usuario.Login, usuario.Nome, usuario.Email, senha));
            else
                usuarioCriadoCoresso = await mediator.Send(new CadastrarUsuarioServicoAcessoCommand(usuario.Login, usuario.Nome, usuario.Email, senha));

            var vinculadoAoGrupoAreaPromotora = await mediator.Send(new VincularPerfilExternoCoreSSOServicoAcessosCommand(usuario.Login, areaPromotora.GrupoId));

            return usuarioCriadoCoresso && vinculadoAoGrupoAreaPromotora;
        }

        private static void ValidarPreenchimento(UsuarioRedeParceriaDTO usuarioRedeParceriaDTO)
        {
            var erros = new List<string>();
            if (!UtilValidacoes.CpfEhValido(usuarioRedeParceriaDTO.Cpf))
                erros.Add(MensagemNegocio.CPF_COM_DIGITO_VERIFICADOR_INVALIDO.Parametros(usuarioRedeParceriaDTO.Cpf));
            if (!UtilValidacoes.NomeComSobrenome(usuarioRedeParceriaDTO.Nome))
                erros.Add(MensagemNegocio.NOME_DEVE_TER_SOBRENOME);
            if (!UtilValidacoes.EmailEhValido(usuarioRedeParceriaDTO.Email))
                erros.Add(MensagemNegocio.EMAIL_INVALIDO);

            if (erros.PossuiElementos())
                throw new NegocioException(erros);
        }
    }
}
