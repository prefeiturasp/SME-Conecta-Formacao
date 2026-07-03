using ConectaFormacao.Dominio.Servicos;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using SME.ConectaFormacao.Aplicacao.Comandos.PublicarNaFilaRabbit;
using SME.ConectaFormacao.Aplicacao.Interfaces.CodafCertificados;
using SME.ConectaFormacao.Dominio.Comum;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Infra;
using SME.ConectaFormacao.Infra.Dados;
using SME.ConectaFormacao.Infra.Dados.Dtos.CodafCertificados;
using SME.ConectaFormacao.Infra.Dados.Estrategias.Interfaces;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.CodafCertificados
{
    public class CasoDeUsoEmitirCertificadoCodaf(
        IRepositorioCodafCertificado repositorioCodafCertificado,
        IKeyedServiceProvider serviceProvider,
        IMediator mediator,
        IPeriodoRealizacaoConsultaService periodoRealizacaoConsultaService,
        IRepositorioCodafSuplementarInscricao repositorioCodafSuplementarInscricao,
        ITransacao transacao) :
        ICasoDeUsoEmitirCertificadoCodaf
    {
        public async Task<Resultado> ExecutarAsync(long codafId, TipoCodaf tipoCodaf)
        {
            var listaDadosCertificado = tipoCodaf switch
            {
                TipoCodaf.ListaPresenca => await repositorioCodafCertificado.ObterDadosParaEmissaoCertificadosCodafAsync(codafId),
                TipoCodaf.Suplementar => await repositorioCodafCertificado.ObterDadosParaEmissaoCertificadosCodafSuplementarAsync(codafId),
                _ => throw new ArgumentOutOfRangeException(nameof(tipoCodaf), tipoCodaf, null)
            };

            if (!listaDadosCertificado.Any())
                return Erro.NaoEncontrado();

            var entidadesParaSalvar = new List<CodafCertificado>();

            foreach (var dados in listaDadosCertificado)
            {
                var tipoEstrategia = DefinirEstrategia(dados);
                var geradorCertificado = serviceProvider.GetRequiredKeyedService<ICertificadoCodafGeradorConteudo>(tipoEstrategia);

                var periodo = await periodoRealizacaoConsultaService.ObterPeriodoRealizacaoAsync(dados.PropostaTurmaId);

                if (periodo != null)
                {
                    dados.DataInicio = periodo.DataInicio;
                    dados.DataFim = periodo.DataFim;
                }

                var htmlCertificado = geradorCertificado.GerarHtml(dados);
                var metadados = new
                {
                    dados.NomeFormacao,
                    dados.HorasTotais,
                    dados.CargaHorariaTotalOutra,
                    dados.ConceitoFinal,
                    dados.PercentualFrequencia,
                    dados.Emissor,
                    dados.TipoFormacao,
                    DataInicio = periodo?.DataInicio.Date,
                    DataFim = periodo?.DataFim.Date
                };

                var novoCertificado = new CodafCertificado(
                    codafId,
                    tipoCodaf,
                    dados.TipoParticipacao,
                    dados.IdReferencia,
                    htmlCertificado,
                    metadados
                );
                entidadesParaSalvar.Add(novoCertificado);
            }

            if (entidadesParaSalvar.Count != 0)
            {
                using var transacaoDb = transacao.Iniciar();
                try
                {
                    await SanitizarCertificadosEmitidosAsync(tipoCodaf, codafId, listaDadosCertificado);
                    await repositorioCodafCertificado.InserirLoteAsync(entidadesParaSalvar);
                    await repositorioCodafCertificado.AtualizaCodigoCertificado(codafId, tipoCodaf);
                    transacaoDb.Commit();
                }
                catch
                {
                    transacaoDb.Rollback();
                    throw;
                }
            }

            await mediator.Send(new PublicarNaFilaRabbitCommand(RotasRabbit.GerarArquivoCertificadosCodaf, codafId));
            return Resultado.DeSucesso();
        }

        private async Task SanitizarCertificadosEmitidosAsync(TipoCodaf tipoCodaf, long codafSuplementarId, IEnumerable<DadosEmissaoCertificadoCodafDto> listaDadosCertificado)
        {
            if (tipoCodaf == TipoCodaf.ListaPresenca) return;

            var inscricaoCursistas = listaDadosCertificado.Where(x => x.TipoParticipacao == TipoParticipacaoCodaf.Cursista).Select(x => x.InscricaoId).ToList();
            var inscritosReprovados = await repositorioCodafSuplementarInscricao.ObterIdInscritosReprovadosAsync(codafSuplementarId);
            var inscritosParaCancelarCertificado = inscricaoCursistas.Union(inscritosReprovados).ToList();
            await repositorioCodafCertificado.InativarCertificadosAnterioresCursistaAsync(inscritosParaCancelarCertificado);
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
