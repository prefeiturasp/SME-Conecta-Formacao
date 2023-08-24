using Shouldly;
using SME.ConectaFormacao.Aplicacao.Dtos.AreaPromotora;
using SME.ConectaFormacao.Aplicacao.Interfaces.AreaPromotora;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Excecoes;
using SME.ConectaFormacao.Dominio.Extensoes;
using SME.ConectaFormacao.TesteIntegracao.AreaPromotora.Mock;
using SME.ConectaFormacao.TesteIntegracao.Setup;
using Xunit;

namespace SME.ConectaFormacao.TesteIntegracao.AreaPromotora
{
    public class Ao_obter_area_promotora : TesteBase
    {
        public Ao_obter_area_promotora(CollectionFixture collectionFixture) : base(collectionFixture)
        {
        }

        [Fact(DisplayName = "Área promotora - Deve obter por id com telefone válido")]
        public async Task Deve_obter_por_id_com_telefone_valido()
        {
            // arrange 
            var areaPromotora = AreaPromotoraMock.GerarAreaPromotora();
            await InserirNaBase(areaPromotora);

            var telefone = AreaPromotoraMock.GerarAreaTelefone(areaPromotora.Id);
            await InserirNaBase(telefone);

            var casoDeUso = ObterCasoDeUso<ICasoDeUsoObterAreaPromotoraPorId>();

            // act 
            var retorno = await casoDeUso.Executar(areaPromotora.Id);

            // assert 
            retorno.ShouldNotBeNull();
            retorno.Nome.ShouldBe(areaPromotora.Nome);
            retorno.Tipo.ShouldBe(areaPromotora.Tipo);
            retorno.Email.ShouldBe(areaPromotora.Email);
            retorno.GrupoId.ShouldBe(areaPromotora.GrupoId);
            retorno.Telefones.FirstOrDefault().Telefone.ShouldBe(telefone.Telefone.AplicarMascara(@"\(00\) 00000\-0000"));

            ValidarAuditoria(areaPromotora, retorno.Auditoria);
        }

        [Fact(DisplayName = "Área promotora - Deve obter por id sem telefone válido")]
        public async Task Deve_obter_por_id_sem_telefone_valido()
        {
            // arrange 
            var areaPromotora = AreaPromotoraMock.GerarAreaPromotora();
            await InserirNaBase(areaPromotora);

            var casoDeUso = ObterCasoDeUso<ICasoDeUsoObterAreaPromotoraPorId>();

            // act 
            var retorno = await casoDeUso.Executar(areaPromotora.Id);

            // assert 
            retorno.ShouldNotBeNull();
            retorno.Nome.ShouldBe(areaPromotora.Nome);
            retorno.Tipo.ShouldBe(areaPromotora.Tipo);
            retorno.Email.ShouldBe(areaPromotora.Email);
            retorno.GrupoId.ShouldBe(areaPromotora.GrupoId);
            retorno.Telefones.Any().ShouldBeFalse();

            ValidarAuditoria(areaPromotora, retorno.Auditoria);
        }

        [Fact(DisplayName = "Área promotora - Deve retornar exceção ao obter por id inválido")]
        public async Task Deve_retornar_excecao_ao_obter_por_id_invalido()
        {
            // arrange 
            var idAleatorio = AreaPromotoraSalvarMock.GerarIdAleatorio();

            var casoDeUso = ObterCasoDeUso<ICasoDeUsoObterAreaPromotoraPorId>();

            // act 
            var excecao = await Should.ThrowAsync<NegocioException>(casoDeUso.Executar(idAleatorio));

            // assert 
            excecao.ShouldNotBeNull();
            excecao.Mensagens.Contains(MensagemNegocio.AREA_PROMOTORA_NAO_ENCONTRADA);
        }

        private static void ValidarAuditoria(Dominio.EntidadeBaseAuditavel entidadeAuditavel, AuditoriaDTO auditoriaDTO)
        {
            auditoriaDTO.CriadoEm.Date.ShouldBe(entidadeAuditavel.CriadoEm.Date);
            auditoriaDTO.CriadoEm.Hour.ShouldBe(entidadeAuditavel.CriadoEm.Hour);
            auditoriaDTO.CriadoPor.ShouldBe(entidadeAuditavel.CriadoPor);
            auditoriaDTO.CriadoLogin.ShouldBe(entidadeAuditavel.CriadoLogin);

            auditoriaDTO.AlteradoEm.GetValueOrDefault().Date.ShouldBe(entidadeAuditavel.AlteradoEm.GetValueOrDefault().Date);
            auditoriaDTO.AlteradoEm.GetValueOrDefault().Hour.ShouldBe(entidadeAuditavel.AlteradoEm.GetValueOrDefault().Hour);
            auditoriaDTO.AlteradoPor.ShouldBe(entidadeAuditavel.AlteradoPor);
            auditoriaDTO.AlteradoLogin.ShouldBe(entidadeAuditavel.AlteradoLogin);
        }
    }
}
