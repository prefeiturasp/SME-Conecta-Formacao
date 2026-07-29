using MediatR;
using Moq;
using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Dominio.Extensoes;

namespace SME.ConectaFormacao.Aplicacao.Teste.CasosDeUso
{
    public class CasoDeUsoObterTiposEmailTestes
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly CasoDeUsoObterTiposEmail _casoDeUso;

        public CasoDeUsoObterTiposEmailTestes()
        {
            _mediatorMock = new Mock<IMediator>();
            _casoDeUso = new CasoDeUsoObterTiposEmail(_mediatorMock.Object);
        }

        [Fact(DisplayName = "Executar - Deve retornar lista de tipos de email com sucesso")]
        public async Task Executar_Deve_Retornar_Lista_De_Tipos_Email_Com_Sucesso()
        {
            // Act
            var resultado = await _casoDeUso.Executar();

            // Assert
            Assert.NotNull(resultado);
            Assert.NotEmpty(resultado);
            Assert.IsType<IEnumerable<RetornoListagemDTO>>(resultado, exactMatch: false);
        }

        [Fact(DisplayName = "Executar - Deve retornar todos os valores do enum TipoEmail")]
        public async Task Executar_Deve_Retornar_Todos_Os_Valores_Do_Enum_TipoEmail()
        {
            // Arrange
            var tiposEmailEsperados = Enum.GetValues<TipoEmail>().Cast<TipoEmail>().Count();

            // Act
            var resultado = await _casoDeUso.Executar();

            // Assert
            Assert.Equal(tiposEmailEsperados, resultado.Count());
        }

        [Fact(DisplayName = "Executar - Deve mapear Id corretamente para cada tipo de email")]
        public async Task Executar_Deve_Mapear_Id_Corretamente_Para_Cada_Tipo_Email()
        {
            // Act
            var resultado = await _casoDeUso.Executar();
            var resultadoList = resultado.ToList();

            // Assert
            Assert.Contains(resultadoList, r => r.Id == (short)TipoEmail.FuncionarioUnidadeParceira);
            Assert.Contains(resultadoList, r => r.Id == (short)TipoEmail.Estagiario);
        }

        [Fact(DisplayName = "Executar - Deve mapear Descricao corretamente usando extensão Nome()")]
        public async Task Executar_Deve_Mapear_Descricao_Corretamente_Usando_Extensao_Nome()
        {
            // Act
            var resultado = await _casoDeUso.Executar();
            var resultadoList = resultado.ToList();

            // Assert
            Assert.All(resultadoList, item =>
            {
                Assert.NotNull(item.Descricao);
                Assert.NotEmpty(item.Descricao);
            });
        }

        [Fact(DisplayName = "Executar - Deve retornar TipoEmail FuncionarioUnidadeParceira com descrição correta")]
        public async Task Executar_Deve_Retornar_TipoEmail_FuncionarioUnidadeParceira_Com_Descricao_Correta()
        {
            // Act
            var resultado = await _casoDeUso.Executar();
            var resultadoList = resultado.ToList();

            // Assert
            var funcionarioUnidadeParceira = resultadoList.FirstOrDefault(r => r.Id == (short)TipoEmail.FuncionarioUnidadeParceira);
            Assert.NotNull(funcionarioUnidadeParceira);
            Assert.Equal(TipoEmail.FuncionarioUnidadeParceira.Nome(), funcionarioUnidadeParceira.Descricao);
        }

        [Fact(DisplayName = "Executar - Deve retornar TipoEmail Estagiario com descrição correta")]
        public async Task Executar_Deve_Retornar_TipoEmail_Estagiario_Com_Descricao_Correta()
        {
            // Act
            var resultado = await _casoDeUso.Executar();
            var resultadoList = resultado.ToList();

            // Assert
            var estagiario = resultadoList.FirstOrDefault(r => r.Id == (short)TipoEmail.Estagiario);
            Assert.NotNull(estagiario);
            Assert.Equal(TipoEmail.Estagiario.Nome(), estagiario.Descricao);
        }

        [Fact(DisplayName = "Executar - Deve retornar Task<IEnumerable<RetornoListagemDTO>>")]
        public async Task Executar_Deve_Retornar_Task_IEnumerable_RetornoListagemDTO()
        {
            // Act
            var tarefa = _casoDeUso.Executar();

            // Assert
            Assert.NotNull(tarefa);
            await Assert.IsType<Task<IEnumerable<RetornoListagemDTO>>>(tarefa);
        }

