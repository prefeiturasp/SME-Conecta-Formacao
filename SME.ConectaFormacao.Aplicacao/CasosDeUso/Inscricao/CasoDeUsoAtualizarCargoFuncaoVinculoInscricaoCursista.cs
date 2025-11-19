using MediatR;
using SME.ConectaFormacao.Aplicacao.Comandos.PublicarNaFilaRabbit;
using SME.ConectaFormacao.Aplicacao.Dtos.Inscricao;
using SME.ConectaFormacao.Aplicacao.Interfaces.Inscricao;
using SME.ConectaFormacao.Infra;
using SME.ConectaFormacao.Infra.Servicos.Rabbit.Dto;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.Inscricao
{
    public class CasoDeUsoAtualizarCargoFuncaoVinculoInscricaoCursista : CasoDeUsoAbstrato, ICasoDeUsoAtualizarCargoFuncaoVinculoInscricaoCursista
    {
        public CasoDeUsoAtualizarCargoFuncaoVinculoInscricaoCursista(IMediator mediator) : base(mediator)
        {
        }

        public async Task<bool> Executar(MensagemRabbit param)
        {
            var inscricoesConfirmadas = await mediator.Send(new ObterInscricoesConfirmadasQuery());
            inscricoesConfirmadas = inscricoesConfirmadas.Where(c => string.IsNullOrEmpty(c.CargoCodigo) || c.TipoVinculo == null || c.TipoVinculo.GetValueOrDefault() == 0);
            if (!inscricoesConfirmadas.Any())
                return false;

            foreach (var inscricao in inscricoesConfirmadas)
            {
                await mediator.Send(new PublicarNaFilaRabbitCommand(RotasRabbit.AtualizarCargoFuncaoVinculoInscricaoCursistaTratar,
                    new AtualizarCargoFuncaoVinculoInscricaoCursistaTratarDto
                    {
                        Id = inscricao.Id,
                        Login = inscricao.Usuario.Login,
                        CargoCodigo = inscricao.CargoCodigo
                    }));
            }

            return true;
        }
    }
}