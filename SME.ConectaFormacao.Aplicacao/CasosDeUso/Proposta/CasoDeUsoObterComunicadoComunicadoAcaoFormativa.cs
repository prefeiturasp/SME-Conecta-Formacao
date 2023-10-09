using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos.Proposta;
using SME.ConectaFormacao.Aplicacao.Interfaces.Proposta;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Dominio.Extensoes;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.Proposta
{
    public class CasoDeUsoObterComunicadoComunicadoAcaoFormativa : CasoDeUsoAbstrato, ICasoDeUsoObterComunicadoAcaoFormativa
    {
        public CasoDeUsoObterComunicadoComunicadoAcaoFormativa(IMediator mediator) : base(mediator)
        {
        }

        public async Task<ComunicadoAcaoFormativaDTO> Executar(long propostaId)
        {
            var proposta = await mediator.Send(new ObterPropostaCompletaPorIdQuery(propostaId));
            if (proposta.AcaoInformativa)
            {
                return new ComunicadoAcaoFormativaDTO()
                {
                    Descricao = proposta.AcaoFormativaTexto,
                    Url = proposta.AcaoFormativaLink
                };
            }
            else
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
}
