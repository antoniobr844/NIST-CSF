// wwwroot/js/core/nist-core.js
class NISTCore {
  constructor (config = {}) {
    this.config = {
      funcaoAtual: config.funcaoAtual || 'governanca',
      modo: config.modo || 'futuro',
      ...config
    }

    // === DADOS E CONFIGURAÇÕES ===
    this.ordemFuncoes = [
      'governanca',
      'identificar',
      'proteger',
      'detectar',
      'responder',
      'recuperar'
    ]

    this.mapeamentoFuncoes = {
      1: 'governanca',
      2: 'identificar',
      3: 'proteger',
      4: 'detectar',
      5: 'responder',
      6: 'recuperar'
    }

    this.functionNames = {
      governanca: 'Governança (GV)',
      identificar: 'Identificar (ID)',
      proteger: 'Proteger (PR)',
      detectar: 'Detectar (DE)',
      responder: 'Responder (RS)',
      recuperar: 'Recuperar (RC)'
    }

    this.cache = {
      categorias: {},
      subcategorias: {},
      dadosAtuais: {},
      dadosFuturos: {},
      prioridades: [],
      niveis: []
    }

    this.selections = {}

    console.log('NISTCore criado para:', this.config.funcaoAtual)
  }

  // === FUNÇÕES BÁSICAS E UTILITÁRIAS ===
  async fetchAPI (url) {
    try {
      console.log(`Fetching: ${url}`)
      const response = await fetch(url)
      if (!response.ok) {
        if (response.status === 404) {
          console.warn(`Endpoint não encontrado: ${url}`)
          return null
        }
        throw new Error(`HTTP ${response.status}: ${response.statusText}`)
      }
      return await response.json()
    } catch (error) {
      console.error(`Erro na API ${url}:`, error.message)
      return null
    }
  }

  safeUpdateElement (elementId, content) {
    const element = document.getElementById(elementId)
    if (element) {
      element.innerHTML = content
    } else {
      console.warn(`Elemento ${elementId} não encontrado`)
    }
  }

  getFunctionNameById (funcId) {
    if (this.mapeamentoFuncoes[funcId]) {
      return this.mapeamentoFuncoes[funcId]
    }
    const funcIdLower = funcId.toString().toLowerCase()
    if (
      funcIdLower.includes('gv') ||
      funcIdLower.includes('govern') ||
      funcIdLower === '1'
    )
      return 'governanca'
    if (
      funcIdLower.includes('id') ||
      funcIdLower.includes('identif') ||
      funcIdLower === '2'
    )
      return 'identificar'
    if (
      funcIdLower.includes('pr') ||
      funcIdLower.includes('proteg') ||
      funcIdLower === '3'
    )
      return 'proteger'
    if (
      funcIdLower.includes('de') ||
      funcIdLower.includes('detect') ||
      funcIdLower === '4'
    )
      return 'detectar'
    if (
      funcIdLower.includes('rs') ||
      funcIdLower.includes('respond') ||
      funcIdLower === '5'
    )
      return 'responder'
    if (
      funcIdLower.includes('rc') ||
      funcIdLower.includes('recuper') ||
      funcIdLower === '6'
    )
      return 'recuperar'
    return funcId
  }

  funcaoEstaSelecionada () {
    for (const funcId in this.selections) {
      const functionName = this.getFunctionNameById(funcId)
      if (functionName === this.config.funcaoAtual) {
        for (const category in this.selections[funcId]) {
          if (this.selections[funcId][category].length > 0) {
            return true
          }
        }
      }
    }
    return false
  }

  // === FUNÇÕES DE CARREGAMENTO DE DADOS ===
  async carregarSelecoes () {
    try {
      const stored = localStorage.getItem('nistSelections')
      if (!stored) {
        throw new Error('Nenhuma seleção encontrada no localStorage')
      }

      this.selections = JSON.parse(stored)
      console.log('Seleções carregadas:', this.selections)

      if (!this.funcaoEstaSelecionada()) {
        this.mostrarAviso(`${this.config.funcaoAtual} não selecionada`)
        return false
      }
      return true
    } catch (error) {
      console.error('Erro ao carregar seleções:', error)
      throw error
    }
  }

  async carregarPrioridades () {
    try {
      console.log('Carregando prioridades...')
      const data = await this.fetchAPI('/api/Dados/prioridades')

      if (data && Array.isArray(data)) {
        this.cache.prioridades = data
        console.log('Prioridades carregadas:', this.cache.prioridades)
        this.preencherDropdownsPrioridades()
      } else {
        throw new Error('Não foi possível carregar prioridades')
      }
    } catch (error) {
      console.error('Erro ao carregar prioridades:', error)
    }
  }

  async carregarNiveis () {
    try {
      console.log('Carregando níveis...')
      const data = await this.fetchAPI('/api/Dados/status')

      if (data && Array.isArray(data)) {
        this.cache.niveis = data
        console.log('Níveis carregados:', this.cache.niveis)
        this.preencherDropdownsNiveis()
      } else {
        throw new Error('Não foi possível carregar níveis')
      }
    } catch (error) {
      console.error('Erro ao carregar níveis:', error)
    }
  }

  async carregarInformacoesDetalhadas () {
    try {
      console.log('Carregando informações detalhadas...')

      for (const funcId in this.selections) {
        const functionName = this.getFunctionNameById(funcId)
        if (functionName !== this.config.funcaoAtual) continue

        try {
          const categorias = await this.fetchAPI(
            `/api/Categorias?funcaoId=${funcId}`
          )
          if (categorias && Array.isArray(categorias)) {
            categorias.forEach(categoria => {
              this.cache.categorias[categoria.id || categoria.ID] = categoria
            })
          }
        } catch (error) {
          console.error('Erro ao carregar categorias:', error)
        }

        for (const categoryId in this.selections[funcId]) {
          for (const subcategoryId of this.selections[funcId][categoryId]) {
            if (!this.cache.subcategorias[subcategoryId]) {
              try {
                const subcategoria = await this.fetchAPI(
                  `/api/Subcategorias/${subcategoryId}`
                )
                if (subcategoria) {
                  this.cache.subcategorias[subcategoryId] = subcategoria
                } else {
                  this.cache.subcategorias[subcategoryId] = {
                    id: subcategoryId,
                    codigo: `SC-${subcategoryId}`,
                    descricao: 'Subcategoria não encontrada'
                  }
                }
              } catch (error) {
                console.error(
                  `Erro ao carregar subcategoria ${subcategoryId}:`,
                  error
                )
                this.cache.subcategorias[subcategoryId] = {
                  id: subcategoryId,
                  codigo: `SC-${subcategoryId}`,
                  descricao: 'Erro ao carregar'
                }
              }
            }
          }
        }
      }
      console.log('Informações detalhadas carregadas')
    } catch (error) {
      console.error('Erro ao carregar informações detalhadas:', error)
    }
  }

  async carregarDadosCenarios () {
    try {
      console.log('Carregando dados dos cenários... Modo:', this.config.modo)

      if (this.config.modo === 'futuro') {
        await this.carregarDadosFuturos()
        await this.carregarDadosAtuaisExistentes() // Mantido para carregar dados de comparação no modo futuro
      } else {
        await this.carregarDadosAtuais() // Carrega os dados atuais mais recentes
        await this.carregarDadosBanco() // Carrega os dados futuros para comparação no modo atual
      }

      console.log('✅ Dados dos cenários carregados com sucesso')
      console.log('📊 Dados atuais no cache:', this.cache.dadosAtuais)
      console.log('📊 Dados futuros no cache:', this.cache.dadosFuturos)
    } catch (error) {
      console.error('Erro ao carregar dados dos cenários:', error)
    }
  }

