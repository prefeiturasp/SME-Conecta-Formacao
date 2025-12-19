using MediatR;
using SME.ConectaFormacao.Dominio.Entidades;

namespace SME.ConectaFormacao.Aplicacao.Comandos.Inscricoes.SalvarInscricaoImportacao
{
    public record SalvarInscricaoImportacaoCommand(Inscricao Inscricao) : IRequest<bool>;
}