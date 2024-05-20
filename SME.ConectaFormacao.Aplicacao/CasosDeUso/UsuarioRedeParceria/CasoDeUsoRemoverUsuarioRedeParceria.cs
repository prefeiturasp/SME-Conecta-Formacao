using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos.Proposta;
using SME.ConectaFormacao.Aplicacao.Interfaces.UsuarioRedeParceria;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Dominio.Excecoes;
using SME.ConectaFormacao.Dominio.Extensoes;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.UsuarioRedeParceria
{
    public class CasoDeUsoRemoverUsuarioRedeParceria : CasoDeUsoAbstrato, ICasoDeUsoRemoverUsuarioRedeParceria
    {
        public CasoDeUsoRemoverUsuarioRedeParceria(IMediator mediator) : base(mediator)
        {
        }

        public async Task<RetornoDTO> Executar(long id)
        {
            var usuario = await mediator.Send(new ObterUsuarioPorIdQuery(id)) ??
                throw new NegocioException(MensagemNegocio.USUARIO_NAO_ENCONTRADO);

            if (!usuario.Tipo.EhRedeParceria())
                throw new NegocioException(MensagemNegocio.USUARIO_NAO_ENCONTRADO);

            var inativadoCoreSSO = await mediator.Send(new InativarUsuarioCoreSSOServicoAcessosCommand(usuario.Login));
            if (!inativadoCoreSSO)
                throw new NegocioException(MensagemNegocio.ERRO_AO_CRIAR_ATUALIZAR_USUARIO_NO_CORESSO);

            var mensagem = MensagemNegocio.USUARIO_EXCLUIDO_COM_SUCESSO;
            var usuarioPossuiProposta = await mediator.Send(new UsuarioPossuiPropostaQuery(usuario.Login));
            if (usuarioPossuiProposta)
            {
                usuario.Situacao = SituacaoUsuario.Inativo;
                await mediator.Send(new SalvarUsuarioCommand(usuario));
                mensagem = MensagemNegocio.USUARIO_FOI_INATIVO_POR_POSSUIR_PROPOSTA_CADASTRADA;
            }
            else
            {
                await mediator.Send(new RemoverUsuarioCommand(id));
            }

            var nomeChave = CacheDistribuidoNomes.Usuario.Parametros(usuario.Login);
            await mediator.Send(new RemoverCacheCommand(nomeChave));

            return RetornoDTO.RetornarSucesso(mensagem, usuario.Id);
        }
    }
}
