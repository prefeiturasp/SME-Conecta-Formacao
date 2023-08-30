using AutoMapper;
using MediatR;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Infra.Dados;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao
{
    public class InserirAreaPromotoraCommandHandler : IRequestHandler<InserirAreaPromotoraCommand, long>
    {
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;
        private readonly ITransacao _transacao;
        private readonly IRepositorioAreaPromotora _repositorioAreaPromotora;

        public InserirAreaPromotoraCommandHandler(IMediator mediator, IMapper mapper, ITransacao transacao, IRepositorioAreaPromotora repositorioAreaPromotora)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _transacao = transacao ?? throw new ArgumentNullException(nameof(transacao));
            _repositorioAreaPromotora = repositorioAreaPromotora ?? throw new ArgumentNullException(nameof(repositorioAreaPromotora));

        }

        public async Task<long> Handle(InserirAreaPromotoraCommand request, CancellationToken cancellationToken)
        {
            await _mediator.Send(new ValidarEmailsAreaPromotoraCommand(request.AreaPromotoraDTO.Emails, request.AreaPromotoraDTO.Tipo), cancellationToken);

            var areaPromotora = _mapper.Map<AreaPromotora>(request.AreaPromotoraDTO);
            var transacao = _transacao.Iniciar();
            try
            {
                long id = await _repositorioAreaPromotora.Inserir(transacao, areaPromotora);

                if (areaPromotora.Telefones != null && areaPromotora.Telefones.Any())
                    await _repositorioAreaPromotora.InserirTelefones(transacao, id, areaPromotora.Telefones);

                transacao.Commit();

                return id;
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
