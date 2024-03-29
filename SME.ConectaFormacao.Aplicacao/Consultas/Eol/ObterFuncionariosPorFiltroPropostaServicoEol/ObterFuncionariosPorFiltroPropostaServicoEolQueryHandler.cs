﻿using MediatR;
using SME.ConectaFormacao.Infra.Servicos.Eol;
using SME.ConectaFormacao.Infra.Servicos.Eol.Interfaces;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ObterFuncionarioPorFiltroPropostaServicoEolQueryHandler : IRequestHandler<ObterFuncionarioPorFiltroPropostaServicoEolQuery, IEnumerable<CursistaServicoEol>>
    {
        private readonly IServicoEol _servicoEol;

        public ObterFuncionarioPorFiltroPropostaServicoEolQueryHandler(IServicoEol servicoEol)
        {
            _servicoEol = servicoEol ?? throw new ArgumentNullException(nameof(servicoEol));
        }

        public async Task<IEnumerable<CursistaServicoEol>> Handle(ObterFuncionarioPorFiltroPropostaServicoEolQuery request, CancellationToken cancellationToken)
        {
            return await _servicoEol.ObterFuncionariosPorCargosFuncoesModalidadeAnosComponentesDres(request.CodigosCargos,
                request.CodigosFuncoes, request.CodigoModalidade, request.AnosTurma, request.CodigosDres,
                request.CodigosComponentesCurriculares, request.EhTipoJornadaJEIF);
        }
    }
}