  // === FUNÇÕES DE CARREGAMENTO ===
  async carregarDadosFuturos () {
    try {
      console.log('=== CARREGANDO DADOS FUTUROS ===')

      for (const funcId in this.selections) {
        const functionName = this.getFunctionNameById(funcId)
        if (functionName !== this.config.funcaoAtual) continue

        for (const categoryId in this.selections[funcId]) {
          for (const subcategoryId of this.selections[funcId][categoryId]) {
            console.log(`🔍 Buscando dados futuros para ${subcategoryId}...`)

            try {
              // ✅ USAR ENDPOINT CORRETO: /api/Cenarios/futuro
              const dadosFuturos = await this.fetchAPI(
                `/api/Cenarios/futuro?subcategoriaId=${subcategoryId}`
              )
              console.log(
                `📦 Dados futuros para ${subcategoryId}:`,
                dadosFuturos
              )

              if (dadosFuturos && dadosFuturos.SUBCATEGORIA) {
                this.cache.dadosFuturos[subcategoryId] = {
                  prioridadeAlvo: dadosFuturos.PRIORIDADE_ALVO || '',
                  nivelAlvo: dadosFuturos.NIVEL_ALVO || '',
                  politicasAlvo: dadosFuturos.POLIT_ALVO || '',
                  praticasAlvo: dadosFuturos.PRAT_ALVO || '',
                  funcoesAlvo: dadosFuturos.FUNC_ALVO || '',
                  referenciasAlvo: dadosFuturos.REF_INFO_ALVO || '',
                  artefatosAlvo: dadosFuturos.ARTEF_ALVO || ''
                }
                console.log(`✅ Dados futuros carregados para ${subcategoryId}`)
              } else {
                console.log(
                  `🆕 Nenhum dado futuro para ${subcategoryId} - criando estrutura vazia`
                )
                this.cache.dadosFuturos[subcategoryId] = {
                  prioridadeAlvo: '',
                  nivelAlvo: '',
                  politicasAlvo: '',
                  praticasAlvo: '',
                  funcoesAlvo: '',
                  referenciasAlvo: '',
                  artefatosAlvo: ''
                }
              }
            } catch (error) {
              console.error(
                `❌ Erro ao carregar dados futuros para ${subcategoryId}:`,
                error
              )
              this.cache.dadosFuturos[subcategoryId] = {
                prioridadeAlvo: '',
                nivelAlvo: '',
                politicasAlvo: '',
                praticasAlvo: '',
                funcoesAlvo: '',
                referenciasAlvo: '',
                artefatosAlvo: ''
              }
            }
          }
        }
      }
      console.log('=== FIM CARREGAMENTO DADOS FUTUROS ===')
    } catch (error) {
      console.error('❌ Erro geral ao carregar dados futuros:', error)
    }
  }

  async carregarDadosAtuaisExistentes () {
    console.log('Carregando dados atuais existentes...')

    for (const funcId in this.selections) {
      const functionName = this.getFunctionNameById(funcId)
      if (functionName !== this.config.funcaoAtual) continue

      for (const categoryId in this.selections[funcId]) {
        for (const subcategoryId of this.selections[funcId][categoryId]) {
          try {
            // ✅ USAR ENDPOINT CORRETO: /api/Cenarios/atual
            const dadosAtuais = await this.fetchAPI(
              `/api/Cenarios/atual?subcategoriaId=${subcategoryId}`
            )

            if (dadosAtuais && dadosAtuais.SUBCATEGORIA) {
              this.cache.dadosAtuais[subcategoryId] = {
                prioridade: dadosAtuais.PRIOR_ATUAL || '',
                status: dadosAtuais.STATUS_ATUAL || '',
                politicasPro: dadosAtuais.POLIT_ATUAL || '',
                praticasInternas: dadosAtuais.PRAT_ATUAL || '',
                funcoesResp: dadosAtuais.FUNC_RESP || '',
                referenciasInfo: dadosAtuais.REF_INFO || '',
                artefatosEvi: dadosAtuais.EVID_ATUAL || '',
                justificativa: dadosAtuais.JUSTIFICATIVA || '',
                notas: dadosAtuais.NOTAS || '',
                consideracoes: dadosAtuais.CONSIDERACOES || ''
              }
              console.log(`✅ Dados atuais carregados para ${subcategoryId}`)
            } else {
              // Fallback para dados futuros (só para modo 'futuro' de comparação)
              const dadosFuturo = this.cache.dadosFuturos[subcategoryId]
              this.cache.dadosAtuais[subcategoryId] = {
                prioridade: dadosFuturo?.prioridadeAlvo || '',
                status: dadosFuturo?.nivelAlvo || '',
                politicasPro: dadosFuturo?.politicasAlvo || '',
                praticasInternas: dadosFuturo?.praticasAlvo || '',
                funcoesResp: dadosFuturo?.funcoesAlvo || '',
                referenciasInfo: dadosFuturo?.referenciasAlvo || '',
                artefatosEvi: dadosFuturo?.artefatosAlvo || '',
                justificativa: 'Registro a ser preenchido',
                notas: '',
                consideracoes: ''
              }
              console.log(
                `🆕 Usando fallback para dados atuais de ${subcategoryId}`
              )
            }
          } catch (error) {
            console.error(
              `❌ Erro ao carregar dados atuais para ${subcategoryId}:`,
              error
            )
            const dadosFuturo = this.cache.dadosFuturos[subcategoryId]
            this.cache.dadosAtuais[subcategoryId] = {
              prioridade: dadosFuturo?.prioridadeAlvo || '',
              status: dadosFuturo?.nivelAlvo || '',
              politicasPro: dadosFuturo?.politicasAlvo || '',
              praticasInternas: dadosFuturo?.praticasAlvo || '',
              funcoesResp: dadosFuturo?.funcoesAlvo || '',
              referenciasInfo: dadosFuturo?.referenciasAlvo || '',
              artefatosEvi: dadosFuturo?.artefatosAlvo || '',
              justificativa: 'Erro ao carregar dados',
              notas: '',
              consideracoes: ''
            }
          }
        }
      }
    }
  }
  async carregarDadosAtuais () {
    try {
      console.log('=== CARREGANDO DADOS ATUAIS ===')
      const isCopiando = localStorage.getItem('modoCopiaFuturoParaAtual')

      for (const funcId in this.selections) {
        const functionName = this.getFunctionNameById(funcId)
        if (functionName !== this.config.funcaoAtual) continue

        for (const categoryId in this.selections[funcId]) {
          for (const subcategoryId of this.selections[funcId][categoryId]) {
            console.log(`🔍 Processando subcategoria ${subcategoryId}...`)

            if (isCopiando) {
              console.log(`📋 Modo cópia ativo para ${subcategoryId}`)
              const dadosFuturos = await this.fetchAPI(
                `/api/Cenarios/futuro?subcategoriaId=${subcategoryId}`
              )
              console.log(
                `📦 Dados futuros recebidos para ${subcategoryId}:`,
                dadosFuturos
              )

              if (dadosFuturos) {
                this.cache.dadosAtuais[subcategoryId] = {
                  prioridade:
                    dadosFuturos.prioridadeAlvo || dadosFuturos.PRIORIDADE_ALVO,
                  status: dadosFuturos.nivelAlvo || dadosFuturos.NIVEL_ALVO,
                  politicasPro:
                    dadosFuturos.politicasAlvo || dadosFuturos.POLIT_ALVO || '',
                  praticasInternas:
                    dadosFuturos.praticasAlvo || dadosFuturos.PRAT_ALVO || '',
                  funcoesResp:
                    dadosFuturos.funcoesAlvo || dadosFuturos.FUNC_ALVO || '',
                  referenciasInfo:
                    dadosFuturos.referenciasAlvo ||
                    dadosFuturos.REF_INFO_ALVO ||
                    '',
                  artefatosEvi:
                    dadosFuturos.artefatosAlvo || dadosFuturos.ARTEF_ALVO || '',
                  justificativa: 'Copiado do Cenário Futuro',
                  notas: '',
                  consideracoes: ''
                }
                console.log(
                  `✅ Dados copiados do futuro para ${subcategoryId}:`,
                  this.cache.dadosAtuais[subcategoryId]
                )
              } else {
                console.warn(
                  `⚠️ Nenhum dado futuro para copiar para ${subcategoryId}`
                )
                this.cache.dadosAtuais[subcategoryId] =
                  this.criarEstruturaDadosVazia()
              }
            } else {
              console.log(`📊 Buscando dados atuais para ${subcategoryId}...`)
              const dadosAtuais = await this.fetchAPI(
                `/api/Cenarios/atual?subcategoriaId=${subcategoryId}`
              )
              console.log(
                `📦 Resposta da API para ${subcategoryId}:`,
                dadosAtuais
              )

              if (
                dadosAtuais &&
                (dadosAtuais.SUBCATEGORIA || dadosAtuais.suBCATEGORIA)
              ) {
                this.cache.dadosAtuais[subcategoryId] = {
                  prioridade:
                    dadosAtuais.PRIOR_ATUAL ||
                    dadosAtuais.prior_Atual ||
                    dadosAtuais.prioridadeAtual ||
                    '',
                  status:
                    dadosAtuais.STATUS_ATUAL ||
                    dadosAtuais.status_Atual ||
                    dadosAtuais.statusAtual ||
                    '',
                  politicasPro:
                    dadosAtuais.POLIT_ATUAL ||
                    dadosAtuais.polit_Atual ||
                    dadosAtuais.politicasAtual ||
                    '',
                  praticasInternas:
                    dadosAtuais.PRAT_ATUAL ||
                    dadosAtuais.prat_Atual ||
                    dadosAtuais.praticasAtual ||
                    '',
                  funcoesResp:
                    dadosAtuais.FUNC_RESP ||
                    dadosAtuais.func_Resp ||
                    dadosAtuais.funcoesResp ||
                    '',
                  referenciasInfo:
                    dadosAtuais.REF_INFO ||
                    dadosAtuais.ref_Info ||
                    dadosAtuais.referenciasInfo ||
                    '',
                  artefatosEvi:
                    dadosAtuais.EVID_ATUAL ||
                    dadosAtuais.evid_Atual ||
                    dadosAtuais.artefatosEvi ||
                    '',
                  justificativa:
                    dadosAtuais.JUSTIFICATIVA ||
                    dadosAtuais.justificativa ||
                    'Registro do sistema',
                  notas: dadosAtuais.NOTAS || dadosAtuais.notas || '',
                  consideracoes:
                    dadosAtuais.CONSIDERACOES || dadosAtuais.consideracoes || ''
                }
                console.log(
                  `✅ Dados atuais carregados para ${subcategoryId}:`,
                  this.cache.dadosAtuais[subcategoryId]
                )
              } else {
                console.log(
                  `🆕 Nenhum dado atual para ${subcategoryId} - criando estrutura vazia`
                )
                this.cache.dadosAtuais[subcategoryId] =
                  this.criarEstruturaDadosVazia()
              }
            }
          }
        }
      }
      console.log('=== FIM CARREGAMENTO DADOS ATUAIS ===')
    } catch (error) {
      console.error('❌ Erro ao carregar dados atuais:', error)
    }
  }

