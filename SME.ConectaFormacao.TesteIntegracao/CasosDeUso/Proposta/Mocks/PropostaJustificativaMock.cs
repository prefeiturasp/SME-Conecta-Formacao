using Bogus;
using SME.ConectaFormacao.Aplicacao.Dtos.Proposta;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Proposta.Mocks
{
    public static class PropostaJustificativaMock
    {

        public static PropostaJustificativaDTO GerarPropostaJustificativaDTO()
        {
            Faker<PropostaJustificativaDTO> faker = new Faker<PropostaJustificativaDTO>();
            faker.RuleFor(r => r.Justificativa, f => f.Lorem.Sentence());

            return faker.Generate();
        }

    }
}
