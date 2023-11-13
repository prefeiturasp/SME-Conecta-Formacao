using AutoMapper;
using MediatR;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Infra.Dados;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao
{
    public class SalvarPropostaTutorCommandHandler : IRequestHandler<SalvarPropostaTutorCommand, long>
    {
        private readonly IMapper _mapper;
        private readonly IRepositorioProposta _repositorioProposta;
        private readonly ITransacao _transacao;

        public SalvarPropostaTutorCommandHandler(IMapper mapper, IRepositorioProposta repositorioProposta, ITransacao transacao)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _repositorioProposta = repositorioProposta ?? throw new ArgumentNullException(nameof(repositorioProposta));
            _transacao = transacao ?? throw new ArgumentNullException(nameof(transacao));
        }

        public async Task<long> Handle(SalvarPropostaTutorCommand request, CancellationToken cancellationToken)
        {
            var tutorAntes = await _repositorioProposta.ObterPropostaTutorPorId(request.PropostaTutorDto.Id);

            var tutorDepois = _mapper.Map<PropostaTutor>(request.PropostaTutorDto);
            var transacao = _transacao.Iniciar();
            try
            {
                if (tutorAntes != null)
                {
                    if (tutorAntes.ProfissionalRedeMunicipal != tutorDepois.ProfissionalRedeMunicipal
                        || tutorAntes.RegistroFuncional != tutorDepois.RegistroFuncional
                        || tutorAntes.NomeTutor != tutorDepois.NomeTutor)
                    {
                        tutorDepois.PropostaId = request.PropostaId;
                        tutorDepois.ManterCriador(tutorAntes);
                        await _repositorioProposta.AtualizarPropostaTutor(tutorDepois);
                    }
                }
                else
                    await _repositorioProposta.InserirPropostaTutor(request.PropostaId, tutorDepois);

                var turmasAntes = await _repositorioProposta.ObterTutorTurmasPorTutorId(tutorDepois.Id);
                var turmasInserir = tutorDepois.Turmas.Where(w => !turmasAntes.Any(a => a.Id == w.Id));
                var turmasExcluir = turmasAntes.Where(w => !tutorDepois.Turmas.Any(a => a.Id == w.Id));

                if (turmasInserir.Any())
                    await _repositorioProposta.InserirPropostaTutorTurma(tutorDepois.Id, turmasInserir);

                if (turmasExcluir.Any())
                    await _repositorioProposta.ExcluirPropostaTutorTurma(turmasExcluir);

                transacao.Commit();
                return tutorDepois.Id;
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