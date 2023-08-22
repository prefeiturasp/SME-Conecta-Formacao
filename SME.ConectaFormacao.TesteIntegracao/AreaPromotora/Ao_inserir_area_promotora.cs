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
            AreaPromotoraInserirMock.Montar();
        }

        [Fact(DisplayName = "Área promotora - Deve inserir área promotora válida")]
        public async Task Deve_inserir_area_promotora_valida()
        {
            // arrange 
            var inserirAreaPromotoraDTO = AreaPromotoraInserirMock.InserirAreaPromotoraDTOValido;
            var casoDeUso = ObterCasoDeUso<ICasoDeUsoInserirAreaPromotora>();

            // act
            var id = await casoDeUso.Executar(inserirAreaPromotoraDTO);

            // assert
            id.ShouldBeGreaterThan(0);

            var areaPromotora = ObterPorId<Dominio.Entidades.AreaPromotora, long>(id);
            areaPromotora.ShouldNotBeNull();

            areaPromotora.Nome.ShouldBe(inserirAreaPromotoraDTO.Nome);
            areaPromotora.Email.ShouldBe(inserirAreaPromotoraDTO.Email);
            areaPromotora.Tipo.ShouldBe(inserirAreaPromotoraDTO.Tipo);
            areaPromotora.GrupoId.ShouldBe(inserirAreaPromotoraDTO.GrupoId);

            var telefones = ObterTodos<AreaPromotoraTelefone>();
            telefones.ShouldNotBeNull();

            foreach (var telefone in telefones)
            {
                telefone.AreaPromotoraId.ShouldBe(id);

                var inserirAreaPromotoraTelefoneDTO = inserirAreaPromotoraDTO.Telefones.FirstOrDefault(t => t.Telefone.SomenteNumeros() == telefone.Telefone);
                inserirAreaPromotoraTelefoneDTO.ShouldNotBeNull();
            }
        }

        [Fact(DisplayName = "Área promotora - Deve retornar exceções preenchimento inválido ao inserir")]
        public async Task Deve_retornar_excecoes_preenchimento_invalido_ao_inserir()
        {
            // arrange 
            var inserirAreaPromotoraDTO = AreaPromotoraInserirMock.InserirAreaPromotoraDTOInvalido;
            var casoDeUso = ObterCasoDeUso<ICasoDeUsoInserirAreaPromotora>();

            // act
            var excecao = await Should.ThrowAsync<NegocioException>(() => casoDeUso.Executar(inserirAreaPromotoraDTO));

            // assert
            excecao.ShouldNotBeNull();

            excecao.Mensagens.Contains("É nescessário informar o nome para inserir a área promotora");
            excecao.Mensagens.Contains("É nescessário informar o perfil para inserir a área promotora");
            excecao.Mensagens.Contains("É nescessário informar um email válido para inserir a área promotora");
        }
    }
}
