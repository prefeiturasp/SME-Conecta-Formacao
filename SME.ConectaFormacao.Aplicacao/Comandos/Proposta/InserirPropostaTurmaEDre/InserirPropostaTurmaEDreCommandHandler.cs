using MediatR;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Extensoes;
using SME.ConectaFormacao.Infra.Dados;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao
{
    public class InserirPropostaTurmaAdicionalCommandHandler : IRequestHandler<InserirPropostaTurmaAdicionalCommand, long>
    {
        private readonly IRepositorioProposta _repositorioProposta;
        private readonly ITransacao _transacao;

        public InserirPropostaTurmaAdicionalCommandHandler(ITransacao transacao, IRepositorioProposta repositorioProposta)
        {
            _transacao = transacao ?? throw new ArgumentNullException(nameof(transacao));
            _repositorioProposta = repositorioProposta ?? throw new ArgumentNullException(nameof(repositorioProposta));
        }

        public async Task<long> Handle(InserirPropostaTurmaAdicionalCommand request, CancellationToken cancellationToken)
        {
            var propostaTurma = await _repositorioProposta.ObterTurmaPorId(request.PropostaTurmaOrigemId);

            var dres = await _repositorioProposta.ObterPropostaTurmasDresPorPropostaTurmaId(request.PropostaTurmaOrigemId);
            var encontros = await _repositorioProposta.ObterEncontrosPorPropostaTurmaId(request.PropostaTurmaOrigemId);
            var regentes = await _repositorioProposta.ObterRegentesPorPropostaTurmaId(request.PropostaTurmaOrigemId);
            var tutores = await _repositorioProposta.ObterTutoresPorPropostaTurmaId(request.PropostaTurmaOrigemId);

            var propostaTurmaAdicional = (PropostaTurma)propostaTurma.Clone();

            //Parte 2...Parte 3...Parte 4
            var indexParte = propostaTurma.Nome.IndexOf(" - Parte ");
            var contador = indexParte > -1 && propostaTurma.CriadoPor == "Sistema" ? int.Parse(propostaTurma.Nome.Substring(indexParte + 9)) : 1;
            propostaTurmaAdicional.Nome = propostaTurmaAdicional.Nome.Replace($" - Parte {contador}", "");

            contador++;
            propostaTurmaAdicional.Nome += $" - Parte {contador}";

            var transacao = _transacao.Iniciar();
            try
            {
                await _repositorioProposta.InserirTurma(propostaTurmaAdicional);

                if (dres.PossuiElementos())
                {
                    foreach (var propostaTurmaDre in dres)
                        propostaTurmaDre.PropostaTurmaId = propostaTurmaAdicional.Id;

                    await _repositorioProposta.InserirPropostaTurmasDres(dres);
                }

                if (encontros.PossuiElementos())
                {
                    foreach (var encontro in encontros)
                    {
                        await _repositorioProposta.InserirEncontroTurmas(encontro.Id,
                            new List<PropostaEncontroTurma>
                            {
                                new PropostaEncontroTurma
                                {
                                    PropostaEncontroId = encontro.Id,
                                    TurmaId = propostaTurmaAdicional.Id
                                }
                            });
                    }
                }

                if (regentes.PossuiElementos())
                {
                    foreach (var regente in regentes)
                    {
                        await _repositorioProposta.InserirPropostaRegenteTurma(regente.Id,
                            new List<PropostaRegenteTurma>
                            {
                                new PropostaRegenteTurma
                                {
                                    PropostaRegenteId = regente.Id,
                                    TurmaId = propostaTurmaAdicional.Id
                                }
                            });
                    }
                }

                if (tutores.PossuiElementos())
                {
                    foreach (var tutor in tutores)
                    {
                        await _repositorioProposta.InserirPropostaTutorTurma(tutor.Id,
                            new List<PropostaTutorTurma>
                            {
                                new PropostaTutorTurma
                                {
                                    PropostaTutorId = tutor.Id,
                                    TurmaId = propostaTurmaAdicional.Id
                                }
                            });
                    }
                }

                transacao.Commit();

                return propostaTurmaAdicional.Id;
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
