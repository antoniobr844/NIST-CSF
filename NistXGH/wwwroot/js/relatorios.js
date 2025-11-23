// relatorios.js - Gerenciador da página de Relatórios 

class RelatoriosManager {
    constructor() {
        this.dadosCenarios = [];
        this.dadosAtuais = [];
        this.funcoes = [];
        this.prioridades = [];
        this.niveis = [];
        this.filtros = {
            tabela: 'FUTURO',
            funcao: '',
            prioridade: '',
            nivel: ''
        };
        this.tabelaAtual = 'FUTURO';
    }

    async init() {
        console.log('🚀 Inicializando RelatoriosManager...');
        await this.carregarDadosBasicos();
        this.configurarEventos();
        await this.carregarDadosCenarioFuturo();
    }

    async carregarDadosBasicos() {
        try {
            console.log('📥 Carregando dados básicos...');

            // Carregar funções
            const funcoesResponse = await this.fetchAPI('/api/Funcoes');
            if (funcoesResponse && Array.isArray(funcoesResponse)) {
                this.funcoes = funcoesResponse;
                this.preencherComboFuncoes();
            } else {
                console.warn('Nenhuma função carregada');
            }

            // Carregar prioridades
            const prioridadesResponse = await this.fetchAPI('/api/Dados/prioridades');
            if (prioridadesResponse && Array.isArray(prioridadesResponse)) {
                this.prioridades = prioridadesResponse;
                this.preencherComboPrioridades();
            } else {
                console.warn('Nenhuma prioridade carregada');
            }

            // Carregar níveis
            const niveisResponse = await this.fetchAPI('/api/Dados/status');
            if (niveisResponse && Array.isArray(niveisResponse)) {
                this.niveis = niveisResponse;
                this.preencherComboNiveis();
            } else {
                console.warn('Nenhum nível carregado');
            }

            console.log('✅ Dados básicos carregados');
        } catch (error) {
            console.error('❌ Erro ao carregar dados básicos:', error);
        }
    }

    preencherComboFuncoes() {
        const select = document.getElementById('filterFunction');
        if (!select) {
            console.error('Elemento filterFunction não encontrado');
            return;
        }

        select.innerHTML = '<option value="">Todas as funções</option>';

        this.funcoes.forEach(funcao => {
            const option = document.createElement('option');
            option.value = funcao.codigo || funcao.CODIGO;
            option.textContent = `${funcao.codigo || funcao.CODIGO} - ${funcao.nome || funcao.NOME}`;
            select.appendChild(option);
        });
    }

    preencherComboPrioridades() {
        const select = document.getElementById('filterPriority');
        if (!select) {
            console.error('Elemento filterPriority não encontrado');
            return;
        }

        select.innerHTML = '<option value="">Todas as prioridades</option>';

        this.prioridades.forEach(prioridade => {
            const option = document.createElement('option');
            option.value = prioridade.id || prioridade.ID;
            option.textContent = prioridade.nivel || prioridade.NIVEL;
            select.appendChild(option);
        });
    }

    preencherComboNiveis() {
        const select = document.getElementById('filterLevel');
        if (!select) {
            console.error('Elemento filterLevel não encontrado');
            return;
        }

        select.innerHTML = '<option value="">Todos os níveis</option>';

        this.niveis.forEach(nivel => {
            const option = document.createElement('option');
            option.value = nivel.id || nivel.ID;
            option.textContent = nivel.status || nivel.STATUS || nivel.nivel || nivel.NIVEL;
            select.appendChild(option);
        });
    }

