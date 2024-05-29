using MediatR;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Dominio.Excecoes;
using SME.ConectaFormacao.Dominio.Extensoes;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao
{
    public class EmEsperaInscricaoCommandHandler : IRequestHandler<EmEsperaInscricaoCommand, bool>
    {
        private readonly IRepositorioInscricao _repositorioInscricao;

        public EmEsperaInscricaoCommandHandler(IRepositorioInscricao repositorioInscricao)
        {
            _repositorioInscricao = repositorioInscricao ?? throw new ArgumentNullException(nameof(repositorioInscricao));
        }

        public async Task<bool> Handle(EmEsperaInscricaoCommand request, CancellationToken cancellationToken)
        {
            var inscricao = await _repositorioInscricao.ObterPorId(request.Id);
            if (inscricao.EhNulo() || inscricao.Excluido)
                throw new NegocioException(MensagemNegocio.INSCRICAO_NAO_ENCONTRADA);

            if (inscricao.Situacao.NaoEhAguardandoAnalise())
                throw new NegocioException(MensagemNegocio.INSCRICAO_SOMENTE_INSCRICAO_AGUARDANDO_ANALISE_PODE_IR_PARA_EM_ESPERA);

            inscricao.Situacao = Dominio.Enumerados.SituacaoInscricao.EmEspera;
            await _repositorioInscricao.Atualizar(inscricao);

            return true;
        }
    }
}
