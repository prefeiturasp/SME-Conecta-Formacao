using ConectaFormacao.Dominio.Servicos;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace SME.ConectaFormacao.Infra.Dados.Servicos
{
    public class PeriodoRealizacaoConsultaService : IPeriodoRealizacaoConsultaService
    {
        private readonly IRepositorioPeriodoRealizacaoConsulta _repositorio;

        public PeriodoRealizacaoConsultaService(
            IRepositorioPeriodoRealizacaoConsulta repositorio)
        {
            _repositorio = repositorio;
        }

        public async Task<PeriodoRealizacao?> ObterPeriodoRealizacaoAsync(long propostaTurmaId)
        {
            if (propostaTurmaId <= 0)
                throw new ArgumentException("Id da turma inválido.");

            var periodo = await _repositorio
                .ObterPeriodoRealizacaoAsync(propostaTurmaId);

            if (periodo is null)
                return null;

            if (periodo.DataInicio > periodo.DataFim)
                throw new InvalidOperationException(
                    "Período de realização inválido: Data início maior que data fim.");

            return periodo;
        }
    }
}
