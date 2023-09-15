using Shouldly;
using SME.ConectaFormacao.Aplicacao.Interfaces.AreaPromotora;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Excecoes;
using SME.ConectaFormacao.Dominio.Extensoes;
using SME.ConectaFormacao.TesteIntegracao.CasosDeUso.AreaPromotora.Mock;
using SME.ConectaFormacao.TesteIntegracao.Mocks;
using SME.ConectaFormacao.TesteIntegracao.Setup;
using Xunit;

namespace SME.ConectaFormacao.TesteIntegracao.CasosDeUso.AreaPromotora
{
    public class Ao_inserir_area_promotora : TesteBase
    {
        public Ao_inserir_area_promotora(CollectionFixture collectionFixture) : base(collectionFixture)
        {
        }


        [Fact(DisplayName = "Área promotora - Deve inserir área promotora válida rede parceira com email sem dominio @sme e @edu.sme")]
        public async Task Deve_inserir_area_promotora_valida_rede_parceira_com_email_sem_dominio_sme()
        {
            // arrange 
            var areaPromotoraDTO = AreaPromotoraSalvarMock.GerarAreaPromotoraDTOValido(Dominio.Enumerados.AreaPromotoraTipo.RedeParceira);

            var casoDeUso = ObterCasoDeUso<ICasoDeUsoInserirAreaPromotora>();

            // act
            var id = await casoDeUso.Executar(areaPromotoraDTO);

            // assert
            var areaPromotoraRetorno = ObterPorId<Dominio.Entidades.AreaPromotora, long>(id);
            areaPromotoraRetorno.ShouldNotBeNull();

            areaPromotoraRetorno.Nome.ShouldBe(areaPromotoraDTO.Nome);
            areaPromotoraRetorno.Email.ShouldBe(string.Join(";", areaPromotoraDTO.Emails.Select(t => t.Email)));
            areaPromotoraRetorno.Tipo.ShouldBe(areaPromotoraDTO.Tipo);
            areaPromotoraRetorno.GrupoId.ShouldBe(areaPromotoraDTO.GrupoId);

            var telefones = ObterTodos<AreaPromotoraTelefone>();
            telefones.ShouldNotBeNull();

            telefones.Count.ShouldBe(areaPromotoraDTO.Telefones.Count());
            foreach (var telefone in telefones)
            {
                telefone.AreaPromotoraId.ShouldBe(id);

                var areaPromotoraTelefoneDTO = areaPromotoraDTO.Telefones.FirstOrDefault(t => t.Telefone.SomenteNumeros() == telefone.Telefone);
                areaPromotoraTelefoneDTO.ShouldNotBeNull();
            }
        }

        [Fact(DisplayName = "Área promotora - Deve inserir área promotora válida rede direta com email com dominio @sme e @edu.sme")]
        public async Task Deve_inserir_area_promotora_valida_rede_parceira_rede_direta_com_email_com_dominio_sme()
        {
            // arrange 
            var areaPromotoraDTO = AreaPromotoraSalvarMock.GerarAreaPromotoraDTOValido(Dominio.Enumerados.AreaPromotoraTipo.RedeDireta, "@sme.com.br");

            var casoDeUso = ObterCasoDeUso<ICasoDeUsoInserirAreaPromotora>();

            // act
            var id = await casoDeUso.Executar(areaPromotoraDTO);

            // assert
            var areaPromotoraRetorno = ObterPorId<Dominio.Entidades.AreaPromotora, long>(id);
            areaPromotoraRetorno.ShouldNotBeNull();

            areaPromotoraRetorno.Nome.ShouldBe(areaPromotoraDTO.Nome);
            areaPromotoraRetorno.Email.ShouldBe(string.Join(";", areaPromotoraDTO.Emails.Select(t => t.Email)));
            areaPromotoraRetorno.Tipo.ShouldBe(areaPromotoraDTO.Tipo);
            areaPromotoraRetorno.GrupoId.ShouldBe(areaPromotoraDTO.GrupoId);

            var telefones = ObterTodos<AreaPromotoraTelefone>();
            telefones.ShouldNotBeNull();

            telefones.Count.ShouldBe(areaPromotoraDTO.Telefones.Count());
            foreach (var telefone in telefones)
            {
                telefone.AreaPromotoraId.ShouldBe(id);

                var areaPromotoraTelefoneDTO = areaPromotoraDTO.Telefones.FirstOrDefault(t => t.Telefone.SomenteNumeros() == telefone.Telefone);
                areaPromotoraTelefoneDTO.ShouldNotBeNull();
            }
        }

        [Fact(DisplayName = "Área promotora - Deve retornar exceções ao inserir email informado fora do dominio sme quando tipo for rede direta")]
        public async Task Deve_retornar_excecoes_inserir_area_promotora_tipo_rede_direta_com_email_fora_do_dominio_sme()
        {
            // arrange 
            var areaPromotoraDTO = AreaPromotoraSalvarMock.GerarAreaPromotoraDTOValido(Dominio.Enumerados.AreaPromotoraTipo.RedeDireta);

            var casoDeUso = ObterCasoDeUso<ICasoDeUsoInserirAreaPromotora>();

            // act
            var excecao = await Should.ThrowAsync<NegocioException>(() => casoDeUso.Executar(areaPromotoraDTO));

            // assert
            excecao.ShouldNotBeNull();
            excecao.Mensagens.Contains(MensagemNegocio.AREA_PROMOTORA_EMAIL_FORA_DOMINIO_REDE_DIRETA).ShouldBeTrue();
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

            excecao.Mensagens.Contains("É nescessário informar a área promotora para inserir").ShouldBeTrue();
            excecao.Mensagens.Contains("É nescessário informar o perfil para inserir a área promotora").ShouldBeTrue();
        }

        [Fact(DisplayName = "Área promotora - Deve retornar exceções ao inserir um grupo existente")]
        public async Task Deve_retornar_excecoes_inserir_grupo_existente()
        {
            // arrange 
            var areaPromotora = AreaPromotoraMock.GerarAreaPromotora();
            await InserirNaBase(areaPromotora);

            var areaPromotoraDTO = AreaPromotoraSalvarMock.GerarAreaPromotoraDTOValido(Dominio.Enumerados.AreaPromotoraTipo.RedeParceira);

            areaPromotoraDTO.GrupoId = areaPromotora.GrupoId;

            var casoDeUso = ObterCasoDeUso<ICasoDeUsoInserirAreaPromotora>();

            // act
            var excecao = await Should.ThrowAsync<NegocioException>(() => casoDeUso.Executar(areaPromotoraDTO));

            // assert
            excecao.ShouldNotBeNull();

            excecao.Mensagens.Contains(MensagemNegocio.AREA_PROMOTORA_EXISTE_GRUPO_CADASTRADO).ShouldBeTrue();
        }
    }
}
