using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Minio;
using Minio.DataModel.Args;
using Minio.Exceptions;
using SME.ConectaFormacao.Dominio.Comum;
using SME.ConectaFormacao.Infra.Servicos.Armazenamento.Interfaces;
using SME.ConectaFormacao.Infra.Servicos.Armazenamento.Opcoes;
using System.Diagnostics.CodeAnalysis;

namespace SME.ConectaFormacao.Infra.Servicos.Armazenamento
{
    [ExcludeFromCodeCoverage]
    public class ServicoArmazenamento(IOptions<ConfiguracaoArmazenamentoOptions> configuracaoArmazenamentoOptions, IConfiguration configuration, IMinioClient minioClient) : IServicoArmazenamento
    {
        private readonly ConfiguracaoArmazenamentoOptions _configuracaoArmazenamentoOptions = configuracaoArmazenamentoOptions?.Value ??
                                                           throw new ArgumentNullException(nameof(configuracaoArmazenamentoOptions));
        private readonly IConfiguration _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        // Tempo de expiração da URL em segundos - TODO: tornar configurável no rancher o tabela de parametros (by Diego Moreno - 1/2026)
        private readonly int _urlExpiracaoEmSegundos = 48 * 60 * 60; // 48 horas

        public async Task<string> ArmazenarTemporaria(string nomeArquivo, Stream stream, string contentType)
        {
            await ArmazenarArquivo(nomeArquivo, stream, contentType, _configuracaoArmazenamentoOptions.BucketTemp);

            return await ObterUrl(nomeArquivo, _configuracaoArmazenamentoOptions.BucketTemp);
        }

        public async Task<string> Armazenar(string nomeArquivo, Stream stream, string contentType)
        {
            return await ArmazenarArquivo(nomeArquivo, stream, contentType, _configuracaoArmazenamentoOptions.BucketArquivos);
        }

        private async Task<string> ArmazenarArquivo(string nomeArquivo, Stream stream, string contentType, string bucket)
        {
            var args = new PutObjectArgs()
                .WithBucket(bucket)
                .WithObject(nomeArquivo)
                .WithStreamData(stream)
                .WithObjectSize(stream.Length)
                .WithVersionId("1.0")
                .WithContentType(contentType);

            await minioClient.PutObjectAsync(args);

            return await ObterUrl(nomeArquivo, bucket);
        }

        private async Task<string> Copiar(string nomeArquivo)
        {
            if (!_configuracaoArmazenamentoOptions.BucketTemp.Equals(_configuracaoArmazenamentoOptions.BucketArquivos))
            {
                var cpSrcArgs = new CopySourceObjectArgs()
                    .WithBucket(_configuracaoArmazenamentoOptions.BucketTemp)
                    .WithObject(nomeArquivo);

                var args = new CopyObjectArgs()
                    .WithBucket(_configuracaoArmazenamentoOptions.BucketArquivos)
                    .WithObject(nomeArquivo)
                    .WithCopyObjectSource(cpSrcArgs);

                await minioClient.CopyObjectAsync(args);
            }

            return $"{_configuracaoArmazenamentoOptions.BucketArquivos}/{nomeArquivo}";
        }

        public async Task<string> Mover(string nomeArquivo)
        {
            if (!_configuracaoArmazenamentoOptions.BucketTemp.Equals(_configuracaoArmazenamentoOptions.BucketArquivos))
            {
                await Copiar(nomeArquivo);

                await Excluir(nomeArquivo, _configuracaoArmazenamentoOptions.BucketTemp);
            }

            return $"{_configuracaoArmazenamentoOptions.BucketArquivos}/{nomeArquivo}";
        }

        public async Task<bool> Excluir(string nomeArquivo, string nomeBucket = "")
        {
            nomeBucket = string.IsNullOrEmpty(nomeBucket)
                ? _configuracaoArmazenamentoOptions.BucketArquivos
                : nomeBucket;

            var args = new RemoveObjectArgs()
                .WithBucket(nomeBucket)
                .WithObject(nomeArquivo);

            await minioClient.RemoveObjectAsync(args);
            return true;
        }

        public async Task<IEnumerable<string>> ObterBuckets()
        {
            var buckets = await minioClient.ListBucketsAsync();
            return buckets.Buckets.Select(b => b.Name).ToList();
        }

        public async Task<string> Obter(string nomeArquivo, bool ehPastaTemp)
        {
            var bucketNome = ObterNomeDoBucket(ehPastaTemp);

            return await ObterUrl(nomeArquivo, bucketNome);
        }

