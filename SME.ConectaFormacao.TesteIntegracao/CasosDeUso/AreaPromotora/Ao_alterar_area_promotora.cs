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
    public class Ao_alterar_area_promotora : TesteBase
    {
        public Ao_alterar_area_promotora(CollectionFixture collectionFixture) : base(collectionFixture)
        {
        }

        [Fact(DisplayName = "Área promotora - Deve alterar área promotora válida rede parceira com email sem dominio @sme e @edu.sme")]
        public async Task Deve_alterar_area_promotora_valida_rede_parceira_com_email_sem_dominio_sme()
        {
            // arrange 
            var areaPromotoraDTO = AreaPromotoraSalvarMock.GerarAreaPromotoraDTOValido(Dominio.Enumerados.AreaPromotoraTipo.RedeParceria);

            var areaPromotora = AreaPromotoraMock.GerarAreaPromotora();
            await InserirNaBase(areaPromotora);

            var areaPromotoraTelefoneExcluir = AreaPromotoraMock.GerarAreaTelefone(areaPromotora.Id);
            await InserirNaBase(areaPromotoraTelefoneExcluir);

            var areaPromotoraTelefoneManter = AreaPromotoraMock.GerarAreaTelefone(areaPromotora.Id, areaPromotoraDTO.Telefones.FirstOrDefault().Telefone);
            await InserirNaBase(areaPromotoraTelefoneManter);

            var casoDeUso = ObterCasoDeUso<ICasoDeUsoAlterarAreaPromotora>();

            // act
            await casoDeUso.Executar(areaPromotora.Id, areaPromotoraDTO);

            // assert
            var areaPromotoraRetorno = ObterPorId<Dominio.Entidades.AreaPromotora, long>(areaPromotora.Id);
            areaPromotoraRetorno.ShouldNotBeNull();

            areaPromotoraRetorno.Nome.ShouldBe(areaPromotoraDTO.Nome);
            areaPromotoraRetorno.Email.ShouldBe(string.Join(";", areaPromotoraDTO.Emails.Select(t => t.Email)));
            areaPromotoraRetorno.Tipo.ShouldBe(areaPromotoraDTO.Tipo);
            areaPromotoraRetorno.GrupoId.ShouldBe(areaPromotoraDTO.GrupoId);

            var telefones = ObterTodos<AreaPromotoraTelefone>();
            telefones.ShouldNotBeNull();

            telefones.Any(t => t.Telefone == areaPromotoraTelefoneExcluir.Telefone && t.Excluido).ShouldBeTrue();

            telefones = telefones.Where(x => !x.Excluido).ToList();

            telefones.Count.ShouldBe(areaPromotoraDTO.Telefones.Count());
            foreach (var telefone in telefones)
            {
                telefone.AreaPromotoraId.ShouldBe(areaPromotora.Id);

                var areaPromotoraTelefoneDTO = areaPromotoraDTO.Telefones.FirstOrDefault(t => t.Telefone.SomenteNumeros() == telefone.Telefone);
                areaPromotoraTelefoneDTO.ShouldNotBeNull();
            }

            telefones.Any(t => t.Telefone == areaPromotoraTelefoneManter.Telefone).ShouldBeTrue();
        }

        [Fact(DisplayName = "Área promotora - Deve alterar área promotora válida rede direta com email com dominio @sme e @edu.sme")]
        public async Task Deve_alterar_area_promotora_valida_rede_direta_com_email_com_dominio_sme()
        {
            // arrange 
            var areaPromotoraDTO = AreaPromotoraSalvarMock.GerarAreaPromotoraDTOValido(Dominio.Enumerados.AreaPromotoraTipo.RedeDireta, "@sme.com.br");

            var areaPromotora = AreaPromotoraMock.GerarAreaPromotora();
            await InserirNaBase(areaPromotora);

            var areaPromotoraTelefoneExcluir = AreaPromotoraMock.GerarAreaTelefone(areaPromotora.Id);
            await InserirNaBase(areaPromotoraTelefoneExcluir);

            var areaPromotoraTelefoneManter = AreaPromotoraMock.GerarAreaTelefone(areaPromotora.Id, areaPromotoraDTO.Telefones.FirstOrDefault().Telefone);
            await InserirNaBase(areaPromotoraTelefoneManter);

            var casoDeUso = ObterCasoDeUso<ICasoDeUsoAlterarAreaPromotora>();

            // act
            await casoDeUso.Executar(areaPromotora.Id, areaPromotoraDTO);

            // assert
            var areaPromotoraRetorno = ObterPorId<Dominio.Entidades.AreaPromotora, long>(areaPromotora.Id);
            areaPromotoraRetorno.ShouldNotBeNull();

            areaPromotoraRetorno.Nome.ShouldBe(areaPromotoraDTO.Nome);
            areaPromotoraRetorno.Email.ShouldBe(string.Join(";", areaPromotoraDTO.Emails.Select(t => t.Email)));
            areaPromotoraRetorno.Tipo.ShouldBe(areaPromotoraDTO.Tipo);
            areaPromotoraRetorno.GrupoId.ShouldBe(areaPromotoraDTO.GrupoId);

            var telefones = ObterTodos<AreaPromotoraTelefone>();
            telefones.ShouldNotBeNull();

            telefones.Any(t => t.Telefone == areaPromotoraTelefoneExcluir.Telefone && t.Excluido).ShouldBeTrue();

            telefones = telefones.Where(x => !x.Excluido).ToList();

            telefones.Count.ShouldBe(areaPromotoraDTO.Telefones.Count());
            foreach (var telefone in telefones)
            {
                telefone.AreaPromotoraId.ShouldBe(areaPromotora.Id);

                var areaPromotoraTelefoneDTO = areaPromotoraDTO.Telefones.FirstOrDefault(t => t.Telefone.SomenteNumeros() == telefone.Telefone);
                areaPromotoraTelefoneDTO.ShouldNotBeNull();
            }

            telefones.Any(t => t.Telefone == areaPromotoraTelefoneManter.Telefone).ShouldBeTrue();
        }

        [Fact(DisplayName = "Área promotora - Deve retornar exceções ao alterar email informado fora do dominio sme quando tipo for rede direta")]
        public async Task Deve_retornar_excecoes_area_promotora_tipo_rede_direta_com_email_fora_do_dominio_sme()
        {
            // arrange 
            var areaPromotoraDTO = AreaPromotoraSalvarMock.GerarAreaPromotoraDTOValido(Dominio.Enumerados.AreaPromotoraTipo.RedeDireta);

            var areaPromotora = AreaPromotoraMock.GerarAreaPromotora();
            await InserirNaBase(areaPromotora);

            var casoDeUso = ObterCasoDeUso<ICasoDeUsoAlterarAreaPromotora>();

            // act
            var excecao = await Should.ThrowAsync<NegocioException>(() => casoDeUso.Executar(areaPromotora.Id, areaPromotoraDTO));

            // assert
            excecao.ShouldNotBeNull();
            excecao.Mensagens.Contains(MensagemNegocio.AREA_PROMOTORA_EMAIL_FORA_DOMINIO_REDE_DIRETA).ShouldBeTrue();
        }

        [Fact(DisplayName = "Área promotora - Deve retornar exceções area promotora não encontrada")]
        public async Task Deve_retornar_excecoes_area_promotora_nao_encontrada()
        {
            // arrange 
            var id = AreaPromotoraSalvarMock.GerarIdAleatorio();
            var areaPromotoraDTO = AreaPromotoraSalvarMock.GerarAreaPromotoraDTOValido(Dominio.Enumerados.AreaPromotoraTipo.RedeParceria);

            var casoDeUso = ObterCasoDeUso<ICasoDeUsoAlterarAreaPromotora>();

            // act
            var excecao = await Should.ThrowAsync<NegocioException>(() => casoDeUso.Executar(id, areaPromotoraDTO));

            // assert
            excecao.ShouldNotBeNull();
            excecao.Mensagens.Contains(MensagemNegocio.AREA_PROMOTORA_NAO_ENCONTRADA).ShouldBeTrue();
        }

        [Fact(DisplayName = "Área promotora - Deve retornar exceções preenchimento inválido ao retornar")]
        public async Task Deve_retornar_excecoes_preenchimento_invalido_ao_alterar()
        {
            // arrange 
            var areaPromotora = AreaPromotoraMock.GerarAreaPromotora();
            await InserirNaBase(areaPromotora);

            var areaPromotoraDTO = AreaPromotoraSalvarMock.GerarAreaPromotoraDTOInvalido();
            var casoDeUso = ObterCasoDeUso<ICasoDeUsoAlterarAreaPromotora>();

            // act
            var excecao = await Should.ThrowAsync<NegocioException>(() => casoDeUso.Executar(areaPromotora.Id, areaPromotoraDTO));

            // assert
            excecao.ShouldNotBeNull();

            excecao.Mensagens.Contains("É nescessário informar a área promotora para alterar").ShouldBeTrue();
            excecao.Mensagens.Contains("É nescessário informar o perfil para alterar a área promotora").ShouldBeTrue();
        }

        [Fact(DisplayName = "Área promotora - Deve retornar exceções ao alterar um grupo existente")]
        public async Task Deve_retornar_excecoes_alterar_grupo_existente()
        {
            // arrange 
            var areasPromotora = AreaPromotoraMock.GerarAreaPromotora(2);
            await InserirNaBase(areasPromotora);

            var areaPromotora = areasPromotora.FirstOrDefault();

            var areaPromotoraDTO = AreaPromotoraSalvarMock.GerarAreaPromotoraDTOValido(Dominio.Enumerados.AreaPromotoraTipo.RedeParceria);

            areaPromotoraDTO.GrupoId = areasPromotora.LastOrDefault().GrupoId;

            var casoDeUso = ObterCasoDeUso<ICasoDeUsoAlterarAreaPromotora>();

            // act
            var excecao = await Should.ThrowAsync<NegocioException>(() => casoDeUso.Executar(areaPromotora.Id, areaPromotoraDTO));

            // assert
            excecao.ShouldNotBeNull();

            excecao.Mensagens.Contains(MensagemNegocio.AREA_PROMOTORA_EXISTE_GRUPO_CADASTRADO).ShouldBeTrue();
        }
    }
}
