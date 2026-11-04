using MediatR;
using SME.ConectaFormacao.Aplicacao.Comandos.Propostas.SalvarPropostaGrupoPeriodo;
using SME.ConectaFormacao.Aplicacao.Comandos.Propostas.SalvarPropostaGrupoPeriodoInscricao;
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

        public async Task<RetornoDTO> Executar(PropostaDTO propostaDTO)
        {
            var grupoUsuarioLogadoId = await mediator.Send(ObterGrupoUsuarioLogadoQuery.Instancia());
            var dres = await mediator.Send(ObterDresUsuarioLogadoQuery.Instancia());

            var areaPromotora = await mediator.Send(new ObterAreaPromotoraPorGrupoIdEDresQuery(grupoUsuarioLogadoId, dres)) ??
                throw new NegocioException(MensagemNegocio.AREA_PROMOTORA_NAO_ENCONTRADA_GRUPO_USUARIO);

            var comunicado = await ObterComunicaddoParametroSistema();
            propostaDTO.AcaoFormativaTexto = comunicado.Descricao;
            propostaDTO.AcaoFormativaLink = comunicado.Url;

            RetornoDTO retornoDto;
            if (propostaDTO.Situacao == SituacaoProposta.Rascunho)
            {
                retornoDto = await mediator.Send(new InserirPropostaRascunhoCommand(areaPromotora.Id, propostaDTO));
            }
            else
            {
                retornoDto = await mediator.Send(new InserirPropostaCommand(areaPromotora.Id, propostaDTO));
            }

            await SalvarMovimentacao(retornoDto.EntidadeId, propostaDTO.Situacao);

            if (propostaDTO.GruposPeriodos != null && propostaDTO.GruposPeriodos.Any())
            {
                var resultadoGrupo = await mediator.Send(new SalvarPropostaGrupoPeriodoCommand(retornoDto.EntidadeId, propostaDTO));
                if (!resultadoGrupo.Sucesso && !propostaDTO.EhProximoPasso)
                    throw new NegocioException(resultadoGrupo.MensagensErro);
            }

            if (propostaDTO.GruposPeriodosInscricao != null && propostaDTO.GruposPeriodosInscricao.Any())
            {
                var resultadoGrupoInscricao = await mediator.Send(new SalvarPropostaGrupoPeriodoInscricaoCommand(retornoDto.EntidadeId, propostaDTO));
                if (!resultadoGrupoInscricao.Sucesso && !propostaDTO.EhProximoPasso)
                    throw new NegocioException(resultadoGrupoInscricao.MensagensErro);
            }

            return retornoDto;
        }

        private async Task SalvarMovimentacao(long propostaId, SituacaoProposta situacao)
        {
            await mediator.Send(new SalvarPropostaMovimentacaoCommand(propostaId, situacao));
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