  async carregarDadosBanco () {
    try {
      console.log('Carregando dados do banco (modo atual)...')
      for (const funcId in this.selections) {
        const functionName = this.getFunctionNameById(funcId)
        if (functionName !== this.config.funcaoAtual) continue

        for (const categoryId in this.selections[funcId]) {
          for (const subcategoryId of this.selections[funcId][categoryId]) {
            const dadosFuturos = await this.fetchAPI(
              `/api/Cenarios/futuro?subcategoriaId=${subcategoryId}`
            )
            if (dadosFuturos && dadosFuturos.SUBCATEGORIA) {
              this.cache.dadosFuturos[subcategoryId] = {
                prioridadeAlvo:
                  dadosFuturos.PRIORIDADE_ALVO || dadosFuturos.prioridadeAlvo,
                nivelAlvo: dadosFuturos.NIVEL_ALVO || dadosFuturos.nivelAlvo,
                politicasAlvo:
                  dadosFuturos.POLIT_ALVO || dadosFuturos.politicasAlvo || '',
                praticasAlvo:
                  dadosFuturos.PRAT_ALVO || dadosFuturos.praticasAlvo || '',
                funcoesAlvo:
                  dadosFuturos.FUNC_ALVO || dadosFuturos.funcoesAlvo || '',
                referenciasAlvo:
                  dadosFuturos.REF_INFO_ALVO ||
                  dadosFuturos.referenciasAlvo ||
                  '',
                artefatosAlvo:
                  dadosFuturos.ARTEF_ALVO || dadosFuturos.artefatosAlvo || ''
              }
              console.log(
                `✅ Dados futuros carregados do banco para ${subcategoryId}`
              )
            } else {
              this.cache.dadosFuturos[subcategoryId] = {
                prioridadeAlvo: '',
                nivelAlvo: '',
                politicasAlvo: '',
                praticasAlvo: '',
                funcoesAlvo: '',
                referenciasAlvo: '',
                artefatosAlvo: ''
              }
            }
          }
        }
      }
      console.log('Dados do banco carregados para modo atual')
    } catch (error) {
      console.error('Erro ao carregar dados do banco:', error)
    }
  }

  // === FUNÇÕES AUXILIARES E DE INTERFACE ===
  preencherDropdownsPrioridades () {
    const prefix = this.config.modo === 'atual' ? 'current' : 'future'
    const selects = document.querySelectorAll(
      `select[id^="${prefix}-prioridade-"]`
    )
    console.log(`Preenchendo ${selects.length} dropdowns de prioridade`)

    selects.forEach(select => {
      if (!select) return

      select.innerHTML = '<option value="">Selecione a prioridade</option>'

      this.cache.prioridades.forEach(prioridade => {
        const option = document.createElement('option')
        option.value = prioridade.id
        option.textContent = prioridade.nivel
        select.appendChild(option)
      })

      select.disabled = false
    })
  }

  preencherDropdownsNiveis () {
    const prefix = this.config.modo === 'atual' ? 'current' : 'future'
    const selects = document.querySelectorAll(`select[id^="${prefix}-nivel-"]`)
    console.log(`Preenchendo ${selects.length} dropdowns de nível`)

    selects.forEach(select => {
      if (!select) return

      select.innerHTML = '<option value="">Selecione o nível</option>'

      this.cache.niveis.forEach(nivel => {
        const option = document.createElement('option')
        option.value = nivel.id
        option.textContent = nivel.status || nivel.nivel || 'Nível não definido'
        select.appendChild(option)
      })

      select.disabled = false
    })
  }

  obterNomeCategoria (categoryId) {
    const categoria = this.cache.categorias[categoryId]
    if (categoria) {
      return `${categoria.codigo || categoria.CODIGO || ''} - ${
        categoria.nome || categoria.NOME || ''
      }`
    }
    return `Categoria ${categoryId}`
  }

  obterNomeSubcategoria (subcategoryId) {
    const subcategoria = this.cache.subcategorias[subcategoryId]
    if (subcategoria) {
      const codigoCompleto =
        subcategoria.codigo || subcategoria.CODIGO || `SC-${subcategoryId}`
      const descricao =
        subcategoria.subcategoria ||
        subcategoria.SUBCATEGORIA ||
        subcategoria.descricao ||
        subcategoria.DESCRICAO ||
        ''
      return `${codigoCompleto} - ${descricao}`
    }
    return `Subcategoria ${subcategoryId}`
  }

  toggleSecao (conteudo, icone) {
    conteudo.classList.toggle('expanded')
  }

