using MediatR;
using Moq;
using SME.ConectaFormacao.Aplicacao.Dtos.Inscricao;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Dominio.Excecoes;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;
using SME.ConectaFormacao.Infra.Servicos.Eol;
using System.Net;
using System.Reflection;

namespace SME.ConectaFormacao.Aplicacao.Teste.Commands.Inscricao
{
    public class TransferirInscricaoCommandHandlerTeste
    {
        private readonly Mock<IRepositorioInscricao> _repoInscricao;
        private readonly Mock<IRepositorioUsuario> _repoUsuario;
        private readonly Mock<IRepositorioProposta> _repoProposta;
        private readonly Mock<IMediator> _mediator;
        private readonly Mock<IRepositorioCargoFuncao> _repoCargoFuncao;

        public TransferirInscricaoCommandHandlerTeste()
        {
            _repoInscricao = new Mock<IRepositorioInscricao>();
            _repoUsuario = new Mock<IRepositorioUsuario>();
            _repoProposta = new Mock<IRepositorioProposta>();
            _mediator = new Mock<IMediator>();
            _repoCargoFuncao = new Mock<IRepositorioCargoFuncao>();
        }

        private TransferirInscricaoCommandHandler CriarHandler()
        {
            return new TransferirInscricaoCommandHandler(
                _repoInscricao.Object,
                _repoCargoFuncao.Object,
                _mediator.Object,
                _repoProposta.Object,
                _repoUsuario.Object
            );
        }

        [Fact]
        public void Construtor_Deve_Lancar_ArgumentNullException_Se_Depencia_Nula()
        {
            Assert.Throws<ArgumentNullException>(() => new TransferirInscricaoCommandHandler(null, _repoCargoFuncao.Object, _mediator.Object, _repoProposta.Object, _repoUsuario.Object));
            Assert.Throws<ArgumentNullException>(() => new TransferirInscricaoCommandHandler(_repoInscricao.Object, _repoCargoFuncao.Object, null, _repoProposta.Object, _repoUsuario.Object));
            Assert.Throws<ArgumentNullException>(() => new TransferirInscricaoCommandHandler(_repoInscricao.Object, _repoCargoFuncao.Object, _mediator.Object, null, _repoUsuario.Object));
            Assert.Throws<ArgumentNullException>(() => new TransferirInscricaoCommandHandler(_repoInscricao.Object, _repoCargoFuncao.Object, _mediator.Object, _repoProposta.Object, null));
        }

