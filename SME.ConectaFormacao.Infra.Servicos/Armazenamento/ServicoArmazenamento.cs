using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Minio;
using SME.ConectaFormacao.Infra.Servicos.Armazenamento.Interfaces;
using SME.ConectaFormacao.Infra.Servicos.Armazenamento.Opcoes;

namespace SME.ConectaFormacao.Infra.Servicos.Armazenamento
{
    public class ServicoArmazenamento : IServicoArmazenamento
    {
        private readonly MinioClient _minioClient;
        private readonly ConfiguracaoArmazenamentoOptions _configuracaoArmazenamentoOptions;
        private readonly IConfiguration _configuration;

        public ServicoArmazenamento(IOptions<ConfiguracaoArmazenamentoOptions> configuracaoArmazenamentoOptions, IConfiguration configuration)
        {
            this._configuracaoArmazenamentoOptions = configuracaoArmazenamentoOptions?.Value ?? throw new ArgumentNullException(nameof(configuracaoArmazenamentoOptions));
            this._configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));

            _minioClient = new MinioClient()
               .WithEndpoint(_configuracaoArmazenamentoOptions.EndPoint, _configuracaoArmazenamentoOptions.Port)
               .WithCredentials(_configuracaoArmazenamentoOptions.AccessKey, _configuracaoArmazenamentoOptions.SecretKey)
               .WithSSL()
               .Build();
        }

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

            await _minioClient.PutObjectAsync(args);

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

                await _minioClient.CopyObjectAsync(args);
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

            await _minioClient.RemoveObjectAsync(args);
            return true;
        }

        public async Task<IEnumerable<string>> ObterBuckets()
        {
            var nomesBuckets = new List<string>();

            var buckets = await _minioClient.ListBucketsAsync();

            foreach (var bucket in buckets.Buckets)
                nomesBuckets.Add(bucket.Name);

            return nomesBuckets;
        }

        public async Task<string> Obter(string nomeArquivo, bool ehPastaTemp)
        {
            var bucketNome = ehPastaTemp
                ? _configuracaoArmazenamentoOptions.BucketTemp
                : _configuracaoArmazenamentoOptions.BucketArquivos;

            return await ObterUrl(nomeArquivo, bucketNome);
        }

        private async Task<string> ObterUrl(string nomeArquivo, string bucketName)
        {
            var hostAplicacao = _configuration["UrlFrontEnd"];
            return $"{hostAplicacao}{bucketName}/{nomeArquivo}";
        }
    }
}