  expandirSecao (secao) {
    const conteudo = secao.querySelector('.function-content, .category-content')
    const icone = secao.querySelector('.toggle-icon')
    if (conteudo && icone) {
      conteudo.classList.add('expanded')
    }
  }

  atualizarInfoSelecao (totalSubcategories) {
    this.safeUpdateElement(
      'selectionInfo',
      `
        <h2>${
          this.functionNames[this.config.funcaoAtual]
        } - Cenário Atual vs Futuro</h2>
        <p><strong>Total de subcategorias selecionadas:</strong> ${totalSubcategories}</p>
        <p><strong>Função atual:</strong> ${
          this.functionNames[this.config.funcaoAtual]
        }</p>
        <p><em>O cenário atual mostra os dados existentes no banco (somente leitura). O cenário futuro permite editar os dados que serão salvos.</em></p>
      `
    )
  }

  mostrarAviso (mensagem) {
    this.safeUpdateElement(
      'selectionInfo',
      `
        <div class="success-message">
          <h2>Aviso</h2>
          <p>${mensagem}</p>
        </div>
      `
    )
  }

  criarEstruturaDadosVazia () {
    return {
      prioridade: '',
      status: '',
      politicasPro: '',
      praticasInternas: '',
      funcoesResp: '',
      referenciasInfo: '',
      artefatosEvi: '',
      justificativa: 'Registro a ser preenchido',
      notas: '',
      consideracoes: ''
    }
  }

  // === FUNÇÕES DE EXIBIÇÃO DE CENÁRIOS ===
  exibirCenarios () {
    try {
      console.log('Exibindo cenários na página... Modo:', this.config.modo)

      const currentContainer = document.getElementById(
        'currentScenarioContainer'
      )
      const futureContainer = document.getElementById('futureScenarioContainer')
      const selectionInfo = document.getElementById('selectionInfo')

      if (!currentContainer || !futureContainer) {
        console.error('Containers não encontrados')
        return
      }

      currentContainer.innerHTML = ''
      futureContainer.innerHTML = ''

      let totalSubcategories = 0

      for (const funcId in this.selections) {
        const functionName = this.getFunctionNameById(funcId)
        if (functionName !== this.config.funcaoAtual) continue

        const currentFunctionDiv = this.criarEstruturaFuncao(functionName)
        const futureFunctionDiv = this.criarEstruturaFuncao(functionName)

        let hasCategories = false

        for (const categoryId in this.selections[funcId]) {
          if (this.selections[funcId][categoryId].length > 0) {
            hasCategories = true

            const currentCategoryDiv = this.criarEstruturaCategoria(categoryId)
            const futureCategoryDiv = this.criarEstruturaCategoria(categoryId)

            this.selections[funcId][categoryId].forEach(subcategoryId => {
              if (this.config.modo === 'atual') {
                const futureSubcategoryDiv = this.criarFormularioFuturoReadonly(
                  subcategoryId,
                  totalSubcategories
                )
                const currentSubcategoryDiv = this.criarFormularioAtualEditavel(
                  subcategoryId,
                  totalSubcategories
                )

                futureCategoryDiv
                  .querySelector('.category-content')
                  .appendChild(futureSubcategoryDiv)
                currentCategoryDiv
                  .querySelector('.category-content')
                  .appendChild(currentSubcategoryDiv)
              } else {
                const currentSubcategoryDiv = this.criarFormularioAtualReadonly(
                  subcategoryId,
                  totalSubcategories
                )
                const futureSubcategoryDiv = this.criarFormularioFuturoEditavel(
                  subcategoryId,
                  totalSubcategories
                )

                currentCategoryDiv
                  .querySelector('.category-content')
                  .appendChild(currentSubcategoryDiv)
                futureCategoryDiv
                  .querySelector('.category-content')
                  .appendChild(futureSubcategoryDiv)
              }

              totalSubcategories++
            })

            currentFunctionDiv
              .querySelector('.function-content')
              .appendChild(currentCategoryDiv)
            futureFunctionDiv
              .querySelector('.function-content')
              .appendChild(futureCategoryDiv)
          }
        }

        if (hasCategories) {
          currentContainer.appendChild(currentFunctionDiv)
          futureContainer.appendChild(futureFunctionDiv)
          this.expandirSecao(currentFunctionDiv)
          this.expandirSecao(futureFunctionDiv)
        }
      }

      this.atualizarInfoSelecao(totalSubcategories)
      console.log(
        `✅ Exibidas ${totalSubcategories} subcategorias no modo: ${this.config.modo}`
      )
    } catch (error) {
      console.error('Erro ao exibir cenários:', error)
    }
  }

  criarEstruturaFuncao (functionName) {
    const functionDiv = document.createElement('div')
    functionDiv.className = 'function-section'

    const functionHeader = document.createElement('div')
    functionHeader.className = 'function-header'
    functionHeader.innerHTML = `
      <h2>${this.functionNames[functionName] || functionName}</h2>
      <span class="toggle-icon"></span>
    `

    const functionContent = document.createElement('div')
    functionContent.className = 'function-content'

    functionHeader.addEventListener('click', () => {
      this.toggleSecao(
        functionContent,
        functionHeader.querySelector('.toggle-icon')
      )
    })

    functionDiv.appendChild(functionHeader)
    functionDiv.appendChild(functionContent)

    return functionDiv
  }

  criarEstruturaCategoria (categoryId) {
    const categoryDiv = document.createElement('div')
    categoryDiv.className = 'category-section'

    const categoryHeader = document.createElement('div')
    categoryHeader.className = 'category-header'

    const categoriaNome = this.obterNomeCategoria(categoryId)
    categoryHeader.innerHTML = `
      <h3>${categoriaNome}</h3>
      <span class="toggle-icon"></span>
    `

    const categoryContent = document.createElement('div')
    categoryContent.className = 'category-content'

    categoryHeader.addEventListener('click', () => {
      this.toggleSecao(
        categoryContent,
        categoryHeader.querySelector('.toggle-icon')
      )
    })

    categoryDiv.appendChild(categoryHeader)
    categoryDiv.appendChild(categoryContent)

    return categoryDiv
  }

  // === FORMULÁRIOS E COMPONENTES DE INTERFACE ===
  criarFormularioAtualReadonly (subcategoryId, formIndex) {
    const subcategoriaTexto = this.obterNomeSubcategoria(subcategoryId)
    const dadosAtuais = this.cache.dadosAtuais[subcategoryId] || {}

    const subcategoryDiv = document.createElement('div')
    subcategoryDiv.className = 'subcategory-item'

    const prioridadeAtual =
      this.cache.prioridades.find(p => p.id == dadosAtuais.prioridade) || {}
    const nivelAtual =
      this.cache.niveis.find(n => n.id == dadosAtuais.status) || {}

    subcategoryDiv.innerHTML = `
      <h4>${subcategoriaTexto}</h4>
      <div class="form-group">
        <label>Prioridade:</label>
        <input type="text" class="form-control readonly-field" 
               value="${prioridadeAtual.nivel || 'Não definida'}" readonly />
      </div>
      <div class="form-group">
        <label>Nível:</label>
        <input type="text" class="form-control readonly-field" 
               value="${
                 nivelAtual.status || nivelAtual.nivel || 'Não definido'
               }" readonly />
      </div>
      <div class="form-group">
        <label>Políticas, Processos e Procedimentos:</label>
        <textarea class="form-control readonly-field" rows="3" readonly>${
          dadosAtuais.politicasPro || 'Não informado'
        }</textarea>
      </div>
      <div class="form-group">
        <label>Práticas internas:</label>
        <textarea class="form-control readonly-field" rows="3" readonly>${
          dadosAtuais.praticasInternas || 'Não informado'
        }</textarea>
      </div>
      <div class="form-group">
        <label>Funções e responsabilidades:</label>
        <textarea class="form-control readonly-field" rows="3" readonly>${
          dadosAtuais.funcoesResp || 'Não informado'
        }</textarea>
      </div>
      <div class="form-group">
        <label>Referências informativas:</label>
        <textarea class="form-control readonly-field" rows="3" readonly>${
          dadosAtuais.referenciasInfo || 'Não informado'
        }</textarea>
      </div>
      <div class="form-group">
        <label>Artefatos e evidências:</label>
        <textarea class="form-control readonly-field" rows="3" readonly>${
          dadosAtuais.artefatosEvi || 'Não informado'
        }</textarea>
      </div>
      <div class="form-group">
        <label>Inclusão no Perfil:</label>
        <input type="text" class="form-control readonly-field" 
               value="${
                 dadosAtuais.incPerfil == 1
                   ? 'Incluído (1)'
                   : 'Não Incluído (0)'
               }" readonly />
      </div>
      <div class="form-group">
        <label>Justificativa:</label>
        <textarea class="form-control readonly-field" rows="2" readonly>${
          dadosAtuais.justificativa || 'Não informado'
        }</textarea>
      </div>
      <div class="form-group">
        <label>Notas:</label>
        <textarea class="form-control readonly-field" rows="2" readonly>${
          dadosAtuais.notas || 'Não informado'
        }</textarea>
      </div>
      <div class="form-group">
        <label>Considerações:</label>
        <textarea class="form-control readonly-field" rows="2" readonly>${
          dadosAtuais.consideracoes || 'Não informado'
        }</textarea>
      </div>
    `

    return subcategoryDiv
  }

