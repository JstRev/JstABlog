/**
 * IRepository — contratto base per tutti i repository.
 * Simula un'interfaccia generica come IRepository<T> in C#.
 *
 * Ogni repository concreto deve estendere questa classe
 * e implementare tutti i metodi, altrimenti verrà lanciato
 * un errore a runtime al momento della chiamata.
 *
 * @template T - il tipo di entità gestita dal repository
 */
export class IRepository {
  /**
   * Recupera tutte le entità.
   * @returns {Promise<T[]>}
   */
  async getAll() {
    throw new Error(`${this.constructor.name} deve implementare getAll()`);
  }

  /**
   * Recupera una singola entità per ID.
   * @param {string} id
   * @returns {Promise<T>}
   */
  async getById(id) {
    throw new Error(`${this.constructor.name} deve implementare getById()`);
  }

  /**
   * Salva un'entità (crea o aggiorna).
   * @param {T} entity
   * @returns {Promise<void>}
   */
  async save(entity) {
    throw new Error(`${this.constructor.name} deve implementare save()`);
  }

  /**
   * Elimina un'entità per ID.
   * @param {string} id
   * @returns {Promise<void>}
   */
  async delete(id) {
    throw new Error(`${this.constructor.name} deve implementare delete()`);
  }
}