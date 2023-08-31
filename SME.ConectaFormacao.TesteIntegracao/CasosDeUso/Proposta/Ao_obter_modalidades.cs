﻿using Shouldly;
using SME.ConectaFormacao.Aplicacao.Interfaces.Proposta;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.TesteIntegracao.Setup;
using Xunit;

namespace SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Proposta
{
    public class Ao_obter_modalidades : TesteBase
    {
        public Ao_obter_modalidades(CollectionFixture collectionFixture) : base(collectionFixture)
        {
        }

        [Fact(DisplayName = "Proposta - obter modalidades")]
        public async Task Deve_obter_modalidades()
        {
            // arrange 
            var casoDeUso = ObterCasoDeUso<ICasoDeUsoObterModalidades>();

            // act 
            var retorno = await casoDeUso.Executar();

            // assert 
            retorno.Any(t => t.Id == (long)Modalidade.Distancia).ShouldBeTrue();
            retorno.Any(t => t.Id == (long)Modalidade.Hibrido).ShouldBeTrue();
            retorno.Any(t => t.Id == (long)Modalidade.Presencial).ShouldBeTrue();
        }
    }
}
