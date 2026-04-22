using MediatR;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Extensoes;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ValidarPublicoAlvoOutrosCommandHandler : IRequestHandler<ValidarPublicoAlvoOutrosCommand, List<string>>
    {
        private readonly IRepositorioCargoFuncao _repositorioCargoFuncao;

        public ValidarPublicoAlvoOutrosCommandHandler(IRepositorioCargoFuncao repositorioCargoFuncao)
        {
            _repositorioCargoFuncao = repositorioCargoFuncao ?? throw new ArgumentNullException(nameof(repositorioCargoFuncao));
        }

        public async Task<List<string>> Handle(ValidarPublicoAlvoOutrosCommand request, CancellationToken cancellationToken)
        {
            var erros = new List<string>();
            if (request.PublicosAlvo.PossuiElementos())
            {
                var ids = request.PublicosAlvo.Select(t => t.CargoFuncaoId).ToArray();
                var existeOpcaoOutros = await _repositorioCargoFuncao.ExisteCargoFuncaoOutros(ids);

                if (existeOpcaoOutros && ids.Count() > 1)
                    erros.Add(MensagemNegocio.PROPOSTA_PUBLICO_ALVO_OUTROS_NAO_PERMITE_MAIS_CARGO_SELECIONADO);

                if (existeOpcaoOutros && string.IsNullOrEmpty(request.PublicoAlvoOutros))
                    erros.Add(MensagemNegocio.PROPOSTA_PUBLICO_ALVO_OUTROS);

                if (existeOpcaoOutros && request.EhPropostaAutomatica)
                    erros.Add(MensagemNegocio.PROPOSTA_PUBLICO_ALVO_OUTROS_NAO_PODE_SER_PROPOSTA_AUTOMATICA);
            }
            return erros;
        }
    }
}
