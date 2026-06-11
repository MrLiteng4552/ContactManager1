using System;

namespace ContactManager.Models;

public class Contact
{
    public int Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public int? DepartmentId { get; set; }

    public virtual Department? Department { get; set; }
}
/* 
  =========================================
  ГЛОБАЛЬНОЕ ОПИСАНИЕ ФАЙЛА (ДЛЯ ЗАЩИТЫ):
  =========================================
  Этот класс является Моделью (Entity). Это простое C#-описание структуры таблицы "Контакты" в базе данных.

  ЧТО ОН ДЕЛАЕТ И ЗАЧЕМ НУЖЕН КРАТКО:
  1. Описание полей — свойства Id (первичный ключ), FullName, Email, Phone хранят текстовые данные контакта.
  2. IsActive (флаг) — ключевое поле для реализации Soft Delete (мягкого удаления). Если оно false — контакт удален.
  3. CreatedAt — фиксирует точную дату занесения сотрудника в систему.
  4. DepartmentId (int?) — внешний ключ связи с отделом. Знак "?" означает поддержку NULL (человек может быть без отдела).
  5. Навигационное свойство Department — позволяет Entity Framework автоматически подгружать объект 
     отдела из таблицы отделов по внешнему ключу (связь "один ко многим").
  =========================================
*/  