    configurarEventos() {
        console.log('⚙️ Configurando eventos...');

        // Evento para trocar tabela
        const filterTable = document.getElementById('filterTable');
        if (filterTable) {
            filterTable.addEventListener('change', (e) => {
                this.filtros.tabela = e.target.value;
                this.trocarTabela(e.target.value);
            });
        }

        // Eventos dos filtros
        const btnFiltrar = document.getElementById('btnFiltrar');
        const btnExportar = document.getElementById('btnExportar');

        if (btnFiltrar) {
            btnFiltrar.addEventListener('click', () => this.aplicarFiltros());
        }

        if (btnExportar) {
            btnExportar.addEventListener('click', () => this.exportarRelatorio());
        }

        // Eventos de change dos combos
        const filterFunction = document.getElementById('filterFunction');
        const filterPriority = document.getElementById('filterPriority');
        const filterLevel = document.getElementById('filterLevel');

        if (filterFunction) {
            filterFunction.addEventListener('change', (e) => {
                this.filtros.funcao = e.target.value;
                this.aplicarFiltros();
            });
        }

        if (filterPriority) {
            filterPriority.addEventListener('change', (e) => {
                this.filtros.prioridade = e.target.value;
                this.aplicarFiltros();
            });
        }

        if (filterLevel) {
            filterLevel.addEventListener('change', (e) => {
                this.filtros.nivel = e.target.value;
                this.aplicarFiltros();
            });
        }

        // Evento de clique nas linhas da tabela
        this.configurarEventosDeClique();
    }

    configurarEventosDeClique() {
        document.addEventListener('click', (e) => {
            const linha = e.target.closest('tr');
            if (linha && linha.parentElement && linha.parentElement.tagName === 'TBODY' && !linha.classList.contains('loading')) {
                this.abrirOpcoesEdicao(linha);
            }
        });
    }

    async fetchAPI(url) {
        try {
            console.log(`🌐 Fetching: ${url}`);
            const response = await fetch(url);
            if (!response.ok) {
                if (response.status === 404) {
                    console.warn(`❌ Endpoint não encontrado: ${url}`);
                    return null;
                }
                throw new Error(`HTTP ${response.status}: ${response.statusText}`);
            }
            const data = await response.json();
            console.log(`✅ Resposta de ${url}:`, data);
            return data;
        } catch (error) {
            console.error(`❌ Erro na API ${url}:`, error.message);
            return null;
        }
    }

    async carregarDadosCenarioFuturo() {
        const tbody = document.getElementById('tbodyCenarios');
        if (!tbody) {
            console.error('❌ Elemento tbodyCenarios não encontrado');
            return;
        }

        try {
            tbody.innerHTML = '<tr><td colspan="10" class="loading">🔄 Carregando dados do cenário futuro...</td></tr>';

            console.log('📥 Buscando dados do cenário futuro...');
            const dados = await this.fetchAPI('/api/Cenarios/futuro/formatados');

            if (!dados) {
                throw new Error('Resposta vazia da API');
            }

            if (!Array.isArray(dados)) {
                console.warn('⚠️ Dados não são um array:', dados);
                throw new Error('Formato de dados inválido');
            }

            console.log(`✅ ${dados.length} registros carregados`);
            this.dadosCenarios = dados;
            this.exibirDados(this.dadosCenarios);
            this.gerarRecomendacoes(this.dadosCenarios);

        } catch (error) {
            console.error('❌ Erro ao carregar dados futuros:', error);
            tbody.innerHTML = `
                <tr>
                    <td colspan="10" class="error">
                        ❌ Erro ao carregar dados futuros: ${error.message}
                        <br><br>
                        <button onclick="relatoriosManager.carregarDadosCenarioFuturo()" class="btn btn-primary">
                            🔄 Tentar Novamente
                        </button>
                    </td>
                </tr>
            `;
        }
    }

