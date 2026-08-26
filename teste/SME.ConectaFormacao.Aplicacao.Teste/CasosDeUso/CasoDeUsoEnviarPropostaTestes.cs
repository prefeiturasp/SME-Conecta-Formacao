using AutoMapper;
using FluentAssertions;
using MediatR;
using Moq;
using Moq.AutoMock;
using SME.ConectaFormacao.Aplicacao.CasosDeUso.Proposta;
using SME.ConectaFormacao.Aplicacao.Comandos.PublicarNaFilaRabbit;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Dominio.Excecoes;
using SME.ConectaFormacao.Infra;
using System.Reflection;
using Xunit;

namespace SME.ConectaFormacao.Aplicacao.Teste.CasosDeUso
{
    public class CasoDeUsoEnviarPropostaTestes
    {
        private readonly AutoMocker _mocker;
        private readonly CasoDeUsoEnviarProposta _sut;

        public CasoDeUsoEnviarPropostaTestes()
        {
            _mocker = new AutoMocker();
            _sut = _mocker.CreateInstance<CasoDeUsoEnviarProposta>();
        }

        [Fact]
        public async Task DadoPropostaInexistente_QuandoExecutar_EntaoLancaExcecao()
        {
            // Arrange
            _mocker.GetMock<IMediator>()
                .Setup(m => m.Send(It.IsAny<ObterPropostaPorIdQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((SME.ConectaFormacao.Dominio.Entidades.Proposta?)null);

            // Act
            Func<Task> act = async () => await _sut.Executar(1);

            // Assert
            await act.Should().ThrowAsync<NegocioException>()
                .WithMessage("Proposta não encontrada");
        }

        [Fact]
        public async Task DadoPropostaNaoPodeSerEnviada_QuandoExecutar_EntaoLancaExcecao()
        {
            // Arrange
            var proposta = CriarPropostaComSituacao(SituacaoProposta.Rascunho);
            _mocker.GetMock<IMediator>()
                .Setup(m => m.Send(It.IsAny<ObterPropostaPorIdQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((SME.ConectaFormacao.Dominio.Entidades.Proposta?)proposta);

            // Act
            Func<Task> act = async () => await _sut.Executar(1);

            // Assert
            await act.Should().ThrowAsync<NegocioException>()
                .WithMessage("Proposta deve estar com situação de cadastrada, devolvida, Aguardando análise do DF ou Aguardando análise do parecer pelo DF para ser enviada para validação");
        }

        [Fact]
        public async Task DadoPropostaComInscricaoAutomaticaEFuncaoOutros_QuandoExecutar_EntaoLancaExcecao()
        {
            // Arrange
            var proposta = CriarPropostaComSituacao(SituacaoProposta.Cadastrada);
            
            _mocker.GetMock<IMediator>()
                .Setup(m => m.Send(It.IsAny<ObterPropostaPorIdQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((SME.ConectaFormacao.Dominio.Entidades.Proposta?)proposta);

            _mocker.GetMock<IMediator>()
                .Setup(m => m.Send(It.IsAny<ExisteCargoFuncaoOutrosNaPropostaQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            _mocker.GetMock<IMediator>()
                .Setup(m => m.Send(It.IsAny<ObterPropostaTipoInscricaoPorIdQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<PropostaTipoInscricao> { new PropostaTipoInscricao { TipoInscricao = TipoInscricao.Automatica } });

            // Act
            Func<Task> act = async () => await _sut.Executar(1);

            // Assert
            await act.Should().ThrowAsync<NegocioException>()
                .WithMessage("Proposta com os Tipos Inscrição Automática e Automática (JEIF) não podem conter a Função específica outros");
        }

        [Fact]
        public async Task DadoPropostaParaPublicar_QuandoExecutar_EntaoPublicaPropostaEPublishNaFila()
        {
            // Arrange
            var proposta = CriarPropostaComSituacao(SituacaoProposta.Cadastrada);
            proposta.FormacaoHomologada = FormacaoHomologada.NaoCursosExtras;

            _mocker.GetMock<IMediator>()
                .Setup(m => m.Send(It.IsAny<ObterPropostaPorIdQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((SME.ConectaFormacao.Dominio.Entidades.Proposta?)proposta);

            _mocker.GetMock<IMediator>()
                .Setup(m => m.Send(It.IsAny<ExisteCargoFuncaoOutrosNaPropostaQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            _mocker.GetMock<IMediator>()
                .Setup(m => m.Send(It.IsAny<ObterPropostaTipoInscricaoPorIdQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<PropostaTipoInscricao> { new PropostaTipoInscricao { TipoInscricao = TipoInscricao.Optativa } });

            _mocker.GetMock<IMediator>()
                .Setup(m => m.Send(It.IsAny<ValidarSeDataInscricaoEhMaiorQueDataRealizacaoCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(string.Empty);

            _mocker.GetMock<IMediator>()
                .Setup(m => m.Send(It.IsAny<ObterPareceristasAdicionadosNaPropostaQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<PropostaParecerista>());

            _mocker.GetMock<IMediator>()
                .Setup(m => m.Send(It.IsAny<ObterGrupoUsuarioLogadoQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(SME.ConectaFormacao.Dominio.Constantes.Perfis.ADMIN_DF);

            // Act
            var resultado = await _sut.Executar(1);

            // Assert
            resultado.Should().BeTrue();
            _mocker.GetMock<IMediator>().Verify(m => m.Send(It.Is<EnviarPropostaCommand>(c => c.Situacao == SituacaoProposta.Publicada), It.IsAny<CancellationToken>()), Times.Once);
            _mocker.GetMock<IMediator>().Verify(m => m.Send(It.Is<PublicarNaFilaRabbitCommand>(c => c.Rota == RotasRabbit.GerarPropostaTurmaVaga), It.IsAny<CancellationToken>()), Times.Once);
        }

        private SME.ConectaFormacao.Dominio.Entidades.Proposta CriarPropostaComSituacao(SituacaoProposta situacao)
        {
            var proposta = new SME.ConectaFormacao.Dominio.Entidades.Proposta();
            proposta.GetType().GetProperty("Situacao")?.SetValue(proposta, situacao);
            return proposta;
        }
    }
}
