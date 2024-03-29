﻿using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Shouldly;
using SME.ConectaFormacao.Aplicacao;
using SME.ConectaFormacao.Aplicacao.Dtos.Grupo;
using SME.ConectaFormacao.Aplicacao.Interfaces.Grupo;
using SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Grupo.Mocks;
using SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Grupo.ServicosFakes;
using SME.ConectaFormacao.TesteIntegracao.Setup;
using Xunit;

namespace SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Grupo
{
    public class Ao_obter_o_grupo : TesteBase
    {
        public Ao_obter_o_grupo(CollectionFixture collectionFixture) : base(collectionFixture)
        {
            AoObterGrupoMock.Montar();
        }

        protected override void RegistrarQueryFakes(IServiceCollection services)
        {
            base.RegistrarQueryFakes(services);
            services.Replace(new ServiceDescriptor(typeof(IRequestHandler<ObterGruposServicoAcessosQuery, IEnumerable<GrupoDTO>>), typeof(ObterGruposServicoAcessosQueryHandlerFake), ServiceLifetime.Scoped));
        }

        [Fact(DisplayName = "Grupo - Deve retornar os grupos do sistema")]
        public async Task Deve_retornar_os_grupos_do_sistema()
        {
            // arrange
            var grupoGestao = AoObterGrupoMock.Grupos.Take(2);

            var casoDeUso = ObterCasoDeUso<ICasoDeUsoObterGrupoSistema>();

            // act
            var grupos = await casoDeUso.Executar();

            // assert
            grupos.Any().ShouldBeTrue();

            grupos.Count().ShouldBe(5);
        }
    }
}
