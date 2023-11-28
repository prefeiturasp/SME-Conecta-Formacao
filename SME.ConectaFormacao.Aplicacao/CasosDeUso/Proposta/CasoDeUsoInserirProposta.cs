using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos.Proposta;
using SME.ConectaFormacao.Aplicacao.Interfaces.Proposta;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Dominio.Excecoes;
using SME.ConectaFormacao.Dominio.Extensoes;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.Proposta
{
    public class CasoDeUsoInserirProposta : CasoDeUsoAbstrato, ICasoDeUsoInserirProposta
    {
        public CasoDeUsoInserirProposta(IMediator mediator) : base(mediator)
        {
        }

        public async Task<long> Executar(PropostaDTO propostaDTO)
        {
            var comunicado = await ObterComunicaddoParametroSistema();
            propostaDTO.AcaoFormativaTexto = comunicado.Descricao;
            propostaDTO.AcaoFormativaLink = comunicado.Url;

            var grupoUsuarioLogadoId = await mediator.Send(ObterGrupoUsuarioLogadoQuery.Instancia());
            var areaPromotora = await mediator.Send(new ObterAreaPromotoraPorGrupoIdQuery(grupoUsuarioLogadoId)) ??
                throw new NegocioException(MensagemNegocio.AREA_PROMOTORA_NAO_ENCONTRADA_GRUPO_USUARIO, System.Net.HttpStatusCode.NotFound);

            if (propostaDTO.Situacao != Dominio.Enumerados.SituacaoProposta.Rascunho)
                return await mediator.Send(new InserirPropostaCommand(areaPromotora.Id, propostaDTO));

            return await mediator.Send(new InserirPropostaRascunhoCommand(areaPromotora.Id, propostaDTO));
        }
        private async Task<ComunicadoAcaoFormativaDTO> ObterComunicaddoParametroSistema()
        {
            var comunicadoAcaoFormativaTexto = await mediator.Send(new ObterParametroSistemaPorTipoEAnoQuery(TipoParametroSistema.ComunicadoAcaoFormativaDescricao, DateTimeExtension.HorarioBrasilia().Year));
            var comunicadoAcaoFormativaUrl = await mediator.Send(new ObterParametroSistemaPorTipoEAnoQuery(TipoParametroSistema.ComunicadoAcaoFormativaUrl, DateTimeExtension.HorarioBrasilia().Year));

            return new ComunicadoAcaoFormativaDTO()
            {
                Descricao = comunicadoAcaoFormativaTexto.Valor,
                Url = comunicadoAcaoFormativaUrl.Valor
            };
        }
    }
}
