using MediatR;
using SME.ConectaFormacao.Infra;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ObtertInscricoesPorPropostaTurmaQueryHandler : IRequestHandler<ObtertInscricoesPorPropostaTurmaQuery, IEnumerable<InscricaoUsuarioInternoDto>>
    {
        private readonly IRepositorioInscricao _repositorioInscricao;

        public ObtertInscricoesPorPropostaTurmaQueryHandler(IRepositorioInscricao repositorioInscricao)
        {
            _repositorioInscricao = repositorioInscricao ?? throw new ArgumentNullException(nameof(repositorioInscricao));
        }

        public async Task<IEnumerable<InscricaoUsuarioInternoDto>> Handle(ObtertInscricoesPorPropostaTurmaQuery request, CancellationToken cancellationToken)
        {
            return await _repositorioInscricao.ObterInscricoesPorPropostasTurmasIdUsuariosInternos(request.TurmasIds);
        }
    }
}