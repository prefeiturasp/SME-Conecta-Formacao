using MediatR;
using SME.ConectaFormacao.Dominio.Constantes;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ValidarCertificacaoPropostaCommandHandler : IRequestHandler<ValidarCertificacaoPropostaCommand, IEnumerable<string>>
    {
        private const int QUANTIDADE_MINIMA_CRITERIO_CERTIFICACAO = 3;
        private const int REALIZACAO_COM_ATIVIDADE_OBRIGATORIA = 4;
        public async Task<IEnumerable<string>> Handle(ValidarCertificacaoPropostaCommand request, CancellationToken cancellationToken)
        {
            var erros = new List<string>();
            var proposta = request.PropostaDto;

            if (!proposta.AcaoInformativa)
                erros.Add(MensagemNegocio.ACAO_INFORMATIVA_NAO_ACEITA);
            if (proposta.CursoComCertificado && proposta.CriterioCertificacao.Count() < QUANTIDADE_MINIMA_CRITERIO_CERTIFICACAO)
                erros.Add(MensagemNegocio.CRITERIOS_PARA_CERTIFICACAO_NAO_INFORMADO);

            var atividadeObrigatorio = proposta.CriterioCertificacao?.Where(x => x.CriterioCertificacaoId == REALIZACAO_COM_ATIVIDADE_OBRIGATORIA);

            if (atividadeObrigatorio != null && string.IsNullOrEmpty(proposta.DescricaoDaAtividade))
                erros.Add(MensagemNegocio.DESCRICAO_DA_CERTIFICACAO_NAO_INFORMADA);

            return erros;
        }
    }
}