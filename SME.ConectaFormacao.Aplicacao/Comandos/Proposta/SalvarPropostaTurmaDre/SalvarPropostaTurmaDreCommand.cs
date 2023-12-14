using FluentValidation;
using MediatR;
using SME.ConectaFormacao.Dominio.Entidades;

namespace SME.ConectaFormacao.Aplicacao
{
    public class SalvarPropostaTurmaDreCommand : IRequest<bool>
    {
        public SalvarPropostaTurmaDreCommand(IEnumerable<PropostaTurmaDre> propostaTurmasDres)
        {
            PropostaTurmasDres = propostaTurmasDres;
        }
        public IEnumerable<PropostaTurmaDre> PropostaTurmasDres { get; }
    }

    // public class SalvarPropostaTurmaDreCommandValidator : AbstractValidator<SalvarPropostaTurmaDreCommand>
    // {
    //     public SalvarPropostaTurmaDreCommandValidator()
    //     {
    //         RuleFor(x => x.PropostaTurmasDres)
    //             .NotEmpty()
    //             .WithMessage("É necessário informar as turmas dres para salvar a proposta turma dre");
    //
    //         RuleForEach(x => x.PropostaTurmasDres)
    //             .SetValidator(new PropostaTurmaDreValidator());
    //     }
    // }
    //
    // public class PropostaTurmaDreValidator : AbstractValidator<PropostaTurmaDre>
    // {
    //     public PropostaTurmaDreValidator()
    //     {
    //         RuleFor(item => item.DreId)
    //             .NotEmpty()
    //             .WithMessage("É necessário informar o id da dre para salvar a proposta turma dre");
    //         
    //         RuleFor(item => item.PropostaTurmaId)
    //             .NotEmpty()
    //             .WithMessage("É necessário informar o id da proposta turma para salvar a proposta turma dre");
    //     }
    // }
}
