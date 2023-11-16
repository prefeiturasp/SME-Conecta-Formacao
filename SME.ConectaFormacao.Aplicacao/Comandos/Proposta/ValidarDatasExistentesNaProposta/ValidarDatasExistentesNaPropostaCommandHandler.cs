using MediatR;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ValidarDatasExistentesNaPropostaCommandHandler : IRequestHandler<ValidarDatasExistentesNaPropostaCommand, IEnumerable<string>>
    {
        private readonly IRepositorioProposta _repositorioProposta;

        public ValidarDatasExistentesNaPropostaCommandHandler(IRepositorioProposta repositorioProposta)
        {
            _repositorioProposta = repositorioProposta ?? throw new ArgumentNullException(nameof(repositorioProposta));
        }

        public async Task<IEnumerable<string>> Handle(ValidarDatasExistentesNaPropostaCommand request, CancellationToken cancellationToken)
        {
            var erros = new List<string>();
            var proposta = request.PropostaDto;
            if(proposta.DataRealizacaoInicio == null || proposta.DataRealizacaoFim == null)
                erros.Add(MensagemNegocio.PERIODO_REALIZACAO_NAO_INFORMADO);
            
            if(proposta.DataInscricaoInicio == null || proposta.DataInscricaoFim == null)
                erros.Add(MensagemNegocio.PERIODO_INCRICAO_NAO_INFORMADO);

            var quantidadeDeTurmasComEncontro = await _repositorioProposta.ObterQuantidadeDeTurmasComEncontro(request.PropostaId);
            if(quantidadeDeTurmasComEncontro < proposta.QuantidadeTurmas)
                erros.Add(MensagemNegocio.QUANTIDADE_TURMAS_COM_ENCONTRO_INFERIOR_QUANTIDADE_DE_TURMAS);

            return erros;
        }
    }
}