using AutoMapper;
using SME.ConectaFormacao.Dominio.Servicos.Interfaces;
using SME.ConectaFormacao.Infra.Dados;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.Codaf.Dependencias
{
    public record CodafConferenciaDependencias(
        IRepositorioCodafConferencia RepositorioCodaf,
        IRepositorioProposta RepositorioProposta,
        ICodafConferenciaInscritosService InscritosService,
        IGerenciadorAnexosCodafConferenciaService AnexoService,
        IMapper Mapper,
        ITransacao Transacao);
}
