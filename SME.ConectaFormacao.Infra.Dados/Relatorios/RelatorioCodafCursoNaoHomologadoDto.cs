#pragma warning disable CS8618
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Infra.Dados.Dtos.CodafCursosNaoHomologados;
using SME.ConectaFormacao.Infra.Dados.Dtos.CodafSuplementares;

namespace SME.ConectaFormacao.Infra.Dados.Relatorios
{
    public sealed class RelatorioCodafCursoNaoHomologadoDto
    {
        public List<TurmaRelatorioCodafDto> Turmas { get; set; } = [];
        private RelatorioCodafCursoNaoHomologadoDto() { }

        public static RelatorioCodafCursoNaoHomologadoDto MapearParaDtoEstruturado(
            DadosPrincipaisRelatorioCodafCursoNaoHomologadoDto dadosBruto)
        {
            var modalidade = dadosBruto.TipoFormacao == TipoFormacao.Evento
                ? ModalidadeRelatorioCodaf.Hibrido
                : dadosBruto.TipoFormato switch
                {
                    Formato.Presencial => ModalidadeRelatorioCodaf.Presencial,
                    Formato.Distancia => ModalidadeRelatorioCodaf.Distancia,
                    Formato.Hibrido => ModalidadeRelatorioCodaf.Hibrido,
                    _ => ModalidadeRelatorioCodaf.NaoInformado
                };

            var participantes = (dadosBruto.Participantes ?? [])
                .OrderBy(p => p.Nome, StringComparer.OrdinalIgnoreCase)
                .ToList();

            var previaInscritosSme = new PreviaInscritosRelatorioCodafDto
            {
                TemRf = true,
                TotalInscritos = participantes.Count(p => p.TemRf),
                TotalAprovados = participantes.Count(p => p.TemRf && p.Participou),
                TotalReprovados = participantes.Count(p => p.TemRf && !p.Participou)
            };

            var previaInscritosSemRf = new PreviaInscritosRelatorioCodafDto
            {
                TotalInscritos = participantes.Count(p => !p.TemRf),
                TotalAprovados = participantes.Count(p => !p.TemRf && p.Participou),
                TotalReprovados = participantes.Count(p => !p.TemRf && !p.Participou)
            };

            var numeroSequencial = 0;

            var turma = new TurmaRelatorioCodafDto
            {
                NomeTurma = dadosBruto.NomeTurma,
                Cabecalho = new CabecalhoRelatorioCodafDto
                {
                    AreaPromotora = dadosBruto.NomeAreaPromotora,
                    TipoFormacao = (TipoFormacaoRelatorioCodaf)dadosBruto.TipoFormacao,
                    NomeFormacao = dadosBruto.NomeFormacao,
                    QuantidadeTurmas = dadosBruto.QuantidadeTurmas,
                    DataPeriodoRealizacaoInicio = dadosBruto.PeriodoRealizacaoInicio,
                    DataPeriodoRealizacaoFim = dadosBruto.PeriodoRealizacaoFim,
                    TipoCertificacao = dadosBruto.CursoComCertificado
                        ? TipoCertificacaoRelatorioCodaf.ComCertificacao
                        : TipoCertificacaoRelatorioCodaf.SemCertificacao,
                    NumeroHomologacao = dadosBruto.NumeroHomologacao,
                    CodigoEventoSigpec = dadosBruto.CodigoEventoSigpec,
                    CargaHorariaTotal = dadosBruto.CargaHorariaTotal,
                    CargaHorariaDistancia = ConverterHoraMinutoParaInteiro(dadosBruto.CargaHorariaDistancia),
                    CargaHorariaPresencial = ConverterHoraMinutoParaInteiro(dadosBruto.CargaHorariaPresencial)
                                           + ConverterHoraMinutoParaInteiro(dadosBruto.CargaHorariaSincrona),
                    Modalidade = modalidade,
                    NumeroComunicado = dadosBruto.NumeroComunicado ?? 0,
                    DataComunicado = dadosBruto.DataPublicacao,
                    DataPublicacaoDom = dadosBruto.DataPublicacaoDom,
                    PaginaDom = dadosBruto.PaginaComunicadoDom ?? 0,
                    Retificacoes = (dadosBruto.Retificacoes is null || !dadosBruto.Retificacoes.Any())
                        ? null
                        : [.. dadosBruto.Retificacoes.Select(r => new RetificacaoRelatorioCodafDto
                            {
                                Data = r.Data,
                                NumeroPagina = r.Pagina
                            })],

                    PreviaInscritosSme = previaInscritosSme,
                    PreviaInscritosSemRf = previaInscritosSemRf,
                    NomeTurma = dadosBruto.NomeTurma,
                    NumeroVagas = dadosBruto.QuantidadeVagasTurma,
                    NomeDre = dadosBruto.NomeDre,
                    Observacao = dadosBruto.Observacao,
                    DataCodaf = dadosBruto.DataCodaf,
                    DataDasAulasSincronas = ExpandirDataAulas(dadosBruto.DataAulas)
                },
                AlunosAprovadosMunicipal = new GrupoAlunosRelatorioCodafDto
                {
                    TituloBloco = "PARTICIPANTES APROVADOS",
                    EhRedeParceira = false,
                    EhCodafNaoHomologado = true,
                    Alunos = MapearAlunos([.. participantes.Where(p => p.TemRf && p.Participou)], ref numeroSequencial)
                },
                AlunosAprovadosParceira = new GrupoAlunosRelatorioCodafDto
                {
                    EhRedeParceira = true,
                    EhCodafNaoHomologado = true,
                    Alunos = MapearAlunos([.. participantes.Where(p => !p.TemRf && p.Participou)], ref numeroSequencial)
                },
                AlunosReprovadosMunicipal = new GrupoAlunosRelatorioCodafDto
                {
                    TituloBloco = "PARTICIPANTES DESISTENTES OU REPROVADOS",
                    EhRedeParceira = false,
                    EhCodafNaoHomologado = true,
                    Alunos = MapearAlunos([.. participantes.Where(p => p.TemRf && !p.Participou)], ref numeroSequencial)
                },
                AlunosReprovadosParceira = new GrupoAlunosRelatorioCodafDto
                {
                    EhRedeParceira = true,
                    EhCodafNaoHomologado = true,
                    Alunos = MapearAlunos([.. participantes.Where(p => !p.TemRf && !p.Participou)], ref numeroSequencial)
                },
                RegentesDaTurma = [.. dadosBruto.RegentesTurma.Select(r => new RegenteTurmaRelatorioCodafDto
                {
                    NomeRegente = r.Nome,
                    RfRegente = r.RegistroFuncional,
                    CodigoCertificado = r.CodigoCertificado
                })]
            };

            return new RelatorioCodafCursoNaoHomologadoDto { Turmas = [turma] };
        }

