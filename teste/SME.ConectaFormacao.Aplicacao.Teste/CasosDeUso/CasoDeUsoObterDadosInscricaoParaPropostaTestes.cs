using System;
using System.Collections.Generic;
using System.Text;
using MediatR;
using Moq;
using SME.ConectaFormacao.Aplicacao.CasosDeUso.Inscricoes;
using SME.ConectaFormacao.Aplicacao.Dtos.Inscricoes;
using SME.ConectaFormacao.Aplicacao.Interfaces.Inscricoes;
using SME.ConectaFormacao.Dominio.Contexto;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Infra.Dados.Dtos;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao.Teste.CasosDeUso
{
    public class CasoDeUsoObterDadosInscricaoParaPropostaTestes
    {
        private readonly Mock<IRepositorioCargoFuncaoEol> _repositorioCargoFuncaoEolMock;
        private readonly Mock<IRepositorioProposta> _repositorioPropostaMock;
        private readonly Mock<IMediator> _mediatorMock;
        private readonly Mock<IContextoAplicacao> _contextoAplicacaoMock;
        private readonly CasoDeUsoObterDadosInscricaoParaProposta _casoDeUso;

        public CasoDeUsoObterDadosInscricaoParaPropostaTestes()
        {
            _repositorioCargoFuncaoEolMock = new Mock<IRepositorioCargoFuncaoEol>();
            _repositorioPropostaMock = new Mock<IRepositorioProposta>();
            _mediatorMock = new Mock<IMediator>();
            _contextoAplicacaoMock = new Mock<IContextoAplicacao>();
            _casoDeUso = new CasoDeUsoObterDadosInscricaoParaProposta(
                _repositorioCargoFuncaoEolMock.Object,
                _repositorioPropostaMock.Object,
                _mediatorMock.Object,
                _contextoAplicacaoMock.Object);
        }

        #region Testes - Cenário: Usuário Externo

        [Fact(DisplayName = "ExecutarAsync - Deve retornar dados básicos para usuário externo")]
        public async Task ExecutarAsync_Deve_Retornar_Dados_Basicos_Para_Usuario_Externo()
        {
            // Arrange
            const long propostaId = 1;
            var usuarioExterno = new Usuario
            {
                Id = 1,
                Nome = "João Silva",
                Cpf = "12345678901",
                Email = "joao@email.com",
                Login = "RF123456",
                Tipo = TipoUsuario.Externo
            };

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<ObterUsuarioLogadoQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(usuarioExterno);

            // Act
            var resultado = await _casoDeUso.ExecutarAsync(propostaId);

            // Assert
            Assert.NotNull(resultado);
            Assert.Equal("João Silva", resultado.UsuarioNome);
            Assert.Equal("RF123456", resultado.UsuarioRf);
            Assert.Equal("joao@email.com", resultado.UsuarioEmail);
            Assert.NotEmpty(resultado.UsuarioCpf!);
            Assert.Empty(resultado.UsuarioCargos);
            Assert.False(resultado.VagaRemanescente);
            _repositorioPropostaMock.Verify(r => r.ObterPorId(It.IsAny<long>()), Times.Never);
        }

        [Fact(DisplayName = "ExecutarAsync - Deve aplicar máscara ao CPF")]
        public async Task ExecutarAsync_Deve_Aplicar_Mascara_Ao_Cpf()
        {
            // Arrange
            const long propostaId = 1;
            var usuarioExterno = new Usuario
            {
                Id = 1,
                Nome = "Maria Santos",
                Cpf = "98765432109",
                Email = "maria@email.com",
                Login = "RF654321",
                Tipo = TipoUsuario.Externo
            };

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<ObterUsuarioLogadoQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(usuarioExterno);

            // Act
            var resultado = await _casoDeUso.ExecutarAsync(propostaId);

            // Assert
            Assert.NotNull(resultado.UsuarioCpf);
            Assert.Matches(@"^\d{3}\.\d{3}\.\d{3}\-\d{2}$", resultado.UsuarioCpf);
        }

        #endregion

        #region Testes - Cenário: Usuário Interno - Proposta não encontrada

        [Fact(DisplayName = "ExecutarAsync - Deve lançar exceção quando proposta não encontrada")]
        public async Task ExecutarAsync_Deve_Lancar_Excecao_Quando_Proposta_Nao_Encontrada()
        {
            // Arrange
            const long propostaId = 999;
            var usuarioInterno = new Usuario
            {
                Id = 2,
                Nome = "Pedro Oliveira",
                Cpf = "11122233344",
                Email = "pedro@email.com",
                Login = "RF111111",
                Tipo = TipoUsuario.Interno
            };

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<ObterUsuarioLogadoQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(usuarioInterno);

            _repositorioPropostaMock
                .Setup(r => r.ObterPorId(propostaId))
                .ReturnsAsync((Proposta?)null);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(async () => await _casoDeUso.ExecutarAsync(propostaId));
            _repositorioPropostaMock.Verify(r => r.ObterPorId(propostaId), Times.Once);
        }

        #endregion

        #region Testes - Cenário: Usuário Interno - Com cargo e função

        [Fact(DisplayName = "ExecutarAsync - Deve retornar dados com cargos e funções para usuário interno")]
        public async Task ExecutarAsync_Deve_Retornar_Dados_Com_Cargos_E_Funcoes()
        {
            // Arrange
            const long propostaId = 1;
            var usuarioInterno = new Usuario
            {
                Id = 3,
                Nome = "Ana Costa",
                Cpf = "55566677788",
                Email = "ana@email.com",
                Login = "RF222222",
                Tipo = TipoUsuario.Interno
            };

            var proposta = new Proposta { Id = propostaId };
            
            var cargosFuncoes = new List<CargoFuncaoEolDto>
            {
                new CargoFuncaoEolDto
                {
                    Codigo = 1,
                    Nome = "Professor",
                    CargoFuncaoId = 10,
                    TipoVinculo = 1,
                    DataPosse = new DateTime(2020, 1, 1),
                    CodigoDre = "DRE01",
                    CodigoUe = "UE001",
                    Funcoes = new List<FuncaoDoCargoEolDto>()
                }
            };

            var publicoAlvo = new List<PropostaPublicoAlvo>
            {
                new PropostaPublicoAlvo { CargoFuncaoId = 20 }
            };

            var vagasRemanescentes = new List<PropostaVagaRemanecente>
            {
                new PropostaVagaRemanecente { CargoFuncaoId = 10 }
            };

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<ObterUsuarioLogadoQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(usuarioInterno);

            _repositorioPropostaMock
                .Setup(r => r.ObterPorId(propostaId))
                .ReturnsAsync(proposta);

            _repositorioCargoFuncaoEolMock
                .Setup(r => r.ObterCargosFuncoesEolDoServidorAsync(usuarioInterno.Login))
                .ReturnsAsync(cargosFuncoes);

            _repositorioPropostaMock
                .Setup(r => r.ObterPublicoAlvoPorId(propostaId))
                .ReturnsAsync(publicoAlvo);

            _repositorioPropostaMock
                .Setup(r => r.ObterVagasRemacenentesPorId(propostaId))
                .ReturnsAsync(vagasRemanescentes);

            // Act
            var resultado = await _casoDeUso.ExecutarAsync(propostaId);

            // Assert
            Assert.NotNull(resultado);
            Assert.Equal("Ana Costa", resultado.UsuarioNome);
            Assert.Equal("RF222222", resultado.UsuarioRf);
            Assert.Single(resultado.UsuarioCargos);
            Assert.True(resultado.VagaRemanescente);
        }

        [Fact(DisplayName = "ExecutarAsync - Deve mapear cargo com funcões corretamente")]
        public async Task ExecutarAsync_Deve_Mapear_Cargo_Com_Funcoes_Corretamente()
        {
            // Arrange
            const long propostaId = 1;
            var usuarioInterno = new Usuario
            {
                Id = 4,
                Nome = "Carlos Mendes",
                Cpf = "99988877766",
                Email = "carlos@email.com",
                Login = "RF333333",
                Tipo = TipoUsuario.Interno
            };

            var proposta = new Proposta { Id = propostaId };

            var cargosFuncoes = new List<CargoFuncaoEolDto>
            {
                new CargoFuncaoEolDto
                {
                    Codigo = 5,
                    Nome = "Coordenador",
                    CargoFuncaoId = 30,
                    TipoVinculo = 2,
                    DataPosse = new DateTime(2021, 6, 15),
                    CodigoDre = "DRE02",
                    CodigoUe = "UE002",
                    Funcoes = new List<FuncaoDoCargoEolDto>
                    {
                        new FuncaoDoCargoEolDto
                        {
                            CodigoFuncao = 100,
                            CargoFuncaoId = 31,
                            Nome = "Função 1",
                            TipoVinculo = 1,
                            DataPosse = new DateTime(2021, 7, 1),
                            CodigoDre = "DRE02",
                            CodigoUe = "UE002"
                        }
                    }
                }
            };

            var publicoAlvo = new List<PropostaPublicoAlvo>();
            var vagasRemanescentes = new List<PropostaVagaRemanecente>();

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<ObterUsuarioLogadoQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(usuarioInterno);

            _repositorioPropostaMock
                .Setup(r => r.ObterPorId(propostaId))
                .ReturnsAsync(proposta);

            _repositorioCargoFuncaoEolMock
                .Setup(r => r.ObterCargosFuncoesEolDoServidorAsync(usuarioInterno.Login))
                .ReturnsAsync(cargosFuncoes);

            _repositorioPropostaMock
                .Setup(r => r.ObterPublicoAlvoPorId(propostaId))
                .ReturnsAsync(publicoAlvo);

            _repositorioPropostaMock
                .Setup(r => r.ObterVagasRemacenentesPorId(propostaId))
                .ReturnsAsync(vagasRemanescentes);

            // Act
            var resultado = await _casoDeUso.ExecutarAsync(propostaId);

            // Assert
            Assert.NotNull(resultado);
            Assert.Single(resultado.UsuarioCargos);
            var cargo = resultado.UsuarioCargos.First();
            Assert.Equal("5", cargo.Codigo);
            Assert.Equal("Coordenador", cargo.Descricao);
            Assert.Equal(2, cargo.TipoVinculo);
            Assert.Equal("DRE02", cargo.DreCodigo);
            Assert.Equal("UE002", cargo.UeCodigo);
            Assert.Single(cargo.Funcoes);
            
            var funcao = cargo.Funcoes.First();
            Assert.Equal("100", funcao.Codigo);
            Assert.Equal("Função 1", funcao.Descricao);
        }

        #endregion

        #region Testes - Cenário: Vaga Remanescente

        [Fact(DisplayName = "ExecutarAsync - Deve retornar false quando cargo está no público alvo")]
        public async Task ExecutarAsync_Deve_Retornar_False_Quando_Cargo_Esta_No_Publico_Alvo()
        {
            // Arrange
            const long propostaId = 1;
            var usuarioInterno = new Usuario
            {
                Id = 5,
                Nome = "Lucia Ferreira",
                Cpf = "44433322211",
                Email = "lucia@email.com",
                Login = "RF444444",
                Tipo = TipoUsuario.Interno
            };

            var proposta = new Proposta { Id = propostaId };

            var cargosFuncoes = new List<CargoFuncaoEolDto>
            {
                new CargoFuncaoEolDto
                {
                    Codigo = 2,
                    Nome = "Diretor",
                    CargoFuncaoId = 40,
                    TipoVinculo = 1,
                    DataPosse = new DateTime(2019, 1, 1),
                    CodigoDre = "DRE01",
                    CodigoUe = "UE001",
                    Funcoes = new List<FuncaoDoCargoEolDto>()
                }
            };

            var publicoAlvo = new List<PropostaPublicoAlvo>
            {
                new PropostaPublicoAlvo { CargoFuncaoId = 40 }
            };

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<ObterUsuarioLogadoQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(usuarioInterno);

            _repositorioPropostaMock
                .Setup(r => r.ObterPorId(propostaId))
                .ReturnsAsync(proposta);

            _repositorioCargoFuncaoEolMock
                .Setup(r => r.ObterCargosFuncoesEolDoServidorAsync(usuarioInterno.Login))
                .ReturnsAsync(cargosFuncoes);

            _repositorioPropostaMock
                .Setup(r => r.ObterPublicoAlvoPorId(propostaId))
                .ReturnsAsync(publicoAlvo);

            // Act
            var resultado = await _casoDeUso.ExecutarAsync(propostaId);

            // Assert
            Assert.False(resultado.VagaRemanescente);
            _repositorioPropostaMock.Verify(r => r.ObterVagasRemacenentesPorId(It.IsAny<long>()), Times.Never);
        }

        [Fact(DisplayName = "ExecutarAsync - Deve retornar true quando cargo está em vaga remanescente")]
        public async Task ExecutarAsync_Deve_Retornar_True_Quando_Cargo_Esta_Em_Vaga_Remanescente()
        {
            // Arrange
            const long propostaId = 1;
            var usuarioInterno = new Usuario
            {
                Id = 6,
                Nome = "Rafael Gomes",
                Cpf = "33344455566",
                Email = "rafael@email.com",
                Login = "RF555555",
                Tipo = TipoUsuario.Interno
            };

            var proposta = new Proposta { Id = propostaId };

            var cargosFuncoes = new List<CargoFuncaoEolDto>
            {
                new CargoFuncaoEolDto
                {
                    Codigo = 3,
                    Nome = "Assistente",
                    CargoFuncaoId = 50,
                    TipoVinculo = 1,
                    DataPosse = new DateTime(2022, 1, 1),
                    CodigoDre = "DRE03",
                    CodigoUe = "UE003",
                    Funcoes = new List<FuncaoDoCargoEolDto>()
                }
            };

            var publicoAlvo = new List<PropostaPublicoAlvo>();

            var vagasRemanescentes = new List<PropostaVagaRemanecente>
            {
                new PropostaVagaRemanecente { CargoFuncaoId = 50 }
            };

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<ObterUsuarioLogadoQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(usuarioInterno);

            _repositorioPropostaMock
                .Setup(r => r.ObterPorId(propostaId))
                .ReturnsAsync(proposta);

            _repositorioCargoFuncaoEolMock
                .Setup(r => r.ObterCargosFuncoesEolDoServidorAsync(usuarioInterno.Login))
                .ReturnsAsync(cargosFuncoes);

            _repositorioPropostaMock
                .Setup(r => r.ObterPublicoAlvoPorId(propostaId))
                .ReturnsAsync(publicoAlvo);

            _repositorioPropostaMock
                .Setup(r => r.ObterVagasRemacenentesPorId(propostaId))
                .ReturnsAsync(vagasRemanescentes);

            // Act
            var resultado = await _casoDeUso.ExecutarAsync(propostaId);

            // Assert
            Assert.True(resultado.VagaRemanescente);
        }

        [Fact(DisplayName = "ExecutarAsync - Deve retornar false quando cargo não está em vaga remanescente")]
        public async Task ExecutarAsync_Deve_Retornar_False_Quando_Cargo_Nao_Esta_Em_Vaga_Remanescente()
        {
            // Arrange
            const long propostaId = 1;
            var usuarioInterno = new Usuario
            {
                Id = 7,
                Nome = "Beatriz Lima",
                Cpf = "22211100099",
                Email = "beatriz@email.com",
                Login = "RF666666",
                Tipo = TipoUsuario.Interno
            };

            var proposta = new Proposta { Id = propostaId };

            var cargosFuncoes = new List<CargoFuncaoEolDto>
            {
                new CargoFuncaoEolDto
                {
                    Codigo = 4,
                    Nome = "Técnico",
                    CargoFuncaoId = 60,
                    TipoVinculo = 1,
                    DataPosse = new DateTime(2023, 1, 1),
                    CodigoDre = "DRE04",
                    CodigoUe = "UE004",
                    Funcoes = new List<FuncaoDoCargoEolDto>()
                }
            };

            var publicoAlvo = new List<PropostaPublicoAlvo>();

            var vagasRemanescentes = new List<PropostaVagaRemanecente>
            {
                new PropostaVagaRemanecente { CargoFuncaoId = 70 }
            };

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<ObterUsuarioLogadoQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(usuarioInterno);

            _repositorioPropostaMock
                .Setup(r => r.ObterPorId(propostaId))
                .ReturnsAsync(proposta);

            _repositorioCargoFuncaoEolMock
                .Setup(r => r.ObterCargosFuncoesEolDoServidorAsync(usuarioInterno.Login))
                .ReturnsAsync(cargosFuncoes);

            _repositorioPropostaMock
                .Setup(r => r.ObterPublicoAlvoPorId(propostaId))
                .ReturnsAsync(publicoAlvo);

            _repositorioPropostaMock
                .Setup(r => r.ObterVagasRemacenentesPorId(propostaId))
                .ReturnsAsync(vagasRemanescentes);

            // Act
            var resultado = await _casoDeUso.ExecutarAsync(propostaId);

            // Assert
            Assert.False(resultado.VagaRemanescente);
        }

        #endregion

        #region Testes - Cenário: Usuário nulo do Mediator

        [Fact(DisplayName = "ExecutarAsync - Deve usar contexto quando usuário do mediator é nulo")]
        public async Task ExecutarAsync_Deve_Usar_Contexto_Quando_Usuario_Mediator_Nulo()
        {
            // Arrange
            const long propostaId = 1;

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<ObterUsuarioLogadoQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Usuario?)null);

            _contextoAplicacaoMock
                .Setup(c => c.UsuarioLogado)
                .Returns("RF777777");

            _contextoAplicacaoMock
                .Setup(c => c.NomeUsuario)
                .Returns("Usuário do Contexto");

            // Act
            var resultado = await _casoDeUso.ExecutarAsync(propostaId);

            // Assert
            Assert.NotNull(resultado);
            Assert.Equal("Usuário do Contexto", resultado.UsuarioNome);
            Assert.Equal("RF777777", resultado.UsuarioRf);
            Assert.Empty(resultado.UsuarioCargos);
            Assert.False(resultado.VagaRemanescente);
            _repositorioPropostaMock.Verify(r => r.ObterPorId(It.IsAny<long>()), Times.Never);
        }

        #endregion

        #region Testes - Cenário: Múltiplos cargos

        [Fact(DisplayName = "ExecutarAsync - Deve retornar múltiplos cargos corretamente")]
        public async Task ExecutarAsync_Deve_Retornar_Multiplos_Cargos_Corretamente()
        {
            // Arrange
            const long propostaId = 1;
            var usuarioInterno = new Usuario
            {
                Id = 8,
                Nome = "Fernanda Rocha",
                Cpf = "11133355577",
                Email = "fernanda@email.com",
                Login = "RF888888",
                Tipo = TipoUsuario.Interno
            };

            var proposta = new Proposta { Id = propostaId };

            var cargosFuncoes = new List<CargoFuncaoEolDto>
            {
                new CargoFuncaoEolDto
                {
                    Codigo = 10,
                    Nome = "Professor Titular",
                    CargoFuncaoId = 100,
                    TipoVinculo = 1,
                    DataPosse = new DateTime(2015, 1, 1),
                    CodigoDre = "DRE05",
                    CodigoUe = "UE005",
                    Funcoes = new List<FuncaoDoCargoEolDto>()
                },
                new CargoFuncaoEolDto
                {
                    Codigo = 11,
                    Nome = "Professor Adjunto",
                    CargoFuncaoId = 101,
                    TipoVinculo = 2,
                    DataPosse = new DateTime(2018, 1, 1),
                    CodigoDre = "DRE06",
                    CodigoUe = "UE006",
                    Funcoes = new List<FuncaoDoCargoEolDto>()
                }
            };

            var publicoAlvo = new List<PropostaPublicoAlvo>();
            var vagasRemanescentes = new List<PropostaVagaRemanecente>();

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<ObterUsuarioLogadoQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(usuarioInterno);

            _repositorioPropostaMock
                .Setup(r => r.ObterPorId(propostaId))
                .ReturnsAsync(proposta);

            _repositorioCargoFuncaoEolMock
                .Setup(r => r.ObterCargosFuncoesEolDoServidorAsync(usuarioInterno.Login))
                .ReturnsAsync(cargosFuncoes);

            _repositorioPropostaMock
                .Setup(r => r.ObterPublicoAlvoPorId(propostaId))
                .ReturnsAsync(publicoAlvo);

            _repositorioPropostaMock
                .Setup(r => r.ObterVagasRemacenentesPorId(propostaId))
                .ReturnsAsync(vagasRemanescentes);

            // Act
            var resultado = await _casoDeUso.ExecutarAsync(propostaId);

            // Assert
            Assert.NotNull(resultado);
            Assert.Equal(2, resultado.UsuarioCargos.Count());
        }

        #endregion

        #region Testes - Cenário: Cargos com nome nulo

        [Fact(DisplayName = "ExecutarAsync - Deve mapear cargo com nome nulo como string vazia")]
        public async Task ExecutarAsync_Deve_Mapear_Cargo_Com_Nome_Nulo_Como_String_Vazia()
        {
            // Arrange
            const long propostaId = 1;
            var usuarioInterno = new Usuario
            {
                Id = 9,
                Nome = "Gabriel Torres",
                Cpf = "99977755533",
                Email = "gabriel@email.com",
                Login = "RF999999",
                Tipo = TipoUsuario.Interno
            };

            var proposta = new Proposta { Id = propostaId };

            var cargosFuncoes = new List<CargoFuncaoEolDto>
            {
                new CargoFuncaoEolDto
                {
                    Codigo = 20,
                    Nome = null,
                    CargoFuncaoId = 200,
                    TipoVinculo = 1,
                    DataPosse = new DateTime(2020, 1, 1),
                    CodigoDre = "DRE07",
                    CodigoUe = "UE007",
                    Funcoes = new List<FuncaoDoCargoEolDto>
                    {
                        new FuncaoDoCargoEolDto
                        {
                            CodigoFuncao = 201,
                            CargoFuncaoId = 201,
                            Nome = null,
                            TipoVinculo = 1,
                            DataPosse = new DateTime(2020, 1, 1),
                            CodigoDre = "DRE07",
                            CodigoUe = "UE007"
                        }
                    }
                }
            };

            var publicoAlvo = new List<PropostaPublicoAlvo>();
            var vagasRemanescentes = new List<PropostaVagaRemanecente>();

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<ObterUsuarioLogadoQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(usuarioInterno);

            _repositorioPropostaMock
                .Setup(r => r.ObterPorId(propostaId))
                .ReturnsAsync(proposta);

            _repositorioCargoFuncaoEolMock
                .Setup(r => r.ObterCargosFuncoesEolDoServidorAsync(usuarioInterno.Login))
                .ReturnsAsync(cargosFuncoes);

            _repositorioPropostaMock
                .Setup(r => r.ObterPublicoAlvoPorId(propostaId))
                .ReturnsAsync(publicoAlvo);

            _repositorioPropostaMock
                .Setup(r => r.ObterVagasRemacenentesPorId(propostaId))
                .ReturnsAsync(vagasRemanescentes);

            // Act
            var resultado = await _casoDeUso.ExecutarAsync(propostaId);

            // Assert
            Assert.NotNull(resultado);
            var cargo = resultado.UsuarioCargos.First();
            Assert.Equal(string.Empty, cargo.Descricao);
            Assert.Equal(string.Empty, cargo.Funcoes.First().Descricao);
        }

        #endregion

        #region Testes - Cenário: Cargos sem funções

        [Fact(DisplayName = "ExecutarAsync - Deve mapear cargos sem funções")]
        public async Task ExecutarAsync_Deve_Mapear_Cargos_Sem_Funcoes()
        {
            // Arrange
            const long propostaId = 1;
            var usuarioInterno = new Usuario
            {
                Id = 10,
                Nome = "Isabela Martins",
                Cpf = "88866644422",
                Email = "isabela@email.com",
                Login = "RF101010",
                Tipo = TipoUsuario.Interno
            };

            var proposta = new Proposta { Id = propostaId };

            var cargosFuncoes = new List<CargoFuncaoEolDto>
            {
                new CargoFuncaoEolDto
                {
                    Codigo = 30,
                    Nome = "Secretário",
                    CargoFuncaoId = 300,
                    TipoVinculo = 1,
                    DataPosse = new DateTime(2021, 1, 1),
                    CodigoDre = "DRE08",
                    CodigoUe = "UE008",
                    Funcoes = new List<FuncaoDoCargoEolDto>()
                }
            };

            var publicoAlvo = new List<PropostaPublicoAlvo>();
            var vagasRemanescentes = new List<PropostaVagaRemanecente>();

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<ObterUsuarioLogadoQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(usuarioInterno);

            _repositorioPropostaMock
                .Setup(r => r.ObterPorId(propostaId))
                .ReturnsAsync(proposta);

            _repositorioCargoFuncaoEolMock
                .Setup(r => r.ObterCargosFuncoesEolDoServidorAsync(usuarioInterno.Login))
                .ReturnsAsync(cargosFuncoes);

            _repositorioPropostaMock
                .Setup(r => r.ObterPublicoAlvoPorId(propostaId))
                .ReturnsAsync(publicoAlvo);

            _repositorioPropostaMock
                .Setup(r => r.ObterVagasRemacenentesPorId(propostaId))
                .ReturnsAsync(vagasRemanescentes);

            // Act
            var resultado = await _casoDeUso.ExecutarAsync(propostaId);

            // Assert
            Assert.NotNull(resultado);
            var cargo = resultado.UsuarioCargos.First();
            Assert.Empty(cargo.Funcoes);
        }

        #endregion

        #region Testes - Cenário: ObterUsuarioDoContexto

        [Fact(DisplayName = "ObterUsuarioDoContexto - Deve retornar usuário com dados do contexto")]
        public void ObterUsuarioDoContexto_Deve_Retornar_Usuario_Com_Dados_Do_Contexto()
        {
            // Arrange
            _contextoAplicacaoMock
                .Setup(c => c.UsuarioLogado)
                .Returns("RF202020");

            _contextoAplicacaoMock
                .Setup(c => c.NomeUsuario)
                .Returns("Usuário Teste Contexto");

            // Act
            var usuario = _casoDeUso.ObterUsuarioDoContexto();

            // Assert
            Assert.NotNull(usuario);
            Assert.Equal("RF202020", usuario.Login);
            Assert.Equal("Usuário Teste Contexto", usuario.Nome);
            Assert.Equal(0, usuario.Id);
        }

        #endregion

        #region Testes - Cenário: Herança e Interface

        [Fact(DisplayName = "CasoDeUsoObterDadosInscricaoParaProposta - Deve herdar de CasoDeUsoAbstrato")]
        public void CasoDeUso_Deve_Herdar_De_CasoDeUsoAbstrato()
        {
            // Assert
            Assert.True(
                typeof(CasoDeUsoObterDadosInscricaoParaProposta)
                    .BaseType?.Name.Contains("CasoDeUsoAbstrato") ?? false,
                "Deve herdar de CasoDeUsoAbstrato");
        }

        [Fact(DisplayName = "CasoDeUsoObterDadosInscricaoParaProposta - Deve implementar ICasoDeUsoObterDadosInscricaoParaProposta")]
        public void CasoDeUso_Deve_Implementar_Interface()
        {
            // Assert
            Assert.True(
                typeof(CasoDeUsoObterDadosInscricaoParaProposta)
                    .GetInterfaces()
                    .Any(i => i.Name == "ICasoDeUsoObterDadosInscricaoParaProposta"),
                "Deve implementar ICasoDeUsoObterDadosInscricaoParaProposta");
        }

        #endregion

        #region Testes - Cenário: Tipo de usuário nulo

        [Fact(DisplayName = "ExecutarAsync - Deve validar tipo de usuário antes de acessar dados")]
        public async Task ExecutarAsync_Deve_Validar_Tipo_Usuario_Antes_De_Acessar_Dados()
        {
            // Arrange
            const long propostaId = 1;
            var usuarioInterno = new Usuario
            {
                Id = 11,
                Nome = "Jaqueline Silva",
                Cpf = "77755533311",
                Email = "jaqueline@email.com",
                Login = "RF111111",
                Tipo = TipoUsuario.Interno
            };

            var proposta = new Proposta { Id = propostaId };

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<ObterUsuarioLogadoQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(usuarioInterno);

            _repositorioPropostaMock
                .Setup(r => r.ObterPorId(propostaId))
                .ReturnsAsync(proposta);

            _repositorioCargoFuncaoEolMock
                .Setup(r => r.ObterCargosFuncoesEolDoServidorAsync(usuarioInterno.Login))
                .ReturnsAsync(new List<CargoFuncaoEolDto>());

            _repositorioPropostaMock
                .Setup(r => r.ObterPublicoAlvoPorId(propostaId))
                .ReturnsAsync(new List<PropostaPublicoAlvo>());

            _repositorioPropostaMock
                .Setup(r => r.ObterVagasRemacenentesPorId(propostaId))
                .ReturnsAsync(new List<PropostaVagaRemanecente>());

            // Act
            var resultado = await _casoDeUso.ExecutarAsync(propostaId);

            // Assert
            Assert.NotNull(resultado);
            _repositorioPropostaMock.Verify(r => r.ObterPorId(propostaId), Times.Once);
        }

        #endregion
    }
}
