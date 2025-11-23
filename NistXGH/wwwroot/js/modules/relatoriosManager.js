// relatoriosManager.js - Gerenciador da página de relatórios
class RelatoriosManager {
    constructor() {
        this.cenarios = [];
        this.subcategorias = [];
        this.funcoes = [];
        this.filtros = {
            funcao: '',
            prioridade: '',
            nivel: ''
        };
    }

    async init() {
        try {
            console.log('=== INICIANDO RELATÓRIOS MANAGER ===');

            this.configurarEventos();
            await this.carregarDados();
            this.aplicarFiltros();

        } catch (error) {
            console.error('Erro na inicialização:', error);
            this.mostrarErro('Erro ao carregar a página de relatórios.');
        }
    }

    configurarEventos() {
        // Evento do botão filtrar
        document.getElementById('btnFiltrar').addEventListener('click', () => {
            this.aplicarFiltros();
        });

        // Evento do botão exportar
        document.getElementById('btnExportar').addEventListener('click', () => {
            this.exportarRelatorio();
        });

        // Eventos de change nos selects
        document.getElementById('filterFunction').addEventListener('change', (e) => {
            this.filtros.funcao = e.target.value;
        });

        document.getElementById('filterPriority').addEventListener('change', (e) => {
            this.filtros.prioridade = e.target.value;
        });

        document.getElementById('filterLevel').addEventListener('change', (e) => {
            this.filtros.nivel = e.target.value;
        });

        // Enter nos filtros
        document.querySelectorAll('.filter-group select').forEach(select => {
            select.addEventListener('keypress', (e) => {
                if (e.key === 'Enter') {
                    this.aplicarFiltros();
                }
            });
        });
    }

    async carregarDados() {
        try {
            this.mostrarLoading(true);

            // Carregar dados em paralelo
            const [cenarios, subcategorias, funcoes] = await Promise.all([
                this.fetchAPI('/api/Cenarios/futuro/'),
                this.fetchAPI('/api/Subcategorias'),
                this.fetchAPI('/api/Funcoes')
            ]);

            this.cenarios = cenarios || [];
            this.subcategorias = subcategorias || [];
            this.funcoes = funcoes || [];

            console.log('📊 Dados carregados:', {
                cenarios: this.cenarios.length,
                subcategorias: this.subcategorias.length,
                funcoes: this.funcoes.length
            });

            if (this.cenarios.length === 0) {
                this.mostrarMensagemVazia('Nenhum dado de cenário futuro encontrado.');
                return;
            }

            this.mostrarLoading(false);

        } catch (error) {
            console.error('Erro ao carregar dados:', error);
            this.mostrarErro('Erro ao carregar dados do servidor.');
            this.mostrarLoading(false);
        }
    }

    aplicarFiltros() {
        let dadosFiltrados = [...this.cenarios];

        // Aplicar filtro de função
        if (this.filtros.funcao) {
            dadosFiltrados = dadosFiltrados.filter(cenario => {
                const subcategoria = this.subcategorias.find(sub => sub.id === cenario.subcategoriaId);
                if (!subcategoria) return false;

                const funcao = this.funcoes.find(f => f.id === subcategoria.funcaoId);
                return funcao && funcao.codigo === this.filtros.funcao;
            });
        }

        // Aplicar filtro de prioridade
        if (this.filtros.prioridade) {
            dadosFiltrados = dadosFiltrados.filter(cenario =>
                cenario.prioridadeAlvo == this.filtros.prioridade
            );
        }

        // Aplicar filtro de nível
        if (this.filtros.nivel) {
            dadosFiltrados = dadosFiltrados.filter(cenario =>
                cenario.nivelAlvo == this.filtros.nivel
            );
        }

        this.exibirDados(dadosFiltrados);
        this.atualizarResumo(dadosFiltrados);
        this.gerarRecomendacoes(dadosFiltrados);
    }

