// nistIntegration.js
import { SelectionManager } from './selectionManager.js'
import { NistDataLoader } from './nistDataLoader.js'

export class NistIntegration {
  constructor () {
    this.selectionManager = new SelectionManager()
    this.dataLoader = new NistDataLoader()
    this.initialized = false
  }

  async initialize () {
    if (this.initialized) return

    try {
      // Carrega funções automaticamente
      await this.dataLoader.getFuncoes()
      this.initialized = true
      console.log('NIST Integration inicializada com sucesso')
    } catch (error) {
      console.error('Erro ao inicializar NIST Integration:', error)
    }
  }

  // Métodos para a página de pré-cadastro
  async avancarParaIteracoes (funcaoId, categoriaId) {
    if (!this.initialized) await this.initialize()

    try {
      // Verifica se há seleções antes de avançar
      const selectedCount = this.selectionManager.getSelectedCountByCategory(
        funcaoId,
        categoriaId
      )

      if (selectedCount === 0) {
        throw new Error(
          'Selecione pelo menos uma subcategoria antes de avançar'
        )
      }

      // Salva as seleções atuais
      this.selectionManager.saveSelections()

      // Retorna dados para a próxima página
      return {
        success: true,
        selectedCount,
        totalSelected: this.selectionManager.getTotalSelectedCount(),
        selections: this.selectionManager.selectedSubcategories
      }
    } catch (error) {
      console.error('Erro ao avançar para iterações:', error)
      return {
        success: false,
        error: error.message
      }
    }
  }

  // Método auxiliar para verificar se pode avançar
  podeAvancar (funcaoId, categoriaId) {
    return (
      this.selectionManager.getSelectedCountByCategory(funcaoId, categoriaId) >
      0
    )
  }

  // Getter para acesso externo
  getSelectionManager () {
    return this.selectionManager
  }

  getDataLoader () {
    return this.dataLoader
  }
}

// Instância global
export const nistIntegration = new NistIntegration()
