// nistDataLoader.js
import { apiService } from './apiService.js'

/**
 * @typedef {Object} Funcao
 * @property {string|number} id
 * @property {string} codigo
 * @property {string} nome
 */

/**
 * @typedef {Object} Categoria
 * @property {string|number} id
 * @property {string} codigo
 * @property {string} nome
 * @property {string} funcaoCodigo
 */

/**
 * @typedef {Object} Subcategoria
 * @property {string|number} id
 * @property {string} codigo
 * @property {string} nome
 * @property {string} descricao
 * @property {string} categoriaCodigo
 * @property {string} funcaoCodigo
 */

/**
 * @typedef {Object} Controle
 * @property {string|number} id
 * @property {string} codigo
 * @property {string} controle
 */

export class NistDataLoader {
  constructor () {
    this.cache = {
      funcoes: null,
      categorias: {}, // chave: funcaoId
      subcategorias: {} // chave: categoriaId
    }
  }

  // =========================
  // MÉTODOS PÚBLICOS
  // =========================

  /** @returns {Promise<Funcao[]>} */
  async getFuncoes () {
    return (this.cache.funcoes ??= await this._fetch('/Funcoes', 'funções'))
  }

  /** @param {string|number} funcaoId
   *  @returns {Promise<Categoria[]>}
   */
  async getCategorias (funcaoId) {
    if (!funcaoId) return []
    return (this.cache.categorias[funcaoId] ??= await this._fetch(
      `/Categorias?funcaoId=${funcaoId}`,
      `categorias da função ${funcaoId}`
    ))
  }

  /** @param {string|number} categoriaId
   *  @returns {Promise<Subcategoria[]>}
   */
  async getSubcategorias (categoriaId) {
    if (!categoriaId) return []
    return (this.cache.subcategorias[categoriaId] ??= await this._fetch(
      `/subcategorias?categoriaId=${categoriaId}`,
      `subcategorias da categoria ${categoriaId}`
    ))
  }

  /** @param {string|number} subcategoriaId
   *  @returns {Promise<{subcategoria: Subcategoria|null, controles: Controle[]}>}
   */
  async getInformacoesSubcategoria (subcategoriaId) {
    if (!subcategoriaId) return { subcategoria: null, controles: [] }

    try {
      const subcategoria = await this._fetch(
        `/Subcategorias/${subcategoriaId}`,
        `subcategoria ${subcategoriaId}`
      )
      const controles = await this.getControles(subcategoriaId)
      return { subcategoria, controles }
    } catch (error) {
      console.error(
        `Erro ao carregar informações da subcategoria ${subcategoriaId}:`,
        error
      )
      return { subcategoria: null, controles: [] }
    }
  }

  /** @param {string|number} subcategoriaId
   *  @returns {Promise<Controle[]>}
   */
  async getControles (subcategoriaId) {
    if (!subcategoriaId) return []
    try {
      const controles = await this._fetch(
        `/Controles?subcategoriaId=${subcategoriaId}`,
        `controles da subcategoria ${subcategoriaId}`
      )
      return Array.isArray(controles) ? controles : []
    } catch {
      return []
    }
  }

  /** Limpar cache */
  limparCache () {
    this.cache = { funcoes: null, categorias: {}, subcategorias: {} }
  }

  // =========================
  // MÉTODOS DE FORMATAÇÃO
  // =========================

  /** Formata a exibição de uma subcategoria como "FUNCAOCATEGORIA - NOME"
   *  @param {Subcategoria} subcategoria
   *  @returns {string}
   */
  formatarSubcategoria (subcategoria) {
    if (!subcategoria) return ''
    const funcao = subcategoria.funcaoCodigo || ''
    const categoria = subcategoria.categoriaCodigo || ''
    // CORREÇÃO: Priorizar codigo, depois nome, depois subcategoria
    const nome =
      subcategoria.codigo ||
      subcategoria.nome ||
      subcategoria.subcategoria ||
      ''
    return `${funcao}${categoria} - ${nome}`
  }

  /** Formata informações detalhadas da subcategoria para text area
   *  @param {Subcategoria} subcategoria
   *  @param {Controle[]} controles
   *  @returns {string}
   */
  formatarInformacoes (subcategoria, controles) {
    if (!subcategoria) return 'Subcategoria não encontrada.'

    // CORREÇÃO: Usar propriedades consistentes
    const nomeSubcategoria =
      subcategoria.codigo ||
      subcategoria.nome ||
      subcategoria.subcategoria ||
      ''

    let texto = ''
    texto += `FUNÇÃO: ${subcategoria.funcaoCodigo || ''} - ${
      subcategoria.funcaoNome || ''
    }\n`
    texto += `CATEGORIA: ${subcategoria.categoriaCodigo || ''} - ${
      subcategoria.categoriaNome || ''
    }\n`
    texto += `SUBCATEGORIA: ${nomeSubcategoria}\n`
    texto += `DESCRIÇÃO: ${
      subcategoria.descricao || 'Descrição não disponível.'
    }\n\n`

    if (controles?.length) {
      texto += 'CONTROLES:\n'
      texto += '─'.repeat(50) + '\n'
      controles.forEach(c => {
        texto += `• ${c.controle || 'Controle'}${
          c.codigo ? ` (${c.codigo})` : ''
        }\n`
      })
    } else {
      texto += 'CONTROLES: Nenhum controle específico cadastrado.'
    }

    return texto
  }

  // =========================
  // MÉTODOS PRIVADOS
  // =========================

  /** Centraliza fetch com log e tratamento de erro
   *  @param {string} endpoint
   *  @param {string} descricao
   */
  async _fetch (endpoint, descricao) {
    try {
      console.log(`Carregando ${descricao}...`)
      const res = await apiService.get(endpoint)
      if (!res) return []
      if (Array.isArray(res)) return res
      return res.data ?? res.subcategorias ?? res
    } catch (error) {
      console.error(`Erro ao carregar ${descricao}:`, error)
      return []
    }
  }

  // Aliases (para compatibilidade)
  carregarFuncoes = this.getFuncoes
  carregarCategorias = this.getCategorias
  carregarSubcategorias = this.getSubcategorias
  carregarInformacoesSubcategoria = this.getInformacoesSubcategoria
}
