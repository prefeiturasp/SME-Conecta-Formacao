using MediatR;
using SME.ConectaFormacao.Aplicacao.Comandos.Propostas.SalvarNumeroHomologacaoProposta;
using SME.ConectaFormacao.Aplicacao.Consultas.Propostas.ObterSePropostaPossuiCodaf;
using SME.ConectaFormacao.Aplicacao.Dtos.Proposta;
using SME.ConectaFormacao.Aplicacao.Interfaces.Proposta;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Excecoes;
using SME.ConectaFormacao.Dominio.Extensoes;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.Proposta
{
    public class CasoDeUsoSalvarNumeroHomologacaoProposta(IMediator mediator)
        : CasoDeUsoAbstrato(mediator), ICasoDeUsoSalvarNumeroHomologacaoProposta
    {
        public async Task<bool> Executar(long propostaId, PropostaNumeroHomologacaoDto dto)
        {
            var proposta = await mediator.Send(new ObterPropostaPorIdQuery(propostaId));
            if (proposta == null || proposta.Excluido)
                throw new NegocioException(MensagemNegocio.PROPOSTA_NAO_ENCONTRADA);

            var perfilUsuarioLogado = await mediator.Send(ObterGrupoUsuarioLogadoQuery.Instancia());
            if (!perfilUsuarioLogado.EhPerfilAdminDF())
                throw new NegocioException(MensagemNegocio.USUARIO_SEM_PERMISSAO_PARA_ALTERAR_NUMERO_HOMOLOGACAO);

            var possuiCodaf = await mediator.Send(new ObterSePropostaPossuiCodafQuery(propostaId));
            if (possuiCodaf)
                throw new NegocioException(MensagemNegocio.PROPOSTA_POSSUI_CODAF_NAO_PERMITE_ALTERAR_NUMERO_HOMOLOGACAO);

            return await mediator.Send(new SalvarNumeroHomologacaoPropostaCommand(propostaId, dto?.NumeroHomologacao));
        }
    }
}
