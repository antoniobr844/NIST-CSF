// ESTE ARQUIVO É RESPONSÁVEL POR GERENCIAR AS CONSULTAS DE DADOS EM JAVASCRIPT TAMBÉM 
document.addEventListener('DOMContentLoaded', async function() {
    const tabelaBody = document.querySelector('#tabelaDados tbody');
    
    try {
        const response = await fetch('api/Dados/consultar');
        const dados = await response.json();
        
        tabelaBody.innerHTML = '';
        
        dados.forEach(item => {
            const row = document.createElement('tr');
            row.innerHTML = `
                <td>${item.id}</td>
                <td>${item.nome}</td>
                <td>${item.email}</td>
                <td>${new Date(item.dataCadastro).toLocaleDateString()}</td>
            `;
            tabelaBody.appendChild(row);
        });
    } catch (error) {
        console.error('Erro:', error);
        tabelaBody.innerHTML = '<tr><td colspan="4">Erro ao carregar dados</td></tr>';
    }
});