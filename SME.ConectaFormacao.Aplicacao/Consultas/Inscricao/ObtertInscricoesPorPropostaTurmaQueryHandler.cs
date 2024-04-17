using MediatR;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ObtertInscricoesPorPropostaTurmaQueryHandler : IRequestHandler<ObtertInscricoesPorPropostaTurmaQuery, IEnumerable<Inscricao>>
    {
        private readonly IRepositorioInscricao _repositorioInscricao;

        public ObtertInscricoesPorPropostaTurmaQueryHandler(IRepositorioInscricao repositorioInscricao)
        {
            _repositorioInscricao =
                repositorioInscricao ?? throw new ArgumentNullException(nameof(repositorioInscricao));
        }

        public async Task<IEnumerable<Inscricao>> Handle(ObtertInscricoesPorPropostaTurmaQuery request, CancellationToken cancellationToken)
        {
            return await _repositorioInscricao.ObterInscricoesPorPropostasTurmasId(request.TurmaId);
        }
    }
}