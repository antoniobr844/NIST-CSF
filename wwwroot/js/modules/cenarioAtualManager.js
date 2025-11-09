// cenarioAtualManager.js - VERSÃO FINAL SIMPLIFICADA
class CenarioAtualManager {
  constructor () {
    this.cenariosFuturo = []
    this.selectedSubcategories = {}

    // Bind dos métodos
    this.init = this.init.bind(this)
    this.configurarEventos = this.configurarEventos.bind(this)
  }

  async init () {
    console.log('CenarioAtualManager iniciando...')

    try {
      this.configurarEventos()
      await this.carregarDadosCenarioFuturo()
      this.atualizarInterface()
    } catch (error) {
      console.error('Erro na inicialização:', error)
      this.mostrarErro()
    }
  }

  configurarEventos () {
    console.log('✅ configurarEventos chamado')

    const btnLimparTudo = document.getElementById('btnLimparTudo')
    const btnAvancar = document.getElementById('btnAvancar')

    if (btnLimparTudo) {
      btnLimparTudo.addEventListener('click', () => this.desmarcarTudo())
    }

    if (btnAvancar) {
      btnAvancar.addEventListener('click', () => this.avancarParaAlteracoes())
    }
  }

  // ✅ MÉTODO SIMPLIFICADO - Foco em fazer funcionar
  // ✅ MÉTODO COM BUSCA DE DETALHES DA SUBCATEGORIA
  async carregarDadosCenarioFuturo () {
    try {
      console.log('🔍 Carregando dados do cenário futuro...')

      const response = await fetch('/api/Cenarios/futuro/')
      if (!response.ok) throw new Error(`Erro HTTP: ${response.status}`)

      const dados = await response.json()
      console.log('✅ Dados recebidos:', dados)

      // ✅ TRATAMENTO SIMPLES: Se é objeto, coloca em array
      if (dados && typeof dados === 'object') {
        this.cenariosFuturo = [dados]
        console.log('✅ Objeto único convertido para array')

        // ✅ TENTAR BUSCAR DETALHES DA SUBCATEGORIA SE HOUVER ID
        await this.buscarDetalhesSubcategoria()
      } else {
        this.cenariosFuturo = []
        console.log('ℹ️ Nenhum dado válido encontrado')
      }

      console.log('✅ Cenários futuros:', this.cenariosFuturo)

      this.carregarSelecoes()
    } catch (error) {
      console.error('❌ Erro ao carregar dados:', error)
      this.cenariosFuturo = []
    }
  }

  // ✅ BUSCAR DETALHES DA SUBCATEGORIA
  async buscarDetalhesSubcategoria () {
    try {
      if (this.cenariosFuturo.length === 0) return

      const cenario = this.cenariosFuturo[0]
      const subcategoriaId = cenario.subcategoria

      if (!subcategoriaId) {
        console.log('ℹ️ Nenhum ID de subcategoria encontrado')
        return
      }

      console.log(`🔍 Buscando detalhes da subcategoria ${subcategoriaId}...`)

      const response = await fetch(`/api/Subcategorias/${subcategoriaId}`)
      if (response.ok) {
        const detalhesSubcategoria = await response.json()
        console.log('✅ Detalhes da subcategoria:', detalhesSubcategoria)

        // ✅ ADICIONAR DETALHES AO CENÁRIO
        this.cenariosFuturo[0].detalhesSubcategoria = detalhesSubcategoria
      } else {
        console.warn(
          `⚠️ Não foi possível carregar detalhes da subcategoria ${subcategoriaId}`
        )
      }
    } catch (error) {
      console.error('❌ Erro ao buscar detalhes:', error)
    }
  }

  carregarSelecoes () {
    try {
      const saved = localStorage.getItem('nistSelections')
      this.selectedSubcategories = saved ? JSON.parse(saved) : {}
      console.log('✅ Seleções carregadas')
    } catch (error) {
      console.error('❌ Erro ao carregar seleções:', error)
      this.selectedSubcategories = {}
    }
  }

  salvarSelecoes () {
    try {
      localStorage.setItem(
        'nistSelections',
        JSON.stringify(this.selectedSubcategories)
      )
      this.atualizarContadores()
    } catch (error) {
      console.error('❌ Erro ao salvar seleções:', error)
    }
  }

