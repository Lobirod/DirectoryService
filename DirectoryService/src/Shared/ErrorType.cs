namespace Shared;

public enum ErrorType
{
    /// <summary>
    /// Ошибка ничего не найдено
    /// </summary>
    NOT_FOUND,

    /// <summary>
    /// Ошибка с валидацией
    /// </summary>
    VALIDATION,

    /// <summary>
    /// Ошибка конфликт
    /// </summary>
    CONFLICT,

    /// <summary>
    /// Ошибка сервера
    /// </summary>
    FAILURE,
}