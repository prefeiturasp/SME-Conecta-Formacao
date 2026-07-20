using FluentAssertions;
using SME.ConectaFormacao.Aplicacao.Mapeamentos;
using SME.ConectaFormacao.Dominio.Enumerados;
using System;
using System.Collections.Generic;
using System.Text;

namespace SME.ConectaFormacao.Aplicacao.Teste.Mapeamentos
{
    public class CriterioCertificacaoFactoryTestes
    {
        [Fact]
        public void DadoUmCriterioDeConceitoParticipacao_QuandoConstruirRegras_EntaoDeveRetornarConceitosAceitos()
        {
            // Arrange
            var criteriosIds = new List<long> { (int)TipoCriterioCertificacao.ConceitoParticipacao };
            
            // Act
            var resultado = CriterioCertificacaoFactory.ConstruirRegras(criteriosIds);

            // Assert
            resultado.Should().NotBeNull();
            resultado.ConceitosAceitos.Should().Contain("P");
            resultado.ConceitosAceitos.Should().Contain("S");
        }

        [Fact]
        public void DadoUmCriterioDeFrequencialIntegral_QuandoConstruirRegras_EntaoDeveRetornarFrequenciaMinima100()
        {
            // Arrange
            var criteriosIds = new List<long> { (int)TipoCriterioCertificacao.FrequenciaIntegral };

            // Act
            var resultado = CriterioCertificacaoFactory.ConstruirRegras(criteriosIds);

            // Assert
            resultado.Should().NotBeNull();
            resultado.FrequenciaMinima.Should().Be(100);
        }

        [Fact]
        public void DadoUmCriterioDeFrequenciaMinima75_QuandoConstruirRegras_EntaoDeveRetornarFrequenciaMinima75()
        {
            // Arrange
            var criteriosIds = new List<long> { (int)TipoCriterioCertificacao.FrequenciaMinima75 };

            // Act
            var resultado = CriterioCertificacaoFactory.ConstruirRegras(criteriosIds);

            // Assert
            resultado.Should().NotBeNull();
            resultado.FrequenciaMinima.Should().Be(75);
        }

        [Fact]
        public void DadoUmCriterioDeAtividadeObrigatoria_QuandoConstruirRegras_EntaoDeveRetornarExigeAtividadeObrigatoriaIgualTrue()
        {
            // Arrange
            var criteriosIds = new List<long> { (int)TipoCriterioCertificacao.AtividadeObrigatoria };

            // Act
            var resultado = CriterioCertificacaoFactory.ConstruirRegras(criteriosIds);

            // Assert
            resultado.Should().NotBeNull();
            resultado.ExigeAtividadeObrigatoria.Should().BeTrue();
        }
    }
}