    exibirDados(cenarios) {
        const tbody = document.getElementById('tbodyCenarios');
        if (!tbody) return;

        if (cenarios.length === 0) {
            tbody.innerHTML = `
                <tr>
                    <td colspan="10" class="empty-message">
                        Nenhum dado encontrado com os filtros aplicados
                    </td>
                </tr>
            `;
            return;
        }

        const html = cenarios.map(cenario => {
            const subcategoriaInfo = this.subcategorias.find(sub => sub.id === cenario.subcategoriaId);
            const subcategoriaFormatada = this.formatarSubcategoria(subcategoriaInfo);

            return `
                <tr>
                    <td>${cenario.id}</td>
                    <td class="subcategoria-completa">${subcategoriaFormatada}</td>
                    <td>${this.formatarPrioridade(cenario.prioridadeAlvo)}</td>
                    <td>
                        ${cenario.nivelAlvo ?
                    `<span class="maturity-level level-${cenario.nivelAlvo}">${cenario.nivelAlvo}</span>` :
                    '-'
                }
                    </td>
                    <td>${cenario.politicasAlvo || '-'}</td>
                    <td>${cenario.praticasAlvo || '-'}</td>
                    <td>${cenario.artefatosAlvo || '-'}</td>
                    <td>${cenario.funcoesAlvo || '-'}</td>
                    <td>${cenario.referenciasAlvo || '-'}</td>
                    <td>${this.formatarData(cenario.dataRegistro)}</td>
                </tr>
            `;
        }).join('');

        tbody.innerHTML = html;
    }

    formatarSubcategoria(subcategoria) {
        if (!subcategoria) return 'Subcategoria não encontrada';

        const funcaoInfo = this.funcoes.find(f => f.id === subcategoria.funcaoId);
        const codigoFuncao = funcaoInfo ? funcaoInfo.codigo : 'FN';

        return `${codigoFuncao}.${subcategoria.categoria}-${subcategoria.codigo}`;
    }

    formatarPrioridade(prioridade) {
        const prioridades = {
            '1': 'Alta',
            '2': 'Média',
            '3': 'Baixa'
        };
        return prioridades[prioridade] || prioridade || '-';
    }

    formatarData(dataString) {
        if (!dataString) return '-';
        try {
            const data = new Date(dataString);
            return data.toLocaleDateString('pt-BR');
        } catch {
            return dataString;
        }
    }

    atualizarResumo(cenarios) {
        const totalRegistros = document.getElementById('totalRegistros');
        const totalSubcategorias = document.getElementById('totalSubcategorias');
        const nivelMedio = document.getElementById('nivelMedio');

        if (totalRegistros) totalRegistros.textContent = cenarios.length;

        // Contar subcategorias únicas
        const subcategoriasUnicas = new Set(cenarios.map(c => c.subcategoriaId));
        if (totalSubcategorias) totalSubcategorias.textContent = subcategoriasUnicas.size;

        // Calcular nível médio
        const niveis = cenarios
            .map(c => parseInt(c.nivelAlvo))
            .filter(nivel => !isNaN(nivel));

        if (niveis.length > 0) {
            const media = niveis.reduce((a, b) => a + b, 0) / niveis.length;
            if (nivelMedio) nivelMedio.textContent = media.toFixed(1);
        } else {
            if (nivelMedio) nivelMedio.textContent = '-';
        }
    }

