using AutoMapper;
using MediatR;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Excecoes;
using SME.ConectaFormacao.Infra.Dados;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;
using SME.ConectaFormacao.Infra.Servicos.Cache;
using System.Net;

namespace SME.ConectaFormacao.Aplicacao
{
    public class AlterarAreaPromotoraCommandHandler : IRequestHandler<AlterarAreaPromotoraCommand, bool>
    {
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;
        private readonly ITransacao _transacao;
        private readonly IRepositorioAreaPromotora _repositorioAreaPromotora;
        private readonly ICacheDistribuido _cacheDistribuido;

        public AlterarAreaPromotoraCommandHandler(
            IMediator mediator, 
            IMapper mapper, 
            ITransacao transacao, 
            IRepositorioAreaPromotora repositorioAreaPromotora,
            ICacheDistribuido cacheDistribuido)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _transacao = transacao ?? throw new ArgumentNullException(nameof(transacao));
            _repositorioAreaPromotora = repositorioAreaPromotora ?? throw new ArgumentNullException(nameof(repositorioAreaPromotora));
            _cacheDistribuido = cacheDistribuido ?? throw new ArgumentNullException(nameof(cacheDistribuido));
        }

        public async Task<bool> Handle(AlterarAreaPromotoraCommand request, CancellationToken cancellationToken)
        {
            if (request.AreaPromotoraDTO.DreId != null)
                await _mediator.Send(new ValidarPerfilDreAreaPromotoraCommand((long)request.AreaPromotoraDTO.DreId, request.AreaPromotoraDTO.GrupoId, request.Id), cancellationToken);

            await _mediator.Send(new ValidarEmailsAreaPromotoraCommand(request.AreaPromotoraDTO.Emails, request.AreaPromotoraDTO.Tipo), cancellationToken);

            if (request.AreaPromotoraDTO.DreId == null)
                await _mediator.Send(new ValidarGrupoAreaPromotoraCommand(request.AreaPromotoraDTO.GrupoId, request.Id), cancellationToken);

            var areaPromotora = await _repositorioAreaPromotora.ObterPorId(request.Id) ?? throw new NegocioException(MensagemNegocio.AREA_PROMOTORA_NAO_ENCONTRADA, HttpStatusCode.NotFound);
            var areaPromotoraDepois = _mapper.Map<AreaPromotora>(request.AreaPromotoraDTO);

            areaPromotoraDepois.Id = areaPromotora.Id;
            areaPromotoraDepois.ManterCriador(areaPromotora);

            var telefonesAntes = await _repositorioAreaPromotora.ObterTelefonesPorId(request.Id);

            var telefonesDepois = _mapper.Map<IEnumerable<AreaPromotoraTelefone>>(request.AreaPromotoraDTO.Telefones);

            var telefonesInserir = telefonesDepois.Where(w => !telefonesAntes.Any(a => a.Telefone == w.Telefone));
            var telefonesExcluir = telefonesAntes.Where(w => !telefonesDepois.Any(a => a.Telefone == w.Telefone));

            var transacao = _transacao.Iniciar();
            try
            {
                await _repositorioAreaPromotora.Atualizar(transacao, areaPromotoraDepois);

                if (telefonesInserir != null && telefonesInserir.Any())
                    await _repositorioAreaPromotora.InserirTelefones(transacao, request.Id, telefonesInserir);

                if (telefonesExcluir != null && telefonesExcluir.Any())
                    await _repositorioAreaPromotora.RemoverTelefones(transacao, request.Id, telefonesExcluir);

                transacao.Commit();

                await _cacheDistribuido.RemoverAsync(CacheDistribuidoNomes.AreaPromotora);

                return true;
            }
            catch
            {
                transacao.Rollback();
                throw;
            }
            finally
            {
                transacao.Dispose();
            }
        }
    }
}