        private async Task<string> ObterUrl(string nomeArquivo, string bucketName)
        {
            var hostAplicacao = _configuration["UrlBucket"];
            return $"{hostAplicacao}{bucketName}/{nomeArquivo}";
        }
        public async Task<Guid> ArmazenarTemporariaGuid(Stream stream, string contentType)
        {
            var arquivoId = Guid.NewGuid();
            var nomeObjeto = arquivoId.ToString();

            await ExecutarUploadInterno(nomeObjeto, stream, contentType, _configuracaoArmazenamentoOptions.BucketTemp);

            return arquivoId;
        }
        public async Task<Guid> MoverGuid(Guid arquivoId)
        {
            var nomeObjeto = arquivoId.ToString();

            if (!_configuracaoArmazenamentoOptions.BucketTemp.Equals(_configuracaoArmazenamentoOptions.BucketArquivos))
            {
                // 1. Copia mantendo o mesmo nome (GUID)
                await CopiarInterno(nomeObjeto, nomeObjeto, _configuracaoArmazenamentoOptions.BucketTemp, _configuracaoArmazenamentoOptions.BucketArquivos);

                // 2. Remove da origem
                await Excluir(nomeObjeto, _configuracaoArmazenamentoOptions.BucketTemp);
            }

            return arquivoId;
        }
        public string ObterUrlPorGuid(Guid arquivoId, bool ehPastaTemp = false)
        {
            var bucketNome = ObterNomeDoBucket(ehPastaTemp);

            return MontarUrl(arquivoId.ToString(), bucketNome);
        }

        public async Task<string> ObterUrlPorChaveObjetoAsync(string chaveObjeto, bool ehPastaTemp = false)
        {
            var bucketNome = ObterNomeDoBucket(ehPastaTemp);

            var nomeObjeto = chaveObjeto;

            var args = new PresignedGetObjectArgs()
                .WithBucket(bucketNome)
                .WithObject(nomeObjeto)
                .WithExpiry(_urlExpiracaoEmSegundos);

            return await minioClient.PresignedGetObjectAsync(args);
        }

        private async Task ExecutarUploadInterno(string nomeObjeto, Stream stream, string contentType, string bucket)
        {
            if (stream.Position > 0) stream.Position = 0;

            var args = new PutObjectArgs()
                .WithBucket(bucket)
                .WithObject(nomeObjeto)
                .WithStreamData(stream)
                .WithObjectSize(stream.Length)
                .WithContentType(contentType);

            await minioClient.PutObjectAsync(args);
        }
        private async Task CopiarInterno(string nomeOrigem, string nomeDestino, string bucketOrigem, string bucketDestino)
        {
            var cpSrcArgs = new CopySourceObjectArgs()
                .WithBucket(bucketOrigem)
                .WithObject(nomeOrigem);

            var args = new CopyObjectArgs()
                .WithBucket(bucketDestino)
                .WithObject(nomeDestino)
                .WithCopyObjectSource(cpSrcArgs);

            await minioClient.CopyObjectAsync(args);
        }
        private string MontarUrl(string nomeArquivo, string bucketName)
        {
            var hostAplicacao = _configuration["UrlBucket"];
            var host = hostAplicacao?.TrimEnd('/');
            return $"{host}/{bucketName}/{nomeArquivo}";
        }

        private string ObterNomeDoBucket(bool ehPastaTemp) =>
            ehPastaTemp
                ? _configuracaoArmazenamentoOptions.BucketTemp
                : _configuracaoArmazenamentoOptions.BucketArquivos;

        public async Task<string> UploadCertificadoCodafAsync(string nomeArquivo, byte[] conteudoPdf)
        {
            using var stream = new MemoryStream(conteudoPdf);
            await minioClient.PutObjectAsync(new PutObjectArgs()
                .WithBucket(_configuracaoArmazenamentoOptions.BucketArquivos)
                .WithObject(nomeArquivo)
                .WithStreamData(stream)
                .WithObjectSize(stream.Length)
                .WithContentType("application/pdf"));

            return nomeArquivo;
        }

        public async Task<Resultado<Stream>> ObterArquivoPorChaveAsync(string chaveObjeto, CancellationToken cancellationToken = default)
        {
            try
            {
                var memoryStream = new MemoryStream();

                var args = new GetObjectArgs()
                    .WithBucket(_configuracaoArmazenamentoOptions.BucketArquivos)
                    .WithObject(chaveObjeto)
                    .WithCallbackStream((stream) =>
                    {
                        stream.CopyTo(memoryStream);
                    });

                await minioClient.GetObjectAsync(args, cancellationToken);
                memoryStream.Position = 0;
                return memoryStream;
            }
            catch (ObjectNotFoundException)
            {
                return Erro.NaoEncontrado("Arquivo não encontrado no storage.");
            }
            catch (Exception ex)
            {
                return new Erro(TipoFalha.ErroInterno, $"Erro ao obter arquivo: {ex.Message}");
            }
        }
    }
}
