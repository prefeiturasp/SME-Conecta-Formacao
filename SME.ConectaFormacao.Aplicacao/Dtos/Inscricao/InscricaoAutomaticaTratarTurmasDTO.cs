using SME.ConectaFormacao.Dominio.ObjetosDeValor;
using SME.ConectaFormacao.Infra.Servicos.Eol;

namespace SME.ConectaFormacao.Aplicacao.Dtos.Inscricao;

public class InscricaoAutomaticaTratarTurmasDTO
{
    public PropostaInscricaoAutomatica PropostaInscricaoAutomatica { get; set; }
    public IEnumerable<CursistaServicoEol> CursistasEOL { get; set; }
}