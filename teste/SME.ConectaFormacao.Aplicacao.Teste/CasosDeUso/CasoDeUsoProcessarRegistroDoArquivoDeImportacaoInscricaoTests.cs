using Bogus;
using Bogus.Extensions.Brazil;
using MediatR;
using Moq;
using Moq.AutoMock;
using RabbitMQ.Client;
using SME.ConectaFormacao.Aplicacao.CasosDeUso.ImportacaoInscricao;
using SME.ConectaFormacao.Aplicacao.Comandos.ImportacaoInscricao.AlterarSituacaoImportacaoArquivo;
using SME.ConectaFormacao.Aplicacao.Comandos.Inscricoes.SalvarInscricaoImportacao;
using SME.ConectaFormacao.Aplicacao.Dtos.ImportacaoArquivo;
using SME.ConectaFormacao.Aplicacao.Dtos.Inscricoes;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Dominio.Excecoes;
using SME.ConectaFormacao.Infra;
using SME.ConectaFormacao.Infra.Servicos.Log;
using SME.ConectaFormacao.Infra.Servicos.Rabbit.Dto;
using System.Text.Json;

namespace SME.ConectaFormacao.Aplicacao.Teste.CasosDeUso
{
    public class CasoDeUsoProcessarRegistroDoArquivoDeImportacaoInscricaoTests
    {
        private readonly AutoMocker _mocker;
        private readonly CasoDeUsoProcessarRegistroDoArquivoDeImportacaoInscricao _casoDeUso;
        private readonly Faker _faker;
        private readonly Mock<IModel> _rabbitChannelMock;

        public CasoDeUsoProcessarRegistroDoArquivoDeImportacaoInscricaoTests()
        {
            _mocker = new AutoMocker();
            _faker = new Faker("pt_BR");

            // Mock do canal do RabbitMQ para verificação de contagem de mensagens
            _rabbitChannelMock = new Mock<IModel>();
            _mocker.GetMock<IConexoesRabbit>()
                .Setup(c => c.Get())
                .Returns(_rabbitChannelMock.Object);

            _casoDeUso = _mocker.CreateInstance<CasoDeUsoProcessarRegistroDoArquivoDeImportacaoInscricao>();
        }

        [Fact]
        public async Task DadoMensagemValida_QuandoProcessamentoSucessoENaoHouverMaisPendencias_EntaoDeveSalvarInscricaoEFinalizarArquivo()
        {
            // Arrange
            var (mensagemRabbit, dtoRegistro) = MontarMensagemValida();

            // Mock: Turma encontrada
            _mocker.GetMock<IMediator>()
                .Setup(m => m.Send(It.IsAny<ObterPropostaTurmaPorIdQuery>(), CancellationToken.None))
                .ReturnsAsync(new PropostaTurma { PropostaId = 1 });

            // Mock: Proposta encontrada
            _mocker.GetMock<IMediator>()
                .Setup(m => m.Send(It.IsAny<ObterPropostaPorIdQuery>(), CancellationToken.None))
                .ReturnsAsync(new Proposta { Id = 1 });

            // Mock: Não há registros validados no banco (Verificação do Finally)
            _mocker.GetMock<IMediator>()
                .Setup(m => m.Send(It.IsAny<PossuiRegistroPorArquivoSituacaoQuery>(), CancellationToken.None))
                .ReturnsAsync(false);

            // Mock: Fila do Rabbit vazia (Verificação do Finally)
            _rabbitChannelMock
                .Setup(x => x.MessageCount(RotasRabbit.RealizarImportacaoInscricaoCursistaValidarItem))
                .Returns(0);

            // Act
            var resultado = await _casoDeUso.Executar(mensagemRabbit);

            // Assert
            Assert.True(resultado);

            // 1. Verifica se salvou a inscrição
            _mocker.GetMock<IMediator>().Verify(m => m.Send(It.IsAny<SalvarInscricaoImportacaoCommand>(), CancellationToken.None), Times.Once);

            // 2. Verifica se atualizou o registro individual para Processado
            _mocker.GetMock<IMediator>().Verify(m => m.Send(
                It.Is<AlterarSituacaoRegistroImportacaoArquivoCommand>(c =>
                    c.RegistroImportacaoId == dtoRegistro.Id &&
                    c.Situacao == SituacaoImportacaoArquivoRegistro.Processado),
                CancellationToken.None), Times.Once);

            // 3. Verifica se atualizou o ARQUIVO PAI para Processado (pois não havia pendências)
            _mocker.GetMock<IMediator>().Verify(m => m.Send(
                It.Is<AlterarSituacaoImportacaoArquivoCommand>(c =>
                    c.Id == dtoRegistro.ImportacaoArquivoId &&
                    c.Situacao == SituacaoImportacaoArquivo.Processado),
                CancellationToken.None), Times.Once);
        }

