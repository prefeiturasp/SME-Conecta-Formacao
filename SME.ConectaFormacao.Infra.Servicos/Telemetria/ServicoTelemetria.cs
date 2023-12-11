using Elastic.Apm;
using Elastic.Apm.Api;
using SME.ConectaFormacao.Infra.Servicos.Telemetria.Options;

namespace SME.ConectaFormacao.Infra.Servicos.Telemetria
{
    public class ServicoTelemetria : IServicoTelemetria
    {
        private readonly TelemetriaOptions _telemetriaOptions;

        public ServicoTelemetria(TelemetriaOptions telemetriaOptions)
        {
            _telemetriaOptions = telemetriaOptions;
        }

        public async Task<dynamic> RegistrarComRetornoAsync<T>(Func<Task<object>> acao, string acaoNome, string telemetriaNome, string telemetriaValor, string parametros = "")
        {
            dynamic result = default;

            if (_telemetriaOptions.Apm)
            {
                var transactionElk = Agent.Tracer.CurrentTransaction;

                await transactionElk.CaptureSpan(telemetriaNome, acaoNome, async (span) =>
                 {
                     span.SetLabel(telemetriaNome, telemetriaValor);
                     span.SetLabel("Parametros", parametros);
                     result = await acao();
                 });
            }
            else
            {
                result = await acao();
            }

            return result;
        }

        public dynamic RegistrarComRetorno<T>(Func<object> acao, string acaoNome, string telemetriaNome, string telemetriaValor, string parametros = "")
        {
            dynamic result = default;

            if (_telemetriaOptions.Apm)
            {
                var transactionElk = Agent.Tracer.CurrentTransaction;

                transactionElk.CaptureSpan(telemetriaNome, acaoNome, (span) =>
                {
                    span.SetLabel(telemetriaNome, telemetriaValor);
                    span.SetLabel("Parametros", parametros);
                    result = acao();
                });
            }
            else
            {
                result = acao();
            }

            return result;
        }

        public void Registrar(Action acao, string acaoNome, string telemetriaNome, string telemetriaValor)
        {
            if (_telemetriaOptions.Apm)
            {
                var transactionElk = Agent.Tracer.CurrentTransaction;

                transactionElk.CaptureSpan(telemetriaNome, acaoNome, (span) =>
                {
                    span.SetLabel(telemetriaNome, telemetriaValor);
                    acao();
                });
            }
            else
            {
                acao();
            }
        }

        public async Task RegistrarAsync(Func<Task> acao, string acaoNome, string telemetriaNome, string telemetriaValor, string parametros = "")
        {
            if (_telemetriaOptions.Apm)
            {
                var transactionElk = Agent.Tracer.CurrentTransaction;

                if (transactionElk != null)
                {
                    await transactionElk.CaptureSpan(telemetriaNome, acaoNome, async (span) =>
                    {
                        span.SetLabel(telemetriaNome, telemetriaValor);
                        span.SetLabel("Parametros", parametros);
                        await acao();
                    });
                }
                else
                    await acao();
            }
            else
                await acao();
        }

        public ITransaction Iniciar(string nome, string tipo)
        {
            return _telemetriaOptions.Apm ?
                Agent.Tracer.StartTransaction(nome, tipo) :
                null;
        }

        public void Finalizar(ITransaction transacao)
        {
            if (_telemetriaOptions.Apm)
                transacao.End();
        }
    }
}
