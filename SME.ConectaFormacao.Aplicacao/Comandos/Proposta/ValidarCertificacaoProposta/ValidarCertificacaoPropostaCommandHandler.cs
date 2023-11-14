using MediatR;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Excecoes;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ValidarCertificacaoPropostaCommandHandler : IRequestHandler<ValidarCertificacaoPropostaCommand>
    {
        public async Task Handle(ValidarCertificacaoPropostaCommand request, CancellationToken cancellationToken)
        {
            var erros = new List<string>();
            var proposta = request.PropostaDto;

            if (!proposta.AcaoInformativa)
                erros.Add(MensagemNegocio.ACAO_INFORMATIVA_NAO_ACEITA);
            if (proposta.CursoComCertificado && proposta.CriterioCertificacao.Count() >= 3)
                erros.Add(MensagemNegocio.CRITERIOS_PARA_CERTIFICACAO_NAO_INFORMADA);

            if (string.IsNullOrEmpty(proposta.DescricaoDaAtividade))
                erros.Add(MensagemNegocio.DESCRICAO_DA_CERTIFICACAO_NAO_INFORMADA);

            if (erros.Any())
                throw new NegocioException(erros);
        }
    }
}