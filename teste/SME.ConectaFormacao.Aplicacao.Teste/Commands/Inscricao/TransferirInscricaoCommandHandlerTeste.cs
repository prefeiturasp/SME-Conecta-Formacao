using FluentAssertions;
using MediatR;
using Moq;
using SME.ConectaFormacao.Aplicacao.Dtos.Inscricao;
using SME.ConectaFormacao.Aplicacao.Dtos.Proposta;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Dominio.Excecoes;
using SME.ConectaFormacao.Infra.Dados;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;
using System.Data;
using System.Net;

namespace SME.ConectaFormacao.Aplicacao.Teste.Commands.Inscricao
{
    public class TransferirInscricaoCommandHandlerTeste
    {
        private readonly Mock<IRepositorioInscricao> _repositorioInscricaoMock = new();
        private readonly Mock<IRepositorioProposta> _repositorioPropostaMock = new();
        private readonly Mock<IRepositorioUsuario> _repositorioUsuarioMock = new();
        private readonly Mock<IMediator> _mediatorMock = new();
        private readonly Mock<ITransacao> _transacaoMock = new();
        private readonly Mock<IDbTransaction> _transacaoDbMock = new();

        private readonly TransferirInscricaoCommandHandler _handler;

        public TransferirInscricaoCommandHandlerTeste()
        {
            _transacaoMock.Setup(t => t.Iniciar()).Returns(_transacaoDbMock.Object);
            _handler = new TransferirInscricaoCommandHandler(
                _transacaoMock.Object,
                _repositorioInscricaoMock.Object,
                null,
                _mediatorMock.Object,
                _repositorioPropostaMock.Object,
                _repositorioUsuarioMock.Object
            );
        }

        [Fact]
        public async Task Deve_Transferir_Inscricao_Com_Sucesso()
        {
            var inscricao = CriarInscricao();
            var usuario = new Usuario { Id = 1, Nome = "João", Login = "123", Cpf = "12345678900" };
            var propostaTurma = new PropostaTurma
            {
                Id = 10,
                PropostaId = 999,
                Dres = new List<PropostaTurmaDre> { new PropostaTurmaDre { DreId = 123456 } }
            };

            var dto = new InscricaoTransferenciaDTO
            {
                IdFormacaoOrigem = 1,
                IdFormacaoDestino = 2,
                IdTurmaOrigem = 1,
                IdTurmaDestino = 2,
                Cursistas = new List<string> { "123" }
            };

            var command = new TransferirInscricaoCommand(5, dto);

            _repositorioInscricaoMock.Setup(r => r.ObterPorId(5)).ReturnsAsync(inscricao);
            _mediatorMock.Setup(m => m.Send(It.IsAny<ObterUsuarioLogadoQuery>(), default)).ReturnsAsync(usuario);
            _repositorioPropostaMock.Setup(r => r.ObterTurmaDaPropostaComDresPorId(2)).ReturnsAsync(propostaTurma);
            _repositorioUsuarioMock.Setup(r => r.ObterPorLogin("123")).ReturnsAsync(usuario);

            _mediatorMock.Setup(m => m.Send(It.IsAny<SalvarInscricaoManualCommand>(), default))
                .ReturnsAsync(new RetornoDTO { Sucesso = true, EntidadeId = 20 });

            var resultado = await _handler.Handle(command, CancellationToken.None);

            resultado.Sucesso.Should().BeTrue();
            resultado.Mensagem.Should().Be("Todas as inscrições foram transferidas com sucesso.");
            _transacaoDbMock.Verify(t => t.Commit(), Times.Never); 
        }

