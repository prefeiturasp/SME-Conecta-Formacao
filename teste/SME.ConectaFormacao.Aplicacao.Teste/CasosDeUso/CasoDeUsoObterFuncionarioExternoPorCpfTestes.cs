using MediatR;
using Moq;
using SME.ConectaFormacao.Aplicacao.CasosDeUso.FuncionarioExterno.ObterFuncionarioExternoPorCpf;
using SME.ConectaFormacao.Aplicacao.Consultas.Eol.ObterDadosFuncionarioExterno;
using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Aplicacao.Dtos.FuncionarioExterno;
using SME.ConectaFormacao.Infra.Servicos.Eol;

namespace SME.ConectaFormacao.Aplicacao.Teste.CasosDeUso
{
    public class CasoDeUsoObterFuncionarioExternoPorCpfTestes
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly CasoDeUsoObterFuncionarioExternoPorCpf _casoDeUso;

        public CasoDeUsoObterFuncionarioExternoPorCpfTestes()
        {
            _mediatorMock = new Mock<IMediator>();
            _casoDeUso = new CasoDeUsoObterFuncionarioExternoPorCpf(_mediatorMock.Object);
        }

        [Fact(DisplayName = "Executar - Deve retornar FuncionarioExternoDTO quando encontrar contratos")]
        public async Task Executar_Deve_Retornar_FuncionarioExternoDTO_Quando_Encontrar_Contratos()
        {
            const string cpf = "12345678901";
            var contratos = new List<FuncionarioExternoServicoEol>
            {
                new()
                {
                    NomePessoa = "João da Silva",
                    Cpf = cpf,
                    CodigoUE = "001",
                    NomeUe = "Escola Municipal A"
                },
                new()
                {
                    NomePessoa = "João da Silva",
                    Cpf = cpf,
                    CodigoUE = "002",
                    NomeUe = "Escola Municipal B"
                },
                new()
                {
                    NomePessoa = "João da Silva",
                    Cpf = cpf,
                    CodigoUE = "001",
                    NomeUe = "Escola Municipal A"
                }
            };

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<ObterDadosFuncionarioExternoQuery>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(contratos);

            var resultado = await _casoDeUso.Executar(cpf);

            Assert.NotNull(resultado);
            Assert.Equal("João da Silva", resultado.NomePessoa);
            Assert.Equal(cpf, resultado.Cpf);
            Assert.Equal("001", resultado.CodigoUE);
            Assert.Equal("Escola Municipal A", resultado.NomeUe);
            Assert.NotNull(resultado.Ues);
            Assert.Equal(2, resultado.Ues.Count()); 

            var uesLista = resultado.Ues.OrderBy(x => x.Id).ToList();
            Assert.Equal(1L, uesLista[0].Id);
            Assert.Equal("Escola Municipal A", uesLista[0].Descricao);
            Assert.Equal(2L, uesLista[1].Id);
            Assert.Equal("Escola Municipal B", uesLista[1].Descricao);

            _mediatorMock.Verify(
                m => m.Send(
                    It.Is<ObterDadosFuncionarioExternoQuery>(q => q.Cpf == cpf),
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact(DisplayName = "Executar - Deve retornar null quando não encontrar contratos")]
        public async Task Executar_Deve_Retornar_Null_Quando_Nao_Encontrar_Contratos()
        {
            const string cpf = "99999999999";

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<ObterDadosFuncionarioExternoQuery>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync((IEnumerable<FuncionarioExternoServicoEol>?)null);

            var resultado = await _casoDeUso.Executar(cpf);

            Assert.Null(resultado);

            _mediatorMock.Verify(
                m => m.Send(
                    It.Is<ObterDadosFuncionarioExternoQuery>(q => q.Cpf == cpf),
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact(DisplayName = "Executar - Deve retornar null quando lista de contratos está vazia")]
        public async Task Executar_Deve_Retornar_Null_Quando_Lista_Contratos_Vazia()
        {
            const string cpf = "11111111111";

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<ObterDadosFuncionarioExternoQuery>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync([]);

            var resultado = await _casoDeUso.Executar(cpf);

            Assert.Null(resultado);
        }

        [Fact(DisplayName = "Executar - Deve usar FirstOrDefault para obter primeiro contrato")]
        public async Task Executar_Deve_Usar_FirstOrDefault_Para_Primeiro_Contrato()
        {
            const string cpf = "22222222222";
            var contratos = new List<FuncionarioExternoServicoEol>
            {
                new()
                {
                    NomePessoa = "Primeiro",
                    Cpf = cpf,
                    CodigoUE = "100",
                    NomeUe = "Unidade Primeira"
                },
                new()
                {
                    NomePessoa = "Segundo",
                    Cpf = cpf,
                    CodigoUE = "200",
                    NomeUe = "Unidade Segunda"
                }
            };

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<ObterDadosFuncionarioExternoQuery>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(contratos);

            var resultado = await _casoDeUso.Executar(cpf);

            Assert.NotNull(resultado);
            Assert.Equal("Primeiro", resultado.NomePessoa);
            Assert.Equal("100", resultado.CodigoUE);
            Assert.Equal("Unidade Primeira", resultado.NomeUe);
        }

        [Fact(DisplayName = "Executar - Deve fazer conversão correta de CodigoUE string para long")]
        public async Task Executar_Deve_Converter_CodigoUE_Para_Long()
        {
            const string cpf = "33333333333";
            var contratos = new List<FuncionarioExternoServicoEol>
            {
                new()
                {
                    NomePessoa = "Teste",
                    Cpf = cpf,
                    CodigoUE = "999",
                    NomeUe = "Unidade Teste"
                }
            };

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<ObterDadosFuncionarioExternoQuery>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(contratos);

            var resultado = await _casoDeUso.Executar(cpf);

            Assert.NotNull(resultado);
            Assert.NotNull(resultado.Ues);
            var ue = resultado.Ues.FirstOrDefault();
            Assert.NotNull(ue);
            Assert.Equal(999L, ue.Id);
            Assert.IsType<long>(ue.Id);
        }

        [Fact(DisplayName = "Executar - Deve aplicar DistinctBy corretamente nas UEs")]
        public async Task Executar_Deve_Aplicar_DistinctBy_Nas_Ues()
        {
            const string cpf = "44444444444";
            var contratos = new List<FuncionarioExternoServicoEol>
            {
                new()
                {
                    NomePessoa = "Pessoa",
                    Cpf = cpf,
                    CodigoUE = "050",
                    NomeUe = "Unidade A"
                },
                new()
                {
                    NomePessoa = "Pessoa",
                    Cpf = cpf,
                    CodigoUE = "050",
                    NomeUe = "Unidade A - Repetida"
                },
                new()
                {
                    NomePessoa = "Pessoa",
                    Cpf = cpf,
                    CodigoUE = "060",
                    NomeUe = "Unidade B"
                },
                new()
                {
                    NomePessoa = "Pessoa",
                    Cpf = cpf,
                    CodigoUE = "050",
                    NomeUe = "Unidade A - Outro NomeSocial"
                }
            };

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<ObterDadosFuncionarioExternoQuery>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(contratos);

            var resultado = await _casoDeUso.Executar(cpf);

            Assert.NotNull(resultado);
            Assert.NotNull(resultado.Ues);
            Assert.Equal(2, resultado.Ues.Count()); 
            
            var uesIds = resultado.Ues.Select(x => x.Id).OrderBy(x => x).ToList();
            Assert.Equal(50L, uesIds[0]);
            Assert.Equal(60L, uesIds[1]);
        }

        [Fact(DisplayName = "Executar - Deve passar CPF correto na query")]
        public async Task Executar_Deve_Passar_Cpf_Correto_Na_Query()
        {
            const string cpf = "55555555555";
            var contratos = new List<FuncionarioExternoServicoEol>
            {
                new()
                {
                    NomePessoa = "Teste",
                    Cpf = cpf,
                    CodigoUE = "001",
                    NomeUe = "Escola"
                }
            };

            ObterDadosFuncionarioExternoQuery? capturedQuery = null;

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<ObterDadosFuncionarioExternoQuery>(),
                    It.IsAny<CancellationToken>()))
                .Callback<IRequest<IEnumerable<FuncionarioExternoServicoEol>>, CancellationToken>(
                    (query, ct) => capturedQuery = query as ObterDadosFuncionarioExternoQuery)
                .ReturnsAsync(contratos);

            await _casoDeUso.Executar(cpf);

            Assert.NotNull(capturedQuery);
            Assert.Equal(cpf, capturedQuery.Cpf);
        }

        [Fact(DisplayName = "Executar - Deve retornar RetornoListagemDTO com dados corretos")]
        public async Task Executar_Deve_Retornar_RetornoListagemDTO_Com_Dados_Corretos()
        {
            const string cpf = "66666666666";
            var contratos = new List<FuncionarioExternoServicoEol>
            {
                new()
                {
                    NomePessoa = "João",
                    Cpf = cpf,
                    CodigoUE = "123",
                    NomeUe = "Escola Principal"
                },
                new()
                {
                    NomePessoa = "João",
                    Cpf = cpf,
                    CodigoUE = "456",
                    NomeUe = "Escola Secundária"
                }
            };

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<ObterDadosFuncionarioExternoQuery>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(contratos);

            var resultado = await _casoDeUso.Executar(cpf);

            Assert.NotNull(resultado);
            Assert.NotNull(resultado.Ues);
            
            var uesLista = resultado.Ues.ToList();
            foreach (var ue in uesLista)
            {
                Assert.NotNull(ue);
                Assert.IsType<RetornoListagemDTO>(ue);
                Assert.NotEqual(0, ue.Id);
                Assert.NotEmpty(ue.Descricao);
            }
        }

        [Fact(DisplayName = "Executar - Deve ser assíncrono")]
        public async Task Executar_Deve_Ser_Assincrono()
        {
            const string cpf = "77777777777";
            var contratos = new List<FuncionarioExternoServicoEol>
            {
                new()
                {
                    NomePessoa = "Async Test",
                    Cpf = cpf,
                    CodigoUE = "001",
                    NomeUe = "Unidade"
                }
            };

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<ObterDadosFuncionarioExternoQuery>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(contratos);

            var tarefa = _casoDeUso.Executar(cpf);

            Assert.NotNull(tarefa);
            await Assert.IsType<Task<FuncionarioExternoDTO?>>(tarefa);

            var resultado = await tarefa;
            Assert.NotNull(resultado);
        }

        [Fact(DisplayName = "Executar - Deve herdar de CasoDeUsoAbstrato")]
        public void Executar_Deve_Herdar_De_CasoDeUsoAbstrato()
        {
            Assert.True(
                typeof(CasoDeUsoObterFuncionarioExternoPorCpf)
                    .BaseType?.Name.Contains("CasoDeUsoAbstrato") ?? false,
                "CasoDeUsoObterFuncionarioExternoPorCpf deve herdar de CasoDeUsoAbstrato");
        }

        [Fact(DisplayName = "Executar - Deve implementar interface ICasoDeUsoObterFuncionarioExternoPorCpf")]
        public void Executar_Deve_Implementar_Interface()
        {
            Assert.True(
                typeof(CasoDeUsoObterFuncionarioExternoPorCpf)
                    .GetInterfaces()
                    .Any(i => i.Name == "ICasoDeUsoObterFuncionarioExternoPorCpf"),
                "CasoDeUsoObterFuncionarioExternoPorCpf deve implementar ICasoDeUsoObterFuncionarioExternoPorCpf");
        }
    }
}
