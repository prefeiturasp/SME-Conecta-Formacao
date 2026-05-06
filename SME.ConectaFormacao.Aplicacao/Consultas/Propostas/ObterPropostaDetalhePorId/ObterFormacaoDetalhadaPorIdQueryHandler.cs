using AutoMapper;
using ConectaFormacao.Dominio.Servicos;
using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Aplicacao.Dtos.Usuario;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Excecoes;
using SME.ConectaFormacao.Dominio.Extensoes;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;
using SME.ConectaFormacao.Infra.Servicos.Cache;
using System.Net;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ObterFormacaoDetalhadaPorIdQueryHandler(
        IRepositorioProposta repositorioProposta, IMapper mapper,
        IMediator mediator, ICacheDistribuido cacheDistribuido, IRepositorioUsuarioAcessibilidade repositorioUsuarioAcessibilidade, IPeriodoRealizacaoConsultaService periodoRealizacaoConsultaService) :
        IRequestHandler<ObterFormacaoDetalhadaPorIdQuery, RetornoFormacaoDetalhadaDTO>
    {

        public async Task<RetornoFormacaoDetalhadaDTO> Handle(ObterFormacaoDetalhadaPorIdQuery request, CancellationToken cancellationToken)
        {
            var service = await periodoRealizacaoConsultaService.ObterPeriodoRealizacaoAsync(15477);

            var chaveRedis = CacheDistribuidoNomes.FormacaoDetalhada.Parametros(request.Id);
            var retornoFormacaoDetalhadaDto = await cacheDistribuido.ObterObjetoAsync<RetornoFormacaoDetalhadaDTO>(chaveRedis);

            if (retornoFormacaoDetalhadaDto.EhNulo())
            {
                var formacaoDetalhada = await repositorioProposta.ObterFormacaoDetalhadaPorId(request.Id) ?? 
                                        throw new NegocioException(MensagemNegocio.FORMACAO_NAO_ENCONTRADA, HttpStatusCode.NotFound);

                retornoFormacaoDetalhadaDto = mapper.Map<RetornoFormacaoDetalhadaDTO>(formacaoDetalhada);

                if (formacaoDetalhada.ArquivoImagemDivulgacao is not null)
                    retornoFormacaoDetalhadaDto.ImagemUrl = await mediator.Send(new ObterEnderecoArquivoServicoArmazenamentoQuery(formacaoDetalhada.ArquivoImagemDivulgacao.NomeArquivoFisico, false), cancellationToken);

                await cacheDistribuido.SalvarAsync(chaveRedis, retornoFormacaoDetalhadaDto);
            }

            if (retornoFormacaoDetalhadaDto.FormacaoHomologada != Dominio.Enumerados.FormacaoHomologada.Sim)
            {
                var turmasComVaga = await repositorioProposta.ObterTurmasComVagaPorId(request.Id);
                foreach (var turma in retornoFormacaoDetalhadaDto.Turmas)
                {
                    turma.InscricaoEncerrada = !turmasComVaga.Any(t => t.Id == turma.Id);
                }
            }

            retornoFormacaoDetalhadaDto.InscricaoEncerrada = retornoFormacaoDetalhadaDto.DataInscricaoFim.Date < DateTimeExtension.HorarioBrasilia().Date || !retornoFormacaoDetalhadaDto.Turmas.Any(a => !a.InscricaoEncerrada);
            retornoFormacaoDetalhadaDto.Turmas = [.. retornoFormacaoDetalhadaDto.Turmas.OrderBy(x => x.Nome)];

            var acessibilidade = await repositorioUsuarioAcessibilidade.ObterAcessibilidadeAtualDoUsuarioAsync();
            retornoFormacaoDetalhadaDto.UsuarioAcessibilidade = mapper.Map<UsuarioAcessibilidadeDto>(acessibilidade);
            return retornoFormacaoDetalhadaDto;
        }
    }
}