using ConectaFormacao.Dominio.Servicos;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using SME.ConectaFormacao.Aplicacao.Comandos.PublicarNaFilaRabbit;
using SME.ConectaFormacao.Aplicacao.Comandos.SalvarLog;
using SME.ConectaFormacao.Aplicacao.Interfaces.CodafDeclaracoes;
using SME.ConectaFormacao.Dominio.Comum;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Dominio.Extensoes;
using SME.ConectaFormacao.Infra;
using SME.ConectaFormacao.Infra.Dados;
using SME.ConectaFormacao.Infra.Dados.Dtos.CodafDeclaracoes;
using SME.ConectaFormacao.Infra.Dados.Estrategias.Interfaces;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;
using SME.ConectaFormacao.Infra.Dominio.Enumerados;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.CodafDeclaracoes
{
    public class CasoDeUsoEmitirDeclaracaoCodaf(
        IRepositorioCodafDeclaracao repositorioCodafDeclaracao,
        IKeyedServiceProvider serviceProvider,
        IMediator mediator,
        IPeriodoRealizacaoConsultaService periodoRealizacaoConsultaService,
        IRepositorioCodafSuplementarInscricao repositorioCodafSuplementarInscricao,
        ITransacao transacao) :
        ICasoDeUsoEmitirDeclaracaoCodaf
    {
        private readonly Guid _identificadorRastreamento = Guid.NewGuid();
        public async Task<Resultado> ExecutarAsync(long codafNaoHomologadoId)
        {
            await mediator.Send(new SalvarLogCommand(typeof(CasoDeUsoEmitirDeclaracaoCodaf).FullName!, LogNivel.Informacao, $"Iniciando emissão de declarações do Codaf {codafNaoHomologadoId}", identificadorRastreamento: _identificadorRastreamento));
            
            var listaDadosDeclaracao = await repositorioCodafDeclaracao.ObterDadosParaEmissaoDeclaracoesCodafAsync(codafNaoHomologadoId) ?? throw new ArgumentOutOfRangeException(null, null, null);
            
            await mediator.Send(new SalvarLogCommand(typeof(CasoDeUsoEmitirDeclaracaoCodaf).FullName!, LogNivel.Informacao, $"Finalizada consulta de dados para emissão de declaracoes do Codaf - {codafNaoHomologadoId}", identificadorRastreamento: _identificadorRastreamento));

            if (!listaDadosDeclaracao.Any())
                return Erro.NaoEncontrado();
            await mediator.Send(new SalvarLogCommand(typeof(CasoDeUsoEmitirDeclaracaoCodaf).FullName!, LogNivel.Informacao, $"Iniciando geração de declarações do Codaf - {codafNaoHomologadoId}", identificadorRastreamento: _identificadorRastreamento));

            var entidadesParaSalvar = new List<CodafDeclaracao>();

            foreach (var dados in listaDadosDeclaracao)
            {
                var tipoEstrategia = DefinirEstrategia(dados);
                var geradorDeclaracao = serviceProvider.GetRequiredKeyedService<IDeclaracaoCodafGeradorConteudo>(tipoEstrategia);
                var periodo = await periodoRealizacaoConsultaService.ObterPeriodoRealizacaoAsync(dados.PropostaTurmaId);

                if (periodo != null)
                {
                    dados.DataInicio = periodo.DataInicio;
                    dados.DataFim = periodo.DataFim;
                }

                var htmlDeclaracao = geradorDeclaracao.GerarHtml(dados);
                var metadados = new
                {
                    dados.NomeFormacao,
                    dados.HorasTotais,
                    dados.CargaHorariaTotalOutra,
                    dados.Emissor,
                    dados.TipoFormacao,
                    DataInicio = periodo?.DataInicio.Date,
                    DataFim = periodo?.DataFim.Date
                };
                var metadadosJson = metadados.ObjetoParaJson();

                var novaDeclaracao = new CodafDeclaracao(
                    codafNaoHomologadoId,
                    dados.TipoParticipacao,
                    dados.IdReferencia,
                    htmlDeclaracao,
                    metadadosJson
                );
                entidadesParaSalvar.Add(novaDeclaracao);
            }

            if (entidadesParaSalvar.Count != 0)
            {
                await mediator.Send(new SalvarLogCommand(typeof(CasoDeUsoEmitirDeclaracaoCodaf).FullName!, LogNivel.Informacao, $"Iniciando registro da emissão de declarações do Codaf - {codafNaoHomologadoId}", identificadorRastreamento: _identificadorRastreamento));
                using var transacaoDb = transacao.Iniciar();
                try
                {                    
                    await SanitizarDeclaracoesEmitidasAsync(codafNaoHomologadoId, listaDadosDeclaracao);
                    await repositorioCodafDeclaracao.InserirLoteAsync(entidadesParaSalvar);
                    await repositorioCodafDeclaracao.AtualizaCodigoDeclaracao(codafNaoHomologadoId);
                    transacaoDb.Commit();                    
                }
                catch (Exception ex)
                {
                    await mediator.Send(new SalvarLogCommand(typeof(CasoDeUsoEmitirDeclaracaoCodaf).FullName!, LogNivel.Critico, $"Erro ao registrar a emissão de declarações do Codaf - {codafNaoHomologadoId}. Erro: {ex.Message}", identificadorRastreamento: _identificadorRastreamento, excecao: ex));
                    transacaoDb.Rollback();
                    throw;
                }
                await mediator.Send(new SalvarLogCommand(typeof(CasoDeUsoEmitirDeclaracaoCodaf).FullName!, LogNivel.Informacao, $"Finalizada registro da emissão de declarações do Codaf - {codafNaoHomologadoId}", identificadorRastreamento: _identificadorRastreamento));
            }
            await mediator.Send(new SalvarLogCommand(typeof(CasoDeUsoEmitirDeclaracaoCodaf).FullName!, LogNivel.Informacao, $"Finalizada geração de declarações do Codaf - {codafNaoHomologadoId}", identificadorRastreamento: _identificadorRastreamento));

            await mediator.Send(new PublicarNaFilaRabbitCommand(RotasRabbit.GerarArquivoDeclaracoesCodaf, codafNaoHomologadoId));
            return Resultado.DeSucesso();
        }

        private async Task SanitizarDeclaracoesEmitidasAsync(long codafNaoHomologadoId, IEnumerable<DadosEmissaoDeclaracaoCodafDto> listaDadosDeclaracao)
        {
            var inscricaoCursistas = listaDadosDeclaracao.Where(x => x.TipoParticipacao == TipoParticipacaoCodaf.Cursista).Select(x => x.InscricaoId).ToList();
            var inscritosReprovados = await repositorioCodafSuplementarInscricao.ObterIdInscritosReprovadosAsync(codafNaoHomologadoId);
            var inscritosParaCancelarDeclaracao = inscricaoCursistas.Union(inscritosReprovados).ToList();
            await repositorioCodafDeclaracao.InativarDeclaracoesAnterioresCursistaAsync(inscritosParaCancelarDeclaracao);
        }

        private static TipoEstrategiaCodaf DefinirEstrategia(DadosEmissaoDeclaracaoCodafDto dto)
        {
            if (dto.TipoParticipacao == TipoParticipacaoCodaf.Regente)
                return dto.TemRf
                    ? TipoEstrategiaCodaf.RegenteComRf
                    : TipoEstrategiaCodaf.RegenteSemRf;

            return dto.TemRf
                ? TipoEstrategiaCodaf.CursistaComRf
                : TipoEstrategiaCodaf.CursistaSemRf;
        }
    }
}