    async trocarTabela(tipoTabela) {
        console.log(`🔄 Trocando para tabela: ${tipoTabela}`);
        this.tabelaAtual = tipoTabela;

        const tbody = document.getElementById('tbodyCenarios');
        if (!tbody) return;

        tbody.innerHTML = '<tr><td colspan="10" class="loading">🔄 Carregando dados...</td></tr>';

        try {
            if (tipoTabela === 'FUTURO') {
                if (this.dadosCenarios.length === 0) {
                    await this.carregarDadosCenarioFuturo();
                } else {
                    this.exibirDados(this.dadosCenarios);
                }
            } else if (tipoTabela === 'ATUAL') {
                if (this.dadosAtuais.length === 0) {
                    await this.carregarDadosCenarioAtual();
                } else {
                    this.exibirDados(this.dadosAtuais);
                }
            }

            this.atualizarTituloPagina(tipoTabela);

        } catch (error) {
            console.error('❌ Erro ao trocar tabela:', error);
            tbody.innerHTML = `
                <tr>
                    <td colspan="10" class="error">
                        ❌ Erro ao carregar dados: ${error.message}
                        <br><br>
                        <button onclick="relatoriosManager.trocarTabela('${tipoTabela}')" class="btn btn-primary">
                            🔄 Tentar Novamente
                        </button>
                    </td>
                </tr>
            `;
        }
    }

    async carregarDadosCenarioAtual() {
        const tbody = document.getElementById('tbodyCenarios');
        if (!tbody) {
            console.error('❌ Elemento tbodyCenarios não encontrado');
            return;
        }

        try {
            tbody.innerHTML = '<tr><td colspan="10" class="loading">🔄 Carregando dados do cenário atual...</td></tr>';

            console.log('📥 Buscando dados do cenário atual...');
            const dados = await this.fetchAPI('/api/Cenarios/atual/formatados');

            if (!dados) {
                throw new Error('Resposta vazia da API');
            }

            if (!Array.isArray(dados)) {
                console.warn('⚠️ Dados não são um array:', dados);
                throw new Error('Formato de dados inválido');
            }

            console.log(`✅ ${dados.length} registros do cenário atual carregados`);
            this.dadosAtuais = dados;
            this.exibirDados(this.dadosAtuais);
            this.gerarRecomendacoes(this.dadosAtuais);

        } catch (error) {
            console.error('❌ Erro ao carregar dados atuais:', error);
            tbody.innerHTML = `
                <tr>
                    <td colspan="10" class="error">
                        ❌ Erro ao carregar dados atuais: ${error.message}
                        <br><br>
                        <button onclick="relatoriosManager.carregarDadosCenarioAtual()" class="btn btn-primary">
                            🔄 Tentar Novamente
                        </button>
                    </td>
                </tr>
            `;
        }
    }

    atualizarTituloPagina(tipoTabela) {
        const titulo = document.querySelector('h1');
        if (titulo) {
            if (tipoTabela === 'FUTURO') {
                titulo.innerHTML = '📊 Relatórios - Cenário Futuro';
            } else {
                titulo.innerHTML = '📈 Relatórios - Cenário Atual';
            }
        }
    }

    aplicarFiltros() {
        console.log('🔍 Aplicando filtros:', this.filtros);

        let dadosParaFiltrar = [];

        if (this.tabelaAtual === 'FUTURO') {
            dadosParaFiltrar = this.dadosCenarios;
        } else {
            dadosParaFiltrar = this.dadosAtuais;
        }

        if (dadosParaFiltrar.length === 0) {
            console.warn('⚠️ Nenhum dado disponível para filtrar');
            return;
        }

        const dadosFiltrados = dadosParaFiltrar.filter(cenario => {
            const matchesFuncao = !this.filtros.funcao ||
                (cenario.funcaoCodigo && cenario.funcaoCodigo === this.filtros.funcao) ||
                (cenario.subcategoriaFormatada && cenario.subcategoriaFormatada.includes(this.filtros.funcao));

            const matchesPrioridade = !this.filtros.prioridade ||
                cenario.prioridade == this.filtros.prioridade;

            const matchesNivel = !this.filtros.nivel ||
                cenario.nivel == this.filtros.nivel;

            return matchesFuncao && matchesPrioridade && matchesNivel;
        });

        console.log(`📊 ${dadosFiltrados.length} registros após filtro`);
        this.exibirDados(dadosFiltrados);
        this.gerarRecomendacoes(dadosFiltrados);
    }

