using System.Runtime.CompilerServices;

namespace SME.ConectaFormacao.Infra.Servicos.Compactacao
{
    public static class ArquivoCompactacaoDtoExtensions
    {
        extension(IAsyncEnumerable<ArquivoCompactacaoDto> arquivos)
        {
            public async IAsyncEnumerable<ArquivoCompactacaoDto> TratarNomesDuplicados([EnumeratorCancellation] CancellationToken cancellationToken = default)
            {
                var nomesProcessados = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

                await foreach(var arquivo in arquivos.WithCancellation(cancellationToken))
                {
                    var nomeFinal = arquivo.NomeArquivo;

                    if (nomesProcessados.Contains(nomeFinal))
                    {
                        var nomeOriginal = Path.GetFileNameWithoutExtension(arquivo.NomeArquivo);
                        var extensao = Path.GetExtension(arquivo.NomeArquivo);
                        var contador = 1;

                        while (nomesProcessados.Contains(nomeFinal))
                        {
                            nomeFinal = $"{nomeOriginal} ({contador}){extensao}";
                            contador++;
                        }
                    }

                    nomesProcessados.Add(nomeFinal);

                    yield return arquivo with { NomeArquivo = nomeFinal };
                }
            }
        }
    }
}