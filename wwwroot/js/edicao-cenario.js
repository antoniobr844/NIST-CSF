// edicao-cenario.js - Gerenciador da página de Edição de Cenário (SEM HISTÓRICO)

class EdicaoCenarioManager {
  constructor () {
    this.config = this.obterConfiguracao()
    this.app = null
    this.inicializado = false
    this.tentativasCarregamento = 0
    this.maxTentativas = 3
  }

  obterConfiguracao () {
    const urlParams = new URLSearchParams(window.location.search)

    const config = {
      modo: 'edicao',
      modoEdicao: {
        ativo: true,
        cenarioId: parseInt(urlParams.get('id')),
        tipoCenario: urlParams.get('tipo'),
        subcategoriaId: parseInt(urlParams.get('subcategoriaId'))
      }
    }

    console.log('🔧 Configuração obtida da URL:', config)

    // Validação rigorosa
    if (
      !config.modoEdicao.cenarioId ||
      !config.modoEdicao.tipoCenario ||
      !config.modoEdicao.subcategoriaId
    ) {
      const erro = `Parâmetros de edição incompletos: id=${config.modoEdicao.cenarioId}, tipo=${config.modoEdicao.tipoCenario}, subcategoriaId=${config.modoEdicao.subcategoriaId}`
      console.error('❌', erro)
      throw new Error(erro)
    }

    return config
  }

  async inicializar () {
    try {
      console.log('🚀 Inicializando modo edição...', this.config.modoEdicao)

      // Verificar se NISTCore está disponível
      if (typeof NISTCore === 'undefined') {
        throw new Error(
          'NISTCore não foi carregado. Verifique o caminho do script.'
        )
      }

      this.configurarEventos()
      this.app = new NISTCore(this.config)

      await this.carregarDados()
      this.inicializado = true

      console.log('✅ Modo edição inicializado com sucesso')
      this.mostrarStatus('Sistema carregado com sucesso!', 'success')
    } catch (error) {
      console.error('❌ Erro ao inicializar edição:', error)

      // Tentar recarregar se for problema de dependência
      if (
        error.message.includes('NISTCore') &&
        this.tentativasCarregamento < this.maxTentativas
      ) {
        this.tentativasCarregamento++
        console.log(
          `🔄 Tentativa ${this.tentativasCarregamento} de recarregamento...`
        )
        setTimeout(() => this.inicializar(), 1000)
        return
      }

      this.mostrarErro(error)
    }
  }

  configurarEventos () {
    // Eventos dos botões - REMOVIDO btnHistorico
    const btnSalvar = document.getElementById('btnSalvar')
    const btnVoltar = document.getElementById('btnVoltar')

    if (btnSalvar) btnSalvar.addEventListener('click', () => this.salvar())
    if (btnVoltar) btnVoltar.addEventListener('click', () => this.voltar())

    // Eventos de teclado
    document.addEventListener('keydown', e => {
      if (e.ctrlKey && e.key === 's') {
        e.preventDefault()
        this.salvar()
      }
    })
  }

  async carregarDados () {
    try {
      console.log('📥 Iniciando carregamento de dados para edição...')

      // Garantir que o modo edição está ativo no NISTCore
      this.app.config.modoEdicao = this.config.modoEdicao
      this.app.config.modo = 'edicao'

      const steps = [
        {
          name: 'Carregando seleção...',
          action: () => this.carregarSelecaoUnica()
        },
        {
          name: 'Carregando prioridades...',
          action: () => this.app.carregarPrioridades()
        },
        {
          name: 'Carregando níveis...',
          action: () => this.app.carregarNiveis()
        },
        {
          name: 'Carregando informações...',
          action: () => this.carregarInformacoesDetalhadasEdicao()
        },
        {
          name: 'Carregando dados do cenário...',
          action: () => this.app.carregarDadosCenarios()
        }
      ]

      for (const step of steps) {
        console.log(`🔄 ${step.name}`)
        this.atualizarStatusCarregamento(step.name)
        await step.action()
      }

      console.log('✅ Todos os dados carregados, exibindo cenário...')

      // Chamar a exibição - deve detectar automaticamente o modo edição
      this.app.exibirCenarios()

      this.habilitarInterface()
    } catch (error) {
      console.error('❌ Erro no carregamento de dados:', error)
      throw error
    }
  }

  async carregarSelecaoUnica () {
    const { subcategoriaId } = this.config.modoEdicao

    this.app.selections = {
      1: {
        1: [subcategoriaId]
      }
    }

    console.log('🎯 Seleção única criada para edição:', this.app.selections)
    return true
  }

  async carregarInformacoesDetalhadasEdicao () {
    try {
      const { subcategoriaId } = this.config.modoEdicao
      console.log('📥 Carregando informações detalhadas para edição...')

      // Carregar subcategoria específica
      if (!this.app.cache.subcategorias[subcategoriaId]) {
        const subcategoria = await this.app.fetchAPI(
          `/api/Subcategorias/${subcategoriaId}`
        )
        if (subcategoria) {
          this.app.cache.subcategorias[subcategoriaId] = subcategoria
        }
      }

      // Carregar categorias relacionadas
      const categorias = await this.app.fetchAPI('/api/Categorias')
      if (categorias?.length) {
        categorias.forEach(categoria => {
          this.app.cache.categorias[categoria.id || categoria.ID] = categoria
        })
      }

      console.log('✅ Informações detalhadas carregadas para edição')
    } catch (error) {
      console.error('❌ Erro ao carregar informações para edição:', error)
      throw error
    }
  }

