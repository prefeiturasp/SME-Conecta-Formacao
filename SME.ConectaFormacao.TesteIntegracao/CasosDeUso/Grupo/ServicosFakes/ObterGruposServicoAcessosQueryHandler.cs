using MediatR;
using SME.ConectaFormacao.Aplicacao;
using SME.ConectaFormacao.Aplicacao.Dtos.Grupo;
using SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Grupo.Mocks;

namespace SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Grupo.ServicosFakes
{
    internal class ObterGruposServicoAcessosQueryHandlerFake : IRequestHandler<ObterGruposServicoAcessosQuery, IEnumerable<GrupoDTO>>
    {
        public async Task<IEnumerable<GrupoDTO>> Handle(ObterGruposServicoAcessosQuery request, CancellationToken cancellationToken)
        {
            var grupos = new List<GrupoDTO>()
            {
                new ()
                {
                    Id = new Guid("CA8D4F09-F7D2-4CFC-9198-13B0B365D635"),
                    Nome = "DIEFEM"
                },
                new ()
                {
                    Id = new Guid("7FCBFE72-957A-4216-936B-174DDA6917C4"),
                    Nome = "CODAE"
                },
                new ()
                {
                    Id = new Guid("2F6218F5-9F79-450D-9BA9-3308F58D130F"),
                    Nome = "DIEE"
                },
                new ()
                {
                    Id = new Guid("20B89885-9688-EE11-97DC-00155DB4374A"),
                    Nome = "Gestão DIEE"
                },
                new ()
                {
                    Id = new Guid("58E6A4FC-9588-EE11-97DC-00155DB4374A"),
                    Nome = "Gestão DIEFEM"
                }
            };

            return grupos.AsEnumerable();
        }
    }
}
