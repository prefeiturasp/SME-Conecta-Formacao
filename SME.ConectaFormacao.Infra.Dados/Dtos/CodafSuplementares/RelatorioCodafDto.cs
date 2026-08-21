#pragma warning disable CS8618
using SME.ConectaFormacao.Dominio.Enumerados;

namespace SME.ConectaFormacao.Infra.Dados.Dtos.CodafSuplementares
{
    public sealed class RelatorioCodafDto
    {
        public List<TurmaRelatorioCodafDto> Turmas { get; set; } = [];
        private RelatorioCodafDto() { }


        public static RelatorioCodafDto MapearParaDtoEstruturado(DadosPrincipaisRelatorioCodafDto dadosBruto)
        {
            var modalidade = dadosBruto.TipoFormato switch
            {
                Formato.Presencial => ModalidadeRelatorioCodaf.Presencial,
                Formato.Distancia => ModalidadeRelatorioCodaf.Distancia,
                Formato.Hibrido => ModalidadeRelatorioCodaf.Hibrido,
                _ => ModalidadeRelatorioCodaf.NaoInformado
            };

            var previaInscritosSme = new PreviaInscritosRelatorioCodafDto
            {
                TemRf = true,
                TotalInscritos = dadosBruto.Participantes?.Count(p => p.TemRf) ?? 0,
                TotalAprovados = dadosBruto.Participantes?.Count(p => p.TemRf && p.Aprovado) ?? 0,
                TotalReprovados = dadosBruto.Participantes?.Count(p => p.TemRf && !p.Aprovado) ?? 0
            };

            var previaInscritosSemRf = new PreviaInscritosRelatorioCodafDto
            {
                TotalInscritos = dadosBruto.Participantes?.Count(p => !p.TemRf) ?? 0,
                TotalAprovados = dadosBruto.Participantes?.Count(p => !p.TemRf && p.Aprovado) ?? 0,
                TotalReprovados = dadosBruto.Participantes?.Count(p => !p.TemRf && !p.Aprovado) ?? 0
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
                    DataPeriodoRealizacaoInicio = dadosBruto.PeriodoRealizacaoInicio ?? DateTime.MinValue,
                    DataPeriodoRealizacaoFim = dadosBruto.PeriodoRealizacaoFim ?? DateTime.MinValue,
                    TipoCertificacao = dadosBruto.CursoComCertificado ? TipoCertificacaoRelatorioCodaf.ComCertificacao : TipoCertificacaoRelatorioCodaf.SemCertificacao,
                    NumeroHomologacao = dadosBruto.NumeroHomologacao,
                    CodigoEventoSigpec = dadosBruto.CodigoEventoSigpec,
                    CargaHorariaTotal = dadosBruto.CargaHorariaTotal,
                    CargaHorariaDistancia = ConverterHoraMinutoParaInteiro(dadosBruto.CargaHorariaDistancia),
                    CargaHorariaPresencial = ConverterHoraMinutoParaInteiro(dadosBruto.CargaHorariaPresencial) + ConverterHoraMinutoParaInteiro(dadosBruto.CargaHorariaSincrona),
                    Modalidade = modalidade,
                    NumeroComunicado = dadosBruto.NumeroComunicado,
                    DataComunicado = dadosBruto.DataPublicacao,
                    DataPublicacaoDom = dadosBruto.DataPublicacaoDom,
                    PaginaDom = dadosBruto.PaginaComunicadoDom,
                    PreviaInscritosSme = previaInscritosSme,
                    PreviaInscritosSemRf = previaInscritosSemRf,
                    NomeTurma = dadosBruto.NomeTurma,
                    NumeroVagas = dadosBruto.QuantidadeVagasTurma,
                    NomeDre = dadosBruto.NomeDre,
                    Observacao = dadosBruto.Observacao,
                    ObservacaoCodafSuplementar = dadosBruto.ObservacaoCodafSuplementar,
                    DataCodaf = dadosBruto.DataCodaf,
                    DataDasAulasSincronas = ExpandirDataAulas(dadosBruto.DataAulas),
                    Retificacoes = dadosBruto.Retificacoes is null 
                                 ? null 
                                 : [.. dadosBruto.Retificacoes.Select(r => new RetificacaoRelatorioCodafDto
                                    {
                                        Data = r.Data,
                                        NumeroPagina = r.Pagina
                                    })]
                },
                AlunosAprovadosMunicipal = new GrupoAlunosRelatorioCodafDto
                {
                    TituloBloco = "PARTICIPANTES APROVADOS",
                    EhRedeParceira = false,
                    Alunos = MapearAlunos([.. dadosBruto.Participantes.Where(p => p.TemRf && p.Aprovado)], ref numeroSequencial)
                },
                AlunosAprovadosParceira = new GrupoAlunosRelatorioCodafDto
                {
                    EhRedeParceira = true,
                    Alunos = MapearAlunos([.. dadosBruto.Participantes.Where(p => !p.TemRf && p.Aprovado)], ref numeroSequencial)
                },
                AlunosReprovadosMunicipal = new GrupoAlunosRelatorioCodafDto
                {
                    TituloBloco = "PARTICIPANTES DESISTENTES E REPROVADOS",
                    EhRedeParceira = false,
                    Alunos = MapearAlunos([.. dadosBruto.Participantes.Where(p => p.TemRf && !p.Aprovado)], ref numeroSequencial)
                },
                AlunosReprovadosParceira = new GrupoAlunosRelatorioCodafDto
                {
                    EhRedeParceira = true,
                    Alunos = MapearAlunos([.. dadosBruto.Participantes.Where(p => !p.TemRf && !p.Aprovado)], ref numeroSequencial)
                },
                RegentesDaTurma = [.. dadosBruto.RegentesTurma.Select(r => new RegenteTurmaRelatorioCodafDto
                {
                    NomeRegente = r.Nome,
                    RfRegente = r.RegistroFuncional,
                    CodigoCertificado = r.CodigoCertificado
                })]
            };

            return new RelatorioCodafDto
            {
                Turmas = [turma]
            };
        }

        private static List<AlunoRelatorioCodafDto> MapearAlunos(List<DadosParticipanteRelatorioCodafDto> participantes, ref int numeroSequencial)
        {
            var alunos = new List<AlunoRelatorioCodafDto>();
            foreach (var participante in participantes)
            {
                alunos.Add(new AlunoRelatorioCodafDto
                {
                    NumeroSequencial = ++numeroSequencial,
                    NomeAluno = participante.Nome,
                    DocumentoAluno = participante.Documento,
                    PercentualFrequencia = (int)participante.PercentualFrequencia,
                    AtividadeObrigatoria = participante.AtividadeObrigatoria,
                    ConceitoFinal = participante.ConceitoFinal,
                    CodigoCertificado = participante.CodigoCertificado
                });
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
            if (periodos == null || !periodos.Any())
                return [];
            var datasExpandidas = new List<DateTime>();

            foreach (var periodo in periodos)
            {
                if (!periodo.DataFim.HasValue || periodo.DataInicio.Date == periodo.DataFim.Value.Date)
                {
                    datasExpandidas.Add(periodo.DataInicio.Date);
                    continue;
                }

                var dataInicio = periodo.DataInicio;
                var dataFim = periodo.DataFim;
                for (var date = dataInicio; date <= dataFim; date = date.AddDays(1))
                {
                    if (date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday)
                        continue;
                    datasExpandidas.Add(date);
                }
            }
            return [.. datasExpandidas
                .Distinct()
                .OrderBy(d => d)];
        }
    }
}