  criarFormularioAtualEditavel (subcategoryId, formIndex) {
    const subcategoriaTexto = this.obterNomeSubcategoria(subcategoryId)
    const dadosAtuais = this.cache.dadosAtuais[subcategoryId] || {}

    const subcategoryDiv = document.createElement('div')
    subcategoryDiv.className = 'subcategory-item'

    subcategoryDiv.innerHTML = `
    <div style="display: flex; justify-content: space-between; align-items: center;">
      <h4>${subcategoriaTexto}</h4>
      <button class="remove-btn" onclick="app.removerSubcategoriaAtual('${subcategoryId}', this)">×</button>
    </div>
    
    <div class="form-group">
      <label for="current-prioridade-${formIndex}">Prioridade:</label>
      <select id="current-prioridade-${formIndex}" class="form-control">
        <option value="">Selecione a prioridade</option>
        ${this.cache.prioridades
          .map(
            p =>
              `<option value="${p.id}" ${
                dadosAtuais.prioridade == p.id ? 'selected' : ''
              }>${p.nivel}</option>`
          )
          .join('')}
      </select>
    </div>
    
    <div class="form-group">
      <label for="current-nivel-${formIndex}">Nível:</label>
      <select id="current-nivel-${formIndex}" class="form-control">
        <option value="">Selecione o nível</option>
        ${this.cache.niveis
          .map(
            n =>
              `<option value="${n.id}" ${
                dadosAtuais.status == n.id ? 'selected' : ''
              }>${n.status || n.nivel}</option>`
          )
          .join('')}
      </select>
    </div>
    
    <div class="form-group">
      <label for="current-politicasPro-${formIndex}">Políticas, Processos e Procedimentos:</label>
      <textarea id="current-politicasPro-${formIndex}" class="form-control" 
                rows="3" placeholder="Descreva as políticas, processos e procedimentos...">${
                  dadosAtuais.politicasPro || ''
                }</textarea>
    </div>
    
    <div class="form-group">
      <label for="current-praticasInternas-${formIndex}">Práticas internas:</label>
      <textarea id="current-praticasInternas-${formIndex}" class="form-control" 
                rows="3" placeholder="Descreva as práticas internas...">${
                  dadosAtuais.praticasInternas || ''
                }</textarea>
    </div>
    
    <div class="form-group">
      <label for="current-funcoesResp-${formIndex}">Funções e responsabilidades:</label>
      <textarea id="current-funcoesResp-${formIndex}" class="form-control" 
                rows="3" placeholder="Descreva as funções e responsabilidades...">${
                  dadosAtuais.funcoesResp || ''
                }</textarea>
    </div>
    
    <div class="form-group">
      <label for="current-referenciasInfo-${formIndex}">Referências informativas:</label>
      <textarea id="current-referenciasInfo-${formIndex}" class="form-control" 
                rows="3" placeholder="Liste as referências informativas...">${
                  dadosAtuais.referenciasInfo || ''
                }</textarea>
    </div>
    
    <div class="form-group">
      <label for="current-artefatosEvi-${formIndex}">Artefatos e evidências:</label>
      <textarea id="current-artefatosEvi-${formIndex}" class="form-control" 
                rows="3" placeholder="Descreva os artefatos e evidências...">${
                  dadosAtuais.artefatosEvi || ''
                }</textarea>
    </div>
    
    <div class="form-group">
      <label for="current-justificativa-${formIndex}">Justificativa:</label>
      <textarea id="current-justificativa-${formIndex}" class="form-control" 
                rows="2" placeholder="Descreva a justificativa...">${
                  dadosAtuais.justificativa ||
                  'Registro atualizado via sistema NIST CSF'
                }</textarea>
    </div>
    
    <div class="form-group">
      <label for="current-notas-${formIndex}">Notas:</label>
      <textarea id="current-notas-${formIndex}" class="form-control" 
                rows="2" placeholder="Adicione notas adicionais...">${
                  dadosAtuais.notas || 'Sem notas adicionais'
                }</textarea>
    </div>
    
    <div class="form-group">
      <label for="current-consideracoes-${formIndex}">Considerações:</label>
      <textarea id="current-consideracoes-${formIndex}" class="form-control" 
                rows="2" placeholder="Adicione considerações...">${
                  dadosAtuais.consideracoes || 'Sem considerações adicionais'
                }</textarea>
    </div>
    
    <input type="hidden" id="current-subcategory-${formIndex}" value="${subcategoryId}">
  `

    return subcategoryDiv
  }

  criarFormularioFuturoReadonly (subcategoryId, formIndex) {
    const subcategoriaTexto = this.obterNomeSubcategoria(subcategoryId)
    const dadosFuturos = this.cache.dadosFuturos[subcategoryId] || {}

    const subcategoryDiv = document.createElement('div')
    subcategoryDiv.className = 'subcategory-item'

    const prioridadeFutura =
      this.cache.prioridades.find(p => p.id == dadosFuturos.prioridadeAlvo) ||
      {}
    const nivelFuturo =
      this.cache.niveis.find(n => n.id == dadosFuturos.nivelAlvo) || {}

    subcategoryDiv.innerHTML = `
    <h4>${subcategoriaTexto}</h4>
    <div class="form-group">
      <label>Prioridade Alvo:</label>
      <input type="text" class="form-control readonly-field" 
             value="${prioridadeFutura.nivel || 'Não definida'}" readonly />
    </div>
    <div class="form-group">
      <label>Nível Alvo:</label>
      <input type="text" class="form-control readonly-field" 
             value="${
               nivelFuturo.status || nivelFuturo.nivel || 'Não definido'
             }" readonly />
    </div>
    <div class="form-group">
      <label>Políticas, Processos e Procedimentos:</label>
      <textarea class="form-control readonly-field" rows="3" readonly>${
        dadosFuturos.politicasAlvo || 'Não informado'
      }</textarea>
    </div>
    <div class="form-group">
      <label>Práticas internas:</label>
      <textarea class="form-control readonly-field" rows="3" readonly>${
        dadosFuturos.praticasAlvo || 'Não informado'
      }</textarea>
    </div>
    <div class="form-group">
      <label>Funções e responsabilidades:</label>
      <textarea class="form-control readonly-field" rows="3" readonly>${
        dadosFuturos.funcoesAlvo || 'Não informado'
      }</textarea>
    </div>
    <div class="form-group">
      <label>Referências informativas:</label>
      <textarea class="form-control readonly-field" rows="3" readonly>${
        dadosFuturos.referenciasAlvo || 'Não informado'
      }</textarea>
    </div>
    <div class="form-group">
      <label>Artefatos e evidências:</label>
      <textarea class="form-control readonly-field" rows="3" readonly>${
        dadosFuturos.artefatosAlvo || 'Não informado'
      }</textarea>
    </div>
  `

    return subcategoryDiv
  }

