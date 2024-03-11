using MediatR;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Excecoes;
using SME.ConectaFormacao.Dominio.Extensoes;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ValidarPublicoAlvoOutrosCommandHandler : IRequestHandler<ValidarPublicoAlvoOutrosCommand>
    {
        private readonly IRepositorioCargoFuncao _repositorioCargoFuncao;

        public ValidarPublicoAlvoOutrosCommandHandler(IRepositorioCargoFuncao repositorioCargoFuncao)
        {
            _repositorioCargoFuncao = repositorioCargoFuncao ?? throw new ArgumentNullException(nameof(repositorioCargoFuncao));
        }

        public async Task Handle(ValidarPublicoAlvoOutrosCommand request, CancellationToken cancellationToken)
        {
            if (request.PublicosAlvo.PossuiElementos())
            {
                var ids = request.PublicosAlvo.Select(t => t.CargoFuncaoId).ToArray();
                var existeOpcaoOutros = await _repositorioCargoFuncao.ExisteCargoFuncaoOutros(ids);

                if (existeOpcaoOutros && ids.Count() > 1)
                    throw new NegocioException(MensagemNegocio.PROPOSTA_PUBLICO_ALVO_OUTROS_NAO_PERMITE_MAIS_CARGO_SELECIONADO);

                if (existeOpcaoOutros && string.IsNullOrEmpty(request.PublicoAlvoOutros))
                    throw new NegocioException(MensagemNegocio.PROPOSTA_PUBLICO_ALVO_OUTROS);

                if(existeOpcaoOutros && request.EhPropostaAutomatica)
                    throw new NegocioException(MensagemNegocio.PROPOSTA_PUBLICO_ALVO_OUTROS_NAO_PODE_SER_PROPOSTA_AUTOMATICA);
            }
        }
    }
}
