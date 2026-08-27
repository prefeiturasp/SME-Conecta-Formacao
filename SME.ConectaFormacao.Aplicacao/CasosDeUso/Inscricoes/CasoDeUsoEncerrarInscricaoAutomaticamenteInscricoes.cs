using MediatR;
using SME.ConectaFormacao.Aplicacao.CasosDeUso;
using SME.ConectaFormacao.Aplicacao.Comandos.PublicarNaFilaRabbit;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Dominio.Extensoes;
using SME.ConectaFormacao.Infra;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;
using SME.ConectaFormacao.Infra.Servicos.Rabbit.Dto;

namespace SME.ConectaFormacao.Aplicacao
{
    public class CasoDeUsoEncerrarInscricaoAutomaticamenteInscricoes(IMediator mediator, IRepositorioInscricao repositorioInscricao) : CasoDeUsoAbstrato(mediator),
        ICasoDeUsoEncerrarInscricaoAutomaticamenteInscricoes
    {
        public async Task<bool> Executar(MensagemRabbit param)
        {
            if (param.Mensagem == null)
                return true;
            
            var mensagem = param.Mensagem.ToString();
            if (string.IsNullOrWhiteSpace(mensagem))
                return true;

            try
            {
                var turmaIds = mensagem.JsonParaObjeto<long>();
                var inscricoes = await repositorioInscricao.ObterInscricoesUsuariosInternosPorPropostasTurmasId(
                    [turmaIds],
                    SituacaoInscricao.Confirmada, SituacaoInscricao.AguardandoAnalise, SituacaoInscricao.Enviada, SituacaoInscricao.EmEspera);

                if (inscricoes.Any())
                {
                    await mediator.Send(
                        new PublicarNaFilaRabbitCommand(RotasRabbit.EncerrarInscricaoAutomaticamenteUsuarios, inscricoes,
                            Guid.NewGuid(), new Dominio.Entidades.Usuario("Sistema", "Sistema", string.Empty)));
                }
            }
            catch (Exception e)
            {
            }

            return true;
        }
    }
}