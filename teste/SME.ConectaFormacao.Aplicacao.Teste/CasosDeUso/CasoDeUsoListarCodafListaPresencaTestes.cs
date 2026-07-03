using AutoMapper;
using Bogus;
using FluentAssertions;
using Moq;
using Moq.AutoMock;
using SME.ConectaFormacao.Aplicacao.CasosDeUso.Codaf;
using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Aplicacao.Dtos.Codaf;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Contexto;
using SME.ConectaFormacao.Infra.Dados.Dtos;
using SME.ConectaFormacao.Infra.Dados.Dtos.CodafListaPresencas;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao.Teste.CasosDeUso
{
    public class CasoDeUsoListarCodafListaPresencaTestes
    {
        private readonly Mock<IRepositorioCodafListaPresenca> _repositorioCodafListaPresencaMock;
        private readonly Mock<IContextoAplicacao> _contextoAplicacaoMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly CasoDeUsoListarCodafListaPresenca _casoDeUsoListarCodafListaPresenca;
        private readonly Faker _faker;

        public CasoDeUsoListarCodafListaPresencaTestes()
        {
            var mocker = new AutoMocker();
            _repositorioCodafListaPresencaMock = mocker.GetMock<IRepositorioCodafListaPresenca>();
            _contextoAplicacaoMock = mocker.GetMock<IContextoAplicacao>();
            _mapperMock = mocker.GetMock<IMapper>();
            _casoDeUsoListarCodafListaPresenca = mocker.CreateInstance<CasoDeUsoListarCodafListaPresenca>();
            _faker = new();
        }

        [Fact]
        public async Task DadoFiltroValido_QuandoChamarExecutar_DeveRetornarResultadoEsperado()
        {
            var filtroDto = new FiltroListaPresencaCodafDto
            {
                NomeFormacao = _faker.Lorem.Word(),
                CodigoFormacao = _faker.Random.Int(1),
                NumeroPagina = 1,
                NumeroRegistros = 10
            };
            var filtroRepositorioDto = new FiltroListagemResultadoCodafListaPresencaDto
            {
                NomeFormacao = filtroDto.NomeFormacao,
                CodigoFormacao = filtroDto.CodigoFormacao.ToString(),
                Pagina = filtroDto.NumeroPagina,
                TamanhoPagina = filtroDto.NumeroRegistros,
                PerfilRestrito = false
            };
            var resultadoRepositorio = new ResultadoPaginado<ListagemResultadoCodafListaPresencaDto>
            {
                Itens = [],
                TotalRegistros = 0
            };
            var resultadoEsperado = new PaginacaoResultadoDto<ListaPresencaCodafResumoDto>([], 0, 0);

            _contextoAplicacaoMock.Setup(c => c.IdPerfilUsuario).Returns(Perfis.ADMIN_DF);
            _mapperMock.Setup(m => m.Map<FiltroListagemResultadoCodafListaPresencaDto>(filtroDto))
                .Returns(filtroRepositorioDto);
            _repositorioCodafListaPresencaMock.Setup(r => r.ObterListagemResultadoCodafListaPresencaPorFiltroAsync(It.IsAny<FiltroListagemResultadoCodafListaPresencaDto>()))
                .ReturnsAsync(resultadoRepositorio);
            _mapperMock.Setup(m => m.Map<List<ListaPresencaCodafResumoDto>>(resultadoRepositorio.Itens))
                .Returns([]);

            var resultado = await _casoDeUsoListarCodafListaPresenca.ExecutarAsync(filtroDto);

            resultado.Should().NotBeNull();
            resultado.Sucesso.Should().BeTrue();
            resultado.Dados.Should().BeEquivalentTo(resultadoEsperado);
        }

        [Fact]
        public async Task DadoUsuarioComPerfilAdmin_QuandoChamarExecutar_DeveRetornarTodosOsItens()
        {
            var filtroDto = new FiltroListaPresencaCodafDto
            {
                NomeFormacao = _faker.Lorem.Word(),
                CodigoFormacao = _faker.Random.Int(1),
                NumeroPagina = 1,
                NumeroRegistros = 10
            };
            var filtroRepositorioDto = new FiltroListagemResultadoCodafListaPresencaDto
            {
                NomeFormacao = filtroDto.NomeFormacao,
                CodigoFormacao = filtroDto.CodigoFormacao.ToString(),
                Pagina = filtroDto.NumeroPagina,
                TamanhoPagina = filtroDto.NumeroRegistros,
                PerfilRestrito = false
            };
            var itensRepositorio = new List<ListagemResultadoCodafListaPresencaDto>
            {
                new()
                {
                    Id = 1,
                    NomeFormacao = _faker.Lorem.Word(),
                    NomeTurma = _faker.Lorem.Word(),
                    NomeAreaPromotora = _faker.Lorem.Word()
                },
                new()
                {
                    Id = 2,
                    NomeFormacao = _faker.Lorem.Word(),
                    NomeTurma = _faker.Lorem.Word(),
                    NomeAreaPromotora = _faker.Lorem.Word()
                }
            };
            var resultadoRepositorio = new ResultadoPaginado<ListagemResultadoCodafListaPresencaDto>
            {
                Itens = itensRepositorio,
                TotalRegistros = 2,
                TamanhoPagina = 10
            };
            var itensDto = new List<ListaPresencaCodafResumoDto>
            {
                new()
                {
                    Id = 1,
                    NomeFormacao = itensRepositorio[0].NomeFormacao,
                    NomeTurma = itensRepositorio[0].NomeTurma,
                    NomeAreaPromotora = itensRepositorio[0].NomeAreaPromotora
                },
                new()
                {
                    Id = 2,
                    NomeFormacao = itensRepositorio[1].NomeFormacao,
                    NomeTurma = itensRepositorio[1].NomeTurma,
                    NomeAreaPromotora = itensRepositorio[1].NomeAreaPromotora
                }
            };

            _contextoAplicacaoMock.Setup(c => c.IdPerfilUsuario).Returns(Perfis.ADMIN_DF);
            _mapperMock.Setup(m => m.Map<FiltroListagemResultadoCodafListaPresencaDto>(filtroDto))
                .Returns(filtroRepositorioDto);
            _repositorioCodafListaPresencaMock.Setup(r => r.ObterListagemResultadoCodafListaPresencaPorFiltroAsync(It.IsAny<FiltroListagemResultadoCodafListaPresencaDto>()))
                .ReturnsAsync(resultadoRepositorio);
            _mapperMock.Setup(m => m.Map<List<ListaPresencaCodafResumoDto>>(resultadoRepositorio.Itens))
                .Returns(itensDto);

            var resultado = await _casoDeUsoListarCodafListaPresenca.ExecutarAsync(filtroDto);

            resultado.Should().NotBeNull();
            resultado.Sucesso.Should().BeTrue();
            resultado.Dados?.Items.Should().HaveCount(2);
            resultado.Dados?.Items.Should().BeEquivalentTo(itensDto);
            resultado.Dados?.TotalRegistros.Should().Be(2);
        }

        [Fact]
        public async Task DadoUsuarioComPerfilRestrito_QuandoChamarExecutar_DevePassarPerfilRestrito()
        {
            var filtroDto = new FiltroListaPresencaCodafDto
            {
                NomeFormacao = _faker.Lorem.Word(),
                CodigoFormacao = _faker.Random.Int(1),
                NumeroPagina = 1,
                NumeroRegistros = 10
            };
            var filtroRepositorioDto = new FiltroListagemResultadoCodafListaPresencaDto
            {
                NomeFormacao = filtroDto.NomeFormacao,
                CodigoFormacao = filtroDto.CodigoFormacao.ToString(),
                Pagina = filtroDto.NumeroPagina,
                TamanhoPagina = filtroDto.NumeroRegistros,
                PerfilRestrito = true
            };
            var itensRepositorio = new List<ListagemResultadoCodafListaPresencaDto>
            {
                new()
                {
                    Id = 1,
                    NomeFormacao = _faker.Lorem.Word(),
                    NomeTurma = _faker.Lorem.Word(),
                    NomeAreaPromotora = _faker.Lorem.Word()
                }
            };
            var resultadoRepositorio = new ResultadoPaginado<ListagemResultadoCodafListaPresencaDto>
            {
                Itens = itensRepositorio,
                TotalRegistros = 1,
                TamanhoPagina = 10
            };
            var itensDto = new List<ListaPresencaCodafResumoDto>
            {
                new()
                {
                    Id = 1,
                    NomeFormacao = itensRepositorio[0].NomeFormacao,
                    NomeTurma = itensRepositorio[0].NomeTurma,
                    NomeAreaPromotora = itensRepositorio[0].NomeAreaPromotora
                }
            };

            _contextoAplicacaoMock.Setup(c => c.IdPerfilUsuario).Returns(Guid.NewGuid());
            _mapperMock.Setup(m => m.Map<FiltroListagemResultadoCodafListaPresencaDto>(filtroDto))
                .Returns(filtroRepositorioDto);
            _repositorioCodafListaPresencaMock.Setup(r => r.ObterListagemResultadoCodafListaPresencaPorFiltroAsync(It.Is<FiltroListagemResultadoCodafListaPresencaDto>(f => f.PerfilRestrito == true)))
                .ReturnsAsync(resultadoRepositorio);
            _mapperMock.Setup(m => m.Map<List<ListaPresencaCodafResumoDto>>(resultadoRepositorio.Itens))
                .Returns(itensDto);

            var resultado = await _casoDeUsoListarCodafListaPresenca.ExecutarAsync(filtroDto);

            resultado.Should().NotBeNull();
            resultado.Sucesso.Should().BeTrue();
            resultado.Dados?.Items.Should().HaveCount(1);
            resultado.Dados?.Items.Should().BeEquivalentTo(itensDto);
            _repositorioCodafListaPresencaMock.Verify(
                r => r.ObterListagemResultadoCodafListaPresencaPorFiltroAsync(It.Is<FiltroListagemResultadoCodafListaPresencaDto>(f => f.PerfilRestrito == true)),
                Times.Once);
        }
    }
}
