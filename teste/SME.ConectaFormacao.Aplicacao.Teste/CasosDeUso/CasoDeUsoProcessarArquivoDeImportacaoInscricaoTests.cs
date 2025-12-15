using Bogus;
using MediatR;
using Moq;
using Moq.AutoMock;
using SME.ConectaFormacao.Aplicacao.CasosDeUso.ImportacaoInscricao;
using SME.ConectaFormacao.Aplicacao.Comandos.ImportacaoInscricao.AlterarSituacaoImportacaoArquivo;
using SME.ConectaFormacao.Aplicacao.Comandos.PublicarNaFilaRabbit;
using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Aplicacao.Dtos.ImportacaoArquivo;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Infra;
using SME.ConectaFormacao.Infra.Servicos.Rabbit.Dto;

namespace SME.ConectaFormacao.Aplicacao.Teste.CasosDeUso
{
    public class CasoDeUsoProcessarArquivoDeImportacaoInscricaoTests
    {
        private readonly AutoMocker _mocker;
        private readonly CasoDeUsoProcessarArquivoDeImportacaoInscricao _casoDeUso;
        private readonly Faker _faker;

        public CasoDeUsoProcessarArquivoDeImportacaoInscricaoTests()
        {
            _mocker = new AutoMocker();
            _faker = new Faker("pt_BR");
            _casoDeUso = _mocker.CreateInstance<CasoDeUsoProcessarArquivoDeImportacaoInscricao>();
        }

        [Fact]
        public async Task DadoMensagemNula_QuandoExecutar_EntaoDeveLancarArgumentNullException()
        {
            // Arrange
            var mensagemRabbit = new MensagemRabbit { Mensagem = null! };

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => _casoDeUso.Executar(mensagemRabbit));
        }

        [Fact]
        public async Task DadoArquivoSemRegistros_QuandoExecutar_EntaoDeveRetornarFalsoENaoAlterarSituacao()
        {
            // Arrange
            var idArquivo = _faker.Random.Long(1);
            var mensagemRabbit = new MensagemRabbit(idArquivo);

            ConfigurarMockParametroSistema("50");

            // Mock: Retorna 0 registros
            _mocker.GetMock<IMediator>()
                .Setup(m => m.Send(It.IsAny<ObterRegistrosImportacaoInscricaoCursistasPaginadosQuery>(), CancellationToken.None))
                .ReturnsAsync(new PaginacaoResultadoDto<ImportacaoArquivoRegistroDto>(new List<ImportacaoArquivoRegistroDto>(), 0, 0));

            // Act
            var resultado = await _casoDeUso.Executar(mensagemRabbit);

            // Assert
            Assert.False(resultado);

            // Verifica que NÃO alterou a situação para Processando
            _mocker.GetMock<IMediator>().Verify(m => m.Send(
                It.IsAny<AlterarSituacaoImportacaoArquivoCommand>(), CancellationToken.None), Times.Never);

            // Verifica que NÃO publicou nada na fila
            _mocker.GetMock<IMediator>().Verify(m => m.Send(
                It.IsAny<PublicarNaFilaRabbitCommand>(), CancellationToken.None), Times.Never);
        }

        [Fact]
        public async Task DadoArquivoComRegistros_QuandoExecutarPaginaUnica_EntaoDevePublicarMensagensEAlterarSituacao()
        {
            // Arrange
            var idArquivo = _faker.Random.Long(1);
            var mensagemRabbit = new MensagemRabbit(idArquivo);
            var quantidadeRegistros = 10;

            ConfigurarMockParametroSistema(quantidadeRegistros.ToString());

            // Mock: Retorna 10 registros com Total 10 (Página única, sai do loop imediatamente)
            var listaRegistros = GerarListaRegistros(quantidadeRegistros);
            var retornoPaginado = new PaginacaoResultadoDto<ImportacaoArquivoRegistroDto>(listaRegistros, quantidadeRegistros, quantidadeRegistros);

            _mocker.GetMock<IMediator>()
                .Setup(m => m.Send(It.IsAny<ObterRegistrosImportacaoInscricaoCursistasPaginadosQuery>(), CancellationToken.None))
                .ReturnsAsync(retornoPaginado);

            // Act
            var resultado = await _casoDeUso.Executar(mensagemRabbit);

            // Assert
            Assert.True(resultado);

            // Verifica alteração de status para Processando
            _mocker.GetMock<IMediator>().Verify(m => m.Send(
                It.Is<AlterarSituacaoImportacaoArquivoCommand>(c =>
                    c.Id == idArquivo &&
                    c.Situacao == SituacaoImportacaoArquivo.Processando),
                CancellationToken.None), Times.Once);

            // Verifica se publicou cada registro na fila
            _mocker.GetMock<IMediator>().Verify(m => m.Send(
                It.Is<PublicarNaFilaRabbitCommand>(c =>
                    c.Rota == RotasRabbit.ProcessarRegistroDoArquivoDeImportacaoInscricao),
                CancellationToken.None), Times.Exactly(quantidadeRegistros));
        }

        // Métodos Auxiliares
        private void ConfigurarMockParametroSistema(string valorRetorno)
        {
            _mocker.GetMock<IMediator>()
                .Setup(m => m.Send(It.IsAny<ObterParametroSistemaPorTipoEAnoQuery>(), CancellationToken.None))
                .ReturnsAsync(new ParametroSistema { Valor = valorRetorno });
        }

        private List<ImportacaoArquivoRegistroDto> GerarListaRegistros(int quantidade)
        {
            return new Faker<ImportacaoArquivoRegistroDto>()
                .RuleFor(r => r.Id, f => f.Random.Long(1))
                .RuleFor(r => r.Conteudo, f => f.Lorem.Sentence())
                .Generate(quantidade);
        }
    }
}
