using Application.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using Application.Dto;
using Application.UseCases;
using CommunityToolkit.Mvvm.Input;

namespace Wpf.ViewModels
{
    public partial class HomeViewModel : ObservableObject
    {
        private readonly IBlogService _blogService;

        [ObservableProperty]
        private ObservableCollection<PostReadDto> _articles = new();

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsArticleSelected))]
        [NotifyCanExecuteChangedFor(nameof(ModifyArticleCommand), nameof(DeleteArticleCommand))]
        private PostReadDto? _selectedArticle;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(CreateArticleCommand))]
        private string _title = string.Empty;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(CreateArticleCommand))]
        private string _content = string.Empty;

        [ObservableProperty]
        private bool _isLoading;

        [ObservableProperty]
        private string _statusMessage = string.Empty;

        [ObservableProperty]
        private string _searchTitleText = string.Empty;

        [ObservableProperty]
        private string _searchContentText = string.Empty;

        [ObservableProperty]
        private DateTime _searchDate = DateTime.Today;

        [ObservableProperty]
        private DateTime _searchStartDate = DateTime.Today.AddDays(-7);

        [ObservableProperty]
        private DateTime _searchEndDate = DateTime.Today;

        public bool IsArticleSelected => SelectedArticle != null;
        public HomeViewModel(IBlogService blogService)
        {
            _blogService = blogService;
            _ = LoadArticlesAsync();
        }

        [RelayCommand]
        private async Task LoadArticlesAsync()
        {
            try
            {
                IsLoading = true;
                StatusMessage = "Caricamento articoli...";

                var articles = await _blogService.GetAllArticlesAsync();

                Articles.Clear();
                foreach (var article in articles)
                {
                    Articles.Add(article);
                }

                StatusMessage = $"Caricati {Articles.Count} articoli";
            }
            catch (Exception ex)
            {
                StatusMessage = $"Errore: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }

        [RelayCommand]
        private async Task CreateArticleAsync()
        {
            try
            {
                IsLoading = true;
                StatusMessage = "Creazione articolo...";
                var newArticle = new PostCreateDto(Title, Content);
                await _blogService.CreateArticleAsync(newArticle);
                StatusMessage = "Articolo creato con successo!";
                OnSelectedArticleChanged(null);
            }
            catch (Exception ex)
            {
                StatusMessage = $"Errore: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }

        [RelayCommand]
        private async Task ModifyArticleAsync()
        {
            try
            {
                if(SelectedArticle == null) return;
                IsLoading = true;
                StatusMessage = "Modifica articolo...";
                var newArticle = new PostCreateDto(Title, Content);
                await _blogService.UpdateArticleAsync(SelectedArticle.Id, newArticle);
                StatusMessage = "Articolo modificato con successo!";
            }
            catch (Exception ex)
            {
                StatusMessage = $"Errore: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }

        [RelayCommand]
        private async Task DeleteArticleAsync()
        {
            try
            {
                if (SelectedArticle == null) return;
                IsLoading = true;
                StatusMessage = "Eliminazione articolo...";
                await _blogService.DeleteArticleAsync(SelectedArticle.Id);
                StatusMessage = "Articolo eliminato con successo!";
            }
            catch (Exception ex)
            {
                StatusMessage = $"Errore: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }

        partial void OnSelectedArticleChanged(PostReadDto? post)
        {
            if(post != null)
            {
                Title = post.Title;
                Content = post.Content;
            }
            else
            {
                Title = string.Empty; 
                Content = string.Empty;
            }
        }

        [RelayCommand]
        private async Task SearchByTitleAsync()
        {
            if (string.IsNullOrWhiteSpace(SearchTitleText)) return;
            await ExecuteSearchAsync(() => _blogService.GetByTitleAsync(SearchTitleText), "Ricerca per titolo...");
        }

        [RelayCommand]
        private async Task SearchByContentAsync()
        {
            if (string.IsNullOrWhiteSpace(SearchContentText)) return;
            await ExecuteSearchAsync(() => _blogService.GetByContentAsync(SearchContentText), "Ricerca per contenuto...");
        }

        [RelayCommand]
        private async Task SearchByDateAsync()
        {
            await ExecuteSearchAsync(() => _blogService.GetByDateAsync(SearchDate), "Ricerca per data...");
        }

        [RelayCommand]
        private async Task SearchInPeriodAsync()
        {
            await ExecuteSearchAsync(() => _blogService.GetInPeriodAsync(SearchStartDate, SearchEndDate), "Ricerca nel periodo...");
        }

        [RelayCommand]
        private async Task CountByDateAsync()
        {
            try
            {
                IsLoading = true;
                StatusMessage = "Calcolo conteggio...";
                var count = await _blogService.CountByDateAsync(SearchDate);
                StatusMessage = $"Ci sono {count} articoli in data {SearchDate:dd/MM/yyyy}.";
            }
            catch (Exception ex)
            {
                StatusMessage = $"Errore: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async Task ExecuteSearchAsync(Func<Task<IEnumerable<PostReadDto>>> searchAction, string loadingMessage)
        {
            try
            {
                IsLoading = true;
                StatusMessage = loadingMessage;

                var results = await searchAction();

                Articles.Clear();
                foreach (var article in results)
                {
                    Articles.Add(article);
                }

                StatusMessage = $"Trovati {Articles.Count} articoli.";
            }
            catch (Exception ex)
            {
                StatusMessage = $"Errore durante la ricerca: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }
    }
}
