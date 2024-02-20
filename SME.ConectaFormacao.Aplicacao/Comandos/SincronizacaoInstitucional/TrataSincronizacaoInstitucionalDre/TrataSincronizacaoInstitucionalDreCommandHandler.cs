using AutoMapper;
using MediatR;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;
using SME.ConectaFormacao.Infra.Servicos.Eol.Dto;

namespace SME.ConectaFormacao.Aplicacao;

public class TrataSincronizacaoInstitucionalDreCommandHandler : IRequestHandler<TrataSincronizacaoInstitucionalDreCommand, bool>
{
    private readonly IRepositorioDre _repositorioDre;
    private readonly IMapper _mapper;

    public TrataSincronizacaoInstitucionalDreCommandHandler(IRepositorioDre repositorioDre, IMapper mapper)
    {
        _repositorioDre = repositorioDre ?? throw new ArgumentNullException(nameof(repositorioDre));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    public async Task<bool> Handle(TrataSincronizacaoInstitucionalDreCommand request, CancellationToken cancellationToken)
    {
        var dreExiste = await _repositorioDre.VerificarSeDreExistePorCodigo(request.NomeAbreviacaoDto.Codigo);
        if (dreExiste)
        {
            var dreConecta = await _repositorioDre.ObterDrePorCodigo(request.NomeAbreviacaoDto.Codigo);
            if (VerificaSeTemAlteracao(request.NomeAbreviacaoDto, dreConecta))
            {
                var mapper = _mapper.Map<Dre>(request.NomeAbreviacaoDto);
                mapper.Id = dreConecta.Id;
                await _repositorioDre.AtualizarDreComEol(mapper);
            }
        }
        else
        {
            var mapper = _mapper.Map<Dre>(request.NomeAbreviacaoDto);
            await _repositorioDre.Inserir(mapper);
        }

        return true;
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