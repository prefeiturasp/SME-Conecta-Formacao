using AutoMapper;
using MediatR;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Extensoes;
using SME.ConectaFormacao.Infra.Dados;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;
using SME.ConectaFormacao.Infra.Servicos.Cache;

namespace SME.ConectaFormacao.Aplicacao
{
    public class SalvarPropostaEncontroCommandHandler : IRequestHandler<SalvarPropostaEncontroCommand, long>
    {
        private readonly IMapper _mapper;
        private readonly IRepositorioProposta _repositorioProposta;
        private readonly ITransacao _transacao;
        private readonly ICacheDistribuido _cacheDistribuido;

        public SalvarPropostaEncontroCommandHandler(IMapper mapper, IRepositorioProposta repositorioProposta, ITransacao transacao, ICacheDistribuido cacheDistribuido)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _repositorioProposta = repositorioProposta ?? throw new ArgumentNullException(nameof(repositorioProposta));
            _transacao = transacao ?? throw new ArgumentNullException(nameof(transacao));
            _cacheDistribuido = cacheDistribuido ?? throw new ArgumentNullException(nameof(cacheDistribuido));
        }

        public async Task<long> Handle(SalvarPropostaEncontroCommand request, CancellationToken cancellationToken)
        {
            var encontroAntes = await _repositorioProposta.ObterEncontroPorId(request.EncontroDTO.Id);

            var encontroDepois = _mapper.Map<PropostaEncontro>(request.EncontroDTO);

            var transacao = _transacao.Iniciar();
            try
            {
                if (encontroAntes != null)
                {
                    if (encontroAntes.HoraInicio != encontroDepois.HoraInicio ||
                        encontroAntes.HoraFim != encontroDepois.HoraFim ||
                        encontroAntes.Local != encontroDepois.Local)
                    {
                        encontroDepois.PropostaId = request.PropostaId;
                        encontroDepois.ManterCriador(encontroAntes);
                        await _repositorioProposta.AtualizarEncontro(encontroDepois);
                    }
                }
                else
                    await _repositorioProposta.InserirEncontro(request.PropostaId, encontroDepois);

                var turmasAntes = await _repositorioProposta.ObterEncontroTurmasPorEncontroId(encontroDepois.Id);

                var turmasInserir = encontroDepois.Turmas.Where(w => !turmasAntes.Any(a => a.Id == w.Id));
                var turmasExcluir = turmasAntes.Where(w => !encontroDepois.Turmas.Any(a => a.Id == w.Id));

                if (turmasInserir.Any())
                    await _repositorioProposta.InserirEncontroTurmas(encontroDepois.Id, turmasInserir);

                if (turmasExcluir.Any())
                    await _repositorioProposta.RemoverEncontroTurmas(turmasExcluir);

                var datasAntes = await _repositorioProposta.ObterEncontroDatasPorEncontroId(encontroDepois.Id);

                var datasInserir = encontroDepois.Datas.Where(w => !datasAntes.Any(a => a.Id == w.Id));
                var datasAlterar = encontroDepois.Datas.Where(w => datasAntes.Any(a => a.Id == w.Id));
                var datasExcluir = datasAntes.Where(w => !encontroDepois.Datas.Any(a => a.Id == w.Id));

                if (datasInserir.Any())
                    await _repositorioProposta.InserirEncontroDatas(encontroDepois.Id, datasInserir);

                if (datasAlterar.Any())
                {
                    foreach (var dataAlterar in datasAlterar)
                    {
                        var dataAntes = datasAntes.FirstOrDefault(t => t.Id == dataAlterar.Id);

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

                transacao.Commit();

                foreach (var turma in turmasAntes)
                    await _cacheDistribuido.RemoverAsync(CacheDistribuidoNomes.PropostaTurmaEncontro.Parametros(turma.TurmaId));
                await _cacheDistribuido.RemoverAsync(CacheDistribuidoNomes.FormacaoDetalhada.Parametros(request.PropostaId));
                return encontroDepois.Id;
            }
            catch
            {
                transacao.Rollback();
                throw;
            }
            finally
            {
                transacao.Dispose();
            }
        }
    }
}
