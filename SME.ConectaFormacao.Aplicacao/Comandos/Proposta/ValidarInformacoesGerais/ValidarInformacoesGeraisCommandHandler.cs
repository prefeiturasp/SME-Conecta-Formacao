using MediatR;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Excecoes;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ValidarInformacoesGeraisCommandHandler : IRequestHandler<ValidarInformacoesGeraisCommand,IEnumerable<string>>
    {
        public async Task<IEnumerable<string>> Handle(ValidarInformacoesGeraisCommand request, CancellationToken cancellationToken)
        {
            var erros = new List<string>();
            var proposta = request.PropostaDTO;
            
            if(proposta.TipoFormacao == null)
                erros.Add(MensagemNegocio.TIPO_FORMACAO_NAO_INFORMADO);
            if(proposta.Modalidade == null)
                erros.Add(MensagemNegocio.MODALIDADE_NAO_INFORMADA);
            if(proposta.TipoInscricao == null)
                erros.Add(MensagemNegocio.TIPO_INSCRICAO_NAO_INFORMADA);
            if(string.IsNullOrWhiteSpace(proposta.NomeFormacao))
                erros.Add(MensagemNegocio.NOME_FORMACAO_NAO_INFORMADO);
            if(proposta.PublicosAlvo.Any())
                erros.Add(MensagemNegocio.PUBLICO_ALVO_NAO_INFORMADO);
            if(proposta.QuantidadeTurmas == 0 || proposta.QuantidadeTurmas == null)
                erros.Add(MensagemNegocio.QUANTIDADE_DE_TURMAS_NAO_INFORMADA);
            if(proposta.QuantidadeVagasTurma == 0 || proposta.QuantidadeVagasTurma == null)
                erros.Add(MensagemNegocio.QUANTIDADE_DE__VAGAS_POR_TURMAS_NAO_INFORMADA);

            return erros;
        }
    }
}