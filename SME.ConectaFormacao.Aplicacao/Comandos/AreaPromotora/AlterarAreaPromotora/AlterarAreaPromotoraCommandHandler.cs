using AutoMapper;
using MediatR;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Excecoes;
using SME.ConectaFormacao.Infra.Dados;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;
using System.Net;

namespace SME.ConectaFormacao.Aplicacao
{
    public class AlterarAreaPromotoraCommandHandler : IRequestHandler<AlterarAreaPromotoraCommand, bool>
    {
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;
        private readonly ITransacao _transacao;
        private readonly IRepositorioAreaPromotora _repositorioAreaPromotora;

        public AlterarAreaPromotoraCommandHandler(IMediator mediator, IMapper mapper, ITransacao transacao, IRepositorioAreaPromotora repositorioAreaPromotora)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _transacao = transacao ?? throw new ArgumentNullException(nameof(transacao));
            _repositorioAreaPromotora = repositorioAreaPromotora ?? throw new ArgumentNullException(nameof(repositorioAreaPromotora));
        }

        public async Task<bool> Handle(AlterarAreaPromotoraCommand request, CancellationToken cancellationToken)
        {
            await _mediator.Send(new ValidarEmailsAreaPromotoraCommand(request.AreaPromotoraDTO.Emails, request.AreaPromotoraDTO.Tipo), cancellationToken);

            var areaPromotora = await _repositorioAreaPromotora.ObterPorId(request.Id);
            if (areaPromotora == null)
                throw new NegocioException(MensagemNegocio.AREA_PROMOTORA_NAO_ENCONTRADA, HttpStatusCode.NotFound);

            var areaPromotoraDepois = _mapper.Map<AreaPromotora>(request.AreaPromotoraDTO);

            areaPromotoraDepois.Id = areaPromotora.Id;
            areaPromotoraDepois.CriadoEm = areaPromotora.CriadoEm;
            areaPromotoraDepois.CriadoPor = areaPromotora.CriadoPor;
            areaPromotoraDepois.CriadoLogin = areaPromotora.CriadoLogin;

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
