import { ArticleService } from '../service/ArticleService.js';

const articleService = new ArticleService();

const state = {
  articles: [],
  selectedId: null,
};

const elements = {
  list: document.getElementById('article-list'),
  title: document.getElementById('title'),
  content: document.getElementById('content'),
  btnReload: document.getElementById('btnReload'),
  btnCreate: document.getElementById('btnCreate'),
  btnSave: document.getElementById('btnSave'),
  btnDelete: document.getElementById('btnDelete'),
  btnSearchTitle: document.getElementById('btnSearchTitle'),
  btnSearchContent: document.getElementById('btnSearchContent'),
  btnSearchDate: document.getElementById('btnSearchDate'),
  btnSearchDateCount: document.getElementById('btnSearchDateCount'),
  btnSearchPeriod: document.getElementById('btnSearchPeriod'),
  searchTitle: document.getElementById('searchTitle'),
  searchContent: document.getElementById('searchContent'),
  searchDate: document.getElementById('searchDate'),
  searchPeriodFrom: document.getElementById('searchPeriodFrom'),
  searchPeriodTo: document.getElementById('searchPeriodTo'),
  status: document.getElementById('statusMessage'),
  statusCount: document.getElementById('statusCount'),
};

init();

function init() {
  bindEvents();
  loadAllArticles();
}

function bindEvents() {
  elements.btnReload.addEventListener('click', () => loadAllArticles('Lista aggiornata'));
  elements.btnCreate.addEventListener('click', createNewArticleDraft);
  elements.btnSave.addEventListener('click', handleSave);
  elements.btnDelete.addEventListener('click', handleDelete);
  elements.btnSearchTitle.addEventListener('click', handleSearchTitle);
  elements.btnSearchContent.addEventListener('click', handleSearchContent);
  elements.btnSearchDate.addEventListener('click', handleSearchDate);
  elements.btnSearchDateCount.addEventListener('click', handleCountByDate);
  elements.btnSearchPeriod.addEventListener('click', handleSearchPeriod);
}

async function loadAllArticles(message = '') {
  await runAction(async () => {
    const articles = await articleService.getAll();
    setArticles(articles);

    if (articles.length > 0) {
      selectArticle(articles[0].Id);
    } else {
      createNewArticleDraft();
    }

    if (message) setStatus(message, 'success');
  }, 'Impossibile caricare gli articoli');
}

function setArticles(articles) {
  state.articles = articles;
  renderList(articles);
  updateCount(articles.length);
}

function renderList(articles) {
  elements.list.innerHTML = '';

  if (articles.length === 0) {
    const empty = document.createElement('div');
    empty.className = 'text-center text-muted small py-4';
    empty.textContent = 'Nessun articolo trovato';
    elements.list.appendChild(empty);
    return;
  }

  articles.forEach(article => {
    const btn = document.createElement('button');
    btn.type = 'button';
    btn.className = `list-group-item list-group-item-action text-start py-2 px-3 ${article.Id === state.selectedId ? 'active' : ''}`;
    btn.innerHTML = `
      <div class="fw-semibold">${escapeHtml(article.Title || 'Senza titolo')}</div>
      <div class="small opacity-75 mt-1">${escapeHtml(article.formattedDate() || 'Senza data')}</div>
    `;
    btn.addEventListener('click', () => selectArticle(article.Id));
    elements.list.appendChild(btn);
  });
}

function selectArticle(id) {
  const article = state.articles.find(item => item.Id === id);
  if (!article) return;

  state.selectedId = article.Id;
  elements.title.value = article.Title ?? '';
  elements.content.value = article.Content ?? '';
  renderList(state.articles);
}

function createNewArticleDraft() {
  state.selectedId = null;
  elements.title.value = '';
  elements.content.value = '';
  renderList(state.articles);
  setStatus('Modalità nuovo articolo', 'info');
}

async function handleSave() {
  await runAction(async () => {
    const saved = await articleService.save({
      Id: state.selectedId,
      Title: elements.title.value,
      Content: elements.content.value,
    });

    await loadAllArticles();
    state.selectedId = saved.Id;
    selectArticle(saved.Id);
    setStatus(state.selectedId ? 'Articolo salvato correttamente' : 'Articolo creato correttamente', 'success');
  }, 'Salvataggio non riuscito');
}

async function handleDelete() {
  if (!state.selectedId) {
    setStatus('Seleziona un articolo da eliminare', 'warning');
    return;
  }

  const confirmed = window.confirm('Vuoi davvero eliminare questo articolo?');
  if (!confirmed) return;

  await runAction(async () => {
    await articleService.delete(state.selectedId);
    state.selectedId = null;
    await loadAllArticles('Articolo eliminato');
  }, 'Eliminazione non riuscita');
}

async function handleSearchTitle() {
  await runAction(async () => {
    const articles = await articleService.searchByTitle(elements.searchTitle.value);
    setArticles(articles);
    if (articles.length > 0) selectArticle(articles[0].Id);
    else createNewArticleDraft();
    setStatus('Ricerca per titolo completata', 'success');
  }, 'Ricerca per titolo non riuscita');
}

async function handleSearchContent() {
  await runAction(async () => {
    const articles = await articleService.searchByContent(elements.searchContent.value);
    setArticles(articles);
    if (articles.length > 0) selectArticle(articles[0].Id);
    else createNewArticleDraft();
    setStatus('Ricerca per contenuto completata', 'success');
  }, 'Ricerca per contenuto non riuscita');
}

async function handleSearchDate() {
  await runAction(async () => {
    const articles = await articleService.searchByExactDate(elements.searchDate.value);
    setArticles(articles);
    if (articles.length > 0) selectArticle(articles[0].Id);
    else createNewArticleDraft();
    setStatus('Ricerca per data completata', 'success');
  }, 'Ricerca per data non riuscita');
}

async function handleCountByDate() {
  await runAction(async () => {
    const count = await articleService.countByExactDate(elements.searchDate.value);
    setStatus(`Articoli trovati nella data selezionata: ${count}`, 'info');
  }, 'Conteggio non riuscito');
}

async function handleSearchPeriod() {
  await runAction(async () => {
    const articles = await articleService.searchByPeriod(
      elements.searchPeriodFrom.value,
      elements.searchPeriodTo.value,
    );
    setArticles(articles);
    if (articles.length > 0) selectArticle(articles[0].Id);
    else createNewArticleDraft();
    setStatus('Ricerca per periodo completata', 'success');
  }, 'Ricerca per periodo non riuscita');
}

async function runAction(action, fallbackMessage) {
  toggleBusy(true);
  try {
    await action();
  } catch (error) {
    console.error(error);
    setStatus(error?.message || fallbackMessage, 'danger');
  } finally {
    toggleBusy(false);
  }
}

function toggleBusy(isBusy) {
  [
    elements.btnReload,
    elements.btnCreate,
    elements.btnSave,
    elements.btnDelete,
    elements.btnSearchTitle,
    elements.btnSearchContent,
    elements.btnSearchDate,
    elements.btnSearchDateCount,
    elements.btnSearchPeriod,
  ].forEach(button => {
    button.disabled = isBusy;
  });
}

function setStatus(message, type = 'info') {
  elements.status.className = `alert alert-${type} py-2 px-3 small mb-3`;
  elements.status.textContent = message;
}

function updateCount(count) {
  elements.statusCount.textContent = `${count} post`;
}

function escapeHtml(value) {
  return String(value)
    .replace(/&/g, '&amp;')
    .replace(/</g, '&lt;')
    .replace(/>/g, '&gt;')
    .replace(/"/g, '&quot;')
    .replace(/'/g, '&#39;');
}
