using Bogus;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Enumerados;

namespace SME.ConectaFormacao.TesteIntegracao.Mocks
{
    public class InscricaoMock : BaseMock
    {
        private static Faker<Inscricao> Gerador(long usuarioId, long propostaTurmaId, SituacaoInscricao? situacao = null)
        {
            var faker = new Faker<Inscricao>();

            faker.RuleFor(t => t.UsuarioId, usuarioId);
            faker.RuleFor(t => t.PropostaTurmaId, propostaTurmaId);
            faker.RuleFor(t => t.Situacao, f => situacao.HasValue ? situacao.Value : f.PickRandom<SituacaoInscricao>());
            faker.RuleFor(t => t.Origem, f => f.PickRandom<OrigemInscricao>());

            AuditoriaFaker(faker);

            return faker;
        }

        public static Inscricao GerarInscricao(long usuarioId, long propostaTurmaId, SituacaoInscricao? situacao = null)
        {
            return Gerador(usuarioId, propostaTurmaId, situacao);
        }

        public static IEnumerable<Inscricao> GerarInscricoes(IEnumerable<Usuario> usuarios, long propostaTurmaId, SituacaoInscricao? situacao = null)
        {
            var retorno = new List<Inscricao>();

            foreach (var usuario in usuarios)
            {
                retorno.Add(Gerador(usuario.Id, propostaTurmaId, situacao));
            }

            return retorno;
        }
    }
}

