using FluentValidation;
using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos.Inscricao;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SME.ConectaFormacao.Aplicacao
{
    public class SalvarInscricaoImportacaoCommand : IRequest<bool>
    {
        public SalvarInscricaoImportacaoCommand(InscricaoCursistaImportacaoDTO inscricaoCursistaImportacaoDTO)
        {
            InscricaoCursistaImportacaoDTO = inscricaoCursistaImportacaoDTO;
        }

        public InscricaoCursistaImportacaoDTO InscricaoCursistaImportacaoDTO { get; }
    }

    public class SalvarInscricaoImportacaoCommandValidator : AbstractValidator<SalvarInscricaoImportacaoCommand>
    {
        public SalvarInscricaoImportacaoCommandValidator()
        {
        }
    }
}
