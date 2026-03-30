using Shouldly;
using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.TesteIntegracao.Api.Base;
using SME.ConectaFormacao.TesteIntegracao.Setup;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http.Json;
using System.Text;
using Xunit;

namespace SME.ConectaFormacao.TesteIntegracao.Api.Criterios
{
    public class Ao_Consultar_Criterios_Certificacao (ConectaWebApplicationFactory factory) : TesteBaseIntegracao(factory)
    {
        [Fact(DisplayName = "Criterio Certificacao - Deve retornar lista com sucesso para Admin")]
        public async Task Dado_usuario_admin_Quando_consultar_criterios_Entao_deve_retornar_lista_com_sucesso()
        {
            // Arrange
            AutenticarComoAdmin();
            // Act
            var response = await Client.GetAsync("/api/v1/CriterioCertificacao");
            // Assert
            response.EnsureSuccessStatusCode();
            response.StatusCode.ShouldBe(HttpStatusCode.OK);

            var listaCriterios = await response.Content.ReadFromJsonAsync<IEnumerable<RetornoListagemDTO>>();
            listaCriterios.ShouldNotBeNull();
            listaCriterios.ShouldNotBeEmpty();
            listaCriterios.ShouldContain(c => c.Descricao == "Conceito P ou S pela participação e envolvimento");
        }
    }
}
