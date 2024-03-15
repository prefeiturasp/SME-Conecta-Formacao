using System.Text.Json;
using AutoMapper;
using ClosedXML.Excel;
using MediatR;
using Newtonsoft.Json;
using SME.ConectaFormacao.Aplicacao.Dtos.ImportacaoArquivo;
using SME.ConectaFormacao.Aplicacao.Dtos.Inscricao;
using SME.ConectaFormacao.Aplicacao.Extensoes;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Dominio.Excecoes;
using SME.ConectaFormacao.Dominio.Extensoes;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao
{
    public class
        InserirConteudoArquivoInscricaoCursistaCommandHandler : IRequestHandler<InserirConteudoArquivoInscricaoCursistaCommand, bool>
    {
        private readonly IMapper _mapper;
        private readonly IRepositorioImportacaoArquivoRegistro _repositorioImportacaoArquivoRegistro;

        private const int INICIO_LINHA_TITULO = 1;
        private const int INICIO_LINHA_DADOS = 2;

        private const string COLUNA_TURMA_TEXTO = "TURMA";
        private const string COLUNA_COLABORADOR_DA_REDE_TEXTO = "COLABORADOR DA REDE";
        private const string COLUNA_REGISTRO_FUNCIONAL_TEXTO = "REGISTRO FUNCIONAL";
        private const string COLUNA_CPF_TEXTO = "CPF";
        private const string COLUNA_NOME_TEXTO = "NOME";

        private const int COLUNA_TURMA_NUMERO = 1;
        private const int COLUNA_COLABORADOR_DA_REDE_NUMERO = 2;
        private const int COLUNA_REGISTRO_FUNCIONAL_NUMERO = 3;
        private const int COLUNA_CPF_NUMERO = 4;
        private const int COLUNA_NOME_NUMERO = 5;

        public InserirConteudoArquivoInscricaoCursistaCommandHandler(IMapper mapper, IRepositorioImportacaoArquivoRegistro repositorioImportacaoArquivoRegistro)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _repositorioImportacaoArquivoRegistro = repositorioImportacaoArquivoRegistro ?? throw new ArgumentNullException(nameof(repositorioImportacaoArquivoRegistro));
        }

        public async Task<bool> Handle(InserirConteudoArquivoInscricaoCursistaCommand request, CancellationToken cancellationToken)
        {
            using (var package = new XLWorkbook(request.StreamArquivo))
            {
                var planilha = package.Worksheets.FirstOrDefault();

                var totalLinhas = planilha.Rows().Count();

                if (totalLinhas <= INICIO_LINHA_TITULO)
                    throw new NegocioException(MensagemNegocio.ARQUIVO_VAZIO);

                ValidarOrdemColunas(planilha, INICIO_LINHA_TITULO);

                for (int numeroLinha = INICIO_LINHA_DADOS; numeroLinha <= totalLinhas; numeroLinha++)
                {
                    var item = new ImportacaoArquivoRegistroDTO()
                    {
                        Linha = numeroLinha,
                        ImportacaoArquivoId = request.ImportacaoArquivoId,
                        Situacao = SituacaoImportacaoArquivoRegistro.CarregamentoInicial,
                        Conteudo =  ObterInscricaoCursistaDto(planilha, numeroLinha).ObjetoParaJson()
                    };

                    await _repositorioImportacaoArquivoRegistro.Inserir(_mapper.Map<ImportacaoArquivoRegistro>(item));
                }
            }

            return true;
        }

        private void ValidarOrdemColunas(IXLWorksheet planilha, int numeroLinha)
        {
            ValidarTituloDaColuna(planilha, numeroLinha, COLUNA_TURMA_TEXTO, COLUNA_TURMA_NUMERO);
            ValidarTituloDaColuna(planilha, numeroLinha, COLUNA_COLABORADOR_DA_REDE_TEXTO, COLUNA_COLABORADOR_DA_REDE_NUMERO);
            ValidarTituloDaColuna(planilha, numeroLinha, COLUNA_REGISTRO_FUNCIONAL_TEXTO, COLUNA_REGISTRO_FUNCIONAL_NUMERO);
            ValidarTituloDaColuna(planilha, numeroLinha, COLUNA_CPF_TEXTO, COLUNA_CPF_NUMERO);
            ValidarTituloDaColuna(planilha, numeroLinha, COLUNA_NOME_TEXTO, COLUNA_NOME_NUMERO);
        }

        private void ValidarTituloDaColuna(IXLWorksheet planilha, int numeroLinha, string nomeDaColuna,
            int numeroDaColuna)
        {
            if (planilha.ObterValorDaCelula(numeroLinha, numeroDaColuna).RemoverAcentuacao()
                .SaoDiferentes(nomeDaColuna.RemoverAcentuacao()))
                throw new NegocioException(string.Format(
                    MensagemNegocio.A_PLANILHA_DE_INSCRICAO_CURSISTA_NAO_TEM_O_NOME_DA_COLUNA_Y_NA_COLUNA_Z,
                    nomeDaColuna, numeroDaColuna));
        }

        private InscricaoCursistaDTO ObterInscricaoCursistaDto(IXLWorksheet planilha, int numeroLinha)
        {
            return new InscricaoCursistaDTO()
            {
                Turma = planilha.ObterValorDaCelula(numeroLinha, COLUNA_TURMA_NUMERO),
                ColaboradorRede = planilha.ObterValorDaCelula(numeroLinha, COLUNA_COLABORADOR_DA_REDE_NUMERO),
                RegistroFuncional = planilha.ObterValorDaCelula(numeroLinha, COLUNA_REGISTRO_FUNCIONAL_NUMERO),
                Cpf = planilha.ObterValorDaCelula(numeroLinha, COLUNA_CPF_NUMERO),
                Nome = planilha.ObterValorDaCelula(numeroLinha, COLUNA_NOME_NUMERO)
            };
        }
    }
}