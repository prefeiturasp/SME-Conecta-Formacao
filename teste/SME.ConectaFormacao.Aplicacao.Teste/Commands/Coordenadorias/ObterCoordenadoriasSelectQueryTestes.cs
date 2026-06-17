using MediatR;
using SME.ConectaFormacao.Aplicacao.Consultas.Coordenadorias.ObterCoordenadoriasSelect;
using SME.ConectaFormacao.Aplicacao.Dtos.Coordenadorias;

namespace SME.ConectaFormacao.Aplicacao.Teste.Commands.Coordenadorias
{
    public class ObterCoordenadoriasSelectQueryTestes
    {
        [Fact]
        public void Deve_Implementar_IRequest_De_Lista_CoordenadoriaDto()
        {
            var query = new ObterCoordenadoriasSelectQuery();

            Assert.IsType<IRequest<List<CoordenadoriaDto>>>(query, exactMatch: false);
        }

        [Fact]
        public void Deve_Criar_Query_Com_Propriedades_Nulas_Por_Padrao()
        {
            var query = new ObterCoordenadoriasSelectQuery();

            Assert.Null(query.Sigla);
            Assert.Null(query.Nome);
        }

        [Fact]
        public void Deve_Atribuir_Valores_As_Propriedades()
        {
            var query = new ObterCoordenadoriasSelectQuery
            {
                Sigla = "DRE",
                Nome = "Diretoria Regional"
            };

            Assert.Equal("DRE", query.Sigla);
            Assert.Equal("Diretoria Regional", query.Nome);
        }
    }
}
