using AutoMapper;
using MediatR;
using SME.ConectaFormacao.Aplicacao.CasosDeUso;
using SME.ConectaFormacao.Aplicacao.Dtos.AnoTurma;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Extensoes;
using SME.ConectaFormacao.Infra;
using SME.ConectaFormacao.Infra.Servicos.Eol.Dto;

namespace SME.ConectaFormacao.Aplicacao;

public class ExecutarSincronizacaoComponentesCurricularesEAnoTurmaEolUseCase : CasoDeUsoAbstrato, IExecutarSincronizacaoComponentesCurricularesEAnoTurmaEOLUseCase
{
    private readonly IMapper _mapper;
    public ExecutarSincronizacaoComponentesCurricularesEAnoTurmaEolUseCase(IMediator mediator,IMapper mapper) : base(mediator)
    {
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    public async Task<bool> Executar(MensagemRabbit param)
    {
        var anoLetivo = param.Mensagem.EhNulo() ?  DateTimeExtension.HorarioBrasilia().Year : param.ObterObjetoMensagem<AnoLetivoDTO>().AnoLetivo;
        
        var componentesCurricularesEAnoTurmaEOL = await mediator.Send(new ObterComponentesCurricularesEAnoTurmaEOLQuery(anoLetivo));

        var anosConecta = await mediator.Send(new ObterTodosOsAnosQuery()); 
            
        var componentesConecta = await mediator.Send(new ObterTodosOsComponentesCurricularesQuery());

        foreach (var componenteEAno in componentesCurricularesEAnoTurmaEOL)
        {
            var anoExistente = ObterAno(anosConecta, componenteEAno, anoLetivo);
            if (anoExistente.EhNulo())
            {
                var ano = _mapper.Map<AnoTurma>(componenteEAno);
                var componenteCurricular = _mapper.Map<ComponenteCurricular>(componenteEAno);
                componenteCurricular.AnoTurmaId = ano.Id;
                await mediator.Send(new TrataSincronizacaoComponentesCurricularesEAnoTurmaEOLCommand(ano, componenteCurricular));
            }
            else
            {
                var mudouAnoTurma = false;
                var mudouComponente = false;
                if (!anoExistente.Descricao.Equals(componenteEAno.DescricaoSerieEnsino) ||
                    !anoExistente.CodigoSerieEnsino.Equals(componenteEAno.CodigoSerieEnsino))
                {
                    anoExistente.Descricao = componenteEAno.DescricaoSerieEnsino;
                    anoExistente.CodigoSerieEnsino = componenteEAno.CodigoSerieEnsino;
                    mudouAnoTurma = true;
                }

                var componenteCurricular = componentesConecta.FirstOrDefault(w => w.AnoTurmaId == anoExistente.Id);

                if (componenteCurricular.NaoEhNulo())
                {
                    if (!componenteCurricular.Nome.Equals(componenteEAno.DescricaoComponenteCurricular))
                    {
                        componenteCurricular.Nome = componenteEAno.DescricaoComponenteCurricular;
                        mudouComponente = true;
                    }
                }
                else
                {
                    componenteCurricular = _mapper.Map<ComponenteCurricular>(componenteEAno);
                    componenteCurricular.AnoTurmaId = anoExistente.Id;
                    mudouComponente = true;
                }

                if (mudouAnoTurma || mudouComponente)
                    await mediator.Send(new TrataSincronizacaoComponentesCurricularesEAnoTurmaEOLCommand(anoExistente, componenteCurricular,mudouAnoTurma, mudouComponente));
            }
        }
        return true;
    }

    private AnoTurma ObterAno(IEnumerable<AnoTurma> anosConecta, ComponenteCurricularAnoTurmaEOLDTO componenteEAno, int anoLetivo)
    {
        var anoRetornado = anosConecta.Where(a=> (a.CodigoEOL.EstaPreenchido() && a.CodigoEOL.Equals(componenteEAno.CodigoAnoTurma))  
                                                 && a.AnoLetivo == anoLetivo
                                                 && a.Modalidade == componenteEAno.Modalidade);
        return anoRetornado.Any() ? anoRetornado.FirstOrDefault() : default;
    }
}