  atualizarInterface () {
    const loading = document.getElementById('loadingMessage')
    const container = document.getElementById('subcategoriasContainer')
    const emptyMsg = document.getElementById('emptyMessage')
    const errorMsg = document.getElementById('errorMessage')
    const btnAvancar = document.getElementById('btnAvancar')

    // Esconder loading e erro
    if (loading) loading.classList.add('hidden')
    if (errorMsg) errorMsg.classList.add('hidden')

    if (this.cenariosFuturo.length === 0) {
      // Mostrar mensagem de vazio
      if (emptyMsg) emptyMsg.classList.remove('hidden')
      if (container) container.classList.add('hidden')
      if (btnAvancar) btnAvancar.disabled = true
    } else {
      // Mostrar dados
      if (emptyMsg) emptyMsg.classList.add('hidden')
      if (container) {
        container.classList.remove('hidden')
        this.renderizarSubcategorias(container)
      }
      if (btnAvancar) btnAvancar.disabled = false
    }

    this.atualizarContadores()
  }

  // ✅ RENDERIZAÇÃO SIMPLES - Baseada no objeto real
  // ✅ RENDERIZAÇÃO MELHORADA - Mostra dados reais do banco
  renderizarSubcategorias (container) {
    const html = this.cenariosFuturo
      .map((cenario, index) => {
        // ✅ EXTRAIR TODOS OS DADOS REIS DO OBJETO
        const subcategoriaId = cenario.subcategoria || 'Não informado'
        const prioridade =
          cenario.prioridade || cenario.prioridade_ALVO || 'Não definida'
        const nivel = cenario.nível || cenario.nível_ALVO || 'Não definido'
        const dataRegistro = cenario.data_REGISTRO || 'Data não disponível'
        const idCenario = cenario.id !== undefined ? cenario.id : 'N/A'

        // ✅ EXTRAIR MAIS INFORMAÇÕES SE DISPONÍVEIS
        const artefato =
          cenario.artef_ALVO || cenario.artefato || 'Não especificado'
        const pratica =
          cenario.prait_ALVO || cenario.pratica || 'Não especificada'
        const funcao = cenario.func_ALVO || cenario.funcao || 'Não especificada'
        const referencia =
          cenario.ref_INFO_ALVO || cenario.referencia || 'Não especificada'

        return `
      <div class="subcategoria-item">
        <label class="subcategoria-checkbox">
          <input type="checkbox" checked 
                 onchange="cenarioAtualManager.toggleSubcategoria(${index})">
          <div class="subcategoria-content">
            <strong>Subcategoria ID: ${subcategoriaId}</strong>
            <span class="subcategoria-descricao">Cenário Futuro - Registro ID: ${idCenario}</span>
            <div class="subcategoria-detalhes">
              <div class="detalhe-linha">
                <span class="detalhe-item"><strong>Prioridade:</strong> ${prioridade}</span>
                <span class="detalhe-item"><strong>Nível:</strong> ${nivel}</span>
              </div>
              <div class="detalhe-linha">
                <span class="detalhe-item"><strong>Data Registro:</strong> ${this.formatarData(
                  dataRegistro
                )}</span>
              </div>
              ${
                artefato !== 'Não especificado'
                  ? `
                <div class="detalhe-linha">
                  <span class="detalhe-item"><strong>Artefato:</strong> ${artefato}</span>
                </div>
              `
                  : ''
              }
              ${
                pratica !== 'Não especificada'
                  ? `
                <div class="detalhe-linha">
                  <span class="detalhe-item"><strong>Prática:</strong> ${pratica}</span>
                </div>
              `
                  : ''
              }
              ${
                funcao !== 'Não especificada'
                  ? `
                <div class="detalhe-linha">
                  <span class="detalhe-item"><strong>Função:</strong> ${funcao}</span>
                </div>
              `
                  : ''
              }
              ${
                referencia !== 'Não especificada'
                  ? `
                <div class="detalhe-linha">
                  <span class="detalhe-item"><strong>Referência:</strong> ${referencia}</span>
                </div>
              `
                  : ''
              }
              <small class="subcategoria-note">✓ Disponível no Cenário Futuro</small>
            </div>
          </div>
        </label>
      </div>
    `
      })
      .join('')

    container.innerHTML =
      html || '<p class="empty-message">Nenhum dado para exibir</p>'
  }

