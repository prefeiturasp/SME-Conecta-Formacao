using MediatR;
using SME.ConectaFormacao.Dominio.Entidades;

namespace SME.ConectaFormacao.Aplicacao.Comandos.Inscricoes.SalvarInscricaoImportacao
{
    public class SalvarInscricaoImportacaoCommand(Inscricao inscricao, bool formacaoHomologada) : IRequest<bool>
    {
        public Inscricao Inscricao { get; } = inscricao;
        public bool FormacaoHomologada { get; } = formacaoHomologada;
    }
}