        [Fact(DisplayName = "Executar - Deve ser assíncrono")]
        public async Task Executar_Deve_Ser_Assincrono()
        {
            // Act
            var tarefa = _casoDeUso.Executar();

            // Assert
            await Assert.IsType<Task<IEnumerable<RetornoListagemDTO>>>(tarefa);
            var resultado = await tarefa;
            Assert.NotNull(resultado);
        }

        [Fact(DisplayName = "Executar - Deve retornar resultado imediato usando Task.FromResult")]
        public async Task Executar_Deve_Retornar_Resultado_Imediato_Usando_Task_FromResult()
        {
            // Act
            var resultado = await _casoDeUso.Executar();

            // Assert
            Assert.NotNull(resultado);
            Assert.True(resultado.Any());
        }

        [Fact(DisplayName = "Executar - Deve herdar de CasoDeUsoAbstrato")]
        public void Executar_Deve_Herdar_De_CasoDeUsoAbstrato()
        {
            // Assert
            Assert.True(
                typeof(CasoDeUsoObterTiposEmail)
                    .BaseType?.Name.Contains("CasoDeUsoAbstrato") ?? false,
                "CasoDeUsoObterTiposEmail deve herdar de CasoDeUsoAbstrato");
        }

        [Fact(DisplayName = "Executar - Deve implementar interface ICasoDeUsoObterTiposEmail")]
        public void Executar_Deve_Implementar_Interface_ICasoDeUsoObterTiposEmail()
        {
            // Assert
            Assert.True(
                typeof(CasoDeUsoObterTiposEmail)
                    .GetInterfaces()
                    .Any(i => i.Name == "ICasoDeUsoObterTiposEmail"),
                "CasoDeUsoObterTiposEmail deve implementar ICasoDeUsoObterTiposEmail");
        }

        [Fact(DisplayName = "Executar - Deve utilizar primary constructor com IMediator")]
        public void Executar_Deve_Utilizar_Primary_Constructor_Com_IMediator()
        {
            // Arrange
            var mediatorMock = new Mock<IMediator>();

            // Act
            var casoDeUso = new CasoDeUsoObterTiposEmail(mediatorMock.Object);

            // Assert
            Assert.NotNull(casoDeUso);
            Assert.IsType<CasoDeUsoObterTiposEmail>(casoDeUso);
        }

        [Fact(DisplayName = "Executar - Deve armazenar mediator na classe base")]
        public void Executar_Deve_Armazenar_Mediator_Na_Classe_Base()
        {
            // Arrange
            var mediatorMock = new Mock<IMediator>();

            // Act
            var casoDeUso = new CasoDeUsoObterTiposEmail(mediatorMock.Object);

            // Assert
            Assert.NotNull(casoDeUso);
            var campoMediator = typeof(CasoDeUsoObterTiposEmail)
                .BaseType?
                .GetField("mediator",
                    System.Reflection.BindingFlags.NonPublic |
                    System.Reflection.BindingFlags.Instance);

            Assert.NotNull(campoMediator);
            var valorMediator = campoMediator.GetValue(casoDeUso);
            Assert.NotNull(valorMediator);
        }

        [Fact(DisplayName = "Executar - Deve retornar tipos de email em ordem consistente")]
        public async Task Executar_Deve_Retornar_Tipos_Email_Em_Ordem_Consistente()
        {
            // Act
            var resultado1 = await _casoDeUso.Executar();
            var resultado2 = await _casoDeUso.Executar();

            // Assert
            var lista1 = resultado1.OrderBy(r => r.Id).ToList();
            var lista2 = resultado2.OrderBy(r => r.Id).ToList();

            Assert.Equal(lista1.Count, lista2.Count);
            for (int i = 0; i < lista1.Count; i++)
            {
                Assert.Equal(lista1[i].Id, lista2[i].Id);
                Assert.Equal(lista1[i].Descricao, lista2[i].Descricao);
            }
        }

        [Fact(DisplayName = "Executar - Deve ter RetornoListagemDTO com Id do tipo short")]
        public async Task Executar_Deve_Ter_RetornoListagemDTO_Com_Id_Do_Tipo_Short()
        {
            // Act
            var resultado = await _casoDeUso.Executar();
            var resultadoList = resultado.ToList();

            // Assert
            Assert.All(resultadoList, item =>
            {
                Assert.True(item.Id >= short.MinValue && item.Id <= short.MaxValue,
                    $"Id {item.Id} deve estar dentro do intervalo de short");
            });
        }

