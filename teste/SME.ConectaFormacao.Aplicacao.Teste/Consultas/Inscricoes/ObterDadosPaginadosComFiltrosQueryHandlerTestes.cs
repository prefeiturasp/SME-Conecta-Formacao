using AutoMapper;
using FluentAssertions;
using Moq;
using Moq.AutoMock;
using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Aplicacao.Dtos.Inscricoes;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Infra;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao.Teste.Consultas.Inscricoes
{
    public class ObterDadosPaginadosComFiltrosQueryHandlerTestes
    {
        private readonly Mock<IRepositorioInscricao> _repositorioInscricao;
        private readonly Mock<IMapper> _mapper;
        private readonly ObterDadosPaginadosComFiltrosQueryHandler _sut;

        public ObterDadosPaginadosComFiltrosQueryHandlerTestes()
        {
            var mocker = new AutoMocker();

            _repositorioInscricao = mocker.GetMock<IRepositorioInscricao>();
            _mapper = mocker.GetMock<IMapper>();

            _sut = mocker.CreateInstance<ObterDadosPaginadosComFiltrosQueryHandler>();
        }

        #region Testes de Validação de Dependências

        [Fact(DisplayName = "Construtor - Deve lançar ArgumentNullException quando repositório é nulo")]
        public void Construtor_Deve_Lancar_ArgumentNullException_Quando_Repositorio_Nulo()
        {
            // Act & Assert
            var excecao = Assert.Throws<ArgumentNullException>(
                () => new ObterDadosPaginadosComFiltrosQueryHandler(null!, _mapper.Object));

            Assert.Equal("repositorioInscricao", excecao.ParamName);
        }

        [Fact(DisplayName = "Construtor - Deve lançar ArgumentNullException quando mapper é nulo")]
        public void Construtor_Deve_Lancar_ArgumentNullException_Quando_Mapper_Nulo()
        {
            // Act & Assert
            var excecao = Assert.Throws<ArgumentNullException>(
                () => new ObterDadosPaginadosComFiltrosQueryHandler(_repositorioInscricao.Object, null!));

            Assert.Equal("mapper", excecao.ParamName);
        }

        #endregion

        #region Testes - Sem Registros

        [Fact(DisplayName = "Handle - Deve retornar resultado vazio quando não há registros")]
        public async Task Handle_Deve_Retornar_Resultado_Vazio_Quando_Sem_Registros()
        {
            // Arrange
            var query = CriarQuery();

            _repositorioInscricao
                .Setup(r => r.ObterDadosPaginadosComFiltrosTotalRegistros(
                    It.IsAny<long?>(),
                    It.IsAny<long?>(),
                    It.IsAny<string?>(),
                    It.IsAny<long?>()))
                .ReturnsAsync(0);

            // Act
            var resultado = await _sut.Handle(query, CancellationToken.None);

            // Assert
            resultado.Should().NotBeNull();
            resultado.Items.Should().BeEmpty();
            resultado.TotalRegistros.Should().Be(0);
            resultado.TotalPaginas.Should().Be(0);
        }

        [Fact(DisplayName = "Handle - Não deve buscar dados quando total de registros é zero")]
        public async Task Handle_Nao_Deve_Buscar_Dados_Quando_Total_Zero()
        {
            // Arrange
            var query = CriarQuery();

            _repositorioInscricao
                .Setup(r => r.ObterDadosPaginadosComFiltrosTotalRegistros(
                    It.IsAny<long?>(),
                    It.IsAny<long?>(),
                    It.IsAny<string?>(),
                    It.IsAny<long?>()))
                .ReturnsAsync(0);

            // Act
            await _sut.Handle(query, CancellationToken.None);

            // Assert
            _repositorioInscricao.Verify(
                r => r.ObterDadosPaginadosComFiltros(
                    It.IsAny<long?>(),
                    It.IsAny<long?>(),
                    It.IsAny<string?>(),
                    It.IsAny<int>(),
                    It.IsAny<int>(),
                    It.IsAny<long?>()),
                Times.Never);
        }

        #endregion

        #region Testes - Com Registros

        [Fact(DisplayName = "Handle - Deve retornar dados paginados quando há registros")]
        public async Task Handle_Deve_Retornar_Dados_Paginados_Quando_Ha_Registros()
        {
            // Arrange
            var query = CriarQuery(numeroPagina: 1, numeroRegistros: 10);
            var propostasOriginais = new List<Proposta>
            {
                CriarFormacaoDTO(1, "Formação 1"),
                CriarFormacaoDTO(2, "Formação 2")
            };
            var propostasDto = new List<DadosListagemFormacaoComTurmaDTO>
            {
                CriarDadosListagemFormacao(1, "Formação 1"),
                CriarDadosListagemFormacao(2, "Formação 2")
            };
            var turmasFormacao = new List<ListagemFormacaoComTurmaDTO>
            {
                CriarTurmaFormacao(1),
                CriarTurmaFormacao(2)
            };
            var tiposInscricao = CriarTiposInscricao();

            ConfigurarMocksComSucesso(2, propostasOriginais, propostasDto, turmasFormacao, tiposInscricao);

            // Act
            var resultado = await _sut.Handle(query, CancellationToken.None);

            // Assert
            resultado.Should().NotBeNull();
            resultado.Items.Should().HaveCount(2);
            resultado.TotalRegistros.Should().Be(2);
            resultado.TotalPaginas.Should().Be(1);
        }

        [Fact(DisplayName = "Handle - Deve calcular paginação corretamente")]
        public async Task Handle_Deve_Calcular_Paginacao_Corretamente()
        {
            // Arrange
            var query = CriarQuery(numeroPagina: 1, numeroRegistros: 5);
            var propostasOriginais = new List<Proposta>
            {
                CriarFormacaoDTO(1, "Formação 1")
            };
            var propostasDto = new List<DadosListagemFormacaoComTurmaDTO>
            {
                CriarDadosListagemFormacao(1, "Formação 1")
            };

            ConfigurarMocksComSucesso(15, propostasOriginais, propostasDto, new List<ListagemFormacaoComTurmaDTO>(), new List<PropostaTipoInscricao>());

            // Act
            var resultado = await _sut.Handle(query, CancellationToken.None);

            // Assert
            resultado.TotalRegistros.Should().Be(15);
            resultado.TotalPaginas.Should().Be(3);
        }

        #endregion

        #region Testes - Filtragem de Parâmetros

        [Fact(DisplayName = "Handle - Deve passar parâmetros corretos para obter total de registros")]
        public async Task Handle_Deve_Passar_Parametros_Para_Obter_Total()
        {
            // Arrange
            const long codigoFormacao = 123;
            const string nomeFormacao = "Formação Teste";
            const long numeroHomologacao = 456;
            const long areaPromotoraId = 789;
            const bool apenasSemCodaf = true;

            var query = new ObterDadosPaginadosComFiltrosQuery(
                numeroPagina: 1,
                numeroRegistros: 10,
                codigoFormacao: codigoFormacao,
                nomeFormacao: nomeFormacao,
                areaPromotoraIdUsuarioLogado: areaPromotoraId,
                numeroHomologacao: numeroHomologacao,
                apenasSemCodaf: apenasSemCodaf);

            _repositorioInscricao
                .Setup(r => r.ObterDadosPaginadosComFiltrosTotalRegistros(
                    areaPromotoraId,
                    codigoFormacao,
                    nomeFormacao,
                    numeroHomologacao))
                .ReturnsAsync(0);

            // Act
            await _sut.Handle(query, CancellationToken.None);

            // Assert
            _repositorioInscricao.Verify(
                r => r.ObterDadosPaginadosComFiltrosTotalRegistros(
                    areaPromotoraId,
                    codigoFormacao,
                    nomeFormacao,
                    numeroHomologacao),
                Times.Once);
        }

        [Fact(DisplayName = "Handle - Deve passar parâmetros corretos para obter dados paginados")]
        public async Task Handle_Deve_Passar_Parametros_Para_Obter_Dados()
        {
            // Arrange
            const long codigoFormacao = 123;
            const string nomeFormacao = "Formação Teste";
            const long numeroHomologacao = 456;
            const long areaPromotoraId = 789;
            const int numeroPagina = 2;
            const int numeroRegistros = 20;

            var query = new ObterDadosPaginadosComFiltrosQuery(
                numeroPagina: numeroPagina,
                numeroRegistros: numeroRegistros,
                codigoFormacao: codigoFormacao,
                nomeFormacao: nomeFormacao,
                areaPromotoraIdUsuarioLogado: areaPromotoraId,
                numeroHomologacao: numeroHomologacao,
                apenasSemCodaf: null);

            var propostasDto = new List<Proposta> { CriarFormacaoDTO(1) };
            ConfigurarMocksComSucesso(1, propostasDto, new List<DadosListagemFormacaoComTurmaDTO>(), new List<ListagemFormacaoComTurmaDTO>(), new List<PropostaTipoInscricao>());

            // Act
            await _sut.Handle(query, CancellationToken.None);

            // Assert
            _repositorioInscricao.Verify(
                r => r.ObterDadosPaginadosComFiltros(
                    areaPromotoraId,
                    codigoFormacao,
                    nomeFormacao,
                    numeroPagina,
                    numeroRegistros,
                    numeroHomologacao),
                Times.Once);
        }

        [Fact(DisplayName = "Handle - Deve filtrar com parâmetros nulos")]
        public async Task Handle_Deve_Filtrar_Com_Parametros_Nulos()
        {
            // Arrange
            var query = new ObterDadosPaginadosComFiltrosQuery(
                numeroPagina: 1,
                numeroRegistros: 10,
                codigoFormacao: null,
                nomeFormacao: null,
                areaPromotoraIdUsuarioLogado: null,
                numeroHomologacao: null,
                apenasSemCodaf: null);

            _repositorioInscricao
                .Setup(r => r.ObterDadosPaginadosComFiltrosTotalRegistros(null, null, null, null))
                .ReturnsAsync(0);

            // Act
            await _sut.Handle(query, CancellationToken.None);

            // Assert
            _repositorioInscricao.Verify(
                r => r.ObterDadosPaginadosComFiltrosTotalRegistros(null, null, null, null),
                Times.Once);
        }

        #endregion

        #region Testes - Mapeamento de Dados

        [Fact(DisplayName = "Handle - Deve mapear propostas para DTOs")]
        public async Task Handle_Deve_Mapear_Propostas_Para_Dtos()
        {
            // Arrange
            var query = CriarQuery();
            var propostasDto = new List<Proposta> { CriarFormacaoDTO(1) };

            ConfigurarMocksComSucesso(1, propostasDto, new List<DadosListagemFormacaoComTurmaDTO>(), new List<ListagemFormacaoComTurmaDTO>(), new List<PropostaTipoInscricao>());

            // Act
            var resultado = await _sut.Handle(query, CancellationToken.None);

            // Assert
            _mapper.Verify(
                m => m.Map<IEnumerable<DadosListagemFormacaoComTurmaDTO>>(It.IsAny<object>()),
                Times.Once);
        }

        [Fact(DisplayName = "Handle - Deve extrair códigos de formação para buscar turmas")]
        public async Task Handle_Deve_Extrair_Codigos_Formacao()
        {
            // Arrange
            var query = CriarQuery();
            var propostasOriginais = new List<Proposta>
            {
                CriarFormacaoDTO(100, "Formação 1"),
                CriarFormacaoDTO(200, "Formação 2")
            };
            var propostasDto = new List<DadosListagemFormacaoComTurmaDTO>
            {
                CriarDadosListagemFormacao(100, "Formação 1"),
                CriarDadosListagemFormacao(200, "Formação 2")
            };
            var turmasFormacao = new List<ListagemFormacaoComTurmaDTO>();

            ConfigurarMocksComSucesso(2, propostasOriginais, propostasDto, turmasFormacao, new List<PropostaTipoInscricao>());

            // Act
            await _sut.Handle(query, CancellationToken.None);

            // Assert
            _repositorioInscricao.Verify(
                r => r.DadosListagemFormacaoComTurma(
                    It.Is<long[]>(x => x.Contains(100) && x.Contains(200))),
                Times.Once);
        }

        #endregion

        #region Testes - Turmas e Permissões

        [Fact(DisplayName = "Handle - Deve associar turmas às formações")]
        public async Task Handle_Deve_Associar_Turmas_As_Formacoes()
        {
            // Arrange
            var query = CriarQuery();
            var formacaoDto = CriarDadosListagemFormacao(1, "Formação 1");
            var propostasOriginais = new List<Proposta> { CriarFormacaoDTO(1) };
            var propostasDto = new List<DadosListagemFormacaoComTurmaDTO> { formacaoDto };

            var turmasFormacao = new List<ListagemFormacaoComTurmaDTO>
            {
                CriarTurmaFormacao(propostaId: 1, nomeTurma: "Turma A", permiteSorteio: true, vagar: 5, excedidas: 2, aguardandoAnalise: 3)
            };

            ConfigurarMocksComSucesso(1, propostasOriginais, propostasDto, turmasFormacao, new List<PropostaTipoInscricao>());

            // Act
            var resultado = await _sut.Handle(query, CancellationToken.None);

            // Assert
            var formacao = resultado.Items.First();
            formacao.Turmas.Should().NotBeEmpty();
            formacao.Turmas.Should().HaveCount(1);
            formacao.Turmas.First().NomeTurma.Should().Be("Turma A");
        }

        [Fact(DisplayName = "Handle - Deve calcular permissão de Sorteio corretamente")]
        public async Task Handle_Deve_Calcular_Permissao_Sorteio_Corretamente()
        {
            // Arrange
            var query = CriarQuery();
            var formacaoDto = CriarDadosListagemFormacao(1);
            var propostasOriginais = new List<Proposta> { CriarFormacaoDTO(1) };
            var propostasDto = new List<DadosListagemFormacaoComTurmaDTO> { formacaoDto };

            var turmasFormacao = new List<ListagemFormacaoComTurmaDTO>
            {
                CriarTurmaFormacao(propostaId: 1, nomeTurma: "Turma 1", permiteSorteio: true, vagar: 5, excedidas: 2, aguardandoAnalise: 3),
                CriarTurmaFormacao(propostaId: 1, nomeTurma: "Turma 2", permiteSorteio: false, vagar: 5, excedidas: 2, aguardandoAnalise: 3),
                CriarTurmaFormacao(propostaId: 1, nomeTurma: "Turma 3", permiteSorteio: true, vagar: 0, excedidas: 2, aguardandoAnalise: 3),
                CriarTurmaFormacao(propostaId: 1, nomeTurma: "Turma 4", permiteSorteio: true, vagar: 5, excedidas: 0, aguardandoAnalise: 3),
                CriarTurmaFormacao(propostaId: 1, nomeTurma: "Turma 5", permiteSorteio: true, vagar: 5, excedidas: 2, aguardandoAnalise: 0)
            };

            ConfigurarMocksComSucesso(1, propostasOriginais, propostasDto, turmasFormacao, new List<PropostaTipoInscricao>());

            // Act
            var resultado = await _sut.Handle(query, CancellationToken.None);

            // Assert
            var turmas = resultado.Items.First().Turmas.ToList();
            turmas[0].Permissao.PodeRealizarSorteio.Should().BeTrue();
            turmas[1].Permissao.PodeRealizarSorteio.Should().BeFalse();
            turmas[2].Permissao.PodeRealizarSorteio.Should().BeFalse();
            turmas[3].Permissao.PodeRealizarSorteio.Should().BeFalse();
            turmas[4].Permissao.PodeRealizarSorteio.Should().BeFalse();
        }

        [Fact(DisplayName = "Handle - Deve remover turmas duplicadas por nome")]
        public async Task Handle_Deve_Remover_Turmas_Duplicadas()
        {
            // Arrange
            var query = CriarQuery();
            var formacaoDto = CriarDadosListagemFormacao(1);
            var propostasOriginais = new List<Proposta> { CriarFormacaoDTO(1) };
            var propostasDto = new List<DadosListagemFormacaoComTurmaDTO> { formacaoDto };

            var turmasFormacao = new List<ListagemFormacaoComTurmaDTO>
            {
                CriarTurmaFormacao(propostaId: 1, nomeTurma: "Turma A", propostaTurmaId: 1),
                CriarTurmaFormacao(propostaId: 1, nomeTurma: "Turma A", propostaTurmaId: 2),
                CriarTurmaFormacao(propostaId: 1, nomeTurma: "Turma B", propostaTurmaId: 3)
            };

            ConfigurarMocksComSucesso(1, propostasOriginais, propostasDto, turmasFormacao, new List<PropostaTipoInscricao>());

            // Act
            var resultado = await _sut.Handle(query, CancellationToken.None);

            // Assert
            var turmas = resultado.Items.First().Turmas.ToList();
            turmas.Should().HaveCount(2);
            turmas.Select(t => t.NomeTurma).Should().ContainSingle(t => t == "Turma A");
            turmas.Select(t => t.NomeTurma).Should().ContainSingle(t => t == "Turma B");
        }

        #endregion

        #region Testes - Tipos de Inscrição

        [Fact(DisplayName = "Handle - Deve associar tipos de inscrição às formações")]
        public async Task Handle_Deve_Associar_Tipos_Inscricao_As_Formacoes()
        {
            // Arrange
            var query = CriarQuery();
            var formacaoDto = CriarDadosListagemFormacao(1);
            var propostasOriginais = new List<Proposta> { CriarFormacaoDTO(1) };
            var propostasDto = new List<DadosListagemFormacaoComTurmaDTO> { formacaoDto };

            var tiposInscricao = new List<PropostaTipoInscricao>
            {
                new PropostaTipoInscricao { PropostaId = 1, TipoInscricao = TipoInscricao.Automatica },
                new PropostaTipoInscricao { PropostaId = 1, TipoInscricao = TipoInscricao.Externa }
            };

            ConfigurarMocksComSucesso(1, propostasOriginais, propostasDto, new List<ListagemFormacaoComTurmaDTO>(), tiposInscricao);

            // Act
            var resultado = await _sut.Handle(query, CancellationToken.None);

            // Assert
            var formacao = resultado.Items.First();
            formacao.TiposInscricoes.Should().NotBeEmpty();
            formacao.TiposInscricoes.Should().HaveCount(2);
            formacao.TiposInscricoes.Should().Contain(TipoInscricao.Automatica);
            formacao.TiposInscricoes.Should().Contain(TipoInscricao.Externa);
        }

        [Fact(DisplayName = "Handle - Deve filtrar tipos de inscrição por proposta")]
        public async Task Handle_Deve_Filtrar_Tipos_Inscricao_Por_Proposta()
        {
            // Arrange
            var query = CriarQuery();
            var propostasOriginais = new List<Proposta>
            {
                CriarFormacaoDTO(1),
                CriarFormacaoDTO(2)
            };
            var propostasDto = new List<DadosListagemFormacaoComTurmaDTO>
            {
                CriarDadosListagemFormacao(1),
                CriarDadosListagemFormacao(2)
            };

            var tiposInscricao = new List<PropostaTipoInscricao>
            {
                new PropostaTipoInscricao { PropostaId = 1, TipoInscricao = TipoInscricao.Automatica },
                new PropostaTipoInscricao { PropostaId = 2, TipoInscricao = TipoInscricao.Optativa }
            };

            ConfigurarMocksComSucesso(2, propostasOriginais, propostasDto, new List<ListagemFormacaoComTurmaDTO>(), tiposInscricao);

            // Act
            var resultado = await _sut.Handle(query, CancellationToken.None);

            // Assert
            var formacoes = resultado.Items.ToList();
            formacoes[0].TiposInscricoes.Should().ContainSingle(t => t == TipoInscricao.Automatica);
            formacoes[1].TiposInscricoes.Should().ContainSingle(t => t == TipoInscricao.Optativa);
        }

        #endregion

        #region Testes - Datas de Turmas

        [Fact(DisplayName = "Handle - Deve agregar datas de turmas com mesmo nome")]
        public async Task Handle_Deve_Agregar_Datas_De_Turmas()
        {
            // Arrange
            var query = CriarQuery();
            var formacaoDto = CriarDadosListagemFormacao(1);
            var propostasOriginais = new List<Proposta> { CriarFormacaoDTO(1) };
            var propostasDto = new List<DadosListagemFormacaoComTurmaDTO> { formacaoDto };

            var turmasFormacao = new List<ListagemFormacaoComTurmaDTO>
            {
                CriarTurmaFormacao(propostaId: 1, nomeTurma: "Turma A", datas: "10/01/2024", propostaTurmaId: 1),
                CriarTurmaFormacao(propostaId: 1, nomeTurma: "Turma A", datas: "20/01/2024", propostaTurmaId: 2)
            };

            ConfigurarMocksComSucesso(1, propostasOriginais, propostasDto, turmasFormacao, new List<PropostaTipoInscricao>());

            // Act
            var resultado = await _sut.Handle(query, CancellationToken.None);

            // Assert
            resultado.Items.Should().NotBeEmpty();
            var turma = resultado.Items.First().Turmas.FirstOrDefault();
            turma.Should().NotBeNull();
            turma.Data.Should().Contain("10/01/2024");
            turma.Data.Should().Contain("20/01/2024");
            turma.Data.Should().Contain(", ");
        }

        [Fact(DisplayName = "Handle - Não deve agregar datas vazias")]
        public async Task Handle_Nao_Deve_Agregar_Datas_Vazias()
        {
            // Arrange
            var query = CriarQuery();
            var formacaoDto = CriarDadosListagemFormacao(1);
            var propostasOriginais = new List<Proposta> { CriarFormacaoDTO(1) };
            var propostasDto = new List<DadosListagemFormacaoComTurmaDTO> { formacaoDto };

            var turmasFormacao = new List<ListagemFormacaoComTurmaDTO>
            {
                CriarTurmaFormacao(propostaId: 1, nomeTurma: "Turma A", datas: "10/01/2024", propostaTurmaId: 1),
                CriarTurmaFormacao(propostaId: 1, nomeTurma: "Turma A", datas: "", propostaTurmaId: 2),
                CriarTurmaFormacao(propostaId: 1, nomeTurma: "Turma A", datas: null, propostaTurmaId: 3)
            };

            ConfigurarMocksComSucesso(1, propostasOriginais, propostasDto, turmasFormacao, new List<PropostaTipoInscricao>());

            // Act
            var resultado = await _sut.Handle(query, CancellationToken.None);

            // Assert
            resultado.Items.Should().NotBeEmpty();
            var turma = resultado.Items.First().Turmas.First();
            turma.Data.Should().Be("10/01/2024");
        }

        #endregion

        #region Testes - Filtro ApenasSemCodaf

        [Fact(DisplayName = "Handle - Deve filtrar apenas turmas sem CodafId quando ApenasSemCodaf é true")]
        public async Task Handle_Deve_Filtrar_Apenas_Turmas_Sem_Codaf_Quando_True()
        {
            // Arrange
            var query = new ObterDadosPaginadosComFiltrosQuery(
                numeroPagina: 1,
                numeroRegistros: 10,
                codigoFormacao: null,
                nomeFormacao: null,
                areaPromotoraIdUsuarioLogado: null,
                numeroHomologacao: null,
                apenasSemCodaf: true);

            var formacaoDto = CriarDadosListagemFormacao(1);
            var propostasOriginais = new List<Proposta> { CriarFormacaoDTO(1) };
            var propostasDto = new List<DadosListagemFormacaoComTurmaDTO> { formacaoDto };

            var turmasFormacao = new List<ListagemFormacaoComTurmaDTO>
            {
                CriarTurmaFormacao(propostaId: 1, nomeTurma: "Turma Sem Codaf", propostaTurmaId: 1, codafId: null),
                CriarTurmaFormacao(propostaId: 1, nomeTurma: "Turma Com Codaf", propostaTurmaId: 2, codafId: 123)
            };

            ConfigurarMocksComSucesso(2, propostasOriginais, propostasDto, turmasFormacao, new List<PropostaTipoInscricao>());

            // Act
            var resultado = await _sut.Handle(query, CancellationToken.None);

            // Assert
            resultado.Items.First().Turmas.Should().HaveCount(1);
            resultado.Items.First().Turmas.First().NomeTurma.Should().Be("Turma Sem Codaf");
        }

        [Fact(DisplayName = "Handle - Deve retornar todas as turmas quando ApenasSemCodaf é false")]
        public async Task Handle_Deve_Retornar_Todas_Turmas_Quando_ApenasSemCodaf_False()
        {
            // Arrange
            var query = new ObterDadosPaginadosComFiltrosQuery(
                numeroPagina: 1,
                numeroRegistros: 10,
                codigoFormacao: null,
                nomeFormacao: null,
                areaPromotoraIdUsuarioLogado: null,
                numeroHomologacao: null,
                apenasSemCodaf: false);

            var formacaoDto = CriarDadosListagemFormacao(1);
            var propostasOriginais = new List<Proposta> { CriarFormacaoDTO(1) };
            var propostasDto = new List<DadosListagemFormacaoComTurmaDTO> { formacaoDto };

            var turmasFormacao = new List<ListagemFormacaoComTurmaDTO>
            {
                CriarTurmaFormacao(propostaId: 1, nomeTurma: "Turma A", propostaTurmaId: 1, codafId: null),
                CriarTurmaFormacao(propostaId: 1, nomeTurma: "Turma B", propostaTurmaId: 2, codafId: 123)
            };

            ConfigurarMocksComSucesso(2, propostasOriginais, propostasDto, turmasFormacao, new List<PropostaTipoInscricao>());

            // Act
            var resultado = await _sut.Handle(query, CancellationToken.None);

            // Assert
            resultado.Items.First().Turmas.Should().HaveCount(2);
        }

        [Fact(DisplayName = "Handle - Deve retornar todas as turmas quando ApenasSemCodaf é null")]
        public async Task Handle_Deve_Retornar_Todas_Turmas_Quando_ApenasSemCodaf_Null()
        {
            // Arrange
            var query = new ObterDadosPaginadosComFiltrosQuery(
                numeroPagina: 1,
                numeroRegistros: 10,
                codigoFormacao: null,
                nomeFormacao: null,
                areaPromotoraIdUsuarioLogado: null,
                numeroHomologacao: null,
                apenasSemCodaf: null);

            var formacaoDto = CriarDadosListagemFormacao(1);
            var propostasOriginais = new List<Proposta> { CriarFormacaoDTO(1) };
            var propostasDto = new List<DadosListagemFormacaoComTurmaDTO> { formacaoDto };

            var turmasFormacao = new List<ListagemFormacaoComTurmaDTO>
            {
                CriarTurmaFormacao(propostaId: 1, nomeTurma: "Turma A", propostaTurmaId: 1, codafId: null),
                CriarTurmaFormacao(propostaId: 1, nomeTurma: "Turma B", propostaTurmaId: 2, codafId: 456)
            };

            ConfigurarMocksComSucesso(2, propostasOriginais, propostasDto, turmasFormacao, new List<PropostaTipoInscricao>());

            // Act
            var resultado = await _sut.Handle(query, CancellationToken.None);

            // Assert
            resultado.Items.First().Turmas.Should().HaveCount(2);
        }

        #endregion

        #region Testes - Fluxo Completo

        [Fact(DisplayName = "Handle - Deve processar fluxo completo com múltiplas formações")]
        public async Task Handle_Deve_Processar_Fluxo_Completo()
        {
            // Arrange
            var query = CriarQuery(numeroPagina: 1, numeroRegistros: 10);

            var propostasOriginais = new List<Proposta>
            {
                CriarFormacaoDTO(1, "Formação 1"),
                CriarFormacaoDTO(2, "Formação 2")
            };

            var propostasDto = new List<DadosListagemFormacaoComTurmaDTO>
            {
                CriarDadosListagemFormacao(1, "Formação 1"),
                CriarDadosListagemFormacao(2, "Formação 2")
            };

            var turmasFormacao = new List<ListagemFormacaoComTurmaDTO>
            {
                CriarTurmaFormacao(1, "Turma A1"),
                CriarTurmaFormacao(1, "Turma A2"),
                CriarTurmaFormacao(2, "Turma B1")
            };

            var tiposInscricao = new List<PropostaTipoInscricao>
            {
                new PropostaTipoInscricao { PropostaId = 1, TipoInscricao = TipoInscricao.Externa },
                new PropostaTipoInscricao { PropostaId = 2, TipoInscricao = TipoInscricao.Automatica }
            };

            ConfigurarMocksComSucesso(2, propostasOriginais, propostasDto, turmasFormacao, tiposInscricao);

            // Act
            var resultado = await _sut.Handle(query, CancellationToken.None);

            // Assert
            resultado.Should().NotBeNull();
            resultado.Items.Should().HaveCount(2);
            resultado.TotalRegistros.Should().Be(2);

            var formacao1 = resultado.Items.First();
            formacao1.Turmas.Should().HaveCount(2);
            formacao1.TiposInscricoes.Should().ContainSingle(t => t == TipoInscricao.Externa);

            var formacao2 = resultado.Items.Last();
            formacao2.Turmas.Should().HaveCount(1);
            formacao2.TiposInscricoes.Should().ContainSingle(t => t == TipoInscricao.Automatica);
        }

        #endregion

        #region Testes - Tratamento de Nulls

        [Fact(DisplayName = "Handle - Deve tratar turmas nulas")]
        public async Task Handle_Deve_Tratar_Turmas_Nulas()
        {
            // Arrange
            var query = CriarQuery();
            var propostasOriginais = new List<Proposta> { CriarFormacaoDTO(1) };
            var propostasDto = new List<DadosListagemFormacaoComTurmaDTO> { CriarDadosListagemFormacao(1) };

            _repositorioInscricao
                .Setup(r => r.ObterDadosPaginadosComFiltrosTotalRegistros(
                    It.IsAny<long?>(),
                    It.IsAny<long?>(),
                    It.IsAny<string?>(),
                    It.IsAny<long?>()))
                .ReturnsAsync(1);

            _repositorioInscricao
                .Setup(r => r.ObterDadosPaginadosComFiltros(
                    It.IsAny<long?>(),
                    It.IsAny<long?>(),
                    It.IsAny<string?>(),
                    It.IsAny<int>(),
                    It.IsAny<int>(),
                    It.IsAny<long?>()))
                .ReturnsAsync(propostasOriginais);

            _mapper
                .Setup(m => m.Map<IEnumerable<DadosListagemFormacaoComTurmaDTO>>(It.IsAny<object>()))
                .Returns(propostasDto);

            _repositorioInscricao
                .Setup(r => r.DadosListagemFormacaoComTurma(It.IsAny<long[]>(), It.IsAny<long?>()))
                .ReturnsAsync((List<ListagemFormacaoComTurmaDTO>)null!);

            _repositorioInscricao
                .Setup(r => r.ObterTiposInscricaoPorPropostaIds(It.IsAny<long[]>()))
                .ReturnsAsync(new List<PropostaTipoInscricao>());

            // Act
            var resultado = await _sut.Handle(query, CancellationToken.None);

            // Assert
            resultado.Should().NotBeNull();
            resultado.Items.First().Turmas.Should().BeEmpty();
        }

        [Fact(DisplayName = "Handle - Deve tratar tipos de inscrição vazios")]
        public async Task Handle_Deve_Tratar_Tipos_Inscricao_Vazios()
        {
            // Arrange
            var query = CriarQuery();
            var propostasOriginais = new List<Proposta> { CriarFormacaoDTO(1) };
            var propostasDto = new List<DadosListagemFormacaoComTurmaDTO> { CriarDadosListagemFormacao(1) };

            ConfigurarMocksComSucesso(1, propostasOriginais, propostasDto, new List<ListagemFormacaoComTurmaDTO>(), new List<PropostaTipoInscricao>());

            // Act
            var resultado = await _sut.Handle(query, CancellationToken.None);

            // Assert
            resultado.Items.First().TiposInscricoes.Should().BeEmpty();
        }

        #endregion

        #region Métodos Auxiliares

        private static DadosListagemFormacaoComTurmaDTO CriarDadosListagemFormacao(
            long id,
            string? nome = null)
        {
            return new DadosListagemFormacaoComTurmaDTO
            {
                Id = id,
                CodigoFormacao = id,
                NomeFormacao = nome ?? $"Formação {id}",
                Turmas = new List<DadosListagemFormacaoTurma>(),
                TiposInscricoes = new List<TipoInscricao>()
            };
        }

        private static ObterDadosPaginadosComFiltrosQuery CriarQuery(
            int numeroPagina = 1,
            int numeroRegistros = 10,
            long? codigoFormacao = null,
            string? nomeFormacao = null,
            long? areaPromotoraIdUsuarioLogado = null,
            long? numeroHomologacao = null,
            bool? apenasSemCodaf = null)
        {
            return new ObterDadosPaginadosComFiltrosQuery(
                numeroPagina: numeroPagina,
                numeroRegistros: numeroRegistros,
                codigoFormacao: codigoFormacao,
                nomeFormacao: nomeFormacao,
                areaPromotoraIdUsuarioLogado: areaPromotoraIdUsuarioLogado,
                numeroHomologacao: numeroHomologacao,
                apenasSemCodaf: apenasSemCodaf
            );
        }

        private void ConfigurarMocksComSucesso(
            int totalRegistros,
            List<Proposta> propostasOriginais,
            List<DadosListagemFormacaoComTurmaDTO> propostasDto,
            List<ListagemFormacaoComTurmaDTO> turmasFormacao,
            List<PropostaTipoInscricao> tiposInscricao)
        {
            _repositorioInscricao
                .Setup(r => r.ObterDadosPaginadosComFiltrosTotalRegistros(
                    It.IsAny<long?>(),
                    It.IsAny<long?>(),
                    It.IsAny<string?>(),
                    It.IsAny<long?>()))
                .ReturnsAsync(totalRegistros);

            _repositorioInscricao
                .Setup(r => r.ObterDadosPaginadosComFiltros(
                    It.IsAny<long?>(),
                    It.IsAny<long?>(),
                    It.IsAny<string?>(),
                    It.IsAny<int>(),
                    It.IsAny<int>(),
                    It.IsAny<long?>()))
                .ReturnsAsync(propostasOriginais);

            _mapper
                .Setup(m => m.Map<IEnumerable<DadosListagemFormacaoComTurmaDTO>>(It.IsAny<object>()))
                .Returns(propostasDto);

            _repositorioInscricao
                .Setup(r => r.DadosListagemFormacaoComTurma(It.IsAny<long[]>(), It.IsAny<long?>()))
                .ReturnsAsync(turmasFormacao);

            _repositorioInscricao
                .Setup(r => r.ObterTiposInscricaoPorPropostaIds(It.IsAny<long[]>()))
                .ReturnsAsync(tiposInscricao);
        }

        private static Proposta CriarFormacaoDTO(long id, string? nome = null)
        {
            return new Proposta
            {
                Id = id,
                NomeFormacao = nome ?? $"Formação {id}",
                TiposInscricao = new List<PropostaTipoInscricao>(),
                Turmas = new List<PropostaTurma>(),
                PublicosAlvo = new List<PropostaPublicoAlvo>(),
                FuncoesEspecificas = new List<PropostaFuncaoEspecifica>(),
                CriteriosValidacaoInscricao = new List<PropostaCriterioValidacaoInscricao>(),
                VagasRemanecentes = new List<PropostaVagaRemanecente>(),
                Encontros = new List<PropostaEncontro>(),
                PalavrasChaves = new List<PropostaPalavraChave>(),
                CriterioCertificacao = new List<PropostaCriterioCertificacao>(),
                Regentes = new List<PropostaRegente>(),
                Tutores = new List<PropostaTutor>(),
                TurmasDres = new List<PropostaTurmaDre>(),
                Modalidades = new List<PropostaModalidade>(),
                AnosTurmas = new List<PropostaAnoTurma>(),
                ComponentesCurriculares = new List<PropostaComponenteCurricular>(),
                CodafListaPresencas = new List<CodafListaPresenca>()
            };
        }

        private static ListagemFormacaoComTurmaDTO CriarTurmaFormacao(
            long propostaId,
            string? nomeTurma = null,
            bool? permiteSorteio = null,
            int? vagar = null,
            int? excedidas = null,
            int? aguardandoAnalise = 0,
            string? datas = null,
            long? propostaTurmaId = 0,
            long? codafId = null)
        {
            return new ListagemFormacaoComTurmaDTO
            {
                PropostaId = propostaId,
                NomeTurma = nomeTurma ?? $"Turma {propostaId}",
                PermiteSorteio = permiteSorteio,
                QuantidadeVagas = vagar,
                Excedidas = excedidas,
                AguardandoAnalise = aguardandoAnalise,
                Datas = datas,
                PropostaTurmaId = propostaTurmaId,
                Disponiveis = vagar,
                CodafId = codafId,
                TotalInscricoes = 0,
                Confirmadas = 0,
                EmEspera = 0,
                Cancelada = 0
            };
        }

        private static List<PropostaTipoInscricao> CriarTiposInscricao(params (long propostaId, TipoInscricao tipo)[] itens)
        {
            if (itens == null || itens.Length == 0)
                return new List<PropostaTipoInscricao>
                {
                    new PropostaTipoInscricao { PropostaId = 1, TipoInscricao = TipoInscricao.Automatica },
                    new PropostaTipoInscricao { PropostaId = 2, TipoInscricao = TipoInscricao.Externa }
                };

            return itens.Select(x => new PropostaTipoInscricao { PropostaId = x.propostaId, TipoInscricao = x.tipo }).ToList();
        }
    }
};
#endregion