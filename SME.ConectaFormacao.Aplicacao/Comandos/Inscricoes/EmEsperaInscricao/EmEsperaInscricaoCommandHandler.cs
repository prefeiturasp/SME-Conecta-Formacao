using MediatR;
using SME.ConectaFormacao.Aplicacao.Comandos.Email.InscricaoEmEspera;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Dominio.Excecoes;
using SME.ConectaFormacao.Dominio.Extensoes;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao.Comandos.Inscricoes.EmEsperaInscricao
{
    public class EmEsperaInscricaoCommandHandler(IRepositorioInscricao repositorioInscricao, IMediator mediator) : IRequestHandler<EmEsperaInscricaoCommand, bool>
    {
        public async Task<bool> Handle(EmEsperaInscricaoCommand request, CancellationToken cancellationToken)
        {
            var inscricao = await repositorioInscricao.ObterPorId(request.Id);
            if (inscricao.EhNulo() || inscricao.Excluido)
                throw new NegocioException(MensagemNegocio.INSCRICAO_NAO_ENCONTRADA);

            if (inscricao.Situacao.NaoEhAguardandoAnalise())
                throw new NegocioException(MensagemNegocio.INSCRICAO_SOMENTE_INSCRICAO_AGUARDANDO_ANALISE_PODE_IR_PARA_EM_ESPERA);

            inscricao.Situacao = Dominio.Enumerados.SituacaoInscricao.EmEspera;
            await repositorioInscricao.Atualizar(inscricao);
            await mediator.Send(new EnviarEmailInscricaoEmEsperaCommand(inscricao.Id), cancellationToken);

            return true;
        }
    }
}
