using Bogus;
using Bogus.Extensions.Brazil;
using SME.ConectaFormacao.Aplicacao.Dtos.UsuarioRedeParceria;
using SME.ConectaFormacao.Dominio.Enumerados;

namespace SME.ConectaFormacao.TesteIntegracao.CasosDeUso.UsuarioRedeParceria.Mock
{
    public static class UsuarioParceriaMock
    {
        public static UsuarioRedeParceriaDTO GetUsuarioRedeParceriaDTOValido(Dominio.Entidades.AreaPromotora areaPromotora)
        {
            var faker = new Faker<UsuarioRedeParceriaDTO>("pt_BR");
            faker.RuleFor(p => p.AreaPromotoraId, areaPromotora.Id);
            faker.RuleFor(p => p.Nome, f => f.Person.FullName);
            faker.RuleFor(p => p.Cpf, f => f.Person.Cpf());
            faker.RuleFor(p => p.Email, f => f.Person.Email);
            faker.RuleFor(p => p.Telefone, f => f.Person.Phone.Replace("+55",""));
            faker.RuleFor(p => p.Situacao, SituacaoUsuario.Ativo);

            return faker.Generate();
        }

        public static UsuarioRedeParceriaDTO GetUsuarioRedeParceriaDTOInvalido(Dominio.Entidades.AreaPromotora areaPromotora)
        {
            var faker = new Faker<UsuarioRedeParceriaDTO>("pt_BR");
            faker.RuleFor(p => p.AreaPromotoraId, areaPromotora.Id);
            faker.RuleFor(p => p.Nome, f => f.Person.FirstName);
            faker.RuleFor(p => p.Cpf, f => "123");
            faker.RuleFor(p => p.Email, f => f.Person.LastName);
            faker.RuleFor(p => p.Telefone, f => f.Person.Phone.Replace("+55", ""));

            return faker.Generate();
        }
    }
}
