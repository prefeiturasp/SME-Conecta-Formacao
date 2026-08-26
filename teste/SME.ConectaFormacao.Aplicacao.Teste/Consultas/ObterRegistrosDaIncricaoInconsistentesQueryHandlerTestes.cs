using FluentAssertions;
using Moq;
using Moq.AutoMock;
using SME.ConectaFormacao.Aplicacao.Consultas.Inscricoes;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao.Teste.Consultas
{
    public class ObterRegistrosDaIncricaoInconsistentesQueryHandlerTestes
    {
        private readonly AutoMocker _mocker;
        private readonly ObterRegistrosDaIncricaoInconsistentesQueryHandler _sut;

        public ObterRegistrosDaIncricaoInconsistentesQueryHandlerTestes()
        {
            _mocker = new AutoMocker();
            _sut = _mocker.CreateInstance<ObterRegistrosDaIncricaoInconsistentesQueryHandler>();
        }

        [Fact]
        public async Task DadoRequestValidoComErros_QuandoExecutar_EntaoRetornaListaComInconsistencias()
        {
            // Arrange
            var query = new ObterRegistrosDaIncricaoInconsistentesQuery(1, 0, 10);

            var registrosErros = new List<ImportacaoArquivoRegistro>
            {
                new() {
                    Linha = 1,
                    Erro = "Erro CPF",
                    Conteudo = """{"Cpf": "123"}"""
                }
            };

            var registrosPaginadosErro = new RegistrosPaginados<ImportacaoArquivoRegistro> { Registros = registrosErros, TotalRegistros = 1 };

            _mocker.GetMock<IRepositorioImportacaoArquivoRegistro>()
                .Setup(m => m.ObterRegistrosComMensagemDeErro(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<long>()))
                .ReturnsAsync(registrosPaginadosErro);

            var registrosPaginadosVazio = new RegistrosPaginados<ImportacaoArquivoRegistro> { Registros = [], TotalRegistros = 0 };

            _mocker.GetMock<IRepositorioImportacaoArquivoRegistro>()
                .Setup(m => m.ObterRegistroPorSituacao(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<long>(), SituacaoImportacaoArquivoRegistro.Validado))
                .ReturnsAsync(registrosPaginadosVazio);

            // Act
            var resultado = await _sut.Handle(query, CancellationToken.None);

            // Assert
            resultado.Should().NotBeNull();
            resultado.TotalRegistros.Should().Be(1);
            resultado.Items.Should().HaveCount(1);
            resultado.Items.First().Linha.Should().Be(1);
            resultado.Items.First().Erro.Should().Be("Erro CPF");
            resultado.Sucesso.Should().BeFalse();
        }

        [Fact]
        public async Task DadoRequestValidoComValidados_QuandoExecutar_EntaoHabilitaBotaoProcessar()
        {
            // Arrange
            var query = new ObterRegistrosDaIncricaoInconsistentesQuery(1, 0, 10);

            var registrosPaginadosVazio = new RegistrosPaginados<ImportacaoArquivoRegistro> { Registros = [], TotalRegistros = 0 };

            _mocker.GetMock<IRepositorioImportacaoArquivoRegistro>()
                .Setup(m => m.ObterRegistrosComMensagemDeErro(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<long>()))
                .ReturnsAsync(registrosPaginadosVazio);

            var registrosPaginadosValidado = new RegistrosPaginados<ImportacaoArquivoRegistro> { Registros = [new()], TotalRegistros = 1 };

            _mocker.GetMock<IRepositorioImportacaoArquivoRegistro>()
                .Setup(m => m.ObterRegistroPorSituacao(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<long>(), SituacaoImportacaoArquivoRegistro.Validado))
                .ReturnsAsync(registrosPaginadosValidado);

            // Act
            var resultado = await _sut.Handle(query, CancellationToken.None);

            // Assert
            resultado.Should().NotBeNull();
            resultado.TotalRegistros.Should().Be(0);
            resultado.Items.Should().BeEmpty();
            resultado.Sucesso.Should().BeTrue();
        }
    }
}
