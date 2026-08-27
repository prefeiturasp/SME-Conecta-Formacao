using Bogus;
using FluentAssertions;
using MediatR;
using Moq;
using Moq.AutoMock;
using SME.ConectaFormacao.Aplicacao.CasosDeUso.Proposta;
using SME.ConectaFormacao.Aplicacao.Dtos.Proposta;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Dominio.Excecoes;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace SME.ConectaFormacao.Aplicacao.Teste.CasosDeUso
{
    public class CasoDeUsoInserirPropostaTestes
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly CasoDeUsoInserirProposta _sut;
        private readonly Faker _faker;

        public CasoDeUsoInserirPropostaTestes()
        {
            var mocker = new AutoMocker();
            _mediatorMock = mocker.GetMock<IMediator>();

            _sut = mocker.CreateInstance<CasoDeUsoInserirProposta>();
            _faker = new Faker();
        }

        [Fact]
        public async Task DadoPropostaComoRascunhoEAreaPromotoraValida_QuandoChamarExecutar_EntaoDeveInserirRascunhoComSucesso()
        {
            // Arrange
            var propostaDTO = new PropostaDTO { Situacao = SituacaoProposta.Rascunho };
            var grupoUsuarioLogadoId = _faker.Random.Long(1);
            var dres = new long[] { _faker.Random.Long(1) };
            var areaPromotora = new AreaPromotora { Id = _faker.Random.Long(1) };
            var parametroDescricao = new ParametroSistema { Valor = "Descricao" };
            var parametroUrl = new ParametroSistema { Valor = "http://url.com" };
            var retornoDto = new RetornoDTO { Sucesso = true, EntidadeId = _faker.Random.Long(1) };

            _mediatorMock.Setup(m => m.Send(It.IsAny<ObterGrupoUsuarioLogadoQuery>(), default)).ReturnsAsync(grupoUsuarioLogadoId);
            _mediatorMock.Setup(m => m.Send(It.IsAny<ObterDresUsuarioLogadoQuery>(), default)).ReturnsAsync(dres);
            _mediatorMock.Setup(m => m.Send(It.IsAny<ObterAreaPromotoraPorGrupoIdEDresQuery>(), default)).ReturnsAsync(areaPromotora);
            _mediatorMock.Setup(m => m.Send(It.Is<ObterParametroSistemaPorTipoEAnoQuery>(q => q.Tipo == TipoParametroSistema.ComunicadoAcaoFormativaDescricao), default)).ReturnsAsync(parametroDescricao);
            _mediatorMock.Setup(m => m.Send(It.Is<ObterParametroSistemaPorTipoEAnoQuery>(q => q.Tipo == TipoParametroSistema.ComunicadoAcaoFormativaUrl), default)).ReturnsAsync(parametroUrl);
            _mediatorMock.Setup(m => m.Send(It.IsAny<InserirPropostaRascunhoCommand>(), default)).ReturnsAsync(retornoDto);
            _mediatorMock.Setup(m => m.Send(It.IsAny<SalvarPropostaMovimentacaoCommand>(), default)).ReturnsAsync(new RetornoDTO { Sucesso = true });

            // Act
            var resultado = await _sut.Executar(propostaDTO);

            // Assert
            resultado.Should().NotBeNull();
            resultado.Sucesso.Should().BeTrue();
            propostaDTO.AcaoFormativaTexto.Should().Be(parametroDescricao.Valor);
            propostaDTO.AcaoFormativaLink.Should().Be(parametroUrl.Valor);
            _mediatorMock.Verify(m => m.Send(It.IsAny<InserirPropostaRascunhoCommand>(), default), Times.Once);
            _mediatorMock.Verify(m => m.Send(It.IsAny<SalvarPropostaMovimentacaoCommand>(), default), Times.Once);
        }

        [Fact]
        public async Task DadoPropostaParaEnvioEAreaPromotoraValida_QuandoChamarExecutar_EntaoDeveInserirPropostaComSucesso()
        {
            // Arrange
            var propostaDTO = new PropostaDTO { Situacao = SituacaoProposta.Cadastrada };
            var grupoUsuarioLogadoId = _faker.Random.Long(1);
            var dres = new long[] { _faker.Random.Long(1) };
            var areaPromotora = new AreaPromotora { Id = _faker.Random.Long(1) };
            var parametroDescricao = new ParametroSistema { Valor = "Descricao" };
            var parametroUrl = new ParametroSistema { Valor = "http://url.com" };
            var retornoDto = new RetornoDTO { Sucesso = true, EntidadeId = _faker.Random.Long(1) };

            _mediatorMock.Setup(m => m.Send(It.IsAny<ObterGrupoUsuarioLogadoQuery>(), default)).ReturnsAsync(grupoUsuarioLogadoId);
            _mediatorMock.Setup(m => m.Send(It.IsAny<ObterDresUsuarioLogadoQuery>(), default)).ReturnsAsync(dres);
            _mediatorMock.Setup(m => m.Send(It.IsAny<ObterAreaPromotoraPorGrupoIdEDresQuery>(), default)).ReturnsAsync(areaPromotora);
            _mediatorMock.Setup(m => m.Send(It.Is<ObterParametroSistemaPorTipoEAnoQuery>(q => q.Tipo == TipoParametroSistema.ComunicadoAcaoFormativaDescricao), default)).ReturnsAsync(parametroDescricao);
            _mediatorMock.Setup(m => m.Send(It.Is<ObterParametroSistemaPorTipoEAnoQuery>(q => q.Tipo == TipoParametroSistema.ComunicadoAcaoFormativaUrl), default)).ReturnsAsync(parametroUrl);
            _mediatorMock.Setup(m => m.Send(It.IsAny<InserirPropostaCommand>(), default)).ReturnsAsync(retornoDto);
            _mediatorMock.Setup(m => m.Send(It.IsAny<SalvarPropostaMovimentacaoCommand>(), default)).ReturnsAsync(new RetornoDTO { Sucesso = true });

            // Act
            var resultado = await _sut.Executar(propostaDTO);

            // Assert
            resultado.Should().NotBeNull();
            resultado.Sucesso.Should().BeTrue();
            _mediatorMock.Verify(m => m.Send(It.IsAny<InserirPropostaCommand>(), default), Times.Once);
            _mediatorMock.Verify(m => m.Send(It.IsAny<SalvarPropostaMovimentacaoCommand>(), default), Times.Once);
        }

        [Fact]
        public async Task DadoAreaPromotoraNaoEncontrada_QuandoChamarExecutar_EntaoDeveLancarNegocioException()
        {
            // Arrange
            var propostaDTO = new PropostaDTO { Situacao = SituacaoProposta.Cadastrada };
            var grupoUsuarioLogadoId = _faker.Random.Long(1);
            var dres = new long[] { _faker.Random.Long(1) };

            _mediatorMock.Setup(m => m.Send(It.IsAny<ObterGrupoUsuarioLogadoQuery>(), default)).ReturnsAsync(grupoUsuarioLogadoId);
            _mediatorMock.Setup(m => m.Send(It.IsAny<ObterDresUsuarioLogadoQuery>(), default)).ReturnsAsync(dres);
            _mediatorMock.Setup(m => m.Send(It.IsAny<ObterAreaPromotoraPorGrupoIdEDresQuery>(), default)).ReturnsAsync((AreaPromotora)null);

            // Act
            var act = async () => await _sut.Executar(propostaDTO);

            // Assert
            await act.Should().ThrowAsync<NegocioException>()
                .WithMessage(MensagemNegocio.AREA_PROMOTORA_NAO_ENCONTRADA_GRUPO_USUARIO);
        }
    }
}
