﻿namespace SME.ConectaFormacao.Dominio.ObjetosDeValor
{
    public class FormacaoTurma
    {
        public long Id { get; set; }
        public string Nome { get; set; }
        public string Local { get; set; }
        public string HoraInicio { get; set; }
        public string HoraFim { get; set; }
        public long PropostaEncontroId { get; set; }
        public IEnumerable<FormacaoTurmaData> Periodos { get; set; }
    }
}