  criarFormularioFuturoEditavel (subcategoryId, formIndex) {
    const subcategoriaTexto = this.obterNomeSubcategoria(subcategoryId)
    const dadosFuturos = this.cache.dadosFuturos[subcategoryId] || {}

    const subcategoryDiv = document.createElement('div')
    subcategoryDiv.className = 'subcategory-item'

    subcategoryDiv.innerHTML = `
    <div style="display: flex; justify-content: space-between; align-items: center;">
      <h4>${subcategoriaTexto}</h4>
      <button class="remove-btn" onclick="app.removerSubcategoriaFutura('${subcategoryId}', this)">×</button>
    </div>
    
    <div class="form-group">
      <label for="future-prioridade-${formIndex}">Prioridade Alvo:</label>
      <select id="future-prioridade-${formIndex}" class="form-control">
        <option value="">Selecione a prioridade</option>
        ${this.cache.prioridades
          .map(
            p =>
              `<option value="${p.id}" ${
                dadosFuturos.prioridadeAlvo == p.id ? 'selected' : ''
              }>${p.nivel}</option>`
          )
          .join('')}
      </select>
    </div>
    
    <div class="form-group">
      <label for="future-nivel-${formIndex}">Nível Alvo:</label>
      <select id="future-nivel-${formIndex}" class="form-control">
        <option value="">Selecione o nível</option>
        ${this.cache.niveis
          .map(
            n =>
              `<option value="${n.id}" ${
                dadosFuturos.nivelAlvo == n.id ? 'selected' : ''
              }>${n.status || n.nivel}</option>`
          )
          .join('')}
      </select>
    </div>
    
    <div class="form-group">
      <label for="future-politicasPro-${formIndex}">Políticas, Processos e Procedimentos:</label>
      <textarea id="future-politicasPro-${formIndex}" class="form-control" 
                rows="3" placeholder="Descreva as políticas, processos e procedimentos desejados...">${
                  dadosFuturos.politicasAlvo || ''
                }</textarea>
    </div>
    
    <div class="form-group">
      <label for="future-praticasInternas-${formIndex}">Práticas internas:</label>
      <textarea id="future-praticasInternas-${formIndex}" class="form-control" 
                rows="3" placeholder="Descreva as práticas internas a serem implementadas...">${
                  dadosFuturos.praticasAlvo || ''
                }</textarea>
    </div>
    
    <div class="form-group">
      <label for="future-funcoesResp-${formIndex}">Funções e responsabilidades:</label>
      <textarea id="future-funcoesResp-${formIndex}" class="form-control" 
                rows="3" placeholder="Descreva as funções e responsabilidades...">${
                  dadosFuturos.funcoesAlvo || ''
                }</textarea>
    </div>
    
    <div class="form-group">
      <label for="future-referenciasInfo-${formIndex}">Referências informativas:</label>
      <textarea id="future-referenciasInfo-${formIndex}" class="form-control" 
                rows="3" placeholder="Liste as referências informativas...">${
                  dadosFuturos.referenciasAlvo || ''
                }</textarea>
    </div>
    
    <div class="form-group">
      <label for="future-artefatosEvi-${formIndex}">Artefatos e evidências:</label>
      <textarea id="future-artefatosEvi-${formIndex}" class="form-control" 
                rows="3" placeholder="Descreva os artefatos e evidências...">${
                  dadosFuturos.artefatosAlvo || ''
                }</textarea>
    </div>
    
    <input type="hidden" id="future-subcategory-${formIndex}" value="${subcategoryId}">
  `

    return subcategoryDiv
  }

  // === FUNÇÕES DE GERENCIAMENTO DE INTERFACE ===
  removerSubcategoriaFutura (subcategoryId, btnElement) {
    const confirmacao = confirm(
      'Deseja remover esta subcategoria do cenário futuro?'
    )
    if (!confirmacao) return

    const subcategoryItem = btnElement.closest('.subcategory-item')
    subcategoryItem.remove()
    this.atualizarInterfaceAposRemocao()
  }

  removerSubcategoriaAtual (subcategoryId, btnElement) {
    const confirmacao = confirm(
      'Deseja remover esta subcategoria do cenário atual?'
    )
    if (!confirmacao) return

    const subcategoryItem = btnElement.closest('.subcategory-item')
    subcategoryItem.remove()
    this.atualizarInterfaceAposRemocao()
  }

  atualizarInterfaceAposRemocao () {
    const formCount = document.querySelectorAll(
      '#futureScenarioContainer .subcategory-item'
    ).length
    this.safeUpdateElement(
      'selectionInfo',
      `
        <h2>${
          this.functionNames[this.config.funcaoAtual]
        } - Cenário Atual vs Futuro</h2>
        <p><strong>Total de subcategorias no futuro:</strong> ${formCount}</p>
      `
    )
  }

  // === FUNÇÕES DE NAVEGAÇÃO ===
  voltarParaAnterior () {
    const indiceAtual = this.ordemFuncoes.indexOf(this.config.funcaoAtual)

    for (let i = indiceAtual - 1; i >= 0; i--) {
      const funcaoAnterior = this.ordemFuncoes[i]
      for (const funcId in this.selections) {
        const functionName = this.getFunctionNameById(funcId)
        if (functionName === funcaoAnterior) {
          for (const category in this.selections[funcId]) {
            if (this.selections[funcId][category].length > 0) {
              const url =
                this.config.modo === 'atual'
                  ? `/Home/${
                      funcaoAnterior.charAt(0).toUpperCase() +
                      funcaoAnterior.slice(1)
                    }Atual`
                  : `/Home/${
                      funcaoAnterior.charAt(0).toUpperCase() +
                      funcaoAnterior.slice(1)
                    }`
              window.location.href = url
              return
            }
          }
        }
      }
    }

    const urlBase =
      this.config.modo === 'atual'
        ? '/Home/PreCadastroAtual'
        : '/Home/Precadastro'
    window.location.href = urlBase
  }

  avancarParaProxima () {
    const indiceAtual = this.ordemFuncoes.indexOf(this.config.funcaoAtual)

    for (let i = indiceAtual + 1; i < this.ordemFuncoes.length; i++) {
      const proximaFuncao = this.ordemFuncoes[i]
      for (const funcId in this.selections) {
        const functionName = this.getFunctionNameById(funcId)
        if (functionName === proximaFuncao) {
          for (const category in this.selections[funcId]) {
            if (this.selections[funcId][category].length > 0) {
              const url =
                this.config.modo === 'atual'
                  ? `/Home/${
                      proximaFuncao.charAt(0).toUpperCase() +
                      proximaFuncao.slice(1)
                    }Atual`
                  : `/Home/${
                      proximaFuncao.charAt(0).toUpperCase() +
                      proximaFuncao.slice(1)
                    }`
              window.location.href = url
              return
            }
          }
        }
      }
    }

    window.location.href = '/Home'
  }

