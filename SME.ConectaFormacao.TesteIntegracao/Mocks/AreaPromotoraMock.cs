using Bogus;
using SME.ConectaFormacao.Aplicacao.Dtos.Proposta;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Dominio.Extensoes;

namespace SME.ConectaFormacao.TesteIntegracao.Mocks
{
    public class AreaPromotoraMock : BaseMock
    {

        private static Faker<AreaPromotora> Gerador(Guid? grupoId = null)
        {
            var faker = new Faker<AreaPromotora>("pt_BR");
            faker.RuleFor(x => x.Nome, f => f.Name.FirstName());
            faker.RuleFor(x => x.Tipo, f => f.PickRandom<AreaPromotoraTipo>());
            faker.RuleFor(x => x.Email, f => f.Person.Email);
            faker.RuleFor(x => x.GrupoId, grupoId.HasValue ? grupoId : Guid.NewGuid());
            faker.RuleFor(x => x.Excluido, false);
            AuditoriaFaker(faker);
            return faker;
        }

        private static Faker<AreaPromotoraTelefone> GeradorTelefone(long areaPromotoraId, string telefone)
        {
            var faker = new Faker<AreaPromotoraTelefone>();
            faker.RuleFor(x => x.AreaPromotoraId, areaPromotoraId);
            faker.RuleFor(x => x.Telefone, f => string.IsNullOrEmpty(telefone) ? f.Phone.PhoneNumber("###########") : telefone.SomenteNumeros());
            faker.RuleFor(x => x.Excluido, false);
            AuditoriaFaker(faker);
            return faker;
        }

        public static AreaPromotora GerarAreaPromotora(Guid? grupoId = null)
        {
            return Gerador(grupoId).Generate();
        }

        public static IEnumerable<AreaPromotora> GerarAreaPromotora(int quantidade, Guid? grupoId = null)
        {
            return Gerador(grupoId).Generate(quantidade);
        }

        public static AreaPromotoraTelefone GerarAreaTelefone(long areaPromotoraId, string telefone = null)
        {
            return GeradorTelefone(areaPromotoraId, telefone).Generate();
        }

        public static IEnumerable<AreaPromotoraTelefone> GerarAreaTelefone(int quantidade, long areaPromotoraId, string telefone = null)
        {
            var faker = new Faker<AreaPromotoraTelefone>();
            faker.RuleFor(x => x.AreaPromotoraId, areaPromotoraId);
            faker.RuleFor(x => x.Telefone, f => string.IsNullOrEmpty(telefone) ? f.Phone.PhoneNumber("###########") : telefone.SomenteNumeros());
            faker.RuleFor(x => x.Excluido, false);
            AuditoriaFaker(faker);
            return faker.Generate(quantidade);
        }

        public static PropostaRegente GerarRegente(long propostaId)
        {
            var faker = new Faker<PropostaRegente>();
            faker.RuleFor(x => x.PropostaId, propostaId);
            faker.RuleFor(x => x.ProfissionalRedeMunicipal, false);
            faker.RuleFor(x => x.RegistroFuncional, string.Empty);
            faker.RuleFor(x => x.NomeRegente, f => f.Name.FindName());
            faker.RuleFor(x => x.MiniBiografia, f => f.Company.CompanyName());
            AuditoriaFaker(faker);
            return faker.Generate();
        }
        public static PropostaRegenteTurma GerarRegenteTurma(long regenteId)
        {
            var faker = new Faker<PropostaRegenteTurma>();
            faker.RuleFor(x => x.PropostaRegenteId, regenteId);
            faker.RuleFor(x => x.Turma, 1);
            AuditoriaFaker(faker);
            return faker.Generate();
        }
        public static PropostaTutorTurma GerarTutorTurma(long tutorId)
        {
            var faker = new Faker<PropostaTutorTurma>();
            faker.RuleFor(x => x.PropostaTutorId, tutorId);
            faker.RuleFor(x => x.Turma, 1);
            AuditoriaFaker(faker);
            return faker.Generate();
        }
        public static PropostaTutor GerarTutor(long propostaId)
        {
            var faker = new Faker<PropostaTutor>();
            faker.RuleFor(x => x.PropostaId, propostaId);
            faker.RuleFor(x => x.ProfissionalRedeMunicipal, false);
            faker.RuleFor(x => x.RegistroFuncional, string.Empty);
            faker.RuleFor(x => x.NomeTutor, f => f.Name.FindName());
            AuditoriaFaker(faker);
            return faker.Generate();
        }
    }
}
