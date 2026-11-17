using FluentAssertions;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Infra.Dados.Dtos.CodafConferencias;

namespace SME.ConectaFormacao.Infra.Dados.Teste.Dtos
{
    public class CodafConferenciaDtosTestes
    {
        [Fact]
        public void DadoResultadoInscritoTurmaCodafConferenciaDto_QuandoPossuirNomeSocial_EntaoNomeExibicaoDeveRetornarNomeSocial()
        {
            // Arrange
            var dto = new ResultadoInscritoTurmaCodafConferenciaDto
            {
                Id = 1,
                Login = "1234567",
                Cpf = "12345678900",
                Nome = "Nome Civil",
                NomeSocial = "Nome Social",
                Participou = true
            };

            // Assert
            dto.Id.Should().Be(1);
            dto.Login.Should().Be("1234567");
            dto.Cpf.Should().Be("12345678900");
            dto.Nome.Should().Be("Nome Civil");
            dto.NomeSocial.Should().Be("Nome Social");
            dto.NomeExibicao.Should().Be("Nome Social");
            dto.Participou.Should().BeTrue();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void DadoResultadoInscritoTurmaCodafConferenciaDto_QuandoNaoPossuirNomeSocial_EntaoNomeExibicaoDeveRetornarNome(string? nomeSocial)
        {
            // Arrange
            var dto = new ResultadoInscritoTurmaCodafConferenciaDto
            {
                Id = 1,
                Login = "1234567",
                Cpf = "12345678900",
                Nome = "Nome Civil",
                NomeSocial = nomeSocial,
                Participou = null
            };

            // Assert
            dto.NomeExibicao.Should().Be("Nome Civil");
            dto.Participou.Should().BeNull();
        }

        [Fact]
        public void DadoFiltroEListagemResultadoCodafConferenciaDto_QuandoAtribuirPropriedades_EntaoDeveManterValores()
        {
            // Arrange
            var data = DateTime.UtcNow;
            var filtro = new FiltroListagemResultadoCodafConferenciaDto
            {
                NomeFormacao = "Conferência Educação",
                CodigoFormacao = "10",
                NumeroHomologacao = "20261",
                PropostaTurmaId = 5,
                AreaPromotoraId = 2,
                Status = StatusCodafConferencia.Aguardando,
                DataCriacao = data,
                Pagina = 1,
                TamanhoPagina = 10
            };

            var item = new ListagemResultadoCodafConferenciaDto
            {
                Id = 100,
                NumeroHomologacao = 20261,
                NomeFormacao = "Conferência Educação",
                CodigoFormacao = 10,
                NomeTurma = "Turma 1",
                NomeAreaPromotora = "DRE Ipiranga",
                Status = StatusCodafConferencia.Aguardando,
                CodigoCursoEol = 999,
                CodigoNivel = 1,
                PossuiAprovacoes = true,
                CriadoEm = data
            };

            // Assert
            filtro.NomeFormacao.Should().Be("Conferência Educação");
            filtro.CodigoFormacao.Should().Be("10");
            filtro.NumeroHomologacao.Should().Be("20261");
            filtro.PropostaTurmaId.Should().Be(5);
            filtro.AreaPromotoraId.Should().Be(2);
            filtro.Status.Should().Be(StatusCodafConferencia.Aguardando);
            filtro.DataCriacao.Should().Be(data);
            filtro.Pagina.Should().Be(1);
            filtro.TamanhoPagina.Should().Be(10);

            item.Id.Should().Be(100);
            item.NumeroHomologacao.Should().Be(20261);
            item.NomeFormacao.Should().Be("Conferência Educação");
            item.CodigoFormacao.Should().Be(10);
            item.NomeTurma.Should().Be("Turma 1");
            item.NomeAreaPromotora.Should().Be("DRE Ipiranga");
            item.Status.Should().Be(StatusCodafConferencia.Aguardando);
            item.CodigoCursoEol.Should().Be(999);
            item.CodigoNivel.Should().Be(1);
            item.PossuiAprovacoes.Should().BeTrue();
            item.CriadoEm.Should().Be(data);
        }
    }
}
