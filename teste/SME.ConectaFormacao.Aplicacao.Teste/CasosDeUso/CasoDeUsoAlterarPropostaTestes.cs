using Bogus;
using FluentAssertions;
using MediatR;
using Moq;
using Moq.AutoMock;
using SME.ConectaFormacao.Aplicacao.CasosDeUso.Proposta;
using SME.ConectaFormacao.Aplicacao.Comandos.Propostas.SalvarPropostaGrupoPeriodo;
using SME.ConectaFormacao.Aplicacao.Dtos.Proposta;
using SME.ConectaFormacao.Dominio.Comum;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Dominio.Excecoes;

namespace SME.ConectaFormacao.Aplicacao.Teste.CasosDeUso
{
    public class CasoDeUsoAlterarPropostaTestes
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly CasoDeUsoAlterarProposta _sut;
        private readonly Faker _faker;

        public CasoDeUsoAlterarPropostaTestes()
        {
            var mocker = new AutoMocker();
            _mediatorMock = mocker.GetMock<IMediator>();

            _sut = mocker.CreateInstance<CasoDeUsoAlterarProposta>();
            _faker = new Faker();
        }

        [Fact]
        public async Task DadoUsuarioComPerfilAdminDF_QuandoChamarExecutar_EntaoDeveAlterarPropostaComSucesso()
        {
            // Arrange
            var propostaId = _faker.Random.Long(1);
            var propostaDTO = new PropostaDTO { Situacao = SituacaoProposta.Rascunho };
            var perfilAdminDF = Perfis.ADMIN_DF;
            var retornoDto = new RetornoDTO { Sucesso = true, EntidadeId = propostaId };

            _mediatorMock.Setup(m => m.Send(It.IsAny<ObterGrupoUsuarioLogadoQuery>(), default)).ReturnsAsync(perfilAdminDF);
            _mediatorMock.Setup(m => m.Send(It.IsAny<AlterarPropostaRascunhoCommand>(), default)).ReturnsAsync(retornoDto);
            _mediatorMock.Setup(m => m.Send(It.IsAny<SalvarPropostaGrupoPeriodoCommand>(), default)).ReturnsAsync(Resultado.DeSucesso());

            // Act
            var resultado = await _sut.Executar(propostaId, propostaDTO);

            // Assert
            resultado.Should().NotBeNull();
            resultado.Sucesso.Should().BeTrue();
            resultado.EntidadeId.Should().Be(propostaId);
            _mediatorMock.Verify(m => m.Send(It.IsAny<AlterarPropostaRascunhoCommand>(), default), Times.Once);
            _mediatorMock.Verify(m => m.Send(It.IsAny<SalvarPropostaGrupoPeriodoCommand>(), default), Times.Once);
        }

        [Fact]
        public async Task DadoUsuarioSemPermissao_QuandoChamarExecutar_EntaoDeveLancarNegocioException()
        {
            // Arrange
            var propostaId = _faker.Random.Long(1);
            var propostaDTO = new PropostaDTO();
            var perfilComum = Perfis.EMFORPEF;

            _mediatorMock.Setup(m => m.Send(It.IsAny<ObterGrupoUsuarioLogadoQuery>(), default)).ReturnsAsync(perfilComum);

            // Act
            var act = async () => await _sut.Executar(propostaId, propostaDTO);

            // Assert
            await act.Should().ThrowAsync<NegocioException>()
                .WithMessage(string.Format(MensagemNegocio.USUARIO_SEM_PERMISSAO_PARA_EDITAR_PROPOSTA, propostaId));
        }

        [Fact]
        public async Task DadoErroAoSalvarGrupoPeriodo_QuandoChamarExecutar_EntaoDeveLancarNegocioException()
        {
            // Arrange
            var propostaId = _faker.Random.Long(1);
            var propostaDTO = new PropostaDTO { Situacao = SituacaoProposta.Cadastrada };
            var perfilAdminDF = Perfis.ADMIN_DF;
            var retornoDto = new RetornoDTO { Sucesso = true, EntidadeId = propostaId };

            _mediatorMock.Setup(m => m.Send(It.IsAny<ObterGrupoUsuarioLogadoQuery>(), default)).ReturnsAsync(perfilAdminDF);
            _mediatorMock.Setup(m => m.Send(It.IsAny<AlterarPropostaCommand>(), default)).ReturnsAsync(retornoDto);
            _mediatorMock.Setup(m => m.Send(It.IsAny<SalvarPropostaGrupoPeriodoCommand>(), default)).ReturnsAsync(Erro.NaoEncontrado());

            // Act
            var act = async () => await _sut.Executar(propostaId, propostaDTO);

            // Assert
            await act.Should().ThrowAsync<NegocioException>();
        }
    }
}
