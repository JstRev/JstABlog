import { Article } from '../domain/article.js';
import { ArticleFirebaseRepository } from '../repository/ArticleFirebaseRepository.js';

export class ArticleService {
  constructor(repository = new ArticleFirebaseRepository()) {
    this._repository = repository;
  }

  async getAll() {
    const articles = await this._repository.getAll();
    return this.sortByDateDesc(articles);
  }

  async getById(id) {
    if (!id?.trim()) throw new Error('ID non valido');
    return this._repository.getById(id);
  }

  async save({ Id = null, Title = '', Content = '' } = {}) {
    const title = Title.trim();
    const content = Content.trim();

    if (!title) throw new Error('Il titolo è obbligatorio');
    if (!content) throw new Error('Il contenuto è obbligatorio');

    const existing = Id?.trim() ? await this.safeGetById(Id) : null;
    const article = new Article({
      Id: Id?.trim() || null,
      Title: title,
      Content: content,
      Timestamp: existing?.Timestamp ?? Math.floor(Date.now() / 1000),
    });

    return this._repository.save(article);
  }

  async delete(id) {
    if (!id?.trim()) throw new Error('Seleziona un articolo da eliminare');
    return this._repository.delete(id);
  }

  async searchByTitle(term = '') {
    const normalized = term.trim().toLowerCase();
    const articles = await this.getAll();
    if (!normalized) return articles;

    return articles.filter(article => article.Title.toLowerCase().includes(normalized));
  }

  async searchByContent(term = '') {
    const normalized = term.trim().toLowerCase();
    const articles = await this.getAll();
    if (!normalized) return articles;

    return articles.filter(article => article.Content.toLowerCase().includes(normalized));
  }

  async searchByExactDate(dateString) {
    if (!dateString) throw new Error('Seleziona una data');
    const target = this.dateKeyFromDateString(dateString);
    const articles = await this.getAll();
    return articles.filter(article => this.dateKeyFromTimestamp(article.Timestamp) === target);
  }

  async countByExactDate(dateString) {
    const results = await this.searchByExactDate(dateString);
    return results.length;
  }

  async searchByPeriod(fromDate, toDate) {
    if (!fromDate || !toDate) throw new Error('Seleziona data iniziale e finale');
    if (fromDate > toDate) throw new Error('La data iniziale non può essere successiva alla finale');

    const fromKey = this.dateKeyFromDateString(fromDate);
    const toKey = this.dateKeyFromDateString(toDate);
    const articles = await this.getAll();

    return articles.filter(article => {
      const articleKey = this.dateKeyFromTimestamp(article.Timestamp);
      return articleKey >= fromKey && articleKey <= toKey;
    });
  }

  async safeGetById(id) {
    try {
      return await this._repository.getById(id);
    } catch {
      return null;
    }
  }

  sortByDateDesc(articles) {
    return [...articles].sort((a, b) => (b.Timestamp ?? 0) - (a.Timestamp ?? 0));
  }

  dateKeyFromTimestamp(timestamp) {
    if (!timestamp) return '';
    return new Date(timestamp * 1000).toISOString().slice(0, 10);
  }

  dateKeyFromDateString(dateString) {
    const date = new Date(`${dateString}T00:00:00`);
    if (Number.isNaN(date.getTime())) throw new Error('Data non valida');
    return date.toISOString().slice(0, 10);
  }
}