  // === FUNÇÕES DE SALVAMENTO ===
  async salvarAlteracoes () {
    try {
      const isCopiando = localStorage.getItem('modoCopiaFuturoParaAtual')

      let endpoint
      let dadosParaSalvar = []

      console.log('🔍 Modo atual:', this.config.modo, 'Cópia:', isCopiando)

      if (this.config.modo === 'atual') {
        endpoint = '/api/Cenarios/atual/salvar'
        dadosParaSalvar = this.coletarDadosFormularioAtual() // Retorna Array
        console.log('💾 Salvando no CENÁRIO ATUAL')
      } else {
        if (isCopiando) {
          endpoint = '/api/Cenarios/atual/salvar'
          console.log('💾 Copiando para CENÁRIO ATUAL')
        } else {
          endpoint = '/api/Cenarios/futuro/salvar'
          console.log('💾 Salvando no CENÁRIO FUTURO')
        }
        dadosParaSalvar = this.coletarDadosFormularioFuturo(isCopiando) // Retorna Array
      }

      console.log(`📤 Endpoint: ${endpoint}`)
      console.log(`📦 Dados para salvar:`, dadosParaSalvar)

      if (dadosParaSalvar.length === 0) {
        alert('⚠️ Nenhum dado para salvar!')
        return
      } // ✅ ENVIAR COMO ARRAY - AGORA CORRETO PARA AMBOS OS ENDPOINTS C#

      const response = await fetch(endpoint, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
          Accept: 'application/json'
        },
        body: JSON.stringify(dadosParaSalvar) // ← SEMPRE array
      })

      console.log(`📥 Resposta:`, response.status)

