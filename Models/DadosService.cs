using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NistXGH.Models;

namespace NistXGH.Models
{
    public class Dados
    {
        public int Id { get; set; }
        // Adicione outras propriedades conforme necessário
    }

    public interface IDadosService
    // Esse serviço é responsável por gerenciar os dados, incluindo cadastro e consulta
    {
        void Cadastrar(Dados dados);
        List<Dados> ConsultarTodos();
        Dados ConsultarPorId(int id);
    }

    public class DadosService : IDadosService
    {
        private static List<Dados> _dados = new List<Dados>();
        private static int _nextId = 1;

        public DadosService() { }

        public void Cadastrar(Dados dados)
        {
            dados.Id = _nextId++;
            _dados.Add(dados);
        }

        public List<Dados> ConsultarTodos()
        {
            return _dados;
        }

        public Dados ConsultarPorId(int id)
        {
            var result = _dados.FirstOrDefault(d => d.Id == id);
            if (result == null)
                throw new KeyNotFoundException($"Dados with Id {id} not found.");
            return result;
        }
    }
}
