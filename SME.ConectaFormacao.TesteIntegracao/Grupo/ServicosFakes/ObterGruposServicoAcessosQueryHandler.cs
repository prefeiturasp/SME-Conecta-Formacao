﻿using MediatR;
using SME.ConectaFormacao.Aplicacao;
using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.TesteIntegracao.Grupo.Mocks;

namespace SME.ConectaFormacao.TesteIntegracao.Grupo.ServicosFakes
{
    internal class ObterGruposServicoAcessosQueryHandlerFake : IRequestHandler<ObterGruposServicoAcessosQuery, IEnumerable<GrupoDTO>>
    {
        public Task<IEnumerable<GrupoDTO>> Handle(ObterGruposServicoAcessosQuery request, CancellationToken cancellationToken)
        {
            return Task.FromResult(GrupoMock.Grupos);
        }
    }
}
