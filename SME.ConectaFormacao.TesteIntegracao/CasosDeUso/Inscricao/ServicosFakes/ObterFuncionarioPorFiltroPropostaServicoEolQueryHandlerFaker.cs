using MediatR;
using SME.ConectaFormacao.Aplicacao;
using SME.ConectaFormacao.Infra.Servicos.Eol;
using SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Inscricao.Mocks;

namespace SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Inscricao.ServicosFakes
{
    public class ObterFuncionarioPorFiltroPropostaServicoEolQueryHandlerFaker : IRequestHandler<ObterFuncionarioPorFiltroPropostaServicoEolQuery, IEnumerable<CursistaServicoEol>>
    {
        public Task<IEnumerable<CursistaServicoEol>> Handle(ObterFuncionarioPorFiltroPropostaServicoEolQuery request, CancellationToken cancellationToken)
        {
            return Task.FromResult(AoRealizarInscricaoAutomaticaMock.ObterCursistasEol(10, Enumerable.Empty<Dominio.Entidades.Dre>()));
        }
    }
}
