using Shouldly;
using SME.ConectaFormacao.Aplicacao.Dtos.UsuarioRedeParceria;
using SME.ConectaFormacao.Aplicacao.Interfaces.AreaPromotora;
using SME.ConectaFormacao.Aplicacao.Interfaces.UsuarioRedeParceria;
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
    public class Ao_obter_usuario_parceria_paginada : TesteBase
    {
        public Ao_obter_usuario_parceria_paginada(CollectionFixture collectionFixture, bool limparBanco = true) : base(collectionFixture, limparBanco)
        {
        }

        [Fact(DisplayName = "Usuário Rede Parceria - Deve retornar registros consulta paginada com filtro")]
        public async Task Deve_retornar_registros_com_filtro()
        {
            // arrange
            var areaPromotora = AreaPromotoraMock.GerarAreaPromotora();
            await InserirNaBase(areaPromotora);

            var usuarios = UsuarioMock.GerarUsuario(areaPromotora, Dominio.Enumerados.TipoUsuario.RedeParceria, 5);
            await InserirNaBase(usuarios);

            var usuarioFiltro = usuarios.FirstOrDefault();

            var filtro = new FiltroUsuarioRedeParceriaDTO()
            {
                AreaPromotoraIds = new[] { areaPromotora.Id },
                Nome = usuarioFiltro.Nome,
                Cpf = usuarioFiltro.Cpf,
                Situacao = usuarioFiltro.Situacao
            };

            var casoDeUso = ObterCasoDeUso<ICasoDeUsoObterUsuarioRedeParceriaPaginada>();

            // act
            var retorno = await casoDeUso.Executar(filtro);

            // assert
            retorno.Items.Any().ShouldBeTrue();
            retorno.TotalPaginas.ShouldBe(1);
        }

        [Fact(DisplayName = "Usuário Rede Parceria - Deve retornar registros consulta paginada sem filtro")]
        public async Task Deve_retornar_registros_sem_filtro()
        {
            // arrange
            var areaPromotora = AreaPromotoraMock.GerarAreaPromotora();
            await InserirNaBase(areaPromotora);

            var usuarios = UsuarioMock.GerarUsuario(areaPromotora, Dominio.Enumerados.TipoUsuario.RedeParceria, 5);
            await InserirNaBase(usuarios);

            var filtro = new FiltroUsuarioRedeParceriaDTO();

            var casoDeUso = ObterCasoDeUso<ICasoDeUsoObterUsuarioRedeParceriaPaginada>();

            // act
            var retorno = await casoDeUso.Executar(filtro);

            // assert
            retorno.Items.Any().ShouldBeTrue();
            retorno.Items.Count().ShouldBe(5);
            retorno.TotalPaginas.ShouldBe(1);
        }
    }
}
