using AutoMapper;
using Bogus;
using FluentAssertions;
using MediatR;
using Moq;
using Moq.AutoMock;
using SME.ConectaFormacao.Aplicacao.CasosDeUso.Codaf;
using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Aplicacao.Dtos.Codaf;
using SME.ConectaFormacao.Aplicacao.Interfaces.Codaf;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Excecoes;
using SME.ConectaFormacao.Infra.Dados.Dtos;
using SME.ConectaFormacao.Infra.Dados.Dtos.CodafListaPresencas;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao.Teste.CasosDeUso
{
    public class CasoDeUsoListarCodafListaPresencaTestes
    {
        private readonly Mock<IRepositorioCodafListaPresenca> _repositorioCodafListaPresencaMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<IValidadorPermissaoCodaf> _validadorPermissaoCodafMock;
        private readonly Mock<IMediator> _mediatorMock;
        private readonly CasoDeUsoListarCodafListaPresenca _casoDeUsoListarCodafListaPresenca;
        private readonly Faker _faker;

        public CasoDeUsoListarCodafListaPresencaTestes()
        {
            var mocker = new AutoMocker();
            _repositorioCodafListaPresencaMock = mocker.GetMock<IRepositorioCodafListaPresenca>();
            _mapperMock = mocker.GetMock<IMapper>();
            _validadorPermissaoCodafMock = mocker.GetMock<IValidadorPermissaoCodaf>();
            _mediatorMock = mocker.GetMock<IMediator>();
            _casoDeUsoListarCodafListaPresenca = mocker.CreateInstance<CasoDeUsoListarCodafListaPresenca>();
            _faker = new();
        }

        [Fact]
        public async Task DadoFiltroValido_QuandoChamarExecutar_DeveRetornarResultadoEsperado()
        {
            var usuarioLogado = new Usuario { Id = 1, Email = _faker.Internet.Email() };
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
                TamanhoPagina = filtroDto.NumeroRegistros
            };
            var resultadoRepositorio = new ResultadoPaginado<ListagemResultadoCodafListaPresencaDto>
            {
                Itens = [],
                TotalRegistros = 0
            };
            var resultadoEsperado = new PaginacaoResultadoDto<ListaPresencaCodafResumoDto>([], 0, 0);
            _mapperMock.Setup(m => m.Map<FiltroListagemResultadoCodafListaPresencaDto>(filtroDto))
                .Returns(filtroRepositorioDto);
            _mediatorMock.Setup(m => m.Send(It.IsAny<ObterUsuarioLogadoQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(usuarioLogado);
            _validadorPermissaoCodafMock.Setup(v => v.BuscarPerfilUsuario())
                .ReturnsAsync(Guid.NewGuid());
            _validadorPermissaoCodafMock.Setup(v => v.UsuarioPossuiPerfilAdminOuEMFORPEF(Guid.NewGuid()))
                .ReturnsAsync(false);
            _repositorioCodafListaPresencaMock.Setup(r => r.ObterListagemResultadoCodafListaPresencaPorFiltroAsync(filtroRepositorioDto))
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
            var usuarioLogado = new Usuario { Id = 1, Email = _faker.Internet.Email() };
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
                TamanhoPagina = filtroDto.NumeroRegistros
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
            var perfilId = Guid.NewGuid();

            _mapperMock.Setup(m => m.Map<FiltroListagemResultadoCodafListaPresencaDto>(filtroDto))
                .Returns(filtroRepositorioDto);
            _mediatorMock.Setup(m => m.Send(It.IsAny<ObterUsuarioLogadoQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(usuarioLogado);
            _validadorPermissaoCodafMock.Setup(v => v.BuscarPerfilUsuario())
                .ReturnsAsync(perfilId);
            _validadorPermissaoCodafMock.Setup(v => v.UsuarioPossuiPerfilAdminOuEMFORPEF(perfilId))
                .ReturnsAsync(true);
            _repositorioCodafListaPresencaMock.Setup(r => r.ObterListagemResultadoCodafListaPresencaPorFiltroAsync(filtroRepositorioDto))
                .ReturnsAsync(resultadoRepositorio);
            _mapperMock.Setup(m => m.Map<IEnumerable<ListaPresencaCodafResumoDto>>(resultadoRepositorio.Itens))
                .Returns(itensDto);

            var resultado = await _casoDeUsoListarCodafListaPresenca.ExecutarAsync(filtroDto);

            resultado.Should().NotBeNull();
            resultado.Sucesso.Should().BeTrue();
            resultado.Dados?.Items.Should().HaveCount(2);
            resultado.Dados?.Items.Should().BeEquivalentTo(itensDto);
            resultado.Dados?.TotalRegistros.Should().Be(2);
        }

        [Fact]
        public async Task DadoUsuarioSemPerfilAdmin_QuandoChamarExecutar_DeveRetornarApenasItensAutorizados()
        {
            var usuarioLogado = new Usuario { Id = 1, Email = _faker.Internet.Email() };
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
                TamanhoPagina = filtroDto.NumeroRegistros
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
            var itemAutorizado = new ListaPresencaCodafResumoDto
            {
                Id = 1,
                NomeFormacao = itensRepositorio[0].NomeFormacao,
                NomeTurma = itensRepositorio[0].NomeTurma,
                NomeAreaPromotora = itensRepositorio[0].NomeAreaPromotora
            };
            var perfilId = Guid.NewGuid();

            _mapperMock.Setup(m => m.Map<FiltroListagemResultadoCodafListaPresencaDto>(filtroDto))
                .Returns(filtroRepositorioDto);
            _mediatorMock.Setup(m => m.Send(It.IsAny<ObterUsuarioLogadoQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(usuarioLogado);
            _validadorPermissaoCodafMock.Setup(v => v.BuscarPerfilUsuario())
                .ReturnsAsync(perfilId);
            _validadorPermissaoCodafMock.Setup(v => v.UsuarioPossuiPerfilAdminOuEMFORPEF(perfilId))
                .ReturnsAsync(false);
            _repositorioCodafListaPresencaMock.Setup(r => r.ObterListagemResultadoCodafListaPresencaPorFiltroAsync(filtroRepositorioDto))
                .ReturnsAsync(resultadoRepositorio);
            _validadorPermissaoCodafMock.Setup(v => v.ValidarSeUsuarioEhCriador(usuarioLogado, 1))
                .ReturnsAsync(true);
            _validadorPermissaoCodafMock.Setup(v => v.ValidarSeUsuarioEhCriador(usuarioLogado, 2))
                .ReturnsAsync(false);
            _mapperMock.Setup(m => m.Map<ListaPresencaCodafResumoDto>(itensRepositorio[0]))
                .Returns(itemAutorizado);

            var resultado = await _casoDeUsoListarCodafListaPresenca.ExecutarAsync(filtroDto);

            resultado.Should().NotBeNull();
            resultado.Sucesso.Should().BeTrue();
            resultado.Dados?.Items.Should().HaveCount(1);
            resultado.Dados?.Items.First().Should().BeEquivalentTo(itemAutorizado);
            resultado.Dados?.TotalRegistros.Should().Be(1);
            _validadorPermissaoCodafMock.Verify(v => v.ValidarSeUsuarioEhCriador(usuarioLogado, It.IsAny<long>()), Times.Exactly(2));
        }

        [Fact]
        public async Task DadoUsuarioNaoLogado_QuandoChamarExecutar_DeveLancarExcecaoNegocio()
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
                TamanhoPagina = filtroDto.NumeroRegistros
            };
            var resultadoRepositorio = new ResultadoPaginado<ListagemResultadoCodafListaPresencaDto>
            {
                Itens = [],
                TotalRegistros = 0
            };

            _mapperMock.Setup(m => m.Map<FiltroListagemResultadoCodafListaPresencaDto>(filtroDto))
                .Returns(filtroRepositorioDto);
            _repositorioCodafListaPresencaMock.Setup(r => r.ObterListagemResultadoCodafListaPresencaPorFiltroAsync(filtroRepositorioDto))
                .ReturnsAsync(resultadoRepositorio);
            _mediatorMock.Setup(m => m.Send(It.IsAny<ObterUsuarioLogadoQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Usuario)null!);

            await FluentAssertions.FluentActions.Invoking(
                async () => await _casoDeUsoListarCodafListaPresenca.ExecutarAsync(filtroDto))
                .Should()
                .ThrowAsync<NegocioException>()
                .WithMessage("*" + MensagemNegocio.USUARIO_NAO_ENCONTRADO + "*");
        }
    }
}
