﻿namespace SME.ConectaFormacao.Infra.Servicos.Options
{
    public abstract class ConfiguracaoRabbit
    {
        public static string Secao => "";
        public string HostName { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string VirtualHost { get; set; }
        public ushort TempoHeartBeat { get; set; }
    }
}
