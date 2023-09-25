using MediatR;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao
{
    public class SalvarPropostaEncontroCommandHandler : IRequestHandler<SalvarPropostaEncontroCommand, bool>
    {
        private readonly IRepositorioProposta _repositorioProposta;

        public SalvarPropostaEncontroCommandHandler(IRepositorioProposta repositorioProposta)
        {
            _repositorioProposta = repositorioProposta ?? throw new ArgumentNullException(nameof(repositorioProposta));
        }

        public async Task<bool> Handle(SalvarPropostaEncontroCommand request, CancellationToken cancellationToken)
        {
            var encontrosAntes = await _repositorioProposta.ObterEncontrosPorId(request.PropostaId);

            var encontrosInserir = request.Encontros.Where(w => !encontrosAntes.Any(a => a.Id == w.Id));
            var encontrosAlterar = request.Encontros.Where(w => encontrosAntes.Any(a => a.Id == w.Id));
            var encontrosExcluir = encontrosAntes.Where(w => !request.Encontros.Any(a => a.Id == w.Id));

            if (encontrosInserir.Any())
            {
                await _repositorioProposta.InserirEncontros(request.PropostaId, encontrosInserir);
                foreach (var encontroInserir in encontrosInserir)
                {
                    await _repositorioProposta.InserirEncontroTurmas(encontroInserir.Id, encontroInserir.Turmas);
                    await _repositorioProposta.InserirEncontroDatas(encontroInserir.Id, encontroInserir.Datas);
                }
            }

            if (encontrosAlterar.Any())
            {
                var turmasAntes = await _repositorioProposta.ObterEncontroTurmasPorEncontroIds(encontrosAlterar.Select(t => t.Id).ToArray());
                var datasAntes = await _repositorioProposta.ObterEncontroDatasPorEncontroIds(encontrosAlterar.Select(t => t.Id).ToArray());

                foreach (var encontroAlterar in encontrosAlterar)
                {
                    var encontroAntes = encontrosAntes.FirstOrDefault(t => t.Id == encontroAlterar.Id);
                    var encontroTurmasAntes = turmasAntes.Where(t => t.PropostaEncontroId == encontroAlterar.Id);
                    var encontroDatasAntes = datasAntes.Where(t => t.PropostaEncontroId == encontroAlterar.Id);

                    if (encontroAlterar.HoraInicio != encontroAntes.HoraInicio ||
                        encontroAlterar.HoraFim != encontroAntes.HoraFim ||
                        encontroAlterar.Local != encontroAntes.Local)
                    {
                        encontroAlterar.ManterCriador(encontroAntes);
                        await _repositorioProposta.AtualizarEncontro(encontroAlterar);
                    }

                    var turmasInserir = encontroAlterar.Turmas.Where(w => !encontroTurmasAntes.Any(a => a.Id == w.Id));
                    var turmasExcluir = encontroTurmasAntes.Where(w => !encontroAlterar.Turmas.Any(a => a.Id == w.Id));

                    if (turmasInserir.Any())
                        await _repositorioProposta.InserirEncontroTurmas(encontroAntes.Id, turmasInserir);

                    if (turmasExcluir.Any())
                        await _repositorioProposta.RemoverEncontroTurmas(turmasExcluir);

                    var datasInserir = encontroAlterar.Datas.Where(w => !encontroDatasAntes.Any(a => a.Id == w.Id));
                    var datasAlterar = encontroAlterar.Datas.Where(w => encontroDatasAntes.Any(a => a.Id == w.Id));
                    var datasExcluir = encontroDatasAntes.Where(w => !encontroAlterar.Datas.Any(a => a.Id == w.Id));

                    if (datasInserir.Any())
                        await _repositorioProposta.InserirEncontroDatas(encontroAlterar.Id, datasInserir);

                    if (datasAlterar.Any())
                    {
                        foreach (var dataAlterar in datasAlterar)
                        {
                            var dataAntes = encontroDatasAntes.FirstOrDefault(t => t.Id == dataAlterar.Id);

                            if (dataAlterar.DataInicio.GetValueOrDefault() != dataAntes.DataInicio.GetValueOrDefault() ||
                                dataAlterar.DataFim.GetValueOrDefault() != dataAntes.DataFim.GetValueOrDefault())
                            {
                                dataAlterar.ManterCriador(dataAntes);
                                await _repositorioProposta.AtualizarEncontroData(dataAlterar);
                            }
                        }
                    }

                    if (datasExcluir.Any())
                        await _repositorioProposta.RemoverEncontroDatas(datasExcluir);
                }
            }

            if (encontrosExcluir.Any())
                await _repositorioProposta.RemoverEncontros(encontrosExcluir);

            return true;
        }
    }
}
