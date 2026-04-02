using AutoMapper;
using SME.ConectaFormacao.Aplicacao.Dtos.Codaf;
using SME.ConectaFormacao.Aplicacao.Interfaces.Codaf;
using SME.ConectaFormacao.Dominio.Comum;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;
using SME.ConectaFormacao.Infra.Servicos.Armazenamento.Interfaces;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.Codaf
{
    public class CasoDeUsoObterCodafListaPresencaPorId(
        IRepositorioCodafListaPresenca repositorioCodafListaPresenca,
        IServicoArmazenamento servicoArmazenamento,
        IRepositorioCodafComentarioListaPresenca repositorioCodafComentarioListaPresenca,
        IRepositorioCodafInscritosListaPresenca repositorioCodafInscritos,
        IMapper mapper) : ICasoDeUsoObterCodafListaPresencaPorId
    {
        public async Task<Resultado<CodafListaPresencaDto>> ExecutarAsync(long listaPresencaId)
        {
            var listaPresenca = await repositorioCodafListaPresenca.ObterPorIdDetalhadoAsync(listaPresencaId);
            if (listaPresenca == null)
                return Erro.NaoEncontrado("Lista de presença não encontrada.");

            var listaPresencaDto = mapper.Map<CodafListaPresencaDto>(listaPresenca);

            if (listaPresencaDto.Anexos != null)
            {
                foreach (var anexo in listaPresencaDto.Anexos)
                {
                    anexo.UrlDownload = await servicoArmazenamento.ObterUrlPorChaveObjetoAsync(anexo.ArquivoCodigo.ToString());
                }
            }

            await ObterComentarioDfAsync(listaPresencaDto);
            await ObterDeltaInscritosAsync(listaPresencaDto);

            return listaPresencaDto;
        }

        private async Task ObterComentarioDfAsync(CodafListaPresencaDto listaPresencaDto)
        {
            if (listaPresencaDto.Status != StatusCodafListaPresenca.DevolvidoParaCorrecao) return;
            listaPresencaDto.Comentario = await repositorioCodafComentarioListaPresenca.ObterUltimoComentarioDevolucaoPorUsuarioAsync(
                listaPresencaDto.Id, StatusCodafListaPresenca.DevolvidoParaCorrecao, StatusCodafListaPresenca.AguardandoDf);
        }

        private async Task ObterDeltaInscritosAsync(CodafListaPresencaDto listaPresencaDto)
        {
            var deltaInscritos = await repositorioCodafInscritos.ObterDeltaInscritosCodafAsync(listaPresencaDto.PropostaTurmaId);
            if (deltaInscritos is null || !deltaInscritos.Any())
            {
                listaPresencaDto.DeltaInscritos = new();
            }
            else
            {
                var removidos = deltaInscritos
                                .Where(d => d.TipoDelta == TipoDeltaInscritoCodaf.Removido)
                                .Select(d =>
                                {
                                    var (documento, tipo) = ResolvedorDocumentoUsuario.Resolver(d.DadosInscrito.Login, d.DadosInscrito.Cpf);
                                    return new InscritoCodafResumidoDto(
                                        Id: d.DadosInscrito.Id,
                                        Nome: d.DadosInscrito.Nome,
                                        Documento: ResolvedorDocumentoUsuario.FormatarValor(documento, tipo)
                                    );
                                }).ToList();
                var adicionados = deltaInscritos.Where(d => d.TipoDelta == TipoDeltaInscritoCodaf.Novo).Select(d => d.DadosInscrito).ToList();

                if (removidos.Count > 0 || adicionados.Count > 0)
                {
                    listaPresencaDto.DeltaInscritos = new()
                    {
                        InscritosNovos = mapper.Map<IList<CodafInscritoTurmaListaPresencaRetornoDto>>(adicionados),
                        InscritosRemovidos = removidos
                    };
                }
            }
        }
    }
}
