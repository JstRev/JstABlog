using Application.Interfaces;
using Domain.Models.Entities;
using Firebase.Database;
using Infrastructure.Dto;
using Infrastructure.Mapper;
using Firebase.Database.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class FirebaseRepository : IBlogRepository
    {
        // Per accedere al database Firebase, utilizziamo FirebaseClient, che è una classe fornita dalla libreria Firebase.
        // e che ci permette di interagire con il database in modo semplice e intuitivo.
        private readonly FirebaseClient _client;
        public const string ArticlesNode = "articles";

        //costruttore che inizializza il client firebase con l'URL del database
        public FirebaseRepository(string firebaseUrl)
        {
            //gli diciamo a quale databse fa riferimento
            _client = new FirebaseClient(firebaseUrl);
        }

        // Metodo per cancellare un articolo dal database Firebase utilizzando il suo ID
        public async Task DeleteAsync(string id)
        {
            // Per cancellare un articolo, utilizziamo DeleteAsync che rimuove completamente il nodo corrispondente all'articolo.
            await _client
                .Child(ArticlesNode)
                .Child(id)
                .DeleteAsync();
        }


        public async Task<IEnumerable<Post>> GetAllAsync()
        {
            // Recupera tutti i nodi sotto "articles" e li mappa in BlogPostPersistenceDto
            var dtos = await _client
                .Child(ArticlesNode)
                .OnceAsync<PostPersistenceDto>();

            // Mappa i DTO in entità Post, ordina per CreatedAt in ordine decrescente e restituisce la lista
            return dtos
                .Select(m => m.Object.ToEntity())
                .OrderByDescending(a => a.CreatedAt)
                .ToList();
        }

        // Metodo per recuperare un articolo specifico dal database Firebase utilizzando
        // il suo ID
        public async Task<Post?> GetByIdAsync(string id)
        {
            var dto = await _client
                .Child(ArticlesNode)
                .Child(id)
                .OnceSingleAsync<PostPersistenceDto>();

            return dto == null ? null : dto.ToEntity();
        }


        // Metodo per salvare un nuovo articolo nel database Firebase
        public async Task SaveAsync(Post article)
        {
            var dto = article.ToPersistenceDto();

            // Per salvare un nuovo articolo, utilizziamo PutAsync con l'ID dell'articolo come chiave.
            await _client
                .Child(ArticlesNode)
                .Child(article.Id.ToString())
                .PutAsync(dto);
        }


        // Metodo per aggiornare un articolo esistente nel database Firebase
        public async Task UpdateAsync(Post article)
        {
            var dto = article.ToPersistenceDto();

            // Per aggiornare un articolo, utilizziamo PutAsync che sovrascrive i dati esistenti con quelli nuovi.
            await _client
                .Child(ArticlesNode)
                .Child(article.Id.ToString())
                .PutAsync(dto);
        }

        public async Task<IEnumerable<Post>> GetByTitleAsync(string title)
        {
            var dto = await GetAllAsync();
            return dto
                .Where(p => p.Title.Contains(title, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }
        public async Task<IEnumerable<Post>> GetByContentAsync(string content)
        {
            var dto = await GetAllAsync();
            return dto
                .Where(p => p.Content.Contains(content, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }
        public async Task<IEnumerable<Post>> GetByDateAsync(DateTime date)
        {
            var dto = await GetAllAsync();
            return dto
                .Where(p => p.CreatedAt.Date == date.Date)
                .ToList();
        }
        public async Task<int> CountByDateAsync(DateTime date)
        {
            var dto = await GetAllAsync();
            return dto
                .Count(p => p.CreatedAt.Date == date.Date);
        }
        public async Task<IEnumerable<Post>> GetInPeriodAsync(DateTime start, DateTime end)
        {
            var dto = await GetAllAsync();
            return dto
                .Where(p => p.CreatedAt >= start && p.CreatedAt <= end)
                .ToList();
        }
    }
}