    gerarRecomendacoes(cenarios) {
        const lista = document.getElementById('listaRecomendacoes');
        if (!lista) return;

        if (cenarios.length === 0) {
            lista.innerHTML = `
                <div class="recommendation-item">
                    Nenhuma recomendação disponível sem dados.
                </div>
            `;
            return;
        }

        const recomendacoes = [];

        // Análise de prioridades
        const altaPrioridade = cenarios.filter(c => c.prioridadeAlvo == '1').length;
        if (altaPrioridade > 0) {
            recomendacoes.push({
                texto: `${altaPrioridade} subcategoria(s) com prioridade alta requerem atenção imediata.`,
                prioridade: 'high'
            });
        }

        // Análise de níveis
        const niveisBaixos = cenarios.filter(c => c.nivelAlvo && c.nivelAlvo <= 2).length;
        if (niveisBaixos > 0) {
            recomendacoes.push({
                texto: `${niveisBaixos} subcategoria(s) estão nos níveis 1 ou 2 - considere ações de melhoria.`,
                prioridade: 'medium'
            });
        }

        // Análise de completude
        const incompletos = cenarios.filter(c =>
            !c.politicasAlvo && !c.praticasAlvo && !c.artefatosAlvo
        ).length;

        if (incompletos > 0) {
            recomendacoes.push({
                texto: `${incompletos} registro(s) estão incompletos - complete as informações para melhor análise.`,
                prioridade: 'medium'
            });
        }

        if (recomendacoes.length === 0) {
            recomendacoes.push({
                texto: 'Os dados analisados estão bem distribuídos. Continue monitorando o progresso.',
                prioridade: 'normal'
            });
        }

        lista.innerHTML = recomendacoes.map(rec => `
            <div class="recommendation-item ${rec.prioridade !== 'normal' ? `priority-${rec.prioridade}` : ''}">
                ${rec.texto}
            </div>
        `).join('');
    }

    exportarRelatorio() {
        try {
            const tabela = document.getElementById('tabelaCenarios');
            const html = tabela.outerHTML;
            const blob = new Blob([html], { type: 'application/vnd.ms-excel' });
            const url = URL.createObjectURL(blob);
            const a = document.createElement('a');

            a.href = url;
            a.download = `relatorio-nist-${new Date().toISOString().split('T')[0]}.xls`;
            document.body.appendChild(a);
            a.click();
            document.body.removeChild(a);
            URL.revokeObjectURL(url);

            this.mostrarMensagem('Relatório exportado com sucesso!', 'success');

        } catch (error) {
            console.error('Erro ao exportar:', error);
            this.mostrarMensagem('Erro ao exportar relatório.', 'error');
        }
    }

    async fetchAPI(endpoint) {
        try {
            const response = await fetch(endpoint);

            if (!response.ok) {
                if (response.status === 404) return null;
                throw new Error(`HTTP ${response.status}`);
            }

            return await response.json();
        } catch (error) {
            console.error(`Erro API ${endpoint}:`, error);
            return null;
        }
    }

    mostrarLoading(mostrar) {
        const tbody = document.getElementById('tbodyCenarios');
        if (!tbody) return;

        if (mostrar) {
            tbody.innerHTML = `
                <tr>
                    <td colspan="10" class="loading-message">
                        Carregando dados...
                    </td>
                </tr>
            `;
        }
    }

    mostrarMensagemVazia(mensagem) {
        const tbody = document.getElementById('tbodyCenarios');
        if (tbody) {
            tbody.innerHTML = `
                <tr>
                    <td colspan="10" class="empty-message">
                        ${mensagem}
                    </td>
                </tr>
            `;
        }
    }

    mostrarErro(mensagem) {
        this.mostrarMensagem(mensagem, 'error');
    }

    mostrarMensagem(mensagem, tipo = 'info') {
        // Remove mensagens anteriores
        const mensagensAntigas = document.querySelectorAll('.message-temporary');
        mensagensAntigas.forEach(msg => msg.remove());

        const messageDiv = document.createElement('div');
        messageDiv.className = `message-temporary ${tipo === 'error' ? 'error-message' : 'success-message'}`;
        messageDiv.textContent = mensagem;

        const container = document.querySelector('.container');
        if (container) {
            container.insertBefore(messageDiv, container.firstChild);

            // Remove após 5 segundos
            setTimeout(() => {
                if (messageDiv.parentNode) {
                    messageDiv.remove();
                }
            }, 5000);
        }
    }
}

// Instância global
const relatoriosManager = new RelatoriosManager();