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
      console.log('Modo Edição config:', this.config.modoEdicao)

      // Verificar se está em modo edição
      const modoEdicao = this.detectarModoEdicao()
      console.log('Modo edição detectado:', modoEdicao)

      if (modoEdicao) {
        console.log('🔧 Modo edição ativo, carregando dados específicos...')
        await this.carregarDadosEdicao()
      } else if (this.config.modo === 'futuro') {
        console.log('🔮 Modo futuro normal...')
        await this.carregarDadosFuturos()
        await this.carregarDadosAtuaisExistentes()
      } else {
        console.log('📊 Modo atual normal...')
        await this.carregarDadosAtuais()
        await this.carregarDadosBanco()
      }

      console.log('✅ Dados dos cenários carregados com sucesso')
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

  mostrarLoadingSalvamento (mostrar) {
    const btnSalvar = document.querySelector('#btnSalvar')
    if (btnSalvar) {
      if (mostrar) {
        btnSalvar.innerHTML =
          '<i class="fas fa-spinner fa-spin"></i> Salvando...'
        btnSalvar.disabled = true
      } else {
        btnSalvar.innerHTML = '<i class="fas fa-save"></i> Salvar Alterações'
        btnSalvar.disabled = false
      }
    }
  }

  mostrarMensagemSucesso (mensagem) {
    if (
      confirm(
        `${mensagem}\n\nDeseja recarregar a página para ver as alterações?`
      )
    ) {
      location.reload()
    }
  }

  mostrarMensagemErro (mensagem) {
    alert(mensagem)
  }

  // === FUNÇÕES DE EXIBIÇÃO DE CENÁRIOS ===
  exibirCenarios () {
    try {
      console.log('Exibindo cenários... Modo:', this.config.modo)
      console.log('Modo Edição ativo:', this.config.modoEdicao?.ativo)

      // === MODO EDIÇÃO ===
      if (this.config.modoEdicao?.ativo) {
        console.log(
          '🎯 Modo edição detectado, exibindo formulário de edição...'
        )
        this.exibirCenarioEdicao()
        return
      }

      // === MODO NORMAL ===
      console.log('📊 Modo normal, exibindo cenários padrão...')
      const currentContainer = document.getElementById(
        'currentScenarioContainer'
      )
      const futureContainer = document.getElementById('futureScenarioContainer')
      const selectionInfo = document.getElementById('selectionInfo')

      console.log('Containers encontrados:', {
        currentContainer: !!currentContainer,
        futureContainer: !!futureContainer,
        selectionInfo: !!selectionInfo
      })

      if (!currentContainer || !futureContainer) {
        console.error('❌ Containers não encontrados para modo normal')
        console.error('- currentScenarioContainer:', currentContainer)
        console.error('- futureScenarioContainer:', futureContainer)
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
    } catch (error) {
      console.error('❌ Erro ao exibir cenários:', error)
    }
  }

  // === FUNÇÃO ESPECÍFICA PARA EDIÇÃO ===
  exibirCenarioEdicao () {
    try {
      console.log('🎯 Exibindo cenário em modo edição...')

      const container = document.getElementById('edicaoScenarioContainer')
      console.log('🔍 Container de edição:', container)

      if (!container) {
        console.error(
          '❌ Container de edição não encontrado: edicaoScenarioContainer'
        )

        // Fallback: tentar encontrar outros containers
        const fallbackContainer =
          document.getElementById('currentScenarioContainer') ||
          document.getElementById('futureScenarioContainer')
        if (fallbackContainer) {
          console.log('🔄 Usando container fallback:', fallbackContainer.id)
          fallbackContainer.innerHTML =
            '<div class="error">Container de edição não configurado corretamente.</div>'
        }
        return
      }

      container.innerHTML = ''

      const { subcategoriaId, tipoCenario } = this.config.modoEdicao
      const subcategoriaTexto = this.obterNomeSubcategoria(subcategoriaId)

      console.log('📝 Criando formulário para:', {
        subcategoriaId,
        tipoCenario,
        subcategoriaTexto
      })

      // Criar header especial para edição
      const headerDiv = document.createElement('div')
      headerDiv.className = 'edicao-header-form'
      headerDiv.innerHTML = `
            <div style="display: flex; justify-content: space-between; align-items: center; margin-bottom: 20px;">
                <h4 style="margin: 0; color: #2c3e50;">${subcategoriaTexto}</h4>
                <span class="badge" style="background: #3498db; color: white; padding: 5px 10px; border-radius: 15px;">
                    ${
                      tipoCenario === 'ATUAL'
                        ? 'Cenário Atual'
                        : 'Cenário Futuro'
                    }
                </span>
            </div>
        `

      container.appendChild(headerDiv)

      // Criar formulário de edição baseado no tipo
      let formularioDiv
      if (tipoCenario === 'ATUAL') {
        console.log('📋 Criando formulário ATUAL editável')
        formularioDiv = this.criarFormularioAtualEditavel(subcategoriaId, 0)
      } else {
        console.log('📋 Criando formulário FUTURO editável')
        formularioDiv = this.criarFormularioFuturoEditavel(subcategoriaId, 0)
      }

      // Adicionar informações de identificação
      const infoDiv = document.createElement('div')
      infoDiv.className = 'edicao-info-form'
      infoDiv.innerHTML = `
            <div style="background: #f8f9fa; padding: 15px; border-radius: 8px; margin: 15px 0; border-left: 4px solid #17a2b8;">
                <small style="color: #6c757d;">
                    <strong>ID do Registro:</strong> ${this.config.modoEdicao.cenarioId} | 
                    <strong>Subcategoria ID:</strong> ${subcategoriaId} |
                    <strong>Tipo:</strong> ${tipoCenario}
                </small>
            </div>
        `

      container.appendChild(infoDiv)
      container.appendChild(formularioDiv)

      console.log('✅ Formulário de edição exibido com sucesso')
    } catch (error) {
      console.error('❌ Erro ao exibir cenário de edição:', error)

      // Mostrar erro no container
      const container = document.getElementById('edicaoScenarioContainer')
      if (container) {
        container.innerHTML = `
                <div class="error">
                    <h3>Erro ao carregar formulário</h3>
                    <p>${error.message}</p>
                    <button onclick="location.reload()" class="btn btn-secondary">Tentar Novamente</button>
                </div>
            `
      }
    }
  }

  atualizarInfoSelecaoEdicao () {
    const { tipoCenario, subcategoriaId } = this.config.modoEdicao
    const subcategoriaTexto = this.obterNomeSubcategoria(subcategoriaId)

    this.safeUpdateElement(
      'selectionInfo',
      `
        <h2><i class="fas fa-edit"></i> Modo Edição - ${
          tipoCenario === 'ATUAL' ? 'Cenário Atual' : 'Cenário Futuro'
        }</h2>
        <p><strong>Subcategoria em edição:</strong> ${subcategoriaTexto}</p>
        <p><strong>ID do Registro:</strong> ${
          this.config.modoEdicao.cenarioId
        }</p>
        <p><em>Você está editando um registro existente. Todas as alterações serão registradas em log.</em></p>
        `
    )
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
      this.mostrarLoadingSalvamento(true)

      // Verificar modo edição
      if (this.config.modoEdicao?.ativo) {
        console.log('🎯 Modo edição detectado, salvando edição...')
        await this.salvarEdicao()
        return
      }

      const isCopiando = localStorage.getItem('modoCopiaFuturoParaAtual')
      let endpoint
      let dadosParaSalvar = []

      console.log('🔍 Modo atual:', this.config.modo, 'Cópia:', isCopiando)

      if (this.config.modo === 'atual') {
        endpoint = '/api/Cenarios/atual/salvar'
        dadosParaSalvar = this.coletarDadosFormularioAtual()
        console.log('💾 Salvando no CENÁRIO ATUAL:', dadosParaSalvar)
      } else {
        if (isCopiando) {
          endpoint = '/api/Cenarios/atual/salvar'
          console.log('💾 Copiando para CENÁRIO ATUAL')
        } else {
          endpoint = '/api/Cenarios/futuro/salvar'
          console.log('💾 Salvando no CENÁRIO FUTURO')
        }
        dadosParaSalvar = this.coletarDadosFormularioFuturo(isCopiando)
      }

      // VALIDAÇÃO: Verificar se há dados para salvar
      if (!dadosParaSalvar || dadosParaSalvar.length === 0) {
        this.mostrarMensagemErro(
          '❌ Nenhum dado válido para salvar. Verifique os campos obrigatórios.'
        )
        return
      }

      console.log('📤 Enviando dados para:', endpoint, dadosParaSalvar)

      const response = await fetch(endpoint, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
          Accept: 'application/json'
        },
        body: JSON.stringify(dadosParaSalvar)
      })

      if (response.ok) {
        const resultado = await response.json()
        console.log('✅ Salvamento bem-sucedido:', resultado)

        // Limpar flag de cópia se existir
        if (isCopiando) {
          localStorage.removeItem('modoCopiaFuturoParaAtual')
        }

        this.mostrarMensagemSucesso(
          resultado.mensagem ||
            `✅ ${dadosParaSalvar.length} registro(s) salvos com sucesso!`
        )

        // Recarregar dados para atualizar interface
        await this.recarregarDados()
      } else {
        const erro = await response.text()
        throw new Error(erro || `Erro HTTP ${response.status}`)
      }
    } catch (error) {
      console.error('❌ Erro ao salvar:', error)
      this.mostrarMensagemErro(`❌ Erro ao salvar: ${error.message}`)
    } finally {
      this.mostrarLoadingSalvamento(false)
    }
  }

  async salvarEdicao () {
    // ✅ CAPTURAR VALORES DIRETAMENTE
    const cenarioId = this.config.modoEdicao.cenarioId
    const tipoCenario = this.config.modoEdicao.tipoCenario
    const subcategoriaId = this.config.modoEdicao.subcategoriaId

    try {
      this.mostrarLoadingSalvamento(true)

      console.log('💾 Iniciando salvamento de edição...', {
        cenarioId,
        tipoCenario,
        subcategoriaId
      })

      // VALIDAÇÃO INICIAL CRÍTICA
      if (!cenarioId || cenarioId <= 0) {
        throw new Error(`ID do registro inválido: ${cenarioId}`)
      }

      if (!subcategoriaId || subcategoriaId <= 0) {
        throw new Error(`SUBCATEGORIA inválida: ${subcategoriaId}`)
      }

      let dadosParaSalvar
      let endpoint

      if (tipoCenario === 'ATUAL') {
        endpoint = '/api/Cenarios/atual/editar'
        dadosParaSalvar = this.coletarDadosFormularioAtualEdicao()
        console.log('📤 Salvando CENÁRIO ATUAL:', dadosParaSalvar)
      } else {
        endpoint = '/api/Cenarios/futuro/editar'
        dadosParaSalvar = this.coletarDadosFormularioFuturoEdicao()
        console.log('📤 Salvando CENÁRIO FUTURO:', dadosParaSalvar)
      }

      // Validação final antes do envio
      if (!dadosParaSalvar) {
        throw new Error('Dados para salvar estão vazios')
      }

      if (!dadosParaSalvar.ID || dadosParaSalvar.ID <= 0) {
        throw new Error(
          `ID inválido nos dados: ${dadosParaSalvar.ID} (esperado: ${cenarioId})`
        )
      }

      if (!dadosParaSalvar.SUBCATEGORIA || dadosParaSalvar.SUBCATEGORIA <= 0) {
        throw new Error(
          `SUBCATEGORIA inválida nos dados: ${dadosParaSalvar.SUBCATEGORIA} (esperado: ${subcategoriaId})`
        )
      }

      console.log('🚀 Enviando para API:', endpoint)
      console.log('📦 Dados enviados:', dadosParaSalvar)

      const response = await fetch(endpoint, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
          Accept: 'application/json'
        },
        body: JSON.stringify(dadosParaSalvar)
      })

      if (response.ok) {
        const resultado = await response.json()
        console.log('✅ Edição salva com sucesso:', resultado)

        this.mostrarMensagemSucesso(
          `✅ Registro atualizado com sucesso! ${
            resultado.alteracoes || 0
          } campo(s) modificado(s).`
        )

        // Redirecionar de volta para relatórios após sucesso
        setTimeout(() => {
          window.location.href = '/Home/Relatorios'
        }, 2000)
      } else {
        let erroTexto = await response.text()
        console.error(
          '❌ Erro da API - Status:',
          response.status,
          'Response:',
          erroTexto
        )

        try {
          const erroJson = JSON.parse(erroTexto)
          throw new Error(erroJson.message || erroJson.title || erroTexto)
        } catch {
          throw new Error(erroTexto || `Erro HTTP ${response.status}`)
        }
      }
    } catch (error) {
      console.error('❌ Erro ao salvar edição:', error)
      this.mostrarMensagemErro(`❌ Erro ao salvar: ${error.message}`)
    } finally {
      this.mostrarLoadingSalvamento(false)
    }
  }

  // === FUNÇÕES DE COLETA DE DADOS PARA EDIÇÃO ===

  coletarDadosFormularioFuturoEdicao () {
    const { cenarioId, subcategoriaId } = this.config.modoEdicao

    console.log('📝 Coletando dados do formulário FUTURO para edição...')

    // Coletar valores dos campos
    const prioridadeElement = document.getElementById('future-prioridade-0')
    const nivelElement = document.getElementById('future-nivel-0')
    const politicasElement = document.getElementById('future-politicasPro-0')
    const praticasElement = document.getElementById('future-praticasInternas-0')
    const funcoesElement = document.getElementById('future-funcoesResp-0')
    const referenciasElement = document.getElementById(
      'future-referenciasInfo-0'
    )
    const evidenciasElement = document.getElementById('future-artefatosEvi-0')

    const prioridade = prioridadeElement?.value
    const nivel = nivelElement?.value
    const politicas = politicasElement?.value
    const praticas = praticasElement?.value
    const funcoes = funcoesElement?.value
    const referencias = referenciasElement?.value
    const evidencias = evidenciasElement?.value

    console.log('📊 Dados coletados FUTURO:', {
      prioridade,
      nivel,
      politicas,
      praticas,
      funcoes,
      referencias,
      evidencias
    })

    // Preparar dados para envio
    const dadosParaEnvio = {
      ID: cenarioId,
      SUBCATEGORIA: subcategoriaId,
      PRIORIDADE_ALVO: prioridade ? parseInt(prioridade) : null,
      NIVEL_ALVO: nivel ? parseInt(nivel) : null,
      POLIT_ALVO: politicas || null,
      PRAT_ALVO: praticas || null,
      FUNC_ALVO: funcoes || null,
      REF_INFO_ALVO: referencias || null,
      ARTEF_ALVO: evidencias || null
    }

    console.log('📤 Dados preparados para API:', dadosParaEnvio)

    return dadosParaEnvio
  }

  // nist-core.js

  coletarDadosFormularioAtualEdicao () {
    const { cenarioId, subcategoriaId } = this.config.modoEdicao
    const prioridade = document.getElementById('current-prioridade-0')?.value
    const nivel = document.getElementById('current-nivel-0')?.value
    const politicas = document.getElementById('current-politicasPro-0')?.value
    const praticas = document.getElementById(
      'current-praticasInternas-0'
    )?.value
    const funcoes = document.getElementById('current-funcoesResp-0')?.value
    const referencias = document.getElementById(
      'current-referenciasInfo-0'
    )?.value
    const evidencias = document.getElementById('current-artefatosEvi-0')?.value
    const justificativa = document.getElementById(
      'current-justificativa-0'
    )?.value
    const notas = document.getElementById('current-notas-0')?.value
    const consideracoes = document.getElementById(
      'current-consideracoes-0'
    )?.value

    return {
      ID: cenarioId,
      SUBCATEGORIA: subcategoriaId,
      PRIOR_ATUAL: prioridade ? parseInt(prioridade) : null,
      STATUS_ATUAL: nivel ? parseInt(nivel) : null,
      POLIT_ATUAL: politicas || null,
      PRAT_ATUAL: praticas || null,
      FUNC_RESP: funcoes || null,
      REF_INFO: referencias || null,
      EVID_ATUAL: evidencias || null,
      JUSTIFICATIVA: justificativa || null,
      NOTAS: notas || null,
      CONSIDERACOES: consideracoes || null
    }
  }

  // === FUNÇÕES DE CARREGAMENTO
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
              console.log(`📋 Modo cópia ativo para ${subcategoryId}`)
              const dadosFuturos = await this.fetchAPI(
                `/api/Cenarios/futuro?subcategoriaId=${subcategoryId}`
              )

              if (dadosFuturos) {
                this.cache.dadosAtuais[subcategoryId] = {
                  prioridade:
                    dadosFuturos.prioridadeAlvo ||
                    dadosFuturos.PRIORIDADE_ALVO ||
                    0,
                  status:
                    dadosFuturos.nivelAlvo || dadosFuturos.NIVEL_ALVO || 0,
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
              )
              const dadosAtuais = await this.fetchAPI(
                `/api/Cenarios/atual?subcategoriaId=${subcategoryId}`
              )

              if (dadosAtuais) {
                this.cache.dadosAtuais[subcategoryId] = {
                  prioridade:
                    dadosAtuais.PRIOR_ATUAL ||
                    dadosAtuais.prior_Atual ||
                    dadosAtuais.prioridadeAtual ||
                    0,
                  status:
                    dadosAtuais.STATUS_ATUAL ||
                    dadosAtuais.status_Atual ||
                    dadosAtuais.statusAtual ||
                    0,
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

      const subcategoryId = parseInt(subcategoryIdInput.value)

      if (isNaN(subcategoryId) || subcategoryId <= 0) {
        console.warn(
          `❌ SubcategoryId inválido: ${subcategoryIdInput.value} - PULANDO`
        )
        continue
      }

      // ✅ CORREÇÃO: Coletar valores com fallback para null
      const prioridadeSelect = document.getElementById(
        `current-prioridade-${i}`
      )
      const nivelSelect = document.getElementById(`current-nivel-${i}`)

      const prioridade = prioridadeSelect?.value || null
      const nivel = nivelSelect?.value || null
      const politicas =
        document.getElementById(`current-politicasPro-${i}`)?.value || null
      const praticas =
        document.getElementById(`current-praticasInternas-${i}`)?.value || null
      const funcoes =
        document.getElementById(`current-funcoesResp-${i}`)?.value || null
      const referencias =
        document.getElementById(`current-referenciasInfo-${i}`)?.value || null
      const evidencias =
        document.getElementById(`current-artefatosEvi-${i}`)?.value || null
      const justificativa =
        document.getElementById(`current-justificativa-${i}`)?.value ||
        'Registro atualizado via sistema NIST CSF'
      const notas = document.getElementById(`current-notas-${i}`)?.value || null
      const consideracoes =
        document.getElementById(`current-consideracoes-${i}`)?.value || null

      // ✅ CORREÇÃO: Converter para números inteiros (mantendo null se vazio)
      const prioridadeValida = prioridade ? parseInt(prioridade) : null
      const statusValido = nivel ? parseInt(nivel) : null // ✅ AGORA É NUMBER (INT)

      // ✅ CORREÇÃO: Validar conversões numéricas
      if (prioridade && isNaN(prioridadeValida)) {
        console.warn(
          `❌ Prioridade inválida para subcategoria ${subcategoryId}: ${prioridade}`
        )
        continue
      }

      if (nivel && isNaN(statusValido)) {
        console.warn(
          `❌ Nível inválido para subcategoria ${subcategoryId}: ${nivel}`
        )
        continue
      }

      dadosParaSalvar.push({
        SUBCATEGORIA: subcategoryId,
        PRIOR_ATUAL: prioridadeValida,
        STATUS_ATUAL: statusValido, // ✅ AGORA É NUMBER (INT)
        POLIT_ATUAL: politicas,
        PRAT_ATUAL: praticas,
        FUNC_RESP: funcoes,
        REF_INFO: referencias,
        EVID_ATUAL: evidencias,
        JUSTIFICATIVA: justificativa,
        NOTAS: notas,
        CONSIDERACOES: consideracoes
      })

      console.log(`✅ Dados coletados para subcategoria ${subcategoryId}:`, {
        prioridade: prioridadeValida,
        status: statusValido,
        subcategoria: subcategoryId
      })
    }

    console.log('📦 Dados atuais coletados para salvar:', dadosParaSalvar)
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

      const subcategoryId = parseInt(subcategoryIdInput.value)

      // ✅ VALIDAÇÃO: Verificar se subcategoryId é válido
      if (isNaN(subcategoryId) || subcategoryId <= 0) {
        console.warn(
          `❌ SubcategoryId inválido: ${subcategoryIdInput.value} - PULANDO`
        )
        continue
      }

      const prioridade = document.getElementById(
        `future-prioridade-${i}`
      )?.value
      const nivel = document.getElementById(`future-nivel-${i}`)?.value
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

      const prioridadeValida = prioridade ? parseInt(prioridade) : null
      const nivelValido = nivel ? parseInt(nivel) : null

      dadosParaSalvar.push({
        SUBCATEGORIA: subcategoryId,
        PRIORIDADE_ALVO: prioridadeValida,
        NIVEL_ALVO: nivelValido,
        POLIT_ALVO: politicas || null,
        PRAT_ALVO: praticas || null,
        FUNC_ALVO: funcoes || null,
        REF_INFO_ALVO: referencias || null,
        ARTEF_ALVO: evidencias || null
      })
    }

    console.log('📦 Dados futuros coletados:', dadosParaSalvar)
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

  // === FUNÇÕES DE EDIÇÃO ===
  detectarModoEdicao () {
    const urlParams = new URLSearchParams(window.location.search)
    const cenarioId = urlParams.get('id')
    const tipoCenario = urlParams.get('tipo')
    const subcategoriaId = urlParams.get('subcategoriaId')

    if (cenarioId && tipoCenario && subcategoriaId) {
      this.config.modoEdicao = {
        ativo: true,
        cenarioId: parseInt(cenarioId),
        tipoCenario: tipoCenario, // 'ATUAL' ou 'FUTURO'
        subcategoriaId: parseInt(subcategoriaId)
      }
      console.log('🔧 Modo edição ativado:', this.config.modoEdicao)
      return true
    }

    this.config.modoEdicao = { ativo: false }
    return false
  }

  async carregarDadosEdicao () {
    if (!this.config.modoEdicao.ativo) return

    const { cenarioId, tipoCenario, subcategoriaId } = this.config.modoEdicao

    console.log(
      `📥 Carregando dados para edição: ${tipoCenario} ID ${cenarioId}`
    )

    try {
      const endpoint =
        tipoCenario === 'ATUAL'
          ? `/api/Cenarios/atual/editar?id=${cenarioId}`
          : `/api/Cenarios/futuro/editar?id=${cenarioId}`

      console.log('🔍 Buscando dados em:', endpoint)

      const response = await this.fetchAPI(endpoint)

      if (!response) {
        throw new Error('Registro não encontrado ou erro ao carregar')
      }

      console.log('📦 Dados recebidos para edição:', response)

      // Preencher o cache com os dados do registro
      if (tipoCenario === 'ATUAL') {
        this.cache.dadosAtuais[subcategoriaId] =
          this.mapearDadosParaFormularioAtual(response)
        console.log(
          '✅ Dados atuais mapeados:',
          this.cache.dadosAtuais[subcategoriaId]
        )
      } else {
        this.cache.dadosFuturos[subcategoriaId] =
          this.mapearDadosParaFormularioFuturo(response)
        console.log(
          '✅ Dados futuros mapeados:',
          this.cache.dadosFuturos[subcategoriaId]
        )
      }
    } catch (error) {
      console.error('❌ Erro ao carregar dados para edição:', error)
      throw new Error(`Erro ao carregar registro para edição: ${error.message}`)
    }
  }

  mapearDadosParaFormularioAtual (dados) {
    return {
      prioridade: dados.PRIOR_ATUAL || '',
      status: dados.STATUS_ATUAL || '',
      politicasPro: dados.POLIT_ATUAL || '',
      praticasInternas: dados.PRAT_ATUAL || '',
      funcoesResp: dados.FUNC_RESP || '',
      referenciasInfo: dados.REF_INFO || '',
      artefatosEvi: dados.EVID_ATUAL || '',
      justificativa: dados.JUSTIFICATIVA || '',
      notas: dados.NOTAS || '',
      consideracoes: dados.CONSIDERACOES || ''
    }
  }

  mapearDadosParaFormularioFuturo (dados) {
    return {
      prioridadeAlvo: dados.PRIORIDADE_ALVO || '',
      nivelAlvo: dados.NIVEL_ALVO || '',
      politicasAlvo: dados.POLIT_ALVO || '',
      praticasAlvo: dados.PRAT_ALVO || '',
      funcoesAlvo: dados.FUNC_ALVO || '',
      referenciasAlvo: dados.REF_INFO_ALVO || '',
      artefatosAlvo: dados.ARTEF_ALVO || ''
    }
  }
}

window.NISTCore = NISTCore
console.log('✅ NISTCore carregado com cache inicializado')
