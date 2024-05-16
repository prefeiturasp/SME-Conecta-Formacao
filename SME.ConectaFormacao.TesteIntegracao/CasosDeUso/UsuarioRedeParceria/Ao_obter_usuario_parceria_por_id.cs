using Shouldly;
using SME.ConectaFormacao.Aplicacao.Interfaces.UsuarioRedeParceria;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Excecoes;
using SME.ConectaFormacao.TesteIntegracao.Mocks;
using SME.ConectaFormacao.TesteIntegracao.Setup;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace SME.ConectaFormacao.TesteIntegracao.CasosDeUso.UsuarioRedeParceria
{
    public class Ao_obter_usuario_parceria_por_id : TesteBase
    {
        public Ao_obter_usuario_parceria_por_id(CollectionFixture collectionFixture, bool limparBanco = true) : base(collectionFixture, limparBanco)
        {
        }

        [Fact(DisplayName = "Usuário Rede Parceria - obter por id")]
        public async Task Deve_obter_por_id()
        {
            // arrange
            var areaPromotora = AreaPromotoraMock.GerarAreaPromotora();
            await InserirNaBase(areaPromotora);

            var usuarios = UsuarioMock.GerarUsuario(areaPromotora, Dominio.Enumerados.TipoUsuario.RedeParceria);
            await InserirNaBase(usuarios);

            var casoDeUso = ObterCasoDeUso<ICasoDeUsoObterUsuarioRedeParceriaPorId>();

            // act
            var retorno = await casoDeUso.Executar(usuarios.FirstOrDefault().Id);

            // assert
            retorno.AreaPromotoraId.ShouldBe(usuarios.FirstOrDefault().AreaPromotoraId.Value);
            retorno.Nome.ShouldBe(usuarios.FirstOrDefault().Nome);
            retorno.Cpf.ShouldBe(usuarios.FirstOrDefault().Cpf);
            retorno.Telefone.ShouldBe(usuarios.FirstOrDefault().Telefone);
        }

        [Fact(DisplayName = "Usuário Rede Parceria - deve retornar excecao usuario nao encontrado")]
        public async Task Deve_retornar_excecao_usuario_nao_encontrado()
        {
            // arrange
            var casoDeUso = ObterCasoDeUso<ICasoDeUsoObterUsuarioRedeParceriaPorId>();

            // act
            var retorno = await Should.ThrowAsync<NegocioException>(casoDeUso.Executar(1));

            // assert
            retorno.Mensagens.Contains(MensagemNegocio.USUARIO_NAO_ENCONTRADO);
        }

        [Fact(DisplayName = "Usuário Rede Parceria - deve retornar excecao usuario nao encontrado quando tipo for diferente de rede parceria")]
        public async Task Deve_retornar_excecao_usuario_nao_encontrado_tipo_diferente_rede_parceria()
        {
            // arrange
            var areaPromotora = AreaPromotoraMock.GerarAreaPromotora();
            await InserirNaBase(areaPromotora);

            var usuarios = UsuarioMock.GerarUsuario(areaPromotora, Dominio.Enumerados.TipoUsuario.Externo);
            await InserirNaBase(usuarios);

            var casoDeUso = ObterCasoDeUso<ICasoDeUsoObterUsuarioRedeParceriaPorId>();

            // act
            var retorno = await Should.ThrowAsync<NegocioException>(casoDeUso.Executar(usuarios.FirstOrDefault().Id));

            // assert
            retorno.Mensagens.Contains(MensagemNegocio.USUARIO_NAO_ENCONTRADO);
        }
    }
}