    exibirDados(dados) {
        const tbody = document.getElementById('tbodyCenarios');
        if (!tbody) {
            console.error('❌ Elemento tbodyCenarios não encontrado');
            return;
        }

        if (!dados || dados.length === 0) {
            tbody.innerHTML = '<tr><td colspan="10" class="no-data">📭 Nenhum dado encontrado com os filtros aplicados.</td></tr>';
            this.atualizarResumos([]);
            return;
        }

        let html = '';
        dados.forEach((cenario, index) => {
            const subcategoria = cenario.subcategoriaFormatada || `ID:${cenario.subcategoriaId}`;
            const prioridade = cenario.prioridade || 'N/A';
            const nivel = cenario.nivel || 'N/A';
            const politica = cenario.politica || 'N/A';
            const pratica = cenario.pratica || 'N/A';
            const artefato = cenario.artefato || 'N/A';
            const funcao = cenario.funcaoCodigo || 'N/A';
            const referencia = cenario.referencia || 'N/A';
            const dataRegistro = cenario.dataRegistro ? new Date(cenario.dataRegistro).toLocaleDateString('pt-BR') : 'N/A';

            html += `
                <tr class="clickable-row" 
                    data-id="${cenario.id || 'N/A'}" 
                    data-subcategoria-id="${cenario.subcategoriaId}">
                    <td>${cenario.id || 'N/A'}</td>
                    <td><strong>${subcategoria}</strong></td>
                    <td><span class="priority-badge priority-${this.getPrioridadeClass(prioridade)}">${this.getPrioridadeTexto(prioridade)}</span></td>
                    <td><span class="level-badge level-${nivel}">Nível ${nivel}</span></td>
                    <td>${this.truncarTexto(politica, 30)}</td>
                    <td>${this.truncarTexto(pratica, 30)}</td>
                    <td>${this.truncarTexto(artefato, 30)}</td>
                    <td>${funcao}</td>
                    <td>${this.truncarTexto(referencia, 30)}</td>
                    <td>${dataRegistro}</td>
                </tr>
            `;
        });

        tbody.innerHTML = html;
        this.atualizarResumos(dados);
    }

    truncarTexto(texto, maxLength) {
        if (!texto || texto === 'N/A') return texto;
        if (texto.length <= maxLength) return texto;
        return texto.substring(0, maxLength) + '...';
    }

    abrirOpcoesEdicao(linha) {
        const id = linha.cells[0].textContent.trim();
        const subcategoria = linha.cells[1].textContent.trim();
        const subcategoriaId = linha.dataset.subcategoriaId;

        if (!id || id === 'N/A') {
            alert('⚠️ Não é possível editar este registro - ID inválido');
            return;
        }

        // Determinar o tipo de cenário baseado na combobox selecionada
        const filterTable = document.getElementById('filterTable');
        const tipoCenario = filterTable ? filterTable.value : 'FUTURO';

        const modal = document.createElement('div');
        modal.className = 'modal-edicao';
        modal.innerHTML = `
            <div class="modal-content">
                <div class="modal-header">
                    <h3>✏️ Editar Registro</h3>
                </div>
                <div class="modal-info">
                    <p><strong>ID:</strong> ${id}</p>
                    <p><strong>Subcategoria:</strong> ${subcategoria}</p>
                    <p><strong>Tipo de Cenário:</strong> <span class="edicao-badge">${tipoCenario}</span></p>
                </div>
                <div class="modal-actions">
                    <button class="btn btn-success" onclick="relatoriosManager.editarCenario('${tipoCenario}', ${id}, ${subcategoriaId})">
                        <i class="fas fa-edit"></i> Editar Cenário ${tipoCenario}
                    </button>
                    <button class="btn btn-secondary" onclick="relatoriosManager.fecharModal()">
                        <i class="fas fa-times"></i> Cancelar
                    </button>
                </div>
            </div>
        `;

        document.body.appendChild(modal);
    }

