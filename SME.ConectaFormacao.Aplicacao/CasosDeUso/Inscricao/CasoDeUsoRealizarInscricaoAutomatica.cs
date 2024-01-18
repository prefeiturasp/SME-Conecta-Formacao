using AutoMapper;
using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos.Inscricao;
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
            
            var formacoesResumidas = await mediator.Send(new ObterPropostaResumidaPorIdQuery(propostaId));

            var anoAtual = DateTimeExtension.HorarioBrasilia().Year;
            var qtdeCursistasSuportadosPorTurma = await mediator.Send(new ObterParametroSistemaPorTipoEAnoQuery(TipoParametroSistema.QtdeCursistasSuportadosPorTurma, anoAtual));

            if (qtdeCursistasSuportadosPorTurma.Valor.NaoEstaPreenchido())
                throw new NegocioException(string.Format(MensagemNegocio.PARAMETRO_QTDE_CURSISTAS_SUPORTADOS_POR_TURMA_NAO_ENCONTRADO,anoAtual));

            foreach (var formacaoResumida in formacoesResumidas)
            {
                var cursistasEOL = await mediator.Send(
                    new ObterFuncionarioPorFiltroPropostaServicoEolQuery(formacaoResumida.PublicosAlvos,
                    formacaoResumida.FuncoesEspecificas, formacaoResumida.Modalidades, formacaoResumida.AnosTurmas, 
                    formacaoResumida.PropostasTurmas.Select(turma => turma.CodigoDre).Distinct(), 
                    formacaoResumida.ComponentesCurriculares, formacaoResumida.EhTipoJornadaJEIF));

                await RealizarTratamentoCargoFuncao(cursistasEOL,formacaoResumida.PublicosAlvos, formacaoResumida.FuncoesEspecificas);

                cursistasEOL = cursistasEOL.Where(w => w.CargoCodigo.EstaPreenchido() && formacaoResumida.PublicosAlvos.Contains(long.Parse(w.CargoCodigo)))
                    .Union(cursistasEOL.Where(w => w.FuncaoCodigo.EstaPreenchido() && formacaoResumida.FuncoesEspecificas.Contains(long.Parse(w.FuncaoCodigo))));

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

        /// <summary>
        /// O EOL está retornando apenas os Rfs com base nos filtros: cargos, funções, ano, modalidade e componente
        /// porém não reflete a ocupação dele, pois para isso é necessário invocar a pesquisa pelo Rf retornado.
        /// Exemplo: é retornado CP como cargo base, mas o mesmo Rf tem um cargo sobreposto ou função com outro código 
        /// </summary>
        /// <param name="cursistasEOL"></param>
        private async Task RealizarTratamentoCargoFuncao(IEnumerable<FuncionarioRfNomeDreCodigoCargoFuncaoDTO> cursistasEOL, IEnumerable<long> publicosAlvos, IEnumerable<long> funcoesEspecificas)
        {
            foreach (var cursistaEOL in cursistasEOL)
            {
                cursistaEOL.CargoCodigo = cursistaEOL.CargoCodigo; 
                cursistaEOL.CargoDreCodigo = cursistaEOL.CargoDreCodigo;
                cursistaEOL.CargoUeCodigo = cursistaEOL.CargoUeCodigo;
                cursistaEOL.DreCodigo = cursistaEOL.CargoDreCodigo;
                
                if (cursistaEOL.CargoSobrepostoCodigo.EstaPreenchido() && publicosAlvos.Contains(long.Parse(cursistaEOL.CargoSobrepostoCodigo)))
                {
                    cursistaEOL.CargoCodigo = cursistaEOL.CargoSobrepostoCodigo; 
                    cursistaEOL.CargoDreCodigo = cursistaEOL.CargoSobrepostoDreCodigo;
                    cursistaEOL.CargoUeCodigo = cursistaEOL.CargoSobrepostoUeCodigo;
                    cursistaEOL.DreCodigo = cursistaEOL.CargoSobrepostoDreCodigo;
                }

                if (cursistaEOL.FuncaoCodigo.EstaPreenchido() && funcoesEspecificas.Contains(long.Parse(cursistaEOL.FuncaoCodigo)))
                {
                    cursistaEOL.FuncaoCodigo = cursistaEOL?.FuncaoCodigo;
                    cursistaEOL.FuncaoDreCodigo = cursistaEOL?.FuncaoDreCodigo;
                    cursistaEOL.FuncaoUeCodigo = cursistaEOL?.FuncaoUeCodigo;
                    cursistaEOL.DreCodigo = cursistaEOL.FuncaoDreCodigo;
                }
            }
        }

        private async Task<IEnumerable<CargoFuncionarioConectaDTO>> ObterCargosFuncoesDresPorRf(string codigoRf)
        {
            var cargosFuncoesEol = await mediator.Send(new ObterCargosFuncoesDresFuncionarioServicoEolQuery(codigoRf));

            if (cargosFuncoesEol.EhNulo())
                throw new NegocioException(MensagemNegocio.CARGO_SOBREPOSTO_FUNCAO_ATIVIDADE_NAO_ENCONTRADO);
            
            return cargosFuncoesEol;
        }
    }
}
