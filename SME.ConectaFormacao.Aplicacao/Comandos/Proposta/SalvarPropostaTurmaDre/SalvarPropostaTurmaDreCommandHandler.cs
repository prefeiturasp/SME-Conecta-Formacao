using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos.Proposta;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao
{
    public class SalvarPropostaTurmaDreCommandHandler : IRequestHandler<SalvarPropostaTurmaDreCommand, bool>
    {
        private readonly IRepositorioProposta _repositorioProposta;

        public SalvarPropostaTurmaDreCommandHandler(IRepositorioProposta repositorioProposta)
        {
            _repositorioProposta = repositorioProposta ?? throw new ArgumentNullException(nameof(repositorioProposta));
        }

        public async Task<bool> Handle(SalvarPropostaTurmaDreCommand request, CancellationToken cancellationToken)
        {
            var propostaTurmasDresAntes = Enumerable.Empty<PropostaTurmaDre>();
            var propostaTurmasDresInserir = Enumerable.Empty<PropostaTurmaDre>();
            var propostaTurmasDresExcluir = Enumerable.Empty<PropostaTurmaDre>();

            var propostaTurmaIds = request.PropostaTurmasDres.Select(t => t.PropostaTurmaId).ToArray();
            propostaTurmasDresAntes = await _repositorioProposta.ObterPropostaTurmasDresPorPropostaTurmaId(propostaTurmaIds);

            propostaTurmasDresInserir = request.PropostaTurmasDres
                .Where(w => !propostaTurmasDresAntes.Any(a => a.PropostaTurmaId == w.PropostaTurmaId && a.DreId == w.DreId));

            propostaTurmasDresExcluir = propostaTurmasDresAntes
                .Where(w => !request.PropostaTurmasDres.Any(a => a.PropostaTurmaId == w.PropostaTurmaId && a.DreId == w.DreId));

            if (propostaTurmasDresInserir.Any())
                await _repositorioProposta.InserirPropostaTurmasDres(propostaTurmasDresInserir);

            if (propostaTurmasDresExcluir.Any())
                await _repositorioProposta.RemoverPropostaTurmasDres(propostaTurmasDresExcluir);

            return true;
        }
    }
}
