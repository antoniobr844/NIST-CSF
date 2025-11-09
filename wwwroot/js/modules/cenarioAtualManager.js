// cenarioAtualManager.js - Versão sem módulos ES6
class CenarioAtualManager {
  constructor () {
    this.subcategoriasSelecionadas = []
    this.funcoes = []
    this.categorias = []
  }

  async init () {
    console.log('CenarioAtualManager iniciando...')

    try {
      // Configurar eventos
      this.configurarEventos()

      // Carregar dados
      await this.carregarSubcategoriasSelecionadas()

      // Atualizar interface
      this.atualizarInterface()
    } catch (error) {
      console.error('Erro na inicialização:', error)
      this.mostrarErro()
    }
  }

  configurarEventos () {
    const btnLimparTudo = document.getElementById('btnLimparTudo')
    const btnAvancar = document.getElementById('btnAvancar')

    if (btnLimparTudo) {
      btnLimparTudo.addEventListener('click', () => this.desmarcarTudo())
    }

    if (btnAvancar) {
      btnAvancar.addEventListener('click', () => this.avancarParaAlteracoes())
    }
  }

  async carregarSubcategoriasSelecionadas () {
    try {
      console.log('Carregando subcategorias selecionadas...')

      // Substitua pela sua chamada API real
      const response = await this.fetchSubcategoriasSelecionadas()

      if (response && Array.isArray(response)) {
        this.subcategoriasSelecionadas = response
        console.log(
          `Carregadas ${this.subcategoriasSelecionadas.length} subcategorias`
        )
      } else {
        this.subcategoriasSelecionadas = []
        console.log('Nenhuma subcategoria encontrada')
      }
    } catch (error) {
      console.error('Erro ao carregar subcategorias:', error)
      this.subcategoriasSelecionadas = []
      throw error
    }
  }

  async fetchSubcategoriasSelecionadas () {
    // EXEMPLO - Substitua pela sua implementação real
    try {
      const response = await fetch('/api/Subcategorias/Selecionadas')
      if (!response.ok) throw new Error('Erro na resposta da API')
      return await response.json()
    } catch (error) {
      console.error('Erro na requisição API:', error)
      // Fallback: retorna array vazio ou dados mock para teste
      return this.getDadosMock()
    }
  }

  getDadosMock () {
    // Dados mock para teste - remova quando a API estiver funcionando
    return [
      {
        id: 1,
        codigo: 'ID.AM-1',
        nome: 'Identificação de Ativos',
        selecionada: true,
        funcao: 'ID',
        categoria: 'AM'
      },
      {
        id: 2,
        codigo: 'PR.AC-1',
        nome: 'Controle de Acesso',
        selecionada: true,
        funcao: 'PR',
        categoria: 'AC'
      },
      {
        id: 3,
        codigo: 'GV.OV-1',
        nome: 'Revisão da Estratégia',
        selecionada: true,
        funcao: 'GV',
        categoria: 'OV'
      }
    ]
  }

  atualizarInterface () {
    const loadingMessage = document.getElementById('loadingMessage')
    const subcategoriasContainer = document.getElementById(
      'subcategoriasContainer'
    )
    const emptyMessage = document.getElementById('emptyMessage')
    const errorMessage = document.getElementById('errorMessage')
    const btnAvancar = document.getElementById('btnAvancar')

    // Esconder loading
    if (loadingMessage) loadingMessage.classList.add('hidden')
    if (errorMessage) errorMessage.classList.add('hidden')

    if (this.subcategoriasSelecionadas.length === 0) {
      // Mostrar mensagem de vazio
      if (emptyMessage) emptyMessage.classList.remove('hidden')
      if (subcategoriasContainer) subcategoriasContainer.classList.add('hidden')
      if (btnAvancar) btnAvancar.disabled = true
    } else {
      // Mostrar subcategorias
      if (emptyMessage) emptyMessage.classList.add('hidden')
      if (subcategoriasContainer) {
        subcategoriasContainer.classList.remove('hidden')
        this.renderizarSubcategorias(subcategoriasContainer)
      }
      if (btnAvancar) btnAvancar.disabled = false
    }

    // Atualizar contadores
    this.atualizarContadores()
  }

  renderizarSubcategorias (container) {
    const html = this.subcategoriasSelecionadas
      .map(
        sub => `
            <div class="subcategoria-item" data-id="${sub.id}">
                <label class="subcategoria-checkbox">
                    <input type="checkbox" ${sub.selecionada ? 'checked' : ''} 
                           onchange="cenarioAtualManager.toggleSubcategoria(${
                             sub.id
                           })">
                    <div class="subcategoria-content">
                        <strong>${sub.codigo}</strong>
                        <span class="subcategoria-descricao">${sub.nome}</span>
                    </div>
                </label>
            </div>
        `
      )
      .join('')

    container.innerHTML = html
  }

  atualizarContadores () {
    const totalSubcategorias = document.getElementById('totalSubcategorias')
    const totalFuncoes = document.getElementById('totalFuncoes')
    const totalCategorias = document.getElementById('totalCategorias')

    const subcategoriasSelecionadas = this.subcategoriasSelecionadas.filter(
      s => s.selecionada
    )

    if (totalSubcategorias) {
      totalSubcategorias.textContent = subcategoriasSelecionadas.length
    }

    // Calcular funções e categorias únicas
    const funcoesUnicas = new Set()
    const categoriasUnicas = new Set()

    subcategoriasSelecionadas.forEach(sub => {
      if (sub.funcao) {
        funcoesUnicas.add(sub.funcao)
      }
      if (sub.categoria) {
        categoriasUnicas.add(sub.categoria)
      }
    })

    if (totalFuncoes) totalFuncoes.textContent = funcoesUnicas.size
    if (totalCategorias) totalCategorias.textContent = categoriasUnicas.size
  }

  toggleSubcategoria (id) {
    const subcategoria = this.subcategoriasSelecionadas.find(s => s.id === id)
    if (subcategoria) {
      subcategoria.selecionada = !subcategoria.selecionada
      this.atualizarContadores()

      // Atualizar visualmente o checkbox
      const checkbox = document.querySelector(
        `[data-id="${id}"] input[type="checkbox"]`
      )
      if (checkbox) {
        checkbox.checked = subcategoria.selecionada
      }
    }
  }

  desmarcarTudo () {
    if (confirm('Tem certeza que deseja limpar todas as seleções?')) {
      this.subcategoriasSelecionadas.forEach(sub => {
        sub.selecionada = false
      })
      this.atualizarInterface()
    }
  }

  avancarParaAlteracoes () {
    // Implemente a navegação para a próxima página
    console.log('Avançando para edição...')
    // window.location.href = '/Cenario/Alteracoes'; // Ajuste a URL
    alert('Funcionalidade "Avançar para Edição" será implementada!')
  }

  mostrarErro () {
    const loadingMessage = document.getElementById('loadingMessage')
    const errorMessage = document.getElementById('errorMessage')

    if (loadingMessage) loadingMessage.classList.add('hidden')
    if (errorMessage) errorMessage.classList.remove('hidden')
  }
}

// Instância global - isso substitui o export
window.cenarioAtualManager = new CenarioAtualManager()
