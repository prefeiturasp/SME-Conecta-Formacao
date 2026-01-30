using MediatR;
using Microsoft.Extensions.DependencyInjection;
using SME.ConectaFormacao.Aplicacao.Comandos.PublicarNaFilaRabbit;
using SME.ConectaFormacao.Aplicacao.Interfaces.Codaf;
using SME.ConectaFormacao.Dominio.Comum;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Infra;
using SME.ConectaFormacao.Infra.Dados.Dtos.CodafCertificados;
using SME.ConectaFormacao.Infra.Dados.Estrategias.Interfaces;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.Codaf
{
    public class CasoDeUsoEmitirCertificadoCodaf(
        IRepositorioCodafCertificado repositorioCodafCertificado,
        IKeyedServiceProvider serviceProvider,
        IMediator mediator) :
        ICasoDeUsoEmitirCertificadoCodaf
    {
        public async Task<Resultado> ExecutarAsync(long codafListaPresencaId)
        {
            var listaDadosCertificado = await repositorioCodafCertificado.ObterDadosParaEmissaoCertificadosCodafAsync(codafListaPresencaId);

            if (!listaDadosCertificado.Any())
                return Erro.NaoEncontrado();

            var entidadesParaSalvar = new List<CodafCertificado>();

            foreach (var dados in listaDadosCertificado)
            {
                var tipoEstrategia = DefinirEstrategia(dados);
                var geradorCertificado = serviceProvider.GetRequiredKeyedService<ICertificadoCodafGeradorConteudo>(tipoEstrategia);

                var htmlCertificado = geradorCertificado.GerarHtml(dados);
                var metadados = new
                {
                    dados.NomeFormacao,
                    dados.HorasTotais,
                    dados.CargaHorariaTotalOutra,
                    dados.ConceitoFinal,
                    dados.PercentualFrequencia
                };

                var novoCertificado = new CodafCertificado(
                    dados.TipoParticipacao,
                    dados.IdReferencia,
                    htmlCertificado,
                    metadados
                    );
                entidadesParaSalvar.Add(novoCertificado);
            }

            if (entidadesParaSalvar.Count != 0)
                await repositorioCodafCertificado.InserirLoteAsync(entidadesParaSalvar);

            await mediator.Send(new PublicarNaFilaRabbitCommand(RotasRabbit.GerarArquivoCertificadosCodaf, codafListaPresencaId));
            return Resultado.DeSucesso();
        }

        private static TipoEstrategiaCertificadoCodaf DefinirEstrategia(DadosEmissaoCertificadoCodafDto dto)
        {
            if (dto.TipoParticipacao == TipoParticipacaoCodaf.Regente)
                return TipoEstrategiaCertificadoCodaf.Regente;

            return dto.TemRf
                ? TipoEstrategiaCertificadoCodaf.CursistaComRf
                : TipoEstrategiaCertificadoCodaf.CursistaSemRf;
        }
    }
}
