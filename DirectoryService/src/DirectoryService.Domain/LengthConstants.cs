namespace DirectoryService.Domain;

public readonly struct LengthConstants
{
    public const int DEPARTMENT_NAME_MIN_LENGTH = 2;
    public const int DEPARTMENT_NAME_MAX_LENGTH = 150;

    public const int DEPARTMENT_IDENTIFIER_MIN_LENGTH = 2;
    public const int DEPARTMENT_IDENTIFIER_MAX_LENGTH = 150;

    public const int DEPARTMENT_PATH_MAX_LENGTH = 500;

    public const int LOCATION_NAME_MIN_LENGTH = 2;
    public const int LOCATION_NAME_MAX_LENGTH = 120;

    public const int LOCATION_ADDRESS_MAX_LENGTH = 200;

    public const int LOCATION_TIMEZONE_MAX_LENGTH = 100;

    public const int POSITION_NAME_MIN_LENGTH = 2;
    public const int POSITION_NAME_MAX_LENGTH = 100;

    public const int POSITION_DESCRIPTION_MAX_LENGTH = 1000;
}