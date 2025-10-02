// ESTE ARQUIVO É RESPONSÁVEL POR GERENCIAR AS CONSULTAS DE DADOS EM JAVASCRIPT
document.addEventListener('DOMContentLoaded', async function() {
    const searchButton = document.getElementById('searchButton');
    const searchInput = document.getElementById('searchInput');
    const resultadosDiv = document.getElementById('resultados');
    
    searchButton.addEventListener('click', buscarDados);
    
    async function buscarDados() {
        try {
            const response = await fetch('api/Dados/consultar');
            const dados = await response.json();
            
            resultadosDiv.innerHTML = '';
            
            if (dados.length === 0) {
                resultadosDiv.innerHTML = '<p>Nenhum dado encontrado.</p>';
                return;
            }
            
            const searchTerm = searchInput.value.toLowerCase();
            const filteredData = dados.filter(item => 
                item.nome.toLowerCase().includes(searchTerm) || 
                item.email.toLowerCase().includes(searchTerm)
            );
            
            if (filteredData.length === 0) {
                resultadosDiv.innerHTML = '<p>Nenhum resultado encontrado para a pesquisa.</p>';
                return;
            }
            
            filteredData.forEach(item => {
                const card = document.createElement('div');
                card.className = 'card';
                card.innerHTML = `
                    <h3>${item.nome}</h3>
                    <p>Email: ${item.email}</p>
                    <p>Cadastrado em: ${new Date(item.dataCadastro).toLocaleDateString()}</p>
                `;
                resultadosDiv.appendChild(card);
            });
        } catch (error) {
            console.error('Erro:', error);
            resultadosDiv.innerHTML = '<p>Ocorreu um erro ao buscar os dados.</p>';
        }
    }
    
    // Carrega dados ao abrir a página
    await buscarDados();
});