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

            var inscritos = mapper.Map<List<CodafInscricaoListaPresenca>>(codafListaPresencaEdicaoDto.Inscritos);
            if(inscritos is not null)
                inscritos.ForEach(i => i.CodafListaPresencaId = codafListaPresencaExistente.Id);

            using var transacaoDb = transacao.Iniciar();

            try
            {
                await repositorioCodafListaPresenca.Atualizar(codafListaPresencaExistente);
                await repositorioCodafInscritosListaPresenca.ExcluirPorListaPresencaIdAsync(codafListaPresencaExistente.Id);
                if (inscritos is not null && inscritos.Count != 0)
                    await repositorioCodafInscritosListaPresenca.InserirVariosAsync(inscritos);

                transacaoDb.Commit();
                return Resultado.DeSucesso();
            }
            catch
            {
                transacaoDb.Rollback();
                return Resultado.DeFalha(TipoFalha.ErroInterno, "Erro ao atualizar a lista de presença.");
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
    }
}
