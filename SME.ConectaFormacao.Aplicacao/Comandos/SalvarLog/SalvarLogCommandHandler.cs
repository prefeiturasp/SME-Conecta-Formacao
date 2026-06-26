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

    public SalvarLogCommandHandler(IRepositorioLog repositorio, IMapper mapper, ITransacao transacao)
    {
        _repositorio = repositorio ?? throw new ArgumentNullException(nameof(repositorio));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _transacao = transacao ?? throw new ArgumentNullException(nameof(transacao));
    }

    public async Task<bool> Handle(SalvarLogCommand request, CancellationToken cancellationToken)
    {
        var transacao = _transacao.Iniciar();

        try
        {
            var log = _mapper.Map<Log>(request.LogDTO);

            await _repositorio.Inserir(transacao, log);

            transacao.Commit();

            return true;
        }
        catch (Exception ex)
        {
            transacao.Rollback();
            throw;
        }
    }
}