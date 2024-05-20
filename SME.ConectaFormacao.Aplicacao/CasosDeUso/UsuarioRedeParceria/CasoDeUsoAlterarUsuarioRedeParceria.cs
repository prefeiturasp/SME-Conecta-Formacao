using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos.Proposta;
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
    public class CasoDeUsoAlterarUsuarioRedeParceria : CasoDeUsoAbstrato, ICasoDeUsoAlterarUsuarioRedeParceria
    {
        public CasoDeUsoAlterarUsuarioRedeParceria(IMediator mediator) : base(mediator)
        {
        }

        public async Task<RetornoDTO> Executar(long id, UsuarioRedeParceriaDTO usuarioRedeParceriaDTO)
        {
            ValidarPreenchimento(usuarioRedeParceriaDTO);

            usuarioRedeParceriaDTO.Cpf = usuarioRedeParceriaDTO.Cpf.SomenteNumeros();
            usuarioRedeParceriaDTO.Telefone = usuarioRedeParceriaDTO.Telefone.SomenteNumeros();

            var usuario = await mediator.Send(new ObterUsuarioPorIdQuery(id)) ??
                throw new NegocioException(MensagemNegocio.USUARIO_NAO_ENCONTRADO);

            if (!usuario.Tipo.EhRedeParceria())
                throw new NegocioException(MensagemNegocio.USUARIO_NAO_ENCONTRADO);

            var areaPromotora = await mediator.Send(new ObterAreaPromotoraPorIdQuery(usuarioRedeParceriaDTO.AreaPromotoraId)) ??
                throw new NegocioException(MensagemNegocio.AREA_PROMOTORA_NAO_ENCONTRADA);

            var areaPromotoraIdAntes = usuario.AreaPromotoraId;

            usuario.Nome = usuarioRedeParceriaDTO.Nome;
            usuario.Email = usuarioRedeParceriaDTO.Email;
            usuario.Telefone = usuarioRedeParceriaDTO.Telefone;
            usuario.AreaPromotoraId = usuarioRedeParceriaDTO.AreaPromotoraId;
            usuario.Situacao = usuarioRedeParceriaDTO.Situacao;

            var criadoCoresso = await AtualizarUsuarioNoCoreSSO(usuario, areaPromotora, areaPromotoraIdAntes.GetValueOrDefault());
            if (!criadoCoresso)
                throw new NegocioException(MensagemNegocio.ERRO_AO_CRIAR_ATUALIZAR_USUARIO_NO_CORESSO);

            await mediator.Send(new SalvarUsuarioCommand(usuario));

            return RetornoDTO.RetornarSucesso(MensagemNegocio.USUARIO_ALTERADO_COM_SUCESSO, usuario.Id);
        }

        private async Task<bool> AtualizarUsuarioNoCoreSSO(Dominio.Entidades.Usuario usuario, Dominio.Entidades.AreaPromotora areaPromotora, long areaPromotoraIdAntes)
        {
            bool usuarioAtualizadoCoresso = await mediator.Send(new AtualizarUsuarioServicoAcessoCommand(usuario.Login, usuario.Nome, usuario.Email, string.Empty));

            bool vinculadoAoGrupoAreaPromotora = true, desvinculadoAoGrupoAreaPromotoraAntiga = true;
            if (areaPromotoraIdAntes != areaPromotora.Id)
            {
                var areaPromotoraAntes = await mediator.Send(new ObterAreaPromotoraPorIdQuery(areaPromotoraIdAntes));

                desvinculadoAoGrupoAreaPromotoraAntiga = await mediator.Send(new DesvincularPerfilExternoCoreSSOServicoAcessosCommand(usuario.Login, areaPromotoraAntes.GrupoId));
                vinculadoAoGrupoAreaPromotora = await mediator.Send(new VincularPerfilExternoCoreSSOServicoAcessosCommand(usuario.Login, areaPromotora.GrupoId));
            }

            return usuarioAtualizadoCoresso && vinculadoAoGrupoAreaPromotora && desvinculadoAoGrupoAreaPromotoraAntiga;
        }

        private static void ValidarPreenchimento(UsuarioRedeParceriaDTO usuarioRedeParceriaDTO)
        {
            if (!UtilValidacoes.NomeComSobrenome(usuarioRedeParceriaDTO.Nome))
                throw new NegocioException(MensagemNegocio.NOME_DEVE_TER_SOBRENOME);
            if (!UtilValidacoes.EmailEhValido(usuarioRedeParceriaDTO.Email))
                throw new NegocioException(MensagemNegocio.EMAIL_INVALIDO);
        }
    }
}