    fecharModal() {
        const modal = document.querySelector('.modal-edicao');
        if (modal) {
            modal.remove();
        }
    }

    editarCenario(tipo, id, subcategoriaId) {
        this.fecharModal();

        if (!id || !subcategoriaId) {
            alert('❌ Erro: Não foi possível identificar o registro para edição');
            return;
        }

        const url = `/Home/EdicaoCenario?id=${id}&tipo=${tipo}&subcategoriaId=${subcategoriaId}`;
        console.log('🔗 Redirecionando para:', url);
        window.location.href = url;
    }

    getPrioridadeClass(prioridade) {
        switch (prioridade.toString()) {
            case '1': return 'low';
            case '2': return 'medium';
            case '3': return 'high';
            default: return 'medium';
        }
    }

    getPrioridadeTexto(prioridade) {
        switch (prioridade.toString()) {
            case '1': return 'Baixa';
            case '2': return 'Média';
            case '3': return 'Alta';
            default: return 'N/A';
        }
    }

    atualizarResumos(dados) {
        const totalRegistros = document.getElementById('totalRegistros');
        const totalSubcategorias = document.getElementById('totalSubcategorias');
        const nivelMedio = document.getElementById('nivelMedio');
        const prioridadeMedia = document.getElementById('prioridadeMedia');

        if (!totalRegistros || !totalSubcategorias || !nivelMedio || !prioridadeMedia) {
            console.error('❌ Elementos de resumo não encontrados');
            return;
        }

        // Total de Registros
        totalRegistros.textContent = dados.length;

        // Subcategorias Únicas
        const subcategoriasUnicas = new Set(dados.map(c => c.subcategoriaFormatada || c.subcategoriaId));
        totalSubcategorias.textContent = subcategoriasUnicas.size;

        // Nível Médio
        const niveis = dados.map(c => parseInt(c.nivel)).filter(n => !isNaN(n));
        const nivelMedia = niveis.length > 0 ? (niveis.reduce((a, b) => a + b, 0) / niveis.length).toFixed(1) : '-';
        nivelMedio.textContent = nivelMedia;

        // Prioridade Média
        const prioridades = dados.map(c => parseInt(c.prioridade)).filter(p => !isNaN(p));
        const prioridadeMediaVal = prioridades.length > 0 ? (prioridades.reduce((a, b) => a + b, 0) / prioridades.length).toFixed(1) : '-';
        prioridadeMedia.textContent = prioridadeMediaVal;
    }

    gerarRecomendacoes(dados) {
        const listaRecomendacoes = document.getElementById('listaRecomendacoes');
        if (!listaRecomendacoes) {
            console.error('❌ Elemento listaRecomendacoes não encontrado');
            return;
        }

        if (!dados || dados.length === 0) {
            listaRecomendacoes.innerHTML = '<div class="recommendation-item">📭 Nenhum dado disponível para gerar recomendações.</div>';
            return;
        }

        let recomendacoes = [];

        const prioridadesAltas = dados.filter(c => c.prioridade == '1').length;
        if (prioridadesAltas > 0) {
            recomendacoes.push(`⚠️ Existem ${prioridadesAltas} subcategorias com prioridade alta que requerem atenção imediata.`);
        }

        const niveis = dados.map(c => parseInt(c.nivel)).filter(n => !isNaN(n));
        if (niveis.length > 0) {
            const nivelMax = Math.max(...niveis);
            const nivelMin = Math.min(...niveis);
            if (nivelMax - nivelMin > 2) {
                recomendacoes.push(`📈 Há uma grande variação nos níveis de maturidade (${nivelMin} a ${nivelMax}). Considere uniformizar a abordagem.`);
            }
        }

        const funcoes = {};
        dados.forEach(c => {
            const funcao = c.funcaoCodigo || (c.subcategoriaFormatada ? c.subcategoriaFormatada.split('.')[0] : 'Outras');
            funcoes[funcao] = (funcoes[funcao] || 0) + 1;
        });

        const funcaoMaisComum = Object.keys(funcoes).reduce((a, b) => funcoes[a] > funcoes[b] ? a : b, '');
        if (funcaoMaisComum && funcoes[funcaoMaisComum] > 0) {
            recomendacoes.push(`👥 A função "${funcaoMaisComum}" possui o maior número de subcategorias (${funcoes[funcaoMaisComum]}).`);
        }

        if (recomendacoes.length === 0) {
            recomendacoes.push('✅ Os dados analisados apresentam uma distribuição equilibrada. Continue monitorando o progresso.');
        }

        listaRecomendacoes.innerHTML = recomendacoes.map(rec =>
            `<div class="recommendation-item">${rec}</div>`
        ).join('');
    }