        [Fact]
        public async Task DadoTurmaNaoEncontrada_QuandoExecutar_EntaoDeveRegistrarErroNoItem()
        {
            // Arrange
            var (mensagemRabbit, dtoRegistro) = MontarMensagemValida();

            // Mock: Turma retorna NULL
            _mocker.GetMock<IMediator>()
                .Setup(m => m.Send(It.IsAny<ObterPropostaTurmaPorIdQuery>(), CancellationToken.None))
                .ReturnsAsync((PropostaTurma)null!);

            // Act
            await _casoDeUso.Executar(mensagemRabbit);

            // Assert
            // Verifica se enviou comando de erro com a mensagem correta
            _mocker.GetMock<IMediator>().Verify(m => m.Send(
                It.Is<AlterarSituacaoImportacaoArquivoRegistroCommand>(c =>
                    c.Id == dtoRegistro.Id &&
                    c.Situacao == SituacaoImportacaoArquivoRegistro.Erro &&
                    c.Erro == MensagemNegocio.TURMA_NAO_ENCONTRADA),
                CancellationToken.None), Times.Once);

            // Garante que NÃO tentou salvar a inscrição
            _mocker.GetMock<IMediator>().Verify(m => m.Send(It.IsAny<SalvarInscricaoImportacaoCommand>(), CancellationToken.None), Times.Never);

            // Verifica que o Finally ainda foi executado (tentativa de fechar arquivo)
            _mocker.GetMock<IMediator>().Verify(m => m.Send(It.IsAny<PossuiRegistroPorArquivoSituacaoQuery>(), CancellationToken.None), Times.Once);
        }

        [Fact]
        public async Task DadoMensagemInvalida_QuandoObjetoNulo_EntaoDeveLancarNegocioExceptionImediata()
        {
            // Arrange
            var mensagemRabbit = new MensagemRabbit { Mensagem = "null" }; // Simula conversão para nulo ou string vazia que retorna nulo no DTO

            // Act & Assert
            // O código lança exceção antes do try/catch, logo o teste deve capturar a exceção
            var exception = await Assert.ThrowsAsync<NegocioException>(() => _casoDeUso.Executar(mensagemRabbit));

            Assert.Equal(MensagemNegocio.IMPORTACAO_ARQUIVO_REGISTRO_NAO_LOCALIZADA, exception.Message);
        }

        private (MensagemRabbit, ImportacaoArquivoRegistroDto) MontarMensagemValida()
        {
            var inscricaoDto = new InscricaoCursistaImportacaoDto
            {
                Cpf = _faker.Person.Cpf(),
                Nome = _faker.Person.FullName,
                RegistroFuncional = _faker.Random.AlphaNumeric(7),
                Turma = "Turma A",
                ColaboradorRede = "Sim",
                Inscricao = new Inscricao { PropostaTurmaId = 10 }
            };

            var conteudoJson = JsonSerializer.Serialize(inscricaoDto);

            var registroDto = new ImportacaoArquivoRegistroDto
            {
                Id = _faker.Random.Long(1),
                ImportacaoArquivoId = _faker.Random.Long(1),
                Conteudo = conteudoJson
            };

            // Serializa o DTO para string para simular o comportamento do Rabbit
            var mensagemJson = JsonSerializer.Serialize(registroDto);
            var mensagemRabbit = new MensagemRabbit(mensagemJson);

            return (mensagemRabbit, registroDto);
        }
    }
}