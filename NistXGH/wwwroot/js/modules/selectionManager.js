export class SelectionManager {
    constructor() {
        this.selectedSubcategories = {};
        this.loadSelections();
    }

    loadSelections() {
        const savedSelections = localStorage.getItem('nistSelections');
        this.selectedSubcategories = savedSelections ? JSON.parse(savedSelections) : {};
    }

    saveSelections() {
        localStorage.setItem('nistSelections', JSON.stringify(this.selectedSubcategories));
    }

    initializeFunction(funcaoId) {
        if (!this.selectedSubcategories[funcaoId]) {
            this.selectedSubcategories[funcaoId] = {};
        }
    }

    initializeCategory(funcaoId, categoriaId) {
        this.initializeFunction(funcaoId);
        if (!this.selectedSubcategories[funcaoId][categoriaId]) {
            this.selectedSubcategories[funcaoId][categoriaId] = [];
        }
    }

    toggleSubcategory(funcaoId, categoriaId, subcategoriaId) {
        this.initializeCategory(funcaoId, categoriaId);

        const categorySelections = this.selectedSubcategories[funcaoId][categoriaId];
        const index = categorySelections.indexOf(subcategoriaId);

        if (index > -1) {
            categorySelections.splice(index, 1);
            return false; // Removido
        } else {
            categorySelections.push(subcategoriaId);
            return true; // Adicionado
        }
    }

    isSubcategorySelected(funcaoId, categoriaId, subcategoriaId) {
        return this.selectedSubcategories[funcaoId]?.[categoriaId]?.includes(subcategoriaId) || false;
    }

    getSelectedCountByCategory(funcaoId, categoriaId) {
        return this.selectedSubcategories[funcaoId]?.[categoriaId]?.length || 0;
    }

    getTotalSelectedCount() {
        let total = 0;
        for (const func in this.selectedSubcategories) {
            for (const categoria in this.selectedSubcategories[func]) {
                total += this.selectedSubcategories[func][categoria].length;
            }
        }
        return total;
    }

    clearCategory(funcaoId, categoriaId) {
        if (this.selectedSubcategories[funcaoId]?.[categoriaId]) {
            this.selectedSubcategories[funcaoId][categoriaId] = [];
        }
    }

    clearAll() {
        this.selectedSubcategories = {};
        this.saveSelections();
    }
}