    exportarRelatorio() {
        let dadosExportar = [];
        let nomeArquivo = '';

        if (this.tabelaAtual === 'FUTURO') {
            dadosExportar = this.dadosCenarios;
            nomeArquivo = 'futuro';
        } else {
            dadosExportar = this.dadosAtuais;
            nomeArquivo = 'atual';
        }

        if (!dadosExportar.length) {
            alert('❌ Não há dados para exportar.');
            return;
        }

        const headers = this.tabelaAtual === 'FUTURO'
            ? ['ID', 'Subcategoria', 'Prioridade Alvo', 'Nível Alvo', 'Política', 'Prática', 'Artefato', 'Função', 'Referência', 'Data Registro']
            : ['ID', 'Subcategoria', 'Prioridade', 'Nível', 'Política', 'Prática', 'Artefato', 'Função', 'Referência', 'Justificativa', 'Notas', 'Considerações', 'Data Registro'];

        const csvContent = [
            headers.join(','),
            ...dadosExportar.map(cenario => {
                const baseFields = [
                    cenario.id || '',
                    `"${(cenario.subcategoriaFormatada || `ID:${cenario.subcategoriaId}`).replace(/"/g, '""')}"`,
                    cenario.prioridade || '',
                    cenario.nivel || '',
                    `"${(cenario.politica || '').replace(/"/g, '""')}"`,
                    `"${(cenario.pratica || '').replace(/"/g, '""')}"`,
                    `"${(cenario.artefato || '').replace(/"/g, '""')}"`,
                    `"${(cenario.funcaoCodigo || '').replace(/"/g, '""')}"`,
                    `"${(cenario.referencia || '').replace(/"/g, '""')}"`
                ];

                if (this.tabelaAtual === 'ATUAL') {
                    baseFields.push(
                        `"${(cenario.justificativa || '').replace(/"/g, '""')}"`,
                        `"${(cenario.notas || '').replace(/"/g, '""')}"`,
                        `"${(cenario.consideracoes || '').replace(/"/g, '""')}"`
                    );
                }

                baseFields.push(cenario.dataRegistro || '');
                return baseFields.join(',');
            })
        ].join('\n');

        const blob = new Blob([csvContent], { type: 'text/csv;charset=utf-8;' });
        const link = document.createElement('a');
        const url = URL.createObjectURL(blob);
        link.setAttribute('href', url);
        link.setAttribute('download', `relatorio_cenario_${nomeArquivo}_${new Date().toISOString().split('T')[0]}.csv`);
        link.style.visibility = 'hidden';
        document.body.appendChild(link);
        link.click();
        document.body.removeChild(link);

        console.log(`📥 Relatório ${nomeArquivo} exportado com sucesso`);
    }
}

// Inicializar quando o DOM estiver pronto
document.addEventListener('DOMContentLoaded', function () {
    console.log('📄 DOM carregado, inicializando RelatoriosManager...');
    window.relatoriosManager = new RelatoriosManager();
    window.relatoriosManager.init().catch(error => {
        console.error('❌ Erro na inicialização:', error);
    });
});