  atualizarStatusCarregamento (mensagem) {
    const element = document.getElementById('selectionInfo')
    if (element) {
      element.innerHTML = `<div class="loading">${mensagem}</div>`
    }
  }

  habilitarInterface () {
    const btnSalvar = document.getElementById('btnSalvar')
    if (btnSalvar) btnSalvar.disabled = false

    const selectionInfo = document.getElementById('selectionInfo')
    if (selectionInfo) {
      selectionInfo.innerHTML = `
                        <div class="alert-info">
                            <i class="fas fa-check-circle"></i>
                            <strong>Pronto para editar!</strong> Preencha os campos abaixo e clique em "Salvar Alterações".
                        </div>
                    `
    }
  }

  async salvar () {
    if (!this.inicializado) return

    try {
      await this.app.salvarAlteracoes()
    } catch (error) {
      console.error('Erro ao salvar:', error)
      this.mostrarStatus('Erro ao salvar alterações!', 'error')
    }
  }

  voltar () {
    if (
      confirm(
        'Tem certeza que deseja voltar? As alterações não salvas serão perdidas.'
      )
    ) {
      window.location.href = '/Home/Relatorios'
    }
  }

  mostrarStatus (mensagem, tipo = 'info') {
    console.log(`[${tipo.toUpperCase()}] ${mensagem}`)
  }

  mostrarErro (error) {
    const errorHtml = `
                    <div class="error">
                        <h3><i class="fas fa-exclamation-triangle"></i> Erro ao carregar dados para edição</h3>
                        <p>${error.message}</p>
                        <p><small>Verifique se o registro ainda existe e se você tem permissão para editá-lo.</small></p>
                        <div style="margin-top: 15px;">
                            <button onclick="location.reload()" class="btn btn-secondary">
                                <i class="fas fa-redo"></i> Tentar Novamente
                            </button>
                            <button onclick="window.location.href = '/Home/Relatorios'" class="btn btn-secondary">
                                <i class="fas fa-arrow-left"></i> Voltar para Relatórios
                            </button>
                        </div>
                    </div>
                `

    const selectionInfo = document.getElementById('selectionInfo')
    if (selectionInfo) {
      selectionInfo.innerHTML = errorHtml
    }
  }
}

// === INICIALIZAÇÃO SEGURA DA PÁGINA ===
function inicializarPaginaEdicao () {
  // Verificar se os elementos necessários existem
  const elementosRequeridos = ['selectionInfo', 'edicaoScenarioContainer']
  const elementosFaltantes = elementosRequeridos.filter(
    id => !document.getElementById(id)
  )

  if (elementosFaltantes.length > 0) {
    console.error(
      '❌ Elementos necessários não encontrados:',
      elementosFaltantes
    )
    return
  }

  console.log('🔍 Inicializando página de edição...')

  // Aguardar o DOM estar completamente pronto
  if (document.readyState === 'loading') {
    document.addEventListener('DOMContentLoaded', () => {
      iniciarEdicaoManager()
    })
  } else {
    iniciarEdicaoManager()
  }
}

async function iniciarEdicaoManager () {
  try {
    console.log('🚀 Iniciando EdicaoCenarioManager...')

    // Verificar dependências críticas
    if (typeof NISTCore === 'undefined') {
      throw new Error(
        'NISTCore não está disponível. Verifique se o script foi carregado corretamente.'
      )
    }

    const editor = new EdicaoCenarioManager()
    await editor.inicializar()

    // Expor globalmente para debug (apenas em desenvolvimento)
    if (
      window.location.hostname === 'localhost' ||
      window.location.hostname === '127.0.0.1'
    ) {
      window.edicaoManager = editor
    }

    console.log('✅ Editor de cenário inicializado com sucesso')
  } catch (error) {
    console.error('❌ Falha crítica na inicialização:', error)

    // Fallback para carregamento manual se NISTCore não estiver disponível
    if (error.message.includes('NISTCore')) {
      console.warn('⚠️ Tentando carregar NISTCore manualmente...')

      const script = document.createElement('script')
      script.src = '/js/core/nist-core.js'
      script.onload = () => {
        console.log(
          '✅ NISTCore carregado manualmente. Reiniciando inicialização...'
        )
        setTimeout(() => iniciarEdicaoManager(), 500)
      }
      script.onerror = () => {
        console.error('❌ Falha ao carregar NISTCore manualmente.')
        mostrarErroCritico(
          'Não foi possível carregar o sistema NISTCore. Verifique se o arquivo existe em /js/core/nist-core.js'
        )
      }
      document.head.appendChild(script)
    } else {
      mostrarErroCritico(error.message)
    }
  }
}

function mostrarErroCritico (mensagem) {
  const errorHtml = `
        <div class="error">
            <h3><i class="fas fa-exclamation-triangle"></i> Erro Crítico</h3>
            <p>${mensagem}</p>
            <div style="margin-top: 15px;">
                <button onclick="location.reload()" class="btn btn-secondary">
                    <i class="fas fa-redo"></i> Tentar Novamente
                </button>
                <button onclick="window.location.href = '/Home/Relatorios'" class="btn btn-secondary">
                    <i class="fas fa-arrow-left"></i> Voltar para Relatórios
                </button>
            </div>
        </div>
    `

  const container =
    document.getElementById('selectionInfo') ||
    document.querySelector('.edicao-container')
  if (container) {
    container.innerHTML = errorHtml
  }
}

// Inicializar quando o script for carregado
inicializarPaginaEdicao()