  // ✅ ADICIONE ESTE MÉTODO PARA FORMATAR DATA
  formatarData (dataString) {
    if (!dataString || dataString === 'Data não disponível') {
      return 'Data não disponível'
    }

    try {
      // Tenta converter a data para formato brasileiro
      const data = new Date(dataString)
      if (isNaN(data.getTime())) {
        return dataString // Retorna original se não for data válida
      }
      return data.toLocaleDateString('pt-BR')
    } catch (error) {
      return dataString // Retorna original em caso de erro
    }
  }

  toggleSubcategoria (index) {
    console.log(`Toggle subcategoria ${index}`)
    // Implementação básica - sempre considera selecionado para teste
    const checkbox = document.querySelectorAll('.subcategoria-checkbox input')[
      index
    ]
    if (checkbox) {
      const isChecked = checkbox.checked
      console.log(`Checkbox ${index}: ${isChecked ? 'marcado' : 'desmarcado'}`)

      // Simular armazenamento de seleção
      if (isChecked) {
        this.selectedSubcategories[index] = true
      } else {
        delete this.selectedSubcategories[index]
      }

      this.salvarSelecoes()
    }
  }

  atualizarContadores () {
    const totalSub = document.getElementById('totalSubcategorias')
    const totalFunc = document.getElementById('totalFuncoes')
    const totalCat = document.getElementById('totalCategorias')

    if (totalSub) totalSub.textContent = this.cenariosFuturo.length
    if (totalFunc) totalFunc.textContent = '1' // Pelo menos uma função
    if (totalCat) totalCat.textContent = '1' // Pelo menos uma categoria
  }

  desmarcarTudo () {
    if (confirm('Tem certeza que deseja desmarcar todas as subcategorias?')) {
      // Desmarcar visualmente
      const checkboxes = document.querySelectorAll(
        '.subcategoria-checkbox input'
      )
      checkboxes.forEach(checkbox => {
        checkbox.checked = false
      })

      // Limpar seleções
      this.selectedSubcategories = {}
      this.salvarSelecoes()

      alert('Todas as seleções foram resetadas!')
    }
  }

  avancarParaAlteracoes () {
    if (this.cenariosFuturo.length === 0) {
      alert('Nenhum cenário futuro carregado.')
      return
    }

    // Criar estrutura básica de seleções
    const selecoesParaSalvar = {
      1: {
        // ID da função Governança
        1: [this.cenariosFuturo[0].subcategoria || '1'] // ID da categoria e subcategoria
      }
    }

    try {
      localStorage.setItem('nistSelections', JSON.stringify(selecoesParaSalvar))
      console.log('✅ Seleções salvas para edição:', selecoesParaSalvar)

      // Redirecionar para Governança
      window.location.href = '/Home/Governanca'
    } catch (error) {
      console.error('❌ Erro ao avançar:', error)
      alert('Erro ao salvar seleções. Tente novamente.')
    }
  }

  mostrarErro () {
    const loading = document.getElementById('loadingMessage')
    const errorMsg = document.getElementById('errorMessage')

    if (loading) loading.classList.add('hidden')
    if (errorMsg) errorMsg.classList.remove('hidden')
  }

  // ✅ MÉTODO DE DEBUG (opcional)
  // ✅ DEBUG COMPLETO - Mostra TODOS os dados do objeto
  debugCompleto () {
    console.clear()
    console.log('=== 🗃️ DEBUG COMPLETO - TODOS OS DADOS ===')

    if (this.cenariosFuturo.length > 0) {
      const cenario = this.cenariosFuturo[0]
      console.log('📦 OBJETO COMPLETO DO CENÁRIO:', cenario)
      console.log('🔑 TODAS AS CHAVES DISPONÍVEIS:', Object.keys(cenario))

      // Mostrar cada propriedade com seu valor
      Object.keys(cenario).forEach(key => {
        console.log(`📋 ${key}:`, cenario[key])
      })
    } else {
      console.log('ℹ️ Nenhum cenário carregado')
    }

    console.log('=== DEBUG COMPLETO FINALIZADO ===')
  }
}

// Instância global
window.cenarioAtualManager = new CenarioAtualManager()
