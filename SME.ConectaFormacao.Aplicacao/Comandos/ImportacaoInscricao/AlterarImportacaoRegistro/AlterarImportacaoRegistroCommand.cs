using MediatR;

namespace SME.ConectaFormacao.Aplicacao.Comandos.ImportacaoInscricao.AlterarImportacaoRegistro
{
    public class AlterarImportacaoRegistroCommand(AlterarImportacaoRegistroDto alterarImportacaoRegistroDto) : IRequest<bool>
    {
        public AlterarImportacaoRegistroDto AlterarImportacaoRegistroDto { get; set; } = alterarImportacaoRegistroDto;
    }
}