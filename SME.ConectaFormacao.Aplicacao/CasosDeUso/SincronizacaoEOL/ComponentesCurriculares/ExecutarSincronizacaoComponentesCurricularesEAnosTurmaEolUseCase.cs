using AutoMapper;
using MediatR;
using SME.ConectaFormacao.Aplicacao.CasosDeUso;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Extensoes;
using SME.ConectaFormacao.Infra;
using SME.ConectaFormacao.Infra.Servicos.Eol.Dto;

namespace SME.ConectaFormacao.Aplicacao;

public class ExecutarSincronizacaoComponentesCurricularesEAnosTurmaEolUseCase : CasoDeUsoAbstrato, IExecutarSincronizacaoComponentesCurricularesEAnosTurmaEOLUseCase
{
    private readonly IMapper _mapper;
    public ExecutarSincronizacaoComponentesCurricularesEAnosTurmaEolUseCase(IMediator mediator,IMapper mapper) : base(mediator)
    {
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    public async Task<bool> Executar(MensagemRabbit param)
    {
        var anoLetivo = param.Mensagem.EhNulo() ?  DateTimeExtension.HorarioBrasilia().Year : param.ObterObjetoMensagem<AnoLetivoDTO>().AnoLetivo;
        
        var componentesCurricularesEAnoTurmaEOL = await mediator.Send(new ObterComponentesCurricularesEAnosTurmaEOLQuery(anoLetivo));

        var anosTurmaConecta = (await mediator.Send(new ObterTodosOsAnosTurmaPorAnoLetivoQuery(anoLetivo))).ToList(); 
            
        var componentesConecta = await mediator.Send(new ObterTodosOsComponentesCurricularesPorAnoLetivoQuery(anoLetivo));
        
        foreach (var componenteEAnoTurma in componentesCurricularesEAnoTurmaEOL)
        {
            var anoTurmaExistente = ObterAnoTurma(anosTurmaConecta, componenteEAnoTurma, anoLetivo);
            if (anoTurmaExistente.EhNulo())
            {
                var anoTurma = _mapper.Map<AnoTurma>(componenteEAnoTurma);
                var componenteCurricular = _mapper.Map<ComponenteCurricular>(componenteEAnoTurma);
                anoTurma.AnoLetivo = anoLetivo;
                componenteCurricular.AnoTurmaId = await mediator.Send(new InserirAnoTurmaCommand(anoTurma));
                anosTurmaConecta.Add(anoTurma);
                await mediator.Send(new InserirComponenteCurricularCommand(componenteCurricular));
            }
            else
            {
                if (!anoTurmaExistente.Descricao.Equals(componenteEAnoTurma.DescricaoSerieEnsino) ||
                    !anoTurmaExistente.Modalidade.Equals(componenteEAnoTurma.Modalidade) ||
                    !anoTurmaExistente.CodigoEOL.Equals(componenteEAnoTurma.CodigoAnoTurma))
                {
                    anoTurmaExistente.Descricao = componenteEAnoTurma.DescricaoSerieEnsino;
                    anoTurmaExistente.Modalidade = componenteEAnoTurma.Modalidade;
                    anoTurmaExistente.CodigoEOL = componenteEAnoTurma.CodigoAnoTurma;
                    await mediator.Send(new AlterarAnoTurmaCommand(anoTurmaExistente));
                    anosTurmaConecta.Add(anoTurmaExistente);
                }

                var componenteCurricular = componentesConecta.LastOrDefault(w => w.AnoTurmaId == anoTurmaExistente.Id && w.CodigoEOL == componenteEAnoTurma.CodigoComponenteCurricular);

                if (componenteCurricular.NaoEhNulo())
                {
                    if (!componenteCurricular.Nome.Equals(componenteEAnoTurma.DescricaoComponenteCurricular))
                    {
                        componenteCurricular.Nome = componenteEAnoTurma.DescricaoComponenteCurricular;
                        await mediator.Send(new AlterarComponenteCurricularCommand(componenteCurricular));
                    }
                }
                else
                {
                    componenteCurricular = _mapper.Map<ComponenteCurricular>(componenteEAnoTurma);
                    componenteCurricular.AnoTurmaId = anoTurmaExistente.Id;
                    await mediator.Send(new InserirComponenteCurricularCommand(componenteCurricular));
                }
            }
        }
        return true;
    }

    private AnoTurma ObterAnoTurma(IEnumerable<AnoTurma> anosTurmaConecta, ComponenteCurricularAnoTurmaEOLDTO componenteEAno, int anoLetivo)
    {
        var anoTurmaRetornado = anosTurmaConecta.Where(a=> a.AnoLetivo == anoLetivo
                                                           && a.CodigoSerieEnsino == componenteEAno.CodigoSerieEnsino);
        return anoTurmaRetornado.Any() ? anoTurmaRetornado.LastOrDefault() : default;
    }
}