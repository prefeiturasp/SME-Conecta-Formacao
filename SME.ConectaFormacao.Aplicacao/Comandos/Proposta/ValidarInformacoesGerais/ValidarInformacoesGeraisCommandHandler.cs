using MediatR;
using SME.ConectaFormacao.Dominio.Constantes;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ValidarInformacoesGeraisCommandHandler : IRequestHandler<ValidarInformacoesGeraisCommand, IEnumerable<string>>
    {
        public async Task<IEnumerable<string>> Handle(ValidarInformacoesGeraisCommand request, CancellationToken cancellationToken)
        {
            var erros = new List<string>();
            var proposta = request.PropostaDTO;
            
            if (proposta.TipoFormacao == null)
                erros.Add(MensagemNegocio.TIPO_FORMACAO_NAO_INFORMADO);
            if (proposta.Formato == null)
                erros.Add(MensagemNegocio.FORMATO_NAO_INFORMADO);
            if (proposta.TipoInscricao == null)
                erros.Add(MensagemNegocio.TIPO_INSCRICAO_NAO_INFORMADA);
            if (string.IsNullOrEmpty(proposta.NomeFormacao))
                erros.Add(MensagemNegocio.NOME_FORMACAO_NAO_INFORMADO);
            if (proposta.QuantidadeTurmas == 0 || proposta.QuantidadeTurmas == null)
                erros.Add(MensagemNegocio.QUANTIDADE_DE_TURMAS_NAO_INFORMADA);
            if (proposta.QuantidadeVagasTurma == 0 || proposta.QuantidadeVagasTurma == null)
                erros.Add(MensagemNegocio.QUANTIDADE_DE__VAGAS_POR_TURMAS_NAO_INFORMADA);

            return erros;
        }
    }
}