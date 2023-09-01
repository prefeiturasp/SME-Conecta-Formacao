using Bogus;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Dominio.Extensoes;

namespace SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Proposta.Mocks
{
    public class PropostaMock
    {
        private static Faker<Dominio.Entidades.Proposta> Gerador(
            long areaPromotoraId,
           TipoFormacao tipoFormacao,
           Modalidade modalidade,
           SituacaoRegistro situacao,
           bool gerarFuncaoEspecificaOutros,
           bool gerarCriterioValidacaoInscricaoOutros)
        {
            var faker = new Faker<Dominio.Entidades.Proposta>();
            faker.RuleFor(x => x.AreaPromotoraId, areaPromotoraId);
            faker.RuleFor(x => x.TipoFormacao, tipoFormacao);
            faker.RuleFor(x => x.Modalidade, modalidade);
            faker.RuleFor(x => x.TipoInscricao, f => f.PickRandom<TipoInscricao>());
            faker.RuleFor(x => x.NomeFormacao, f => f.Lorem.Sentence(3));
            faker.RuleFor(x => x.QuantidadeTurmas, f => f.Random.Number(1, 999));
            faker.RuleFor(x => x.QuantidadeVagasTurma, f => f.Random.Number(1, 999));
            faker.RuleFor(x => x.Excluido, false);
            faker.RuleFor(x => x.CriadoPor, f => f.Name.FullName());
            faker.RuleFor(x => x.CriadoEm, DateTimeExtension.HorarioBrasilia());
            faker.RuleFor(x => x.CriadoLogin, f => f.Name.FirstName());
            faker.RuleFor(x => x.AlteradoPor, f => f.Name.FullName());
            faker.RuleFor(x => x.AlteradoEm, DateTimeExtension.HorarioBrasilia());
            faker.RuleFor(x => x.AlteradoLogin, f => f.Name.FirstName());

            if (gerarFuncaoEspecificaOutros)
                faker.RuleFor(x => x.FuncaoEspecificaOutros, f => f.Lorem.Sentence(3));

            if (gerarCriterioValidacaoInscricaoOutros)
                faker.RuleFor(x => x.CriterioValidacaoInscricaoOutros, f => f.Lorem.Sentence(3));

            faker.RuleFor(x => x.Situacao, situacao);

            return faker;
        }

        public static Dominio.Entidades.Proposta GerarPropostaValida(long areaPromotoraId,
            TipoFormacao tipoFormacao,
            Modalidade modalidade,
            SituacaoRegistro situacao,
            bool gerarFuncaoEspecificaOutros,
            bool gerarCriterioValidacaoInscricaoOutros)
        {
            return Gerador(areaPromotoraId, tipoFormacao, modalidade, situacao, gerarFuncaoEspecificaOutros, gerarCriterioValidacaoInscricaoOutros);
        }

        public static Dominio.Entidades.Proposta GerarPropostaRascunho(long areaPromotoraId)
        {
            var faker = new Faker<Dominio.Entidades.Proposta>();
            faker.RuleFor(x => x.AreaPromotoraId, areaPromotoraId);
            faker.RuleFor(x => x.Excluido, false);
            faker.RuleFor(x => x.CriadoPor, f => f.Name.FullName());
            faker.RuleFor(x => x.CriadoEm, DateTimeExtension.HorarioBrasilia());
            faker.RuleFor(x => x.CriadoLogin, f => f.Name.FirstName());
            faker.RuleFor(x => x.AlteradoPor, f => f.Name.FullName());
            faker.RuleFor(x => x.AlteradoEm, DateTimeExtension.HorarioBrasilia());
            faker.RuleFor(x => x.AlteradoLogin, f => f.Name.FirstName());

            return faker.Generate();
        }
    }
}
