﻿using Bogus;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Infra.Servicos.Eol;

namespace SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Funcionario
{
    public static class UsuarioPerfilServicoEolMock
    {
        public static IEnumerable<UsuarioPerfilServicoEol> GerarListaUsuariosPerfis()
        {
            var perfis = new[] { Perfis.ADMIN_DF, "19db5e0c-80d4-439a-a2e8-91c23bda45ed" };

            var faker = new Faker<UsuarioPerfilServicoEol>();
            faker.RuleFor(x => x.Login, f => f.Random.Int(min: 1, max: 100).ToString().PadLeft(7, '0'));
            faker.RuleFor(x => x.Nome, "Nome Do Usuário");
            faker.RuleFor(x => x.Perfil, f =>
            {
                var index = new Random().Next(perfis.Length);
                var perfil = perfis[index];
                return perfil;
            });

            return faker.Generate(10);
        }
    }
}