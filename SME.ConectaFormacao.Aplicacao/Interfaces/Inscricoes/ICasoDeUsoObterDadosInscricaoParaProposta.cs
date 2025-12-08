using SME.ConectaFormacao.Aplicacao.Dtos.Inscricoes;

namespace SME.ConectaFormacao.Aplicacao.Interfaces.Inscricoes;

public interface ICasoDeUsoObterDadosInscricaoParaProposta
{
    Task<DadosInscricaoPropostaDto> ExecutarAsync(long propostaId);
}