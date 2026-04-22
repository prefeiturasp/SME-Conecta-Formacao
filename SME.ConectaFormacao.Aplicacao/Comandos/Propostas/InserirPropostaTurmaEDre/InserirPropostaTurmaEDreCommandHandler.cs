using MediatR;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Extensoes;
using SME.ConectaFormacao.Infra.Dados;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao
{
    public class InserirPropostaTurmaAdicionalCommandHandler(
        ITransacao transacao, 
        IRepositorioProposta repositorioProposta,
        IRepositorioPropostaEncontro repositorioPropostaEncontro) : 
        IRequestHandler<InserirPropostaTurmaAdicionalCommand, long>
    {
        public async Task<long> Handle(InserirPropostaTurmaAdicionalCommand request, CancellationToken cancellationToken)
        {
            var propostaTurma = await repositorioProposta.ObterTurmaPorId(request.PropostaTurmaOrigemId);

            var dres = await repositorioProposta.ObterPropostaTurmasDresPorPropostaTurmaId(request.PropostaTurmaOrigemId);
            var encontros = await repositorioPropostaEncontro.ObterEncontrosPorPropostaTurmaAsync(request.PropostaTurmaOrigemId);
            var regentes = await repositorioProposta.ObterRegentesPorPropostaTurmaId(request.PropostaTurmaOrigemId);
            var tutores = await repositorioProposta.ObterTutoresPorPropostaTurmaId(request.PropostaTurmaOrigemId);

            var propostaTurmaAdicional = (PropostaTurma)propostaTurma.Clone();

            //Parte 2...Parte 3...Parte 4
            var indexParte = propostaTurma.Nome.IndexOf(" - Parte ");
            var contador = indexParte > -1 && propostaTurma.CriadoPor == "Sistema" ? int.Parse(propostaTurma.Nome.Substring(indexParte + 9)) : 1;
            propostaTurmaAdicional.Nome = propostaTurmaAdicional.Nome.Replace($" - Parte {contador}", "");

            contador++;
            propostaTurmaAdicional.Nome += $" - Parte {contador}";

            var transacaoAtual = transacao.Iniciar();
            try
            {
                await repositorioProposta.InserirTurma(propostaTurmaAdicional);

                if (dres.PossuiElementos())
                {
                    foreach (var propostaTurmaDre in dres)
                        propostaTurmaDre.PropostaTurmaId = propostaTurmaAdicional.Id;

                    await repositorioProposta.InserirPropostaTurmasDres(dres);
                }

                if (encontros.PossuiElementos())
                {
                    foreach (var encontro in encontros)
                    {
                        await repositorioPropostaEncontro.InserirEncontroTurmasAsync(encontro.Id,
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
                        await repositorioProposta.InserirPropostaRegenteTurma(regente.Id,
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
                        await repositorioProposta.InserirPropostaTutorTurma(tutor.Id,
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

                await repositorioProposta.InserirPropostaTurmaVagas(new PropostaTurmaVaga
                {
                    PropostaTurmaId = propostaTurmaAdicional.Id
                }, request.QuantidadeVagasTurma);

                transacaoAtual.Commit();

                return propostaTurmaAdicional.Id;
            }
            catch
            {
                transacaoAtual.Rollback();
                throw;
            }
            finally
            {
                transacaoAtual.Dispose();
            }
        }
    }
}
