using MediatR;
using SME.ConectaFormacao.Aplicacao.CasosDeUso;
using SME.ConectaFormacao.Aplicacao.Comandos.PublicarNaFilaRabbit;
using SME.ConectaFormacao.Dominio.Extensoes;
using SME.ConectaFormacao.Infra;
using SME.ConectaFormacao.Infra.Servicos.Rabbit.Dto;

namespace SME.ConectaFormacao.Aplicacao
{
    public class CasoDeUsoEncerrarInscricaoAutomaticamenteInscricoes : CasoDeUsoAbstrato,
        ICasoDeUsoEncerrarInscricaoAutomaticamenteInscricoes
    {
        public CasoDeUsoEncerrarInscricaoAutomaticamenteInscricoes(IMediator mediator) : base(mediator)
        {
        }

        public async Task<bool> Executar(MensagemRabbit param)
        {
            if (param.Mensagem == null)
                return true;
            
            var mensagem = param.Mensagem.ToString();
            if (string.IsNullOrWhiteSpace(mensagem))
                return true;
            
            var turmaIds = mensagem.JsonParaObjeto<long>();
            var inscricoes = await mediator.Send(new ObtertInscricoesPorPropostaTurmaQuery(new[] { turmaIds }));
            if (inscricoes.Any())
            {
                await mediator.Send(
                    new PublicarNaFilaRabbitCommand(RotasRabbit.EncerrarInscricaoAutomaticamenteUsuarios, inscricoes,
                        Guid.NewGuid(), new Dominio.Entidades.Usuario("Sistema", "Sistema", string.Empty)));
            }

            return true;
        }
    }
}