      if (response.ok) {
        const resultado = await response.json()
        console.log('✅ Dados salvos com sucesso:', resultado)

        let successMessage
        if (this.config.modo === 'atual') {
          successMessage = `✅ ${dadosParaSalvar.length} registros salvos com sucesso no CENÁRIO ATUAL!`
        } else if (isCopiando) {
          successMessage = `✅ ${dadosParaSalvar.length} dados copiados do Cenário Futuro para CENÁRIO ATUAL com sucesso!`
        } else {
          successMessage = `✅ ${dadosParaSalvar.length} registros salvos com sucesso no CENÁRIO FUTURO!`
        }

        alert(successMessage)

        if (isCopiando) {
          localStorage.removeItem('modoCopiaFuturoParaAtual')
        }

        await this.recarregarDados()
      } else {
        const errorText = await response.text()
        console.error('❌ Erro ao salvar:', errorText)
        alert('❌ Erro ao salvar: ' + errorText)
      }
    } catch (error) {
      console.error('❌ Erro ao salvar:', error)
      alert('❌ Erro ao salvar: ' + error.message)
    }
  }

  // === FUNÇÕES DE CARREGAMENTO - MODIFICADAS PARA PEGAR O MAIS RECENTE ===
  async carregarDadosFuturos () {
    try {
      console.log('=== CARREGANDO DADOS FUTUROS ===')

      for (const funcId in this.selections) {
        const functionName = this.getFunctionNameById(funcId)
        if (functionName !== this.config.funcaoAtual) continue

        for (const categoryId in this.selections[funcId]) {
          for (const subcategoryId of this.selections[funcId][categoryId]) {
            console.log(`🔍 Buscando dados futuros para ${subcategoryId}...`)

            try {
              // ✅ USAR ENDPOINT CORRETO: /api/Cenarios/futuro (já busca o mais recente por DATA_REGISTRO)
              const dadosFuturos = await this.fetchAPI(
                `/api/Cenarios/futuro?subcategoriaId=${subcategoryId}`
              )
              console.log(
                `📦 Dados futuros para ${subcategoryId}:`,
                dadosFuturos
              )

              if (dadosFuturos && dadosFuturos.SUBCATEGORIA) {
                this.cache.dadosFuturos[subcategoryId] = {
                  prioridadeAlvo: dadosFuturos.PRIORIDADE_ALVO || '',
                  nivelAlvo: dadosFuturos.NIVEL_ALVO || '',
                  politicasAlvo: dadosFuturos.POLIT_ALVO || '',
                  praticasAlvo: dadosFuturos.PRAT_ALVO || '',
                  funcoesAlvo: dadosFuturos.FUNC_ALVO || '',
                  referenciasAlvo: dadosFuturos.REF_INFO_ALVO || '',
                  artefatosAlvo: dadosFuturos.ARTEF_ALVO || ''
                }
                console.log(`✅ Dados futuros carregados para ${subcategoryId}`)
              } else {
                console.log(
                  `🆕 Nenhum dado futuro para ${subcategoryId} - criando estrutura vazia`
                )
                this.cache.dadosFuturos[subcategoryId] = {
                  prioridadeAlvo: '',
                  nivelAlvo: '',
                  politicasAlvo: '',
                  praticasAlvo: '',
                  funcoesAlvo: '',
                  referenciasAlvo: '',
                  artefatosAlvo: ''
                }
              }
            } catch (error) {
              console.error(
                `❌ Erro ao carregar dados futuros para ${subcategoryId}:`,
                error
              )
              this.cache.dadosFuturos[subcategoryId] = {
                prioridadeAlvo: '',
                nivelAlvo: '',
                politicasAlvo: '',
                praticasAlvo: '',
                funcoesAlvo: '',
                referenciasAlvo: '',
                artefatosAlvo: ''
              }
            }
          }
        }
      }
      console.log('=== FIM CARREGAMENTO DADOS FUTUROS ===')
    } catch (error) {
      console.error('❌ Erro geral ao carregar dados futuros:', error)
    }
  }

  async carregarDadosAtuais () {
    try {
      console.log('=== CARREGANDO DADOS ATUAIS (MAIS RECENTES) ===')
      const isCopiando = localStorage.getItem('modoCopiaFuturoParaAtual')

      for (const funcId in this.selections) {
        const functionName = this.getFunctionNameById(funcId)
        if (functionName !== this.config.funcaoAtual) continue

        for (const categoryId in this.selections[funcId]) {
          for (const subcategoryId of this.selections[funcId][categoryId]) {
            console.log(`🔍 Processando subcategoria ${subcategoryId}...`)

            if (isCopiando) {
              console.log(`📋 Modo cópia ativo para ${subcategoryId}`) // Buscar o mais recente do futuro para copiar
              const dadosFuturos = await this.fetchAPI(
                `/api/Cenarios/futuro?subcategoriaId=${subcategoryId}` // Endpoint futuro correto
              )

              if (dadosFuturos) {
                this.cache.dadosAtuais[subcategoryId] = {
                  prioridade:
                    dadosFuturos.prioridadeAlvo ||
                    dadosFuturos.PRIORIDADE_ALVO ||
                    0, // 🚨 Usar 0 como default para numérico
                  status:
                    dadosFuturos.nivelAlvo || dadosFuturos.NIVEL_ALVO || 0, // 🚨 Usar 0 como default para numérico
                  politicasPro:
                    dadosFuturos.politicasAlvo || dadosFuturos.POLIT_ALVO || '',
                  praticasInternas:
                    dadosFuturos.praticasAlvo || dadosFuturos.PRAT_ALVO || '',
                  funcoesResp:
                    dadosFuturos.funcoesAlvo || dadosFuturos.FUNC_ALVO || '',
                  referenciasInfo:
                    dadosFuturos.referenciasAlvo ||
                    dadosFuturos.REF_INFO_ALVO ||
                    '',
                  artefatosEvi:
                    dadosFuturos.artefatosAlvo || dadosFuturos.ARTEF_ALVO || '',
                  justificativa: 'Copiado do Cenário Futuro',
                  notas: '',
                  consideracoes: ''
                }
              } else {
                console.warn(
                  `⚠️ Nenhum dado futuro para copiar para ${subcategoryId}`
                )
                this.cache.dadosAtuais[subcategoryId] =
                  this.criarEstruturaDadosVazia()
              }
            } else {
              console.log(
                `📊 Buscando dados atuais mais recentes para ${subcategoryId}...`
              ) // 🚨 CORREÇÃO: Endpoint é apenas /atual, que já retorna o mais recente
              const dadosAtuais = await this.fetchAPI(
                `/api/Cenarios/atual?subcategoriaId=${subcategoryId}`
              )

              if (dadosAtuais) {
                this.cache.dadosAtuais[subcategoryId] = {
                  prioridade:
                    dadosAtuais.PRIOR_ATUAL ||
                    dadosAtuais.prior_Atual ||
                    dadosAtuais.prioridadeAtual ||
                    0, // 🚨 Usar 0
                  status:
                    dadosAtuais.STATUS_ATUAL ||
                    dadosAtuais.status_Atual ||
                    dadosAtuais.statusAtual ||
                    0, // 🚨 Usar 0
                  politicasPro:
                    dadosAtuais.POLIT_ATUAL ||
                    dadosAtuais.polit_Atual ||
                    dadosAtuais.politicasAtual ||
                    '',
                  praticasInternas:
                    dadosAtuais.PRAT_ATUAL ||
                    dadosAtuais.prat_Atual ||
                    dadosAtuais.praticasAtual ||
                    '',
                  funcoesResp:
                    dadosAtuais.FUNC_RESP ||
                    dadosAtuais.func_Resp ||
                    dadosAtuais.funcoesResp ||
                    '',
                  referenciasInfo:
                    dadosAtuais.REF_INFO ||
                    dadosAtuais.ref_Info ||
                    dadosAtuais.referenciasInfo ||
                    '',
                  artefatosEvi:
                    dadosAtuais.EVID_ATUAL ||
                    dadosAtuais.evid_Atual ||
                    dadosAtuais.artefatosEvi ||
                    '',
                  justificativa:
                    dadosAtuais.JUSTIFICATIVA ||
                    dadosAtuais.justificativa ||
                    'Registro do sistema',
                  notas: dadosAtuais.NOTAS || dadosAtuais.notas || '',
                  consideracoes:
                    dadosAtuais.CONSIDERACOES || dadosAtuais.consideracoes || ''
                }
                console.log(
                  `✅ Dados atuais mais recentes carregados para ${subcategoryId}:`,
                  this.cache.dadosAtuais[subcategoryId]
                )
              } else {
                console.log(
                  `🆕 Nenhum dado atual para ${subcategoryId} - criando estrutura vazia`
                )
                this.cache.dadosAtuais[subcategoryId] =
                  this.criarEstruturaDadosVazia()
              }
            }
          }
        }
      }
      console.log('=== FIM CARREGAMENTO DADOS ATUAIS ===')
    } catch (error) {
      console.error('❌ Erro ao carregar dados atuais:', error)
    }
  }

  coletarDadosFormularioAtual () {
    const forms = document.querySelectorAll(
      '#currentScenarioContainer .subcategory-item'
    )
    const dadosParaSalvar = []

    console.log(
      `🔍 Coletando dados de ${forms.length} formulários do modo atual`
    )

    for (let i = 0; i < forms.length; i++) {
      const subcategoryIdInput = document.getElementById(
        `current-subcategory-${i}`
      )

      if (!subcategoryIdInput || !subcategoryIdInput.value) {
        console.warn(`❌ Formulário ${i} sem subcategoryId válido - PULANDO`)
        continue
      }

      const subcategoryId = subcategoryIdInput.value

      const prioridade = document.getElementById(
        `current-prioridade-${i}`
      )?.value
      const nivel = document.getElementById(`current-nivel-${i}`)?.value
      const politicas = document.getElementById(
        `current-politicasPro-${i}`
      )?.value
      const praticas = document.getElementById(
        `current-praticasInternas-${i}`
      )?.value
      const funcoes = document.getElementById(`current-funcoesResp-${i}`)?.value
      const referencias = document.getElementById(
        `current-referenciasInfo-${i}`
      )?.value
      const evidencias = document.getElementById(
        `current-artefatosEvi-${i}`
      )?.value
      const justificativa = document.getElementById(
        `current-justificativa-${i}`
      )?.value
      const notas = document.getElementById(`current-notas-${i}`)?.value
      const consideracoes = document.getElementById(
        `current-consideracoes-${i}`
      )?.value // 🚨 CORREÇÃO: Usar parseInt ou 0/1 para campos numéricos e null para strings vazias

      const prioridadeValida = prioridade ? parseInt(prioridade) : 1 // Usar 1 como default se vazio
      const statusValido = nivel ? parseInt(nivel) : 1 // Usar 1 como default se vazio

      dadosParaSalvar.push({
        SUBCATEGORIA: parseInt(subcategoryId),
        PRIOR_ATUAL: prioridadeValida,
        STATUS_ATUAL: statusValido,
        POLIT_ATUAL: politicas || null,
        PRAT_ATUAL: praticas || null,
        FUNC_RESP: funcoes || null,
        REF_INFO: referencias || null,
        EVID_ATUAL: evidencias || null,
        JUSTIFICATIVA: justificativa || null,
        NOTAS: notas || null,
        CONSIDERACOES: consideracoes || null,
        DATA_REGISTRO: new Date().toISOString().split('T')[0]
      })
    }

    console.log('📦 Dados atuais coletados:', dadosParaSalvar)
    return dadosParaSalvar
  }

  coletarDadosFormularioFuturo (isCopiando) {
    const forms = document.querySelectorAll(
      '#futureScenarioContainer .subcategory-item'
    )
    const dadosParaSalvar = []

    console.log(
      `🔍 Coletando dados de ${forms.length} formulários do modo futuro`
    )

    for (let i = 0; i < forms.length; i++) {
      const subcategoryIdInput = document.getElementById(
        `future-subcategory-${i}`
      )

      if (!subcategoryIdInput || !subcategoryIdInput.value) {
        console.warn(`❌ Formulário ${i} sem subcategoryId válido - PULANDO`)
        continue
      }

      const subcategoryId = subcategoryIdInput.value

      const prioridade = document.getElementById(
        `future-prioridade-${i}`
      )?.value
      const nivel = document.getElementById(`future-nivel-${i}`)?.value
      // 🚨 CORREÇÃO NO JS: Coleta o valor. Se for string vazia/nula,
      // o mapeamento abaixo o converterá em null, o que é o ideal para o backend.
      const politicas = document.getElementById(
        `future-politicasPro-${i}`
      )?.value
      const praticas = document.getElementById(
        `future-praticasInternas-${i}`
      )?.value
      const funcoes = document.getElementById(`future-funcoesResp-${i}`)?.value
      const referencias = document.getElementById(
        `future-referenciasInfo-${i}`
      )?.value
      const evidencias = document.getElementById(
        `future-artefatosEvi-${i}`
      )?.value

      // ✅ ESTRUTURA CORRETA para CenarioFuturoDto
      dadosParaSalvar.push({
        SUBCATEGORIA: parseInt(subcategoryId),
        PRIORIDADE_ALVO: prioridade ? parseInt(prioridade) : null,
        NIVEL_ALVO: nivel ? parseInt(nivel) : null,
        // Usamos o operador || null para garantir que se a string for vazia ('') ou undefined,
        // ela seja enviada como null no JSON, o que se alinha com string? no C#.
        POLIT_ALVO: politicas || null,
        PRAT_ALVO: praticas || null,
        FUNC_ALVO: funcoes || null,
        REF_INFO_ALVO: referencias || null,
        ARTEF_ALVO: evidencias || null
      })
    }

    console.log('📦 Dados coletados:', dadosParaSalvar)
    return dadosParaSalvar
  }

  async recarregarDados () {
    try {
      this.cache.dadosAtuais = {}
      this.cache.dadosFuturos = {}
      await this.carregarDadosCenarios()
      this.exibirCenarios()
      console.log('Dados recarregados após salvamento')
    } catch (error) {
      console.error('Erro ao recarregar dados:', error)
    }
  }
}

window.NISTCore = NISTCore
console.log('✅ NISTCore carregado com cache inicializado')
