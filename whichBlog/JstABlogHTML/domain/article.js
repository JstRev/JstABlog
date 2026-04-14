/**
 * Article — modello di dominio puro.
 * Non conosce Firebase, non valida regole di business:
 * contiene solo la struttura dei dati e metodi di utilità.
 */
export class Article {
  /**
   * @param {string|null}  [Id=null]        - UUID dell'articolo
   * @param {string}       [Title='']       - Titolo
   * @param {string}       [Content='']     - Contenuto testuale
   * @param {number|null}  [Timestamp=null] - Unix timestamp in secondi
   */
  constructor({ Id = null, Title = '', Content = '', Timestamp = null } = {}) {
    this.Id        = Id;
    this.Title     = Title;
    this.Content   = Content;
    this.Timestamp = Timestamp;
  }

  /**
   * Restituisce la data di pubblicazione formattata in italiano.
   * @returns {string}
   */
  formattedDate() {
    if (!this.Timestamp) return '';
    return new Date(this.Timestamp * 1000).toLocaleDateString('it-IT', {
      day: '2-digit', month: 'long', year: 'numeric',
    });
  }

  /**
   * Serializza l'articolo come plain object (es. per la persistenza).
   * @returns {{ Id: string, Title: string, Content: string, Timestamp: number }}
   */
  toJSON() {
    return {
      Id:        this.Id,
      Title:     this.Title,
      Content:   this.Content,
      Timestamp: this.Timestamp,
    };
  }

  /**
   * Genera un UUID v4.
   * @returns {string}
   */
  static generateId() {
    return 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, c => {
      const r = (Math.random() * 16) | 0;
      return (c === 'x' ? r : (r & 0x3) | 0x8).toString(16);
    });
  }
}