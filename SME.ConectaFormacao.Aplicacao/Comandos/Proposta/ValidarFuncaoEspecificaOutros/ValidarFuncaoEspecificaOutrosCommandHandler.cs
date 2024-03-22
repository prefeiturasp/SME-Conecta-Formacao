using MediatR;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Excecoes;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ValidarFuncaoEspecificaOutrosCommandHandler : IRequestHandler<ValidarFuncaoEspecificaOutrosCommand, List<string>>
    {
        private readonly IRepositorioCargoFuncao _repositorioCargoFuncao;

        public ValidarFuncaoEspecificaOutrosCommandHandler(IRepositorioCargoFuncao repositorioCargoFuncao)
        {
            _repositorioCargoFuncao = repositorioCargoFuncao ?? throw new ArgumentNullException(nameof(repositorioCargoFuncao));
        }

        public async Task<List<string>> Handle(ValidarFuncaoEspecificaOutrosCommand request, CancellationToken cancellationToken)
        {
            var erros = new List<string>();
            if (request.PropostaFuncoesEspecificas != null && request.PropostaFuncoesEspecificas.Any())
            {
                var ids = request.PropostaFuncoesEspecificas.Select(t => t.CargoFuncaoId).ToArray();
                var existeOpcaoOutros = await _repositorioCargoFuncao.ExisteCargoFuncaoOutros(ids);

                if (existeOpcaoOutros && string.IsNullOrEmpty(request.FuncaoEspecificaOutros))
                    erros.Add(MensagemNegocio.PROPOSTA_FUNCAO_ESPECIFICA_OUTROS);
            }
            return erros;
        }
    }
}
