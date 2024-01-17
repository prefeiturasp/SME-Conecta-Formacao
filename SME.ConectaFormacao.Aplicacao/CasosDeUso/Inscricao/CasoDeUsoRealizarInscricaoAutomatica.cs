using AutoMapper;
using MediatR;
using SME.ConectaFormacao.Aplicacao.Interfaces.Inscricao;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Dominio.Excecoes;
using SME.ConectaFormacao.Dominio.Extensoes;
using SME.ConectaFormacao.Infra;
using SME.ConectaFormacao.Infra.Servicos.Eol.Dto;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.Inscricao
{
    public class CasoDeUsoRealizarInscricaoAutomatica : CasoDeUsoAbstrato, ICasoDeUsoRealizarInscricaoAutomatica
    {
        private readonly IMapper _mapper;
        
        public CasoDeUsoRealizarInscricaoAutomatica(IMediator mediator,IMapper mapper) : base(mediator)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<bool> Executar(MensagemRabbit param)
        {
            var propostaId = long.Parse(param.Mensagem.ToString());
            
            var formacoesResumidas = await mediator.Send(
                new ObterPropostaResumidaPorIdQuery(propostaId));

            var anoAtual = DateTimeExtension.HorarioBrasilia().Year;
            var qtdeCursistasSuportadosPorTurma = await mediator.Send(new ObterParametroSistemaPorTipoEAnoQuery(TipoParametroSistema.QtdeCursistasSuportadosPorTurma, anoAtual));

            if (qtdeCursistasSuportadosPorTurma.Valor.NaoEstaPreenchido())
                throw new NegocioException(string.Format(MensagemNegocio.PARAMETRO_QTDE_CURSISTAS_SUPORTADOS_POR_TURMA_NAO_ENCONTRADO,anoAtual));

            foreach (var formacaoResumida in formacoesResumidas)
            {
                var cursistasEOL = await mediator.Send(
                    new ObterFuncionarioPorFiltroPropostaServicoEolQuery(formacaoResumida.PublicosAlvos,
                    formacaoResumida.FuncoesEspecificas, formacaoResumida.Modalidades, formacaoResumida.AnosTurmas, 
                    formacaoResumida.PropostasTurmas.Select(turma => turma.CodigoDre), 
                    formacaoResumida.ComponentesCurriculares, formacaoResumida.EhTipoJornadaJEIF));

                var inscricaoCursista = new InscricaoCursistaDTO
                {
                    FormacaoResumida = _mapper.Map<FormacaoResumidaDTO>(formacaoResumida),
                    CursistasEOL = cursistasEOL,
                    QtdeCursistasSuportadosPorTurma = int.Parse(qtdeCursistasSuportadosPorTurma.Valor)
                };

                await mediator.Send(new PublicarNaFilaRabbitCommand(RotasRabbit.RealizarInscricaoAutomaticaTratar, inscricaoCursista, Guid.NewGuid(), null));
            }

            return true;
        }
    }
}
