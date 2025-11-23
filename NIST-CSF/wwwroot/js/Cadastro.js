// ESTE AR1QUIVO É RESPONSÁVEL POR GERENCIAR O CADASTRO DE USUÁRIOS EM JAVASCRIPT
document.getElementById('formCadastro').addEventListener('submit', async function(e) {
    e.preventDefault();
    
    const formData = {
        nome: document.getElementById('nome').value,
        email: document.getElementById('email').value
    };
    
    try {
        const response = await fetch('api/Dados/cadastrar', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(formData)
        });
        
        if (response.ok) {
            alert('Cadastro realizado com sucesso!');
            document.getElementById('formCadastro').reset();
        } else {
            throw new Error('Erro no cadastro');
        }
    } catch (error) {
        console.error('Erro:', error);
        alert('Ocorreu um erro ao cadastrar.');
    }
});