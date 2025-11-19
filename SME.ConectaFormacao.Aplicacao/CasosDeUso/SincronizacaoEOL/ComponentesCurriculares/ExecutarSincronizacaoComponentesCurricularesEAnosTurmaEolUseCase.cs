using AutoMapper;
using MediatR;
using SME.ConectaFormacao.Aplicacao.CasosDeUso;
using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Extensoes;
using SME.ConectaFormacao.Infra.Servicos.Eol;
using SME.ConectaFormacao.Infra.Servicos.Rabbit.Dto;

namespace SME.ConectaFormacao.Aplicacao;

public class ExecutarSincronizacaoComponentesCurricularesEAnosTurmaEolUseCase : CasoDeUsoAbstrato, IExecutarSincronizacaoComponentesCurricularesEAnosTurmaEOLUseCase
{
    private readonly IMapper _mapper;
    public ExecutarSincronizacaoComponentesCurricularesEAnosTurmaEolUseCase(IMediator mediator, IMapper mapper) : base(mediator)
    {
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    public async Task<bool> Executar(MensagemRabbit param)
    {
        var anoLetivo = param.Mensagem.EhNulo() ? DateTimeExtension.HorarioBrasilia().Year : param.ObterObjetoMensagem<AnoLetivoDTO>().AnoLetivo;

        var componentesCurricularesEAnoTurmaEOL = await mediator.Send(new ObterComponentesCurricularesEAnosTurmaEOLQuery(anoLetivo));

        var anosTurmaConecta = (await mediator.Send(new ObterTodosOsAnosTurmaPorAnoLetivoQuery(anoLetivo))).ToList();

        var componentesConecta = await mediator.Send(new ObterTodosOsComponentesCurricularesPorAnoLetivoQuery(anoLetivo));

        foreach (var componenteEAnoTurma in componentesCurricularesEAnoTurmaEOL)
        {
            var anoTurmaExistente = ObterAnoTurma(anosTurmaConecta, componenteEAnoTurma, anoLetivo);

            var anoTurmaId = await InserirOuAtualizarAnoTurma(anoTurmaExistente, componenteEAnoTurma, anoLetivo, anosTurmaConecta);

            await InserirOuAtualizarComponenteCurricular(componentesConecta, anoTurmaExistente, componenteEAnoTurma, anoTurmaId);
        }
        return true;
    }

    private async Task InserirOuAtualizarComponenteCurricular(IEnumerable<ComponenteCurricular> componentesConecta, AnoTurma anoTurmaExistente,
        ComponenteCurricularAnoTurmaServicoEol componenteEAnoTurma, long anoTurmaId)
    {
        var componenteCurricular = componentesConecta.LastOrDefault(w => w.AnoTurmaId == anoTurmaExistente?.Id && w.CodigoEOL == componenteEAnoTurma.CodigoComponenteCurricular);

        if (componenteCurricular.NaoEhNulo())
        {
            if (!componenteCurricular.Nome.Equals(componenteEAnoTurma.DescricaoComponenteCurricular))
            {
                componenteCurricular.Nome = componenteEAnoTurma.DescricaoComponenteCurricular;
                await mediator.Send(new AlterarComponenteCurricularCommand(componenteCurricular));
                return;
            }
        }

        componenteCurricular = _mapper.Map<ComponenteCurricular>(componenteEAnoTurma);
        componenteCurricular.AnoTurmaId = anoTurmaId;
        await mediator.Send(new InserirComponenteCurricularCommand(componenteCurricular));
    }

    private async Task<long> InserirOuAtualizarAnoTurma(AnoTurma anoTurmaExistente, ComponenteCurricularAnoTurmaServicoEol componenteEAnoTurma, int anoLetivo, List<AnoTurma> anosTurmaConecta)
    {
        if (anoTurmaExistente.EhNulo())
        {
            var anoTurma = _mapper.Map<AnoTurma>(componenteEAnoTurma);
            anoTurma.AnoLetivo = anoLetivo;
            var anoTurmaId = await mediator.Send(new InserirAnoTurmaCommand(anoTurma));
            anosTurmaConecta.Add(anoTurma);
            return anoTurmaId;
        }

        if (!anoTurmaExistente.Descricao.Equals(componenteEAnoTurma.DescricaoSerieEnsino) ||
            !anoTurmaExistente.Modalidade.Equals(componenteEAnoTurma.Modalidade) ||
            !anoTurmaExistente.CodigoEOL.Equals(componenteEAnoTurma.CodigoAnoTurma))
        {
            anosTurmaConecta.Remove(anoTurmaExistente);
            anoTurmaExistente.Descricao = componenteEAnoTurma.DescricaoSerieEnsino;
            anoTurmaExistente.Modalidade = componenteEAnoTurma.Modalidade;
            anoTurmaExistente.CodigoEOL = componenteEAnoTurma.CodigoAnoTurma;
            await mediator.Send(new AlterarAnoTurmaCommand(anoTurmaExistente));
            anosTurmaConecta.Add(anoTurmaExistente);
        }
        return anoTurmaExistente.Id;
    }

    private AnoTurma ObterAnoTurma(IEnumerable<AnoTurma> anosTurmaConecta, ComponenteCurricularAnoTurmaServicoEol componenteEAno, int anoLetivo)
    {
        var anoTurmaRetornado = anosTurmaConecta.Where(a => a.AnoLetivo == anoLetivo
                                                           && a.CodigoSerieEnsino == componenteEAno.CodigoSerieEnsino);
        return anoTurmaRetornado.Any() ? anoTurmaRetornado.LastOrDefault() : default;
    }
}