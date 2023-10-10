using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using SME.ConectaFormacao.Aplicacao;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Dominio.Extensoes;
using SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Usuario.Mocks;
using SME.ConectaFormacao.TesteIntegracao.Setup;
using Xunit;

namespace SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Usuario
{
    public class Ao_recuperar_parametro_sistema : TesteBase
    {
        public Ao_recuperar_parametro_sistema(CollectionFixture collectionFixture) : base(collectionFixture, false)
        {
            UsuarioRecuperarSenhaMock.Montar();
        }

        [Fact(DisplayName = "Parâmetro Sistema - Recuperar por tipo e por ano")]
        public async Task Deve_retornar_parametro_sistema_por_tipo_e_por_ano()
        {
           // arrange
           var valorComunicadoAcaoFormativaTexto = "Declaro a ação formativa está em conformidade com o Comunicado nº1.043, de 16 de dezembro de 2020";

           var valorComunicadoAcaoFormativaUrl = "https://educacao.sme.prefeitura.sp.gov.br/";
           
            var mediator = ServiceProvider.GetService<IMediator>();

            // act 
            var parametroComunicadoAcaoFormativaTexto = await mediator.Send(new ObterParametroSistemaPorTipoEAnoQuery(TipoParametroSistema.ComunicadoAcaoFormativaDescricao, DateTimeExtension.HorarioBrasilia().Year));
            var parametroComunicadoAcaoFormativaUrl = await mediator.Send(new ObterParametroSistemaPorTipoEAnoQuery(TipoParametroSistema.ComunicadoAcaoFormativaUrl, DateTimeExtension.HorarioBrasilia().Year));

            // assert
            parametroComunicadoAcaoFormativaTexto.ShouldNotBeNull();
            parametroComunicadoAcaoFormativaTexto.Valor.Equals(valorComunicadoAcaoFormativaTexto);
            
            parametroComunicadoAcaoFormativaUrl.ShouldNotBeNull();
            parametroComunicadoAcaoFormativaUrl.Valor.Equals(valorComunicadoAcaoFormativaUrl);
        }
    }
}
