using FluentAssertions;
using Moq;
using Moq.AutoMock;
using SME.ConectaFormacao.Aplicacao.Dtos.Proposta;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;
using Xunit;

namespace SME.ConectaFormacao.Aplicacao.Teste.Commands.Propostas
{
    public class ValidarDatasExistentesNaPropostaCommandHandlerTestes
    {
        private readonly Mock<IRepositorioPropostaEncontro> _repositorioPropostaEncontro;
        private readonly ValidarDatasExistentesNaPropostaCommandHandler _sut;

        public ValidarDatasExistentesNaPropostaCommandHandlerTestes()
        {
            var mocker = new AutoMocker();
            _repositorioPropostaEncontro = mocker.GetMock<IRepositorioPropostaEncontro>();
            _sut = mocker.CreateInstance<ValidarDatasExistentesNaPropostaCommandHandler>();
        }

        [Fact]
        public async Task DadoDatasValidasEQuantidadeTurmasIgual_QuandoValidar_EntaoNaoDeveRetornarErros()
        {
            var dto = new PropostaDTO
            {
                DataRealizacaoInicio = new DateTime(2026, 10, 13),
                DataRealizacaoFim = new DateTime(2026, 10, 20),
                DataInscricaoInicio = new DateTime(2026, 10, 13, 0, 0, 0),
                DataInscricaoFim = new DateTime(2026, 10, 20, 23, 59, 59, 999),
                QuantidadeTurmas = 2
            };

            _repositorioPropostaEncontro
                .Setup(r => r.ObterQuantidadeDeTurmasComEncontroAsync(It.IsAny<long>()))
                .ReturnsAsync(2);

            var comando = new ValidarDatasExistentesNaPropostaCommand(1, dto);
            var resultado = await _sut.Handle(comando, CancellationToken.None);

            resultado.Should().BeEmpty();
        }

        [Fact]
        public async Task DadoDataInscricaoInicioMaiorQueFim_QuandoValidar_EntaoDeveRetornarErroCorrespondente()
        {
            var dto = new PropostaDTO
            {
                DataRealizacaoInicio = new DateTime(2026, 10, 13),
                DataRealizacaoFim = new DateTime(2026, 10, 20),
                DataInscricaoInicio = new DateTime(2026, 10, 15, 10, 0, 0),
                DataInscricaoFim = new DateTime(2026, 10, 13, 10, 0, 0),
                QuantidadeTurmas = 1
            };

            _repositorioPropostaEncontro
                .Setup(r => r.ObterQuantidadeDeTurmasComEncontroAsync(It.IsAny<long>()))
                .ReturnsAsync(1);

            var comando = new ValidarDatasExistentesNaPropostaCommand(1, dto);
            var resultado = await _sut.Handle(comando, CancellationToken.None);

            resultado.Should().Contain(MensagemNegocio.DATA_INSCRICAO_INICIO_MAIOR_QUE_DATA_INSCRICAO_FIM);
        }

        [Fact]
        public async Task DadoPeriodoInscricaoNaoInformado_QuandoValidar_EntaoDeveRetornarErroDePeriodoInscricao()
        {
            var dto = new PropostaDTO
            {
                DataRealizacaoInicio = new DateTime(2026, 10, 13),
                DataRealizacaoFim = new DateTime(2026, 10, 20),
                DataInscricaoInicio = null,
                DataInscricaoFim = null,
                QuantidadeTurmas = 1
            };

            _repositorioPropostaEncontro
                .Setup(r => r.ObterQuantidadeDeTurmasComEncontroAsync(It.IsAny<long>()))
                .ReturnsAsync(1);

            var comando = new ValidarDatasExistentesNaPropostaCommand(1, dto);
            var resultado = await _sut.Handle(comando, CancellationToken.None);

            resultado.Should().Contain(MensagemNegocio.PERIODO_INCRICAO_NAO_INFORMADO);
        }
    }
}
