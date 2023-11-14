using MediatR;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Excecoes;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ValidarInformacoesGeraisCommandHandler : IRequestHandler<ValidarInformacoesGeraisCommand>
    {
        public async Task Handle(ValidarInformacoesGeraisCommand request, CancellationToken cancellationToken)
        {
            var erros = new List<string>();
            var proposta = request.PropostaDTO;
            
            if(proposta.TipoFormacao == null)
                erros.Add(string.Format(MensagemNegocio.CAMPO_OBRIGATORIO_NAO_INFORMADO,"o tipo da formação"));
            if(proposta.Modalidade == null)
                erros.Add(string.Format(MensagemNegocio.CAMPO_OBRIGATORIO_NAO_INFORMADO,"a modalidade"));
            if(proposta.TipoInscricao == null)
                erros.Add(string.Format(MensagemNegocio.CAMPO_OBRIGATORIO_NAO_INFORMADO,"o tipo da inscrição"));
            if(string.IsNullOrWhiteSpace(proposta.NomeFormacao))
                erros.Add(string.Format(MensagemNegocio.CAMPO_OBRIGATORIO_NAO_INFORMADO,"o nome da formação"));
            if(proposta.PublicosAlvo.Any())
                erros.Add(string.Format(MensagemNegocio.CAMPO_OBRIGATORIO_NAO_INFORMADO,"o público alvo"));
            if(proposta.QuantidadeTurmas == 0 || proposta.QuantidadeTurmas == null)
                erros.Add(string.Format(MensagemNegocio.CAMPO_OBRIGATORIO_NAO_INFORMADO,"a quantidade de turmas"));
            if(proposta.QuantidadeVagasTurma == 0 || proposta.QuantidadeVagasTurma == null)
                erros.Add(string.Format(MensagemNegocio.CAMPO_OBRIGATORIO_NAO_INFORMADO,"a quantidade de vagas por turma"));

            if (erros.Any())
                throw new NegocioException(erros);
        }
    }
}