using MediatR;
using SME.ConectaFormacao.Aplicacao.Comandos.Inscricoes.CancelarInscricao;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Excecoes;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao
{
    public class SortearInscricaoCommandHandler : IRequestHandler<SortearInscricaoCommand, bool>
    {
        private const string MOTIVO_CANCELAMENTO = "Você não foi contemplado no sorteio.";

        private readonly IMediator _mediator;
        private readonly IRepositorioInscricao _repositorioInscricao;

        public SortearInscricaoCommandHandler(IMediator mediator, IRepositorioInscricao repositorioInscricao)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _repositorioInscricao = repositorioInscricao ?? throw new ArgumentNullException(nameof(repositorioInscricao));
        }

        public async Task<bool> Handle(SortearInscricaoCommand request, CancellationToken cancellationToken)
        {
            var propostaTurma = await _mediator.Send(new ObterPropostaTurmaPorIdQuery(request.PropostaTurmaId), cancellationToken) ??
                throw new NegocioException(MensagemNegocio.TURMA_NAO_ENCONTRADA, System.Net.HttpStatusCode.NotFound);

            var dadosTurmas = await _repositorioInscricao.DadosListagemFormacaoComTurma(new long[] { propostaTurma.PropostaId }, propostaTurma.Id);

            var dadosTurma = dadosTurmas.FirstOrDefault();

            if (!dadosTurma.PermiteSorteio.GetValueOrDefault())
                throw new NegocioException(MensagemNegocio.PROPOSTA_NAO_PERMITE_SORTEIO);

            if (dadosTurma.Disponiveis.GetValueOrDefault() <= 0)
                throw new NegocioException(MensagemNegocio.PROPOSTA_TURMA_NAO_POSSUI_VAGA_DISPONIVEL_PARA_SORTEIO);

            var idsInscricoesAguardandoAnalise = await _repositorioInscricao.ObterIdsInscricoesAguardandoAnalise(propostaTurma.Id);

            if (idsInscricoesAguardandoAnalise.Any())
            {
                var (idsSorteados, idsNaoSorteados) = SortearInscricoes(dadosTurma.Disponiveis.GetValueOrDefault(), idsInscricoesAguardandoAnalise);

                foreach (var id in idsSorteados)
                    await _mediator.Send(new ConfirmarInscricaoCommand(id), cancellationToken);

                foreach (var id in idsNaoSorteados)
                    await _mediator.Send(new CancelarInscricaoCommand(id, MOTIVO_CANCELAMENTO), cancellationToken);
            }

            return true;
        }

        private static (IEnumerable<long>, IEnumerable<long>) SortearInscricoes(int vagas, IEnumerable<long> idsInscricoesAguardandoAnalise)
        {
            var random = new Random();
            var numerosSorteados = new List<long>();
            var listaSorteio = idsInscricoesAguardandoAnalise.ToList();

            for (int i = 0; i < vagas; i++)
            {
                int indice = random.Next(listaSorteio.Count);
                if (!listaSorteio.Any()) continue;
                numerosSorteados.Add(listaSorteio[indice]);
                listaSorteio.RemoveAt(indice);
            }

            return (numerosSorteados, listaSorteio);
        }
    }
}
