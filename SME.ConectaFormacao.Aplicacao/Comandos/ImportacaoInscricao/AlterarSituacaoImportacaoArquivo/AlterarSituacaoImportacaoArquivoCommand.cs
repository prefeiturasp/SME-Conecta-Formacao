using MediatR;
using SME.ConectaFormacao.Dominio.Enumerados;

namespace SME.ConectaFormacao.Aplicacao.Comandos.ImportacaoInscricao.AlterarSituacaoImportacaoArquivo
{
    public class AlterarSituacaoImportacaoArquivoCommand(long id, SituacaoImportacaoArquivo situacao) : IRequest<bool>
    {
        public long Id { get; } = id;
        public SituacaoImportacaoArquivo Situacao { get; } = situacao;
    }
}