        private static List<AlunoRelatorioCodafDto> MapearAlunos(
            List<DadosParticipanteRelatorioCodafCursoNaoHomologadoDto> participantes, ref int numeroSequencial)
        {
            var alunos = new List<AlunoRelatorioCodafDto>();
            foreach (var participante in participantes)
            {
                alunos.Add(new AlunoRelatorioCodafDto
                {
                    NumeroSequencial = ++numeroSequencial,
                    NomeAluno = participante.Nome,
                    DocumentoAluno = participante.Documento,
                    Participacao = participante.Participou ? "SIM" : "NÃO",
                    NumeroRegistroDeclaracao = participante.CodigoCertificadoDeclaracao.HasValue
                        ? participante.CodigoCertificadoDeclaracao.Value.ToString()
                        : "***"
                    });

                    // PercentualFrequencia, AtividadeObrigatoria, ConceitoFinal, CodigoCertificado
                    // ficam null — não se aplicam a este relatório
            }
            return alunos;
        }

        private static int ConverterHoraMinutoParaInteiro(string? horaMinuto)
        {
            if (string.IsNullOrWhiteSpace(horaMinuto) || !horaMinuto.Contains(':')) return 0;
            var partes = horaMinuto.Split(':');
            if (partes.Length > 0 && int.TryParse(partes[0], out var hora))
                return hora;
            return 0;
        }

        private static List<DateTime> ExpandirDataAulas(IEnumerable<DataAulaTurmaRelatorioCodafDto> periodos)
        {
            if (periodos == null || !periodos.Any()) return [];
            var datasExpandidas = new List<DateTime>();

            foreach (var periodo in periodos)
            {
                if (!periodo.DataFim.HasValue || periodo.DataInicio.Date == periodo.DataFim.Value.Date)
                {
                    datasExpandidas.Add(periodo.DataInicio.Date);
                    continue;
                }

                for (var date = periodo.DataInicio; date <= periodo.DataFim; date = date.AddDays(1))
                {
                    if (date.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday) continue;
                    datasExpandidas.Add(date);
                }
            }
            return [.. datasExpandidas.Distinct().OrderBy(d => d)];
        }
    }
}