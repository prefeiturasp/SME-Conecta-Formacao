using AutoMapper;
using FluentValidation;
using SME.ConectaFormacao.Aplicacao.Dtos.Codaf;
using SME.ConectaFormacao.Aplicacao.Interfaces.Codaf;
using SME.ConectaFormacao.Dominio.Comum;
using SME.ConectaFormacao.Dominio.Contexto;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Servicos.Interfaces;
using SME.ConectaFormacao.Infra.Dados;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.Codaf
{
    public class CasoDeUsoAtualizarCodafListaPresenca(
        IRepositorioCodafListaPresenca repositorioCodafListaPresenca,
        IRepositorioCodafInscritosListaPresenca repositorioCodafInscritosListaPresenca,
        IRepositorioCodafRetificacaoListaPresenca repositorioCodafRetificacaoListaPresenca,
        IValidadorCodafListaPresencaService validadorCodafListaPresencaService,
        IValidator<CodafListaPresencaEdicaoDto> validator,
        IMapper mapper,
        ITransacao transacao,
        IContextoAplicacao contextoAplicacao) :
        ICasoDeUsoAtualizarCodafListaPresenca
    {
        public async Task<Resultado> ExecutarAsync(CodafListaPresencaEdicaoDto codafListaPresencaEdicaoDto, long id)
        {
            var codafListaPresencaExistente = await repositorioCodafListaPresenca.ObterPorId(id);
            if (codafListaPresencaExistente is null)
                return Erro.NaoEncontrado("Lista de presença não encontrada.");

            var erroValidacao = await ValidarRegrasDeNegocio(codafListaPresencaEdicaoDto, id);
            if (erroValidacao is not null)
                return erroValidacao;

            codafListaPresencaExistente.AtualizarInformacoes(
                codafListaPresencaEdicaoDto.DataPublicacao,
                codafListaPresencaEdicaoDto.DataPublicacaoDom,
                codafListaPresencaEdicaoDto.NumeroComunicado,
                codafListaPresencaEdicaoDto.PaginaComunicadoDom,
                codafListaPresencaEdicaoDto.CodigoCursoEol,
                codafListaPresencaEdicaoDto.CodigoNivel,
                codafListaPresencaEdicaoDto.Observacao,
                contextoAplicacao.IdPerfilUsuario);

            using var transacaoDb = transacao.Iniciar();

            try
            {
                await repositorioCodafListaPresenca.Atualizar(codafListaPresencaExistente);
                await SalvarInscritosAsync(codafListaPresencaEdicaoDto, codafListaPresencaExistente);
                await SalvarRetificacoesAsync(codafListaPresencaEdicaoDto, codafListaPresencaExistente.Id);

                transacaoDb.Commit();
                return Resultado.DeSucesso();
            }
            catch
            {
                transacaoDb.Rollback();
                return Resultado.DeFalha(TipoFalha.ErroInterno, $"Erro ao atualizar a lista de presença.");
            }
        }

        private async Task<Erro?> ValidarRegrasDeNegocio(CodafListaPresencaEdicaoDto codafListaPresencaEdicaoDto, long id)
        {
            var validationResult = await validator.ValidateAsync(codafListaPresencaEdicaoDto);
            if (!validationResult.IsValid)
                return validationResult.ToErroValidacao();

            var erroVinculo = await validadorCodafListaPresencaService.ValidarVinculoPropostaTurmaAsync(
                codafListaPresencaEdicaoDto.PropostaId,
                codafListaPresencaEdicaoDto.PropostaTurmaId);

            if (erroVinculo is not null)
                return erroVinculo;

            var erroUnicidadeTurma = await validadorCodafListaPresencaService.ValidarUnicidadeTurmaListaDePresencaAsync(
                codafListaPresencaEdicaoDto.PropostaTurmaId, id);

            if (erroUnicidadeTurma is not null)
                return erroUnicidadeTurma;

            return null;
        }

        private async Task SalvarInscritosAsync(CodafListaPresencaEdicaoDto codafListaPresencaEdicaoDto, CodafListaPresenca codafListaPresenca)
        {
            await repositorioCodafInscritosListaPresenca.ExcluirPorListaPresencaIdAsync(codafListaPresenca.Id);
            var inscritos = mapper.Map<List<CodafInscricaoListaPresenca>>(codafListaPresencaEdicaoDto.Inscritos);
            if (inscritos is not null && inscritos.Count != 0)
            {
                inscritos.ForEach(i => i.CodafListaPresencaId = codafListaPresenca.Id);
                await repositorioCodafInscritosListaPresenca.InserirVariosAsync(inscritos);
            }
        }

        private async Task SalvarRetificacoesAsync(CodafListaPresencaEdicaoDto codafListaPresencaEdicaoDto, long codafListaPresencaId)
        {
            var retificacoesExistentes = await repositorioCodafRetificacaoListaPresenca.ObterPorListaPresencaIdAsync(codafListaPresencaId);
            var retificacoesEnviadas = codafListaPresencaEdicaoDto.Retificacoes ?? [];
            var retificacoesEnviadasIds = retificacoesEnviadas.Where(r => r.Id > 0).Select(r => r.Id).ToHashSet();

            foreach (var retificacaoExistente in retificacoesExistentes)
            {
                if (!retificacoesEnviadasIds.Contains(retificacaoExistente.Id))
                {
                    await repositorioCodafRetificacaoListaPresenca.Remover(retificacaoExistente);
                }
            }

            foreach (var retificacaoDto in retificacoesEnviadas)
            {
                if (retificacaoDto.Id > 0)
                {
                    var retificacaoExistente = retificacoesExistentes.FirstOrDefault(r => r.Id == retificacaoDto.Id);
                    if (retificacaoExistente != null)
                    {
                        retificacaoExistente.AtualizarInformacoes(
                            retificacaoDto.DataRetificacao,
                            retificacaoDto.PaginaRetificacaoDom);
                        await repositorioCodafRetificacaoListaPresenca.Atualizar(retificacaoExistente);
                    }
                }
                else
                {
                    var novaRetificacao = mapper.Map<CodafRetificacaoListaPresenca>(retificacaoDto);
                    novaRetificacao.CodafListaPresencaId = codafListaPresencaId;
                    await repositorioCodafRetificacaoListaPresenca.Inserir(novaRetificacao);
                }
            }
        }
    }
}