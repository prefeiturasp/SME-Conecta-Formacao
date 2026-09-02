using FluentAssertions;
using Moq.AutoMock;
using SME.ConectaFormacao.Infra.Dados.Dtos.Propostas;
using SME.ConectaFormacao.Infra.Dados.Servicos.Formatadores;
using System;
using System.Collections.Generic;

namespace SME.ConectaFormacao.Infra.Dados.Teste.Servicos.Formatadores
{
    public class CronogramaHtmlFormatterTeste
    {
        public CronogramaHtmlFormatterTeste()
        {
        }

        [Fact]
        public void DadoPropostaComCronogramaValido_QuandoChamarFormatar_EntaoRetornaHtmlFormatado()
        {
            // Arrange
            var dados = new PropostaLaudaCompletaDto
            {
                DataRealizacaoInicio = new DateTime(2026, 2, 16),
                DataRealizacaoFim = new DateTime(2026, 10, 31),
                CronogramaTurmas = new List<TurmaLaudaDto>
                {
                    new TurmaLaudaDto
                    {
                        Identificacao = "Turma 4",
                        Local = "CEU PARQUE DO CARMO",
                        DataInicio = new DateTime(2026, 3, 3),
                        HoraInicio = "19:30",
                        HoraFim = "21:30"
                    },
                    new TurmaLaudaDto
                    {
                        Identificacao = "Turma 4",
                        Local = "CEU PARQUE DO CARMO",
                        DataInicio = new DateTime(2026, 3, 4),
                        HoraInicio = "19:30",
                        HoraFim = "21:30"
                    }
                }
            };

            // Act
            var resultado = CronogramaHtmlFormatter.Formatar(dados);

            // Assert
            resultado.Should().NotBeNullOrWhiteSpace();
            resultado.Should().Contain("PERÍODO DE REALIZAÇÃO: 16/02/2026 ATÉ 31/10/2026");
            resultado.Should().Contain("03/03; 04/03 - DAS 19H30 ÀS 21H30");
            resultado.Should().Contain("LOCAL: CEU PARQUE DO CARMO");
            resultado.Should().Contain("<strong>TURMA 4:</strong>");
        }

        [Fact]
        public void DadoPropostaSemLocal_QuandoChamarFormatar_EntaoRetornaLocalADefinir()
        {
            // Arrange
            var dados = new PropostaLaudaCompletaDto
            {
                CronogramaTurmas = new List<TurmaLaudaDto>
                {
                    new TurmaLaudaDto
                    {
                        Identificacao = "Turma 1",
                        Local = null,
                        DataInicio = new DateTime(2026, 3, 3),
                        HoraInicio = "19:30",
                        HoraFim = "21:30"
                    }
                }
            };

            // Act
            var resultado = CronogramaHtmlFormatter.Formatar(dados);

            // Assert
            resultado.Should().Contain("LOCAL: A DEFINIR");
        }
        
        [Fact]
        public void DadoPropostaComMultiplosLocais_QuandoChamarFormatar_EntaoRetornaSeparadorHr()
        {
            // Arrange
            var dados = new PropostaLaudaCompletaDto
            {
                CronogramaTurmas = new List<TurmaLaudaDto>
                {
                    new TurmaLaudaDto { Identificacao = "T1", Local = "Local 1" },
                    new TurmaLaudaDto { Identificacao = "T2", Local = "Local 2" }
                }
            };

            // Act
            var resultado = CronogramaHtmlFormatter.Formatar(dados);

            // Assert
            resultado.Should().Contain("<hr>");
        }
    }
}
