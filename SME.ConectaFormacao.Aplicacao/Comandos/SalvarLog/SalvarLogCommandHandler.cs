using AutoMapper;
using MediatR;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Infra.Dados;
using SME.ConectaFormacao.Infra.Dados.Repositorios;
namespace SME.ConectaFormacao.Aplicacao.Comandos.SalvarLog;

public class SalvarLogCommandHandler : IRequestHandler<SalvarLogCommand, bool>
{
    private readonly IRepositorioLog _repositorio;
    private readonly IMapper _mapper;
    private readonly ITransacao _transacao;
    private readonly IMediator _mediator;

    public SalvarLogCommandHandler(IRepositorioLog repositorio, IMapper mapper, ITransacao transacao, IMediator mediator)
    {
        _repositorio = repositorio ?? throw new ArgumentNullException(nameof(repositorio));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _transacao = transacao ?? throw new ArgumentNullException(nameof(transacao));
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    public async Task<bool> Handle(SalvarLogCommand request, CancellationToken cancellationToken)
    {
        Usuario usuarioLogado = await _mediator.Send(new ObterUsuarioLogadoQuery(), cancellationToken);

        usuarioLogado ??= new Usuario
            {
                Id = 1,
                Login = "Sistema",
            };

        var transacao = _transacao.Iniciar();

        try
        {
            var log = _mapper.Map<Log>(request);
            log.CriadoPor = usuarioLogado.Id.ToString();
            log.CriadoLogin = usuarioLogado.Login;
            log.CriadoEm = DateTime.Now;

            await _repositorio.Inserir(transacao, log);

            transacao.Commit();

            return true;
        }
        catch
        {
            transacao.Rollback();
            throw;
        }
    }
}