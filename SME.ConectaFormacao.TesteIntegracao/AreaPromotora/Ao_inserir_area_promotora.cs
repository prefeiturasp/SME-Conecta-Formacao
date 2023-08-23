using Shouldly;
using SME.ConectaFormacao.Aplicacao.Interfaces.AreaPromotora;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Excecoes;
using SME.ConectaFormacao.Dominio.Extensoes;
using SME.ConectaFormacao.TesteIntegracao.AreaPromotora.Mock;
using SME.ConectaFormacao.TesteIntegracao.Setup;
using Xunit;

namespace SME.ConectaFormacao.TesteIntegracao.AreaPromotora
{
    public class Ao_inserir_area_promotora : TesteBase
    {
        public Ao_inserir_area_promotora(CollectionFixture collectionFixture) : base(collectionFixture)
        {
        }

        [Fact(DisplayName = "Área promotora - Deve inserir área promotora válida")]
        public async Task Deve_inserir_area_promotora_valida()
        {
            // arrange 
            var areaPromotoraDTO = AreaPromotoraSalvarMock.GerarAreaPromotoraDTOValido();
            var casoDeUso = ObterCasoDeUso<ICasoDeUsoInserirAreaPromotora>();

            // act
            var id = await casoDeUso.Executar(areaPromotoraDTO);

            // assert
            id.ShouldBeGreaterThan(0);

            var areaPromotora = ObterPorId<Dominio.Entidades.AreaPromotora, long>(id);
            areaPromotora.ShouldNotBeNull();

            areaPromotora.Nome.ShouldBe(areaPromotoraDTO.Nome);
            areaPromotora.Email.ShouldBe(areaPromotoraDTO.Email);
            areaPromotora.Tipo.ShouldBe(areaPromotoraDTO.Tipo);
            areaPromotora.GrupoId.ShouldBe(areaPromotoraDTO.GrupoId);

            var telefones = ObterTodos<AreaPromotoraTelefone>();
            telefones.ShouldNotBeNull();

            foreach (var telefone in telefones)
            {
                telefone.AreaPromotoraId.ShouldBe(id);

                var areaPromotoraTelefoneDTO = areaPromotoraDTO.Telefones.FirstOrDefault(t => t.Telefone.SomenteNumeros() == telefone.Telefone);
                areaPromotoraTelefoneDTO.ShouldNotBeNull();
            }
        }

        [Fact(DisplayName = "Área promotora - Deve retornar exceções preenchimento inválido ao inserir")]
        public async Task Deve_retornar_excecoes_preenchimento_invalido_ao_inserir()
        {
            // arrange 
            var areaPromotoraDTO = AreaPromotoraSalvarMock.GerarAreaPromotoraDTOInvalido();
            var casoDeUso = ObterCasoDeUso<ICasoDeUsoInserirAreaPromotora>();

            // act
            var excecao = await Should.ThrowAsync<NegocioException>(() => casoDeUso.Executar(areaPromotoraDTO));

            // assert
            excecao.ShouldNotBeNull();

            excecao.Mensagens.Contains("É nescessário informar o nome para inserir a área promotora");
            excecao.Mensagens.Contains("É nescessário informar o perfil para inserir a área promotora");
            excecao.Mensagens.Contains("É nescessário informar um email válido para inserir a área promotora");
        }
    }
}
