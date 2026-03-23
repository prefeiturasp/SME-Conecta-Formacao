using MediatR;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ValidarDatasExistentesNaPropostaCommandHandler(IRepositorioPropostaEncontro repositorioPropostaEncontro) : 
        IRequestHandler<ValidarDatasExistentesNaPropostaCommand, IEnumerable<string>>
    {
        public async Task<IEnumerable<string>> Handle(ValidarDatasExistentesNaPropostaCommand request, CancellationToken cancellationToken)
        {
            var erros = new List<string>();
            var proposta = request.PropostaDto;
            if (proposta.DataRealizacaoInicio == null || proposta.DataRealizacaoFim == null)
                erros.Add(MensagemNegocio.PERIODO_REALIZACAO_NAO_INFORMADO);

            if (proposta.DataInscricaoInicio == null || proposta.DataInscricaoFim == null)
                erros.Add(MensagemNegocio.PERIODO_INCRICAO_NAO_INFORMADO);

            var quantidadeDeTurmasComEncontro = await repositorioPropostaEncontro.ObterQuantidadeDeTurmasComEncontroAsync(request.PropostaId);
            if (quantidadeDeTurmasComEncontro != proposta.QuantidadeTurmas)
                erros.Add(MensagemNegocio.QUANTIDADE_TURMAS_COM_ENCONTRO_DIFERENTE_QUANTIDADE_DE_TURMAS);

            return erros;
        }
    }
}