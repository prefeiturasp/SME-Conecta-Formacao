using MediatR;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Extensoes;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao
{
    public class SalvarPropostaTurmaDreCommandHandler : IRequestHandler<SalvarPropostaTurmaDreCommand, bool>
    {
        private readonly IRepositorioProposta _repositorioProposta;
        private readonly IMediator _mediator;

        public SalvarPropostaTurmaDreCommandHandler(IRepositorioProposta repositorioProposta, IMediator mediator)
        {
            _repositorioProposta = repositorioProposta ?? throw new ArgumentNullException(nameof(repositorioProposta));
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task<bool> Handle(SalvarPropostaTurmaDreCommand request, CancellationToken cancellationToken)
        {
            var propostaTurmasDresInserir = Enumerable.Empty<PropostaTurmaDre>();
            var propostaTurmasDresExcluir = Enumerable.Empty<PropostaTurmaDre>();

            var propostaTurmaIds = request.PropostaTurmasDres.Select(t => t.PropostaTurmaId).ToArray();

            var propostaTurmasDresAntes = await _repositorioProposta.ObterPropostaTurmasDresPorPropostaTurmaId(propostaTurmaIds);

            propostaTurmasDresInserir = request.PropostaTurmasDres
                .Where(w => !propostaTurmasDresAntes.Any(a => a.PropostaTurmaId == w.PropostaTurmaId && a.DreId == w.DreId));

            propostaTurmasDresExcluir = propostaTurmasDresAntes
                .Where(w => !request.PropostaTurmasDres.Any(a => a.PropostaTurmaId == w.PropostaTurmaId && a.DreId == w.DreId));

            if (propostaTurmasDresInserir.Any())
                await _repositorioProposta.InserirPropostaTurmasDres(propostaTurmasDresInserir);

            if (propostaTurmasDresExcluir.Any())
                await _repositorioProposta.RemoverPropostaTurmasDres(propostaTurmasDresExcluir);

            foreach (var propostaTurmaId in propostaTurmaIds)
            {
                await _mediator.Send(new RemoverCacheCommand(CacheDistribuidoNomes.PropostaTurma.Parametros(propostaTurmaId)), cancellationToken);
                await _mediator.Send(new RemoverCacheCommand(CacheDistribuidoNomes.PropostaTurmaDre.Parametros(propostaTurmaId)), cancellationToken);
            }

            return true;
        }
    }
}
