// nistNavigation.js
export class NistNavigation {
    constructor() {
        this.ordemFuncoes = ["governanca", "identificar", "proteger", "detectar", "responder", "recuperar"];
        this.mapeamentoFuncoes = {
            '1': 'governanca',
            '2': 'identificar', 
            '3': 'proteger',
            '4': 'detectar',
            '5': 'responder',
            '6': 'recuperar'
        };
        this.functionNames = {
            'governanca': 'Governança (GV)',
            'identificar': 'Identificar (ID)',
            'proteger': 'Proteger (PR)',
            'detectar': 'Detectar (DE)',
            'responder': 'Responder (RS)',
            'recuperar': 'Recuperar (RC)'
        };
    }

    getFunctionNameById(funcId) {
        if (this.mapeamentoFuncoes[funcId]) {
            return this.mapeamentoFuncoes[funcId];
        }
        const funcIdLower = funcId.toString().toLowerCase();
        if (funcIdLower.includes('gv') || funcIdLower.includes('govern') || funcIdLower === '1') return 'governanca';
        if (funcIdLower.includes('id') || funcIdLower.includes('identif') || funcIdLower === '2') return 'identificar';
        if (funcIdLower.includes('pr') || funcIdLower.includes('proteg') || funcIdLower === '3') return 'proteger';
        if (funcIdLower.includes('de') || funcIdLower.includes('detect') || funcIdLower === '4') return 'detectar';
        if (funcIdLower.includes('rs') || funcIdLower.includes('respond') || funcIdLower === '5') return 'responder';
        if (funcIdLower.includes('rc') || funcIdLower.includes('recuper') || funcIdLower === '6') return 'recuperar';
        return funcId;
    }

    funcaoEstaSelecionada(selections, funcaoAlvo) {
        for (const funcId in selections) {
            const functionName = this.getFunctionNameById(funcId);
            if (functionName === funcaoAlvo) {
                for (const category in selections[funcId]) {
                    if (selections[funcId][category].length > 0) {
                        return true;
                    }
                }
            }
        }
        return false;
    }

    navegarParaAnterior(funcaoAtual) {
        const selections = JSON.parse(localStorage.getItem('nistSelections') || '{}');
        const indiceAtual = this.ordemFuncoes.indexOf(funcaoAtual);

        // Buscar funções anteriores (do índice atual para trás)
        for (let i = indiceAtual - 1; i >= 0; i--) {
            const funcaoAnterior = this.ordemFuncoes[i];
            if (this.funcaoEstaSelecionada(selections, funcaoAnterior)) {
                window.location.href = `/Home/${funcaoAnterior.charAt(0).toUpperCase() + funcaoAnterior.slice(1)}`;
                return;
            }
        }

        // Se não encontrou nenhuma função anterior com seleções, volta para a seleção
        window.location.href = '/Home/Precadastro';
    }

    navegarParaProxima(funcaoAtual) {
        const selections = JSON.parse(localStorage.getItem('nistSelections') || '{}');
        const indiceAtual = this.ordemFuncoes.indexOf(funcaoAtual);

        for (let i = indiceAtual + 1; i < this.ordemFuncoes.length; i++) {
            const proximaFuncao = this.ordemFuncoes[i];
            if (this.funcaoEstaSelecionada(selections, proximaFuncao)) {
                window.location.href = `/Home/${proximaFuncao.charAt(0).toUpperCase() + proximaFuncao.slice(1)}`;
                return;
            }
        }

        // Se não há próxima função, vai para a página inicial
        window.location.href = '/Home';
    }

    getFunctionDisplayName(funcao) {
        return this.functionNames[funcao] || funcao;
    }
}