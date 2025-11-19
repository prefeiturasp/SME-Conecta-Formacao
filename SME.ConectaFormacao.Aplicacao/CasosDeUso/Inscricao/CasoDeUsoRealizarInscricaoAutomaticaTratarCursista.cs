using MediatR;
using SME.ConectaFormacao.Aplicacao.Comandos.PublicarNaFilaRabbit;
using SME.ConectaFormacao.Aplicacao.Dtos.Inscricao;
using SME.ConectaFormacao.Aplicacao.Interfaces.Inscricao;
using SME.ConectaFormacao.Infra;
using SME.ConectaFormacao.Infra.Servicos.Rabbit.Dto;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.Inscricao
{
    public class CasoDeUsoRealizarInscricaoAutomaticaTratarCursista : CasoDeUsoAbstrato, ICasoDeUsoRealizarInscricaoAutomaticaTratarCursista
    {
        public CasoDeUsoRealizarInscricaoAutomaticaTratarCursista(IMediator mediator) : base(mediator)
        {
        }

        public async Task<bool> Executar(MensagemRabbit param)
        {
            var inscricaoCursistaDto = param.ObterObjetoMensagem<InserirInscricaoDTO>();

            foreach (var propostaTurmaCursista in inscricaoCursistaDto.InscricaoAutomaticaPropostaTurmaCursistasDTO)
            {
                foreach (var cursista in propostaTurmaCursista.Cursistas)
                {
                    var inscricaoAutomaticaDTO = new InscricaoAutomaticaDTO()
                    {
                        UsuarioRf = cursista.Rf,
                        UsuarioNome = cursista.Nome,
                        UsuarioCpf = cursista.Cpf,
                        PropostaId = inscricaoCursistaDto.PropostaId,
                        PropostaTurmaId = propostaTurmaCursista.Id,
                        CargoCodigo = cursista.CargoCodigo,
                        CargoDreCodigo = cursista.CargoDreCodigo,
                        CargoUeCodigo = cursista.CargoUeCodigo,
                        FuncaoCodigo = cursista.FuncaoCodigo,
                        FuncaoDreCodigo = cursista.FuncaoDreCodigo,
                        FuncaoUeCodigo = cursista.FuncaoUeCodigo,
                        TipoVinculo = cursista.TipoVinculo
                    };

                    await mediator.Send(new PublicarNaFilaRabbitCommand(RotasRabbit.RealizarInscricaoAutomaticaIncreverCursista, inscricaoAutomaticaDTO));
                }
            }

            return true;
        }
    }
}
