import { IRepository } from './IRepository.js';
import { Article } from '../domain/article.js';

const DB_BASE_URL = 'https://blog-4n-default-rtdb.europe-west1.firebasedatabase.app';
const ARTICLES_PATH = 'articles';

function buildUrl(path = '') {
  const normalized = path ? `/${path.replace(/^\/+/, '')}` : '';
  return `${DB_BASE_URL}${normalized}.json`;
}

function mapFirebaseRecord(key, raw = {}) {
  return new Article({
    Id: raw.Id ?? key,
    Title: raw.Title ?? '',
    Content: raw.Content ?? '',
    Timestamp: raw.Timestamp ?? null,
  });
}

export class ArticleFirebaseRepository extends IRepository {
  async getAll() {
    const res = await fetch(buildUrl(ARTICLES_PATH));
    if (!res.ok) throw new Error(`Errore HTTP ${res.status}`);

    const data = await res.json();
    if (!data) return [];

    return Object.entries(data).map(([key, raw]) => mapFirebaseRecord(key, raw));
  }

  async getById(id) {
    if (!id?.trim()) throw new Error('ID non valido');

    const res = await fetch(buildUrl(`${ARTICLES_PATH}/${id}`));
    if (!res.ok) throw new Error(`Errore HTTP ${res.status}`);

    const data = await res.json();
    if (!data) throw new Error(`Articolo con id "${id}" non trovato`);

    return mapFirebaseRecord(id, data);
  }

  async save(article) {
    if (!(article instanceof Article)) {
      article = new Article(article);
    }

    const id = article.Id?.trim() || Article.generateId();
    const payload = new Article({
      Id: id,
      Title: article.Title?.trim() ?? '',
      Content: article.Content?.trim() ?? '',
      Timestamp: article.Timestamp ?? Math.floor(Date.now() / 1000),
    }).toJSON();

    const res = await fetch(buildUrl(`${ARTICLES_PATH}/${id}`), {
      method: 'PUT',
      headers: {
        'Content-Type': 'application/json',
      },
      body: JSON.stringify(payload),
    });

    if (!res.ok) throw new Error(`Salvataggio fallito: HTTP ${res.status}`);
    return new Article(payload);
  }

  async delete(id) {
    if (!id?.trim()) throw new Error('ID non valido');

    const res = await fetch(buildUrl(`${ARTICLES_PATH}/${id}`), {
      method: 'DELETE',
    });

    if (!res.ok) throw new Error(`Eliminazione fallita: HTTP ${res.status}`);
  }
}
