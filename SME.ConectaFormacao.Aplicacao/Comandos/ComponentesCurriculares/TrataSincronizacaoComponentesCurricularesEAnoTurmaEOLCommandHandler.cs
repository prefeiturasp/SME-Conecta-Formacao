using AutoMapper;
using MediatR;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Infra.Dados;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;
using SME.ConectaFormacao.Infra.Servicos.Eol.Dto;

namespace SME.ConectaFormacao.Aplicacao;

public class TrataSincronizacaoComponentesCurricularesEAnoTurmaEOLCommandHandler : IRequestHandler<TrataSincronizacaoComponentesCurricularesEAnoTurmaEOLCommand, bool>
{
    private readonly IRepositorioComponenteCurricular _repositorioComponenteCurricular;
    private readonly IRepositorioAno _repositorioAno;
    private readonly IMapper _mapper;
    private readonly ITransacao _transacao;

    public TrataSincronizacaoComponentesCurricularesEAnoTurmaEOLCommandHandler(IRepositorioComponenteCurricular repositorioComponenteCurricular, 
        IMapper mapper,IRepositorioAno repositorioAno, ITransacao transacao)
    {
        _repositorioComponenteCurricular = repositorioComponenteCurricular ?? throw new ArgumentNullException(nameof(repositorioComponenteCurricular));
        _repositorioAno = repositorioAno ?? throw new ArgumentNullException(nameof(repositorioAno));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _transacao = transacao ?? throw new ArgumentNullException(nameof(transacao));
    }

    public async Task<bool> Handle(TrataSincronizacaoComponentesCurricularesEAnoTurmaEOLCommand request, CancellationToken cancellationToken)
    {
        var transacao = _transacao.Iniciar();
        try
        {
            if (request.MudouAnoTurma)
            {
                if (request.Ano.Id == 0)
                    request.ComponenteCurricular.Id = await _repositorioAno.Inserir(request.Ano);
                else
                    await _repositorioAno.Atualizar(request.Ano);
            }

            if (request.MudouComponente)
            {
                if (request.ComponenteCurricular.Id == 0)
                    await _repositorioComponenteCurricular.Inserir(request.ComponenteCurricular);
                else
                    await _repositorioComponenteCurricular.Atualizar(request.ComponenteCurricular);
            }
            
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

    private bool VerificaSeTemAlteracao(DreNomeAbreviacaoDTO dreEol, Dre dreConecta)
    {
        if (dreEol.Nome.Trim() != dreConecta.Nome.Trim() ||
            dreEol.Codigo != dreConecta.Codigo ||
            dreEol.Abreviacao != dreConecta.Abreviacao)
            return true;

        return false;
    }
}