using Bogus;
using SME.ConectaFormacao.Dominio.Entidades;

namespace SME.ConectaFormacao.TesteIntegracao.Mocks
{
    public class InscricaoMock : BaseMock
    {
        private static Faker<Inscricao> Gerador(long usuarioId, long propostaTurmaId)
        {
            var faker = new Faker<Inscricao>();

            faker.RuleFor(t => t.UsuarioId, usuarioId);
            faker.RuleFor(t => t.PropostaTurmaId, propostaTurmaId);

            AuditoriaFaker(faker);

            return faker;
        }

        public static Inscricao GerarInscricao(long usuarioId, long propostaTurmaId)
        {
            return Gerador(usuarioId, propostaTurmaId);
        }
    }
}