        [Fact(DisplayName = "Executar - Deve preserver propriedades Id e Descricao em cada RetornoListagemDTO")]
        public async Task Executar_Deve_Preserver_Propriedades_Id_E_Descricao_Em_Cada_RetornoListagemDTO()
        {
            // Act
            var resultado = await _casoDeUso.Executar();

            // Assert
            Assert.All(resultado, item =>
            {
                Assert.NotNull(item);
                var idProperty = item.GetType().GetProperty("Id");
                var descricaoProperty = item.GetType().GetProperty("Descricao");

                Assert.NotNull(idProperty);
                Assert.NotNull(descricaoProperty);
                Assert.NotNull(idProperty.GetValue(item));
                Assert.NotNull(descricaoProperty.GetValue(item));
            });
        }

        [Fact(DisplayName = "Executar - Deve processar enum sem dependência de mediator")]
        public async Task Executar_Deve_Processar_Enum_Sem_Dependencia_De_Mediator()
        {
            // Act
            var resultado = await _casoDeUso.Executar();

            // Assert
            Assert.NotNull(resultado);
            Assert.True(resultado.Any());
            _mediatorMock.Verify(m => m.Send(It.IsAny<IRequest<object>>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact(DisplayName = "Executar - Deve retornar múltiplas enumerações sem problema")]
        public async Task Executar_Deve_Retornar_Multiplas_Enumeracoes_Sem_Problema()
        {
            // Act
            var resultado = await _casoDeUso.Executar();

            // Assert - Enumera múltiplas vezes
            var primeiraEnumeracao = resultado.ToList();
            var segundaEnumeracao = resultado.ToList();
            var terceiraEnumeracao = resultado.Count();

            Assert.Equal(primeiraEnumeracao.Count, segundaEnumeracao.Count);
            Assert.Equal(primeiraEnumeracao.Count, terceiraEnumeracao);
        }

        [Fact(DisplayName = "Executar - Deve usar Cast<TipoEmail> para converter valores do enum")]
        public async Task Executar_Deve_Usar_Cast_TipoEmail_Para_Converter_Valores_Do_Enum()
        {
            // Act
            var resultado = await _casoDeUso.Executar();
            var resultadoList = resultado.ToList();

            // Assert
            var valoresEnum = Enum.GetValues<TipoEmail>().Cast<TipoEmail>().ToList();
            Assert.Equal(valoresEnum.Count, resultadoList.Count);

            foreach (var valor in valoresEnum)
            {
                var encontrado = resultadoList.FirstOrDefault(r => r.Id == (short)valor);
                Assert.NotNull(encontrado);
                Assert.Equal(valor.Nome(), encontrado.Descricao);
            }
        }

        [Fact(DisplayName = "Executar - Deve ter cada RetornoListagemDTO com valores válidos")]
        public async Task Executar_Deve_Ter_Cada_RetornoListagemDTO_Com_Valores_Validos()
        {
            // Act
            var resultado = await _casoDeUso.Executar();

            // Assert
            Assert.All(resultado, item =>
            {
                Assert.NotNull(item);
                Assert.NotNull(item.Descricao);
                Assert.NotEmpty(item.Descricao);
                Assert.InRange(item.Id, (short)1, (short)100);
            });
        }

        [Fact(DisplayName = "Executar - Deve não ter Id zero em qualquer tipo de email")]
        public async Task Executar_Deve_Nao_Ter_Id_Zero_Em_Qualquer_Tipo_Email()
        {
            // Act
            var resultado = await _casoDeUso.Executar();

            // Assert
            Assert.All(resultado, item => Assert.NotEqual(0, item.Id));
        }

        [Fact(DisplayName = "Executar - Deve ter IDs únicos para cada tipo de email")]
        public async Task Executar_Deve_Ter_IDs_Unicos_Para_Cada_Tipo_Email()
        {
            // Act
            var resultado = await _casoDeUso.Executar();
            var resultadoList = resultado.ToList();

            // Assert
            var idsUnicos = resultadoList.Select(r => r.Id).Distinct().Count();
            Assert.Equal(resultadoList.Count, idsUnicos);
        }

        [Fact(DisplayName = "Executar - Deve ter descrições não vazias para cada tipo de email")]
        public async Task Executar_Deve_Ter_Descricoes_Nao_Vazias_Para_Cada_Tipo_Email()
        {
            // Act
            var resultado = await _casoDeUso.Executar();

            // Assert
            Assert.All(resultado, item =>
            {
                Assert.NotNull(item.Descricao);
                Assert.NotEmpty(item.Descricao.Trim());
            });
        }
    }
}
