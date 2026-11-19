using FluentValidation;
using SME.ConectaFormacao.Aplicacao.CasosDeUso.Codaf.Dependencias;
using SME.ConectaFormacao.Aplicacao.Dtos.CodafSuplementares;
using SME.ConectaFormacao.Aplicacao.Interfaces.CodafSuplementares;
using SME.ConectaFormacao.Dominio.Comum;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Dominio.ObjetosDeValor;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;
using System.Diagnostics.CodeAnalysis;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.CodafSuplementares
{
    [ExcludeFromCodeCoverage]
    public class CasoDeUsoCriarCodafSuplementar(
        CodafSuplementarDependencias dependencias,
        IValidator<CodafSuplementarCadastroDto> validator,
        IRepositorioCodafCursoNaoHomologado repositorioCodafNaoHomologado) : ICasoDeUsoCriarCodafSuplementar
    {
        public async Task<Resultado<CodafSuplementarDetalhadoDto>> ExecutarAsync(CodafSuplementarCadastroDto codafSuplementarCadastroDto)
        {
            var validationResult = await validator.ValidateAsync(codafSuplementarCadastroDto);
            if (!validationResult.IsValid)
                return validationResult.ToErroValidacao();

            var dadosPublicacao = new DadosPublicacaoLista(
                codafSuplementarCadastroDto.DataPublicacao,
                codafSuplementarCadastroDto.DataPublicacaoDom,
                codafSuplementarCadastroDto.NumeroComunicado,
                codafSuplementarCadastroDto.PaginaComunicadoDom,
                codafSuplementarCadastroDto.CodigoCursoEol,
                codafSuplementarCadastroDto.CodigoNivel,
                codafSuplementarCadastroDto.Observacao);

            long propostaId;
            CodafSuplementar codafSuplementar;

            if (codafSuplementarCadastroDto.Tipo == TipoCodafSuplementar.NaoHomologado)
            {
                var codafNaoHomologado = await repositorioCodafNaoHomologado
                    .ObterNaoExcluidosPorIdAsync(codafSuplementarCadastroDto.CodafCursoNaoHomologadoId!.Value);

                if (codafNaoHomologado is null)
                    return Erro.Validacao("Codaf não homologado não encontrado para a turma informada");

                propostaId = codafNaoHomologado.PropostaId;
                codafSuplementar = CodafSuplementar.CriarNaoHomologado(codafNaoHomologado.Id, dadosPublicacao);
            }
            else
            {
                var codafOriginal = await dependencias.RepositorioLista.ObterNaoExcluidosPorIdAsync(codafSuplementarCadastroDto.CodafId);

                if (codafOriginal is null)
                    return Erro.Validacao("Codaf não encontrado para a turma informada");

                propostaId = codafOriginal.PropostaId;
                codafSuplementar = new CodafSuplementar(codafSuplementarCadastroDto.CodafId, dadosPublicacao);
            }


            using var transacaoDb = dependencias.Transacao.Iniciar();

            try
            {
                var id = await dependencias.RepositorioCodaf.Inserir(codafSuplementar);
                codafSuplementar.Id = id;

                await SalvarInscritosAsync(codafSuplementarCadastroDto, codafSuplementar);
                await SalvarRetificacoesAsync(codafSuplementarCadastroDto, codafSuplementar.Id);
                var anexos = dependencias.Mapper.Map<List<CodafSuplementarAnexo>>(codafSuplementarCadastroDto.Anexos);
                await dependencias.AnexoService.ProcessarAnexosAsync(codafSuplementar.Id, anexos);

                codafSuplementar.CodafAnexos = anexos;
                codafSuplementar.DefinirStatus();

                transacaoDb.Commit();
                var codafSuplementarDetalhadoDto = new CodafSuplementarDetalhadoDto
                {
                    AlteradoEm = codafSuplementar.AlteradoEm,
                    AlteradoLogin = codafSuplementar.AlteradoLogin,
                    AlteradoPor = codafSuplementar.AlteradoPor,
                    CodigoCursoEol = codafSuplementar.CodigoCursoEol,
                    CodigoNivel = codafSuplementar.CodigoNivel,
                    CriadoEm = codafSuplementar.CriadoEm,
                    CriadoLogin = codafSuplementar.CriadoLogin,
                    CriadoPor = codafSuplementar.CriadoPor,
                    DataPublicacao = codafSuplementar.DataPublicacao,
                    DataPublicacaoDom = codafSuplementar.DataPublicacaoDom,
                    Id = codafSuplementar.Id,
                    CodigoFormacao = propostaId,
                    PropostaId = propostaId
                };

                return codafSuplementarDetalhadoDto;
            }
            catch
            {
                transacaoDb.Rollback();
                return new Erro(TipoFalha.ErroInterno, "Erro ao salvar CODAF Suplementar");
            }
        }

        private async Task SalvarInscritosAsync(CodafSuplementarCadastroDto codafSuplementarCadastroDto, CodafSuplementar codafSuplementar)
        {
            var inscritos = dependencias.Mapper.Map<List<CodafSuplementarInscricao>>(codafSuplementarCadastroDto.Inscritos);
            await dependencias.InscritosService.SalvarInscritosAsync(inscritos, codafSuplementar.Id);
            codafSuplementar.CodafInscricoes = inscritos;
        }

        private async Task SalvarRetificacoesAsync(CodafSuplementarCadastroDto dto, long codafSuplementarId)
        {
            if (dto.Retificacoes is not null && dto.Retificacoes.Any())
            {
                var retificacoes = dependencias.Mapper.Map<List<CodafSuplementarRetificacao>>(dto.Retificacoes);
                foreach (var retificacao in retificacoes)
                {
                    retificacao.CodafSuplementarId = codafSuplementarId;
                    await dependencias.RepositorioRetificacao.Inserir(retificacao);
                }
            }
        }
    }
}