        [Fact(DisplayName = "Handle deve retornar sucesso")]
        public async Task Handle_Deve_Retornar_Sucesso()
        {
            var inscricao = new SME.ConectaFormacao.Dominio.Entidades.Inscricao
            {
                Id = 1,
                Situacao = SituacaoInscricao.Confirmada,
                CargoCodigo = "1",
                CargoDreCodigo = "DRE1"
            };

            var usuario = new Usuario { Nome = "João", Login = "123", Cpf = "999" };

            var propostaTurma = new PropostaTurma
            {
                PropostaId = 99,
                Dres = new List<PropostaTurmaDre> { new PropostaTurmaDre { DreId = 1 } }
            };

            var proposta = new Proposta { Id = 99 };

            _repoInscricao.Setup(r => r.ObterPorId(It.IsAny<long>())).ReturnsAsync(inscricao);
            _repoUsuario.Setup(r => r.ObterPorLogin(It.IsAny<string>())).ReturnsAsync(usuario);

            _mediator.Setup(m => m.Send(It.IsAny<ObterCargosFuncoesDresFuncionarioServicoEolQuery>(), default))
                     .ReturnsAsync(new List<CursistaCargoServicoEol>
                     {
                 new CursistaCargoServicoEol
                 {
                     CdCargoBase = 1,
                     CdFuncaoAtividade = 111,
                     CdUeCargoBase = "U1",
                     RF = 123,
                     Cpf = "999"
                 }
                     });

            _repoProposta.Setup(r => r.ObterTurmaDaPropostaComDresPorId(It.IsAny<long>()))
                         .ReturnsAsync(propostaTurma);

            _repoProposta.Setup(r => r.ObterPropostasResumidasPorId(It.IsAny<long[]>()))
                         .ReturnsAsync(new[] { proposta });

            var handler = CriarHandler();

            var dto = new InscricaoTransferenciaDTO
            {
                IdTurmaOrigem = 1,
                IdTurmaDestino = 2,
                Cursistas = new List<InscricaoTransferenciaDTOCursista>
                {
                    new InscricaoTransferenciaDTOCursista { IdInscricao = 1, Rf = "123" }
                }
            };

            var comando = new TransferirInscricaoCommand(dto);

            var resultado = await handler.Handle(comando, default);

            Assert.Equal((int)HttpStatusCode.OK, resultado.Status);
            Assert.Contains("sucesso", resultado.Mensagem, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public async Task Handle_Deve_Retornar_Falha_Se_Usuario_Nao_Encontrado()
        {
            var inscricao = new SME.ConectaFormacao.Dominio.Entidades.Inscricao { Id = 1, Situacao = SituacaoInscricao.Confirmada };
            _repoInscricao.Setup(r => r.ObterPorId(It.IsAny<long>())).ReturnsAsync(inscricao);
            _repoUsuario.Setup(r => r.ObterPorLogin(It.IsAny<string>())).ReturnsAsync((Usuario)null);

            var handler = CriarHandler();

            var dto = new InscricaoTransferenciaDTO
            {
                IdTurmaOrigem = 1,
                IdTurmaDestino = 2,
                Cursistas = new List<InscricaoTransferenciaDTOCursista>
            {
                new InscricaoTransferenciaDTOCursista { IdInscricao = 1, Rf = "123" }
            }
            };

            var comando = new TransferirInscricaoCommand(dto);
            var resultado = await handler.Handle(comando, default);

            Assert.Equal((int)HttpStatusCode.BadRequest, resultado.Status);
            Assert.NotEmpty(resultado.Cursistas);
        }

        [Fact(DisplayName = "Handle deve buscar CargoCodigo em ObterCargoFuncaoOutrosQuery")]
        public async Task Handle_Deve_Buscar_CargoCodigo_Em_ObterCargoFuncaoOutrosQuery()
        {
            var inscricao = new SME.ConectaFormacao.Dominio.Entidades.Inscricao
            {
                Id = 1,
                Situacao = SituacaoInscricao.Confirmada,
                CargoCodigo = "1",
                CargoDreCodigo = "DRE1"
            };

            var usuario = new Usuario { Nome = "João", Login = "123", Cpf = "999" };
            var propostaTurma = new PropostaTurma
            {
                PropostaId = 99,
                Dres = new List<PropostaTurmaDre> { new PropostaTurmaDre { DreId = 1 } }
            };
            var proposta = new Proposta { Id = 99 };

            _repoInscricao.Setup(r => r.ObterPorId(It.IsAny<long>())).ReturnsAsync(inscricao);
            _repoUsuario.Setup(r => r.ObterPorLogin(It.IsAny<string>())).ReturnsAsync(usuario);

            _mediator.Setup(m => m.Send(It.IsAny<ObterCargosFuncoesDresFuncionarioServicoEolQuery>(), default))
                     .ReturnsAsync(new List<CursistaCargoServicoEol>
                     {
                 new CursistaCargoServicoEol
                 {
                     CdCargoBase = null,
                     CdFuncaoAtividade = 1,
                     CdUeCargoBase = "U1",
                     RF = 123,
                     Cpf = "999"
                 }
                     });

            _mediator.Setup(m => m.Send(It.IsAny<ObterCargoFuncaoOutrosQuery>(), default))
                     .ReturnsAsync(new CargoFuncao
                     {
                         Id = 777,
                         Nome = "Cargo Outros",
                         Tipo = CargoFuncaoTipo.Outros,
                         Ordem = 1
                     });

            _repoProposta.Setup(r => r.ObterTurmaDaPropostaComDresPorId(It.IsAny<long>()))
                         .ReturnsAsync(propostaTurma);

            _repoProposta.Setup(r => r.ObterPropostasResumidasPorId(It.IsAny<long[]>()))
                         .ReturnsAsync(new[] { proposta });

            var handler = CriarHandler();

            var dto = new InscricaoTransferenciaDTO
            {
                IdTurmaOrigem = 1,
                IdTurmaDestino = 2,
                Cursistas = new List<InscricaoTransferenciaDTOCursista>
                {
                    new InscricaoTransferenciaDTOCursista { IdInscricao = 1, Rf = "123" }
                }
            };

            var comando = new TransferirInscricaoCommand(dto);

            var resultado = await handler.Handle(comando, default);

            Assert.Equal((int)HttpStatusCode.OK, resultado.Status);
            Assert.Equal("Todas as inscrições foram transferidas com sucesso.", resultado.Mensagem);
        }

        [Fact(DisplayName = "Handle deve retornar erro se proposta for nula")]
        public async Task Handle_Deve_Retornar_Erro_Se_Proposta_Nula()
        {
            var inscricao = new SME.ConectaFormacao.Dominio.Entidades.Inscricao
            {
                Id = 1,
                Situacao = SituacaoInscricao.Confirmada,
                CargoCodigo = "1",
                CargoDreCodigo = "DRE1"
            };

            var usuario = new Usuario { Nome = "João", Login = "123", Cpf = "999" };

            var propostaTurma = new PropostaTurma
            {
                PropostaId = 99,
                Dres = new List<PropostaTurmaDre> { new PropostaTurmaDre { DreId = 1 } }
            };

            _repoInscricao.Setup(r => r.ObterPorId(It.IsAny<long>())).ReturnsAsync(inscricao);
            _repoUsuario.Setup(r => r.ObterPorLogin(It.IsAny<string>())).ReturnsAsync(usuario);

            _mediator.Setup(m => m.Send(It.IsAny<ObterCargosFuncoesDresFuncionarioServicoEolQuery>(), default))
                     .ReturnsAsync(new List<CursistaCargoServicoEol>
                     {
                 new CursistaCargoServicoEol
                 {
                     CdCargoBase = 1,
                     CdFuncaoAtividade = 1,
                     CdUeCargoBase = "U1",
                     RF = 123,
                     Cpf = "999"
                 }
                     });

            _mediator.Setup(m => m.Send(It.IsAny<ObterCargoFuncaoOutrosQuery>(), default))
                     .ReturnsAsync(new CargoFuncao { Id = 99 });

            _repoProposta.Setup(r => r.ObterTurmaDaPropostaComDresPorId(It.IsAny<long>())).ReturnsAsync(propostaTurma);
            _repoProposta.Setup(r => r.ObterPropostasResumidasPorId(It.IsAny<long[]>())).ReturnsAsync(Array.Empty<Proposta>());

            var handler = CriarHandler();

            var dto = new InscricaoTransferenciaDTO
            {
                IdTurmaOrigem = 1,
                IdTurmaDestino = 2,
                Cursistas = new List<InscricaoTransferenciaDTOCursista>
                {
                    new InscricaoTransferenciaDTOCursista { IdInscricao = 1, Rf = "123" }
                }
            };

            var comando = new TransferirInscricaoCommand(dto);

            var resultado = await handler.Handle(comando, default);

            Assert.Equal((int)HttpStatusCode.BadRequest, resultado.Status);
            Assert.Contains(resultado.Cursistas, c => c.Mensagem.Contains(MensagemNegocio.PROPOSTA_NAO_ENCONTRADA));
        }

        [Fact]
        public async Task Handle_Deve_Lancar_Se_Mesma_Turma()
        {
            var handler = CriarHandler();

            var dto = new InscricaoTransferenciaDTO
            {
                IdTurmaOrigem = 1,
                IdTurmaDestino = 1,
                Cursistas = new List<InscricaoTransferenciaDTOCursista> { new InscricaoTransferenciaDTOCursista() }
            };

            var comando = new TransferirInscricaoCommand(dto);

            await Assert.ThrowsAsync<NegocioException>(() => handler.Handle(comando, default));
        }

        [Fact]
        public async Task Handle_Deve_Lancar_Se_Cursistas_Vazio()
        {
            var handler = CriarHandler();

            var dto = new InscricaoTransferenciaDTO
            {
                IdTurmaOrigem = 1,
                IdTurmaDestino = 2,
                Cursistas = new List<InscricaoTransferenciaDTOCursista>()
            };

            var comando = new TransferirInscricaoCommand(dto);

            await Assert.ThrowsAsync<NegocioException>(() => handler.Handle(comando, default));
        }

        [Fact]
        public void Validar_Inscricao_Deve_Lancar_Se_Null_Ou_Cancelada()
        {
            var metodo = typeof(TransferirInscricaoCommandHandler).GetMethod("ValidarInscricao", BindingFlags.NonPublic | BindingFlags.Static);

            Assert.Throws<TargetInvocationException>(() => metodo.Invoke(null, new object[] { null }));
            Assert.Throws<TargetInvocationException>(() => metodo.Invoke(null, new object[] { new SME.ConectaFormacao.Dominio.Entidades.Inscricao { Situacao = SituacaoInscricao.Cancelada } }));
        }

        [Fact]
        public void Validar_Cargo_Transferencia_Deve_Lancar_Se_CargoOrigem_Nulo()
        {
            var metodo = typeof(TransferirInscricaoCommandHandler)
                .GetMethod("ValidarCargoTransferencia", BindingFlags.NonPublic | BindingFlags.Static);

            var ex = Assert.Throws<TargetInvocationException>(() =>
                metodo.Invoke(null, new object[] { null, "2", "3" })
            );

            var negocio = Assert.IsType<NegocioException>(ex.InnerException);
            Assert.Equal(MensagemNegocio.ERRO_OBTER_CARGOS_FUNCIONARIO_EOL, negocio.Message);
            Assert.Equal((int)HttpStatusCode.NotFound, negocio.StatusCode);
        }

        [Fact]
        public void Validar_Cargo_Transferencia_Deve_Lancar_Excecao_Se_CargoBase_Vazio()
        {
            var metodo = typeof(TransferirInscricaoCommandHandler)
                .GetMethod("ValidarCargoTransferencia", BindingFlags.NonPublic | BindingFlags.Static);

            var ex = Assert.Throws<TargetInvocationException>(() =>
                metodo.Invoke(null, new object[] { "1", "", "3" })
            );

            var negocio = Assert.IsType<NegocioException>(ex.InnerException);
            Assert.Equal(MensagemNegocio.ERRO_OBTER_CARGOS_FUNCIONARIO_EOL, negocio.Message);
            Assert.Equal((int)HttpStatusCode.NotFound, negocio.StatusCode);
        }

        [Fact]
        public void Validar_Cargo_Transferencia_Deve_Lancar_Excecao_Se_Cargos_Diferentes()
        {
            var metodo = typeof(TransferirInscricaoCommandHandler)
                .GetMethod("ValidarCargoTransferencia", BindingFlags.NonPublic | BindingFlags.Static);

            var ex = Assert.Throws<TargetInvocationException>(() =>
                metodo.Invoke(null, new object[] { "1", "2", null })
            );

            var negocio = Assert.IsType<NegocioException>(ex.InnerException);
            Assert.Equal(MensagemNegocio.INSCRICAO_TRANSFERENCIA_CARGOS_DIFERENTES, negocio.Message);
            Assert.Equal((int)HttpStatusCode.BadRequest, negocio.StatusCode);
        }

    }
}
