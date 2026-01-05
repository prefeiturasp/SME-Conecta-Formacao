using MediatR;
using Moq;
using Moq.AutoMock;
using SME.ConectaFormacao.Aplicacao.CasosDeUso.ImportacaoInscricao;
using SME.ConectaFormacao.Aplicacao.Comandos.ImportacaoInscricao.AlterarSituacaoImportacaoArquivo;
using SME.ConectaFormacao.Aplicacao.Comandos.PublicarNaFilaRabbit;
using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Aplicacao.Dtos.ImportacaoArquivo;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Dominio.Excecoes;
using SME.ConectaFormacao.Infra;
using SME.ConectaFormacao.Infra.Servicos.Rabbit.Dto;
using System.Text.Json;

namespace SME.ConectaFormacao.Aplicacao.Teste.CasosDeUso
{
    public class CasoDeUsoImportacaoInscricaoCursistaValidarTests
    {
        private readonly AutoMocker _mocker;
        private readonly CasoDeUsoImportacaoInscricaoCursistaValidar _casoDeUso;

        public CasoDeUsoImportacaoInscricaoCursistaValidarTests()
        {
            _mocker = new AutoMocker();
            _casoDeUso = _mocker.CreateInstance<CasoDeUsoImportacaoInscricaoCursistaValidar>();
        }

        [Fact]
        public async Task DadoMensagemRabbitSemConteudoValidoQuandoExecutarDeveLancarNegocioException()
        {
            // Arrange
            var mensagemRabbit = new MensagemRabbit
            {
                Mensagem = "null" // Simula um JSON que resulta em objeto nulo ou inválido
            };

            // Act & Assert
            var excecao = await Assert.ThrowsAsync<NegocioException>(() =>
                _casoDeUso.Executar(mensagemRabbit));

            Assert.Equal(MensagemNegocio.IMPORTACAO_ARQUIVO_NAO_LOCALIZADA, excecao.Message);
        }

        [Fact]
        public async Task DadoMensagemValidaQuandoExecutarDeveIterarRegistrosEPublicarFilaItem()
        {
            // Arrange
            var propostaId = 10;
            var importacaoId = 100;
            var qtdeRegistrosParametro = 50;

            var importacaoDto = new ImportacaoArquivoDTO(propostaId, "arquivo.xlsx", TipoImportacaoArquivo.Inscricao_Manual, SituacaoImportacaoArquivo.CarregamentoInicial)
            {
                Id = importacaoId
            };

            var mensagemRabbit = new MensagemRabbit
            {
                Mensagem = JsonSerializer.Serialize(importacaoDto)
            };

            // Mock: Obter Parametro do Sistema (Quantidade de registros)
            _mocker.GetMock<IMediator>()
                .Setup(m => m.Send(It.IsAny<ObterParametroSistemaPorTipoEAnoQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ParametroSistema { Valor = qtdeRegistrosParametro.ToString() });

            // Mock: Obter Registros Paginados
            // Cabe em uma página para evitar o loop infinito lógico mencionado
            var listaRegistros = new List<ImportacaoArquivoRegistroDto>
            {
                new() { Id = 1, Linha = 1, Conteudo = "Dados1" },
                new() { Id = 2, Linha = 2, Conteudo = "Dados2" }
            };

            var paginacaoRetorno = new PaginacaoResultadoDto<ImportacaoArquivoRegistroDto>(listaRegistros,2,2);

            _mocker.GetMock<IMediator>()
                .Setup(m => m.Send(It.IsAny<ObterRegistrosImportacaoInscricaoCursistasPaginadosQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(paginacaoRetorno);

            // Act
            var resultado = await _casoDeUso.Executar(mensagemRabbit);

            // Assert
            Assert.True(resultado);

            // 1. Verifica se consultou o parametro de sistema
            _mocker.GetMock<IMediator>().Verify(m => m.Send(
                It.Is<ObterParametroSistemaPorTipoEAnoQuery>(q =>
                    q.TipoParametroSistema == TipoParametroSistema.QtdeRegistrosImportacaoArquivoInscricaoCursista),
                It.IsAny<CancellationToken>()), Times.Once);

            // 2. Verifica se consultou os registros paginados
            _mocker.GetMock<IMediator>().Verify(m => m.Send(
                It.Is<ObterRegistrosImportacaoInscricaoCursistasPaginadosQuery>(q =>
                    q.ImportacaoArquivoId == importacaoId &&
                    q.NumeroRegistros == qtdeRegistrosParametro),
                It.IsAny<CancellationToken>()), Times.AtLeastOnce);

            // 3. Verifica se publicou na fila de validação de ITEM para cada registro encontrado (2 vezes)
            _mocker.GetMock<IMediator>().Verify(m => m.Send(
                It.Is<PublicarNaFilaRabbitCommand>(c =>
                    c.Rota == RotasRabbit.RealizarImportacaoInscricaoCursistaValidarItem &&
                    ((ImportacaoArquivoRegistroDto)c.Filtros).PropostaId == propostaId),
                It.IsAny<CancellationToken>()), Times.Exactly(2));

            // 4. Verifica se alterou a situação da importação para 'Validando' ao final
            _mocker.GetMock<IMediator>().Verify(m => m.Send(
                It.Is<AlterarSituacaoImportacaoArquivoCommand>(c =>
                    c.Id == importacaoId &&
                    c.Situacao == SituacaoImportacaoArquivo.Validando),
                It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