        [Fact]
        public async Task Deve_Lancar_Excecao_Se_Inscricao_Nao_Encontrada()
        {
            _repositorioInscricaoMock.Setup(r => r.ObterPorId(It.IsAny<long>())).ReturnsAsync((SME.ConectaFormacao.Dominio.Entidades.Inscricao)null);
            var command = new TransferirInscricaoCommand(1, new InscricaoTransferenciaDTO());

            var act = async () => await _handler.Handle(command, CancellationToken.None);

            (await Assert.ThrowsAsync<NegocioException>(act)).StatusCode.Should().Be((int)HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task Deve_Lancar_Excecao_Para_Inscricao_Cancelada()
        {
            var inscricao = CriarInscricao();
            inscricao.Situacao = SituacaoInscricao.Cancelada;

            _repositorioInscricaoMock.Setup(r => r.ObterPorId(It.IsAny<long>())).ReturnsAsync(inscricao);

            var command = new TransferirInscricaoCommand(1, new InscricaoTransferenciaDTO());

            var act = async () => await _handler.Handle(command, CancellationToken.None);

            (await Assert.ThrowsAsync<NegocioException>(act)).StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task Deve_Lancar_Excecao_Para_Inscricao_Transferida()
        {
            var inscricao = CriarInscricao();
            inscricao.Situacao = SituacaoInscricao.Transferida;

            _repositorioInscricaoMock.Setup(r => r.ObterPorId(It.IsAny<long>())).ReturnsAsync(inscricao);

            var command = new TransferirInscricaoCommand(1, new InscricaoTransferenciaDTO());

            var act = async () => await _handler.Handle(command, CancellationToken.None);

            (await Assert.ThrowsAsync<NegocioException>(act)).StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task Deve_Lancar_Excecao_Para_Mesma_Turma()
        {
            var inscricao = CriarInscricao();

            _repositorioInscricaoMock.Setup(r => r.ObterPorId(It.IsAny<long>())).ReturnsAsync(inscricao);

            var command = new TransferirInscricaoCommand(1, new InscricaoTransferenciaDTO
            {
                IdTurmaOrigem = 1,
                IdTurmaDestino = 1,
                Cursistas = new List<string> { "123" }
            });

            var act = async () => await _handler.Handle(command, CancellationToken.None);

            (await Assert.ThrowsAsync<NegocioException>(act)).StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task Deve_Lancar_Excecao_Para_Lista_Cursistas_Vazia()
        {
            var inscricao = CriarInscricao();

            _repositorioInscricaoMock.Setup(r => r.ObterPorId(It.IsAny<long>())).ReturnsAsync(inscricao);

            var command = new TransferirInscricaoCommand(1, new InscricaoTransferenciaDTO
            {
                IdTurmaOrigem = 1,
                IdTurmaDestino = 2,
                Cursistas = new List<string>()
            });

            var act = async () => await _handler.Handle(command, CancellationToken.None);

            (await Assert.ThrowsAsync<NegocioException>(act)).StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task Deve_Lancar_Excecao_Se_Usuario_Logado_Nao_Encontrado()
        {
            var inscricao = CriarInscricao();

            _repositorioInscricaoMock.Setup(r => r.ObterPorId(It.IsAny<long>())).ReturnsAsync(inscricao);
            _mediatorMock.Setup(m => m.Send(It.IsAny<ObterUsuarioLogadoQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync((Usuario)null);

            var command = new TransferirInscricaoCommand(1, new InscricaoTransferenciaDTO
            {
                IdTurmaOrigem = 1,
                IdTurmaDestino = 2,
                Cursistas = new List<string> { "123" }
            });

            var act = async () => await _handler.Handle(command, CancellationToken.None);

            (await Assert.ThrowsAsync<NegocioException>(act)).Message.Should().Be(MensagemNegocio.USUARIO_NAO_ENCONTRADO);
        }

        private SME.ConectaFormacao.Dominio.Entidades.Inscricao CriarInscricao()
        {
            return new SME.ConectaFormacao.Dominio.Entidades.Inscricao
            {
                Id = 5,
                PropostaTurmaId = 1,
                UsuarioId = 999,
                Situacao = SituacaoInscricao.Confirmada,
                CargoCodigo = "COD1",
                CargoDreCodigo = "123456",
                CargoUeCodigo = "UE1",
                CargoId = 1,
                FuncaoCodigo = "FC1",
                FuncaoDreCodigo = "DF1",
                FuncaoUeCodigo = "UF1",
                FuncaoId = 2,
                TipoVinculo = 1
            };
